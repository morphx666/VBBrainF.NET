Imports System.Threading

Public Class FormMain
    Inherits Form

    Private dlgMemory As FormMemory = New FormMemory()
    Private compilerStartTime As Integer

    Private Const ocIncVal As Byte = Asc("+")
    Private Const ocDecVal As Byte = Asc("-")
    Private Const ocIncPtr As Byte = Asc(">")
    Private Const ocDecPtr As Byte = Asc("<")
    Private Const ocPrnChr As Byte = Asc(".")
    Private Const ocInpChr As Byte = Asc(",")
    Private Const ocWleStr As Byte = Asc("[")
    Private Const ocWleEnd As Byte = Asc("]")

    Friend WithEvents MenuItem2 As MenuItem
    Friend WithEvents MenuItemOutTextSize As MenuItem
    Friend WithEvents MenuItemOTSSmallest As MenuItem
    Friend WithEvents MenuItemOTSSmall As MenuItem
    Friend WithEvents MenuItemOTSNormal As MenuItem
    Friend WithEvents MenuItemOTSLarge As MenuItem
    Friend WithEvents MenuItemOTSLargest As MenuItem
    Friend WithEvents StatusBarPanelElapsed As StatusBarPanel
    Friend WithEvents TextBoxCode As TextBox

    Private IsTranslated As Boolean
    Friend WithEvents TabPageBoF As TabPage
    Friend WithEvents TextBoxBoF As TextBox
    Friend WithEvents MenuItemFileSaveBoF As MenuItem
    Private pFileName As String = "Untitled.bf"

    Private brq() As Integer

    Private BFCodePointers() As Integer
    Private BasicCodePointers() As Integer
    Private CCodePointers() As Integer
    Private JSCodePointers() As Integer
    Private BoFCodePointers() As Integer

    Private Breakpoints() As Boolean

    Private prgLen As Integer

    Private nextStep As Boolean
    Private nextStepOver As Boolean
    Private nextStepPos As Integer
    Private nextStepOut As Boolean
    Private runToCursorPos As Integer = -1
    Private liveTrace As Boolean

    Private sOut As String

    Private fuckThread As Thread
    Private fuckEvent As AutoResetEvent

    Private refreshDisplayThread As Thread
    Private refreshEvent As AutoResetEvent

    Private translateThread As Thread

    Private debugEvent As AutoResetEvent

    Private ipsThread As Thread
    Private ipsEvent As AutoResetEvent
    Private insPerSec As Integer

    Private IsDragging As Boolean

    'Private txtCode As VBBFCTextBox

    Private Sub Run(ByVal s As String)
        InitProgram()

        forceAbort = False
        nextStep = False
        nextStepOver = False
        nextStepPos = 0

        If GetProgram(s) Then fuckEvent.Set()
    End Sub

    Private Function GetProgram(ByVal s As String) As Boolean
        If s <> "" Then
            Erase prg
            prg = (New System.Text.ASCIIEncoding).GetBytes(s)
            prgLen = UBound(prg)

            ReDim Preserve Breakpoints(prgLen)
            ReDim Preserve BFCodePointers(prgLen)

            If prgLen < 32767 Then
                maxMem = 32676
            Else
                If prgLen < 65536 Then
                    maxMem = 32768 * 2
                Else
                    maxMem = 32768 * 3
                End If
            End If

            FilterOpcodes()
            OptimizeBraquets()

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub InitProgram()
        ReDim mem(maxMem)

        TextBoxOut.Text = vbNullString
        sOut = vbNullString
        ptr = 0
    End Sub

    Private Sub RefreshDisplaySub()
        While True
            refreshEvent.WaitOne(500, True)
            UpdateDisplay()
        End While
    End Sub

    Private Sub OptimizeBraquets()
        Dim q As Integer

        Try
            If prgLen <= 0 Then Exit Sub
            ReDim brq(prgLen - 1)
            For i As Integer = 0 To prgLen
                If prg(i) = ocWleStr Then ' [
                    q = 0
                    For j As Integer = i To prgLen
                        If prg(j) = ocWleEnd Then
                            For k As Integer = 0 To prgLen
                                If brq(k) = -q Then
                                    brq(k) = j
                                    Exit For
                                End If
                            Next k
                            q -= 1 ' ]
                        Else
                            If prg(j) = ocWleStr AndAlso j < prgLen Then
                                q += 1 ' [
                                brq(j) = -q
                            End If
                        End If
                        If q = 0 Then Exit For
                    Next j
                End If
            Next i
        Catch ex As Exception
        End Try
    End Sub

    Private Sub UpdateDisplay(Optional ByVal IsEOP As Boolean = False)
        Static nCode As String
        With TextBoxOut
            If nCode <> sOut Or IsEOP Then
                Try
                    nCode = sOut
                    .Text += Replace(FixEOL(nCode), Chr(0), "").Substring(.Text.Length)
                    If IsEOP Then
                        .Text += vbCrLf + New String("-"c, 20) + vbCrLf + "End of Program"
                        If forceAbort Then .Text += vbCrLf + "User Aborted!"
                        .Text += vbCrLf + CalcCompileTime()
                        compilerStartTime = 0

                        dlgMemory.Refresh()

                        TextBoxCode.SelectionStart = Len(TextBoxCode.Text)
                        TextBoxCode.SelectionLength = 0

                        TextBoxBasic.SelectionStart = Len(TextBoxBasic.Text)
                        TextBoxBasic.SelectionLength = 0

                        TextBoxC.SelectionStart = Len(TextBoxC.Text)
                        TextBoxC.SelectionLength = 0

                        TextBoxJS.SelectionStart = Len(TextBoxJS.Text)
                        TextBoxJS.SelectionLength = 0

                        TextBoxBoF.SelectionStart = Len(TextBoxBoF.Text)
                        TextBoxBoF.SelectionLength = 0
                    End If

                    .SelectionStart = .Text.Length
                    .ScrollToCaret()
                Catch
                End Try
            End If
        End With
    End Sub

    Private Function CalcCompileTime() As String
        If compilerStartTime = 0 Then Return "0h 0m 0s 0ms"

        Dim span As TimeSpan = TimeSpan.FromTicks((My.Computer.Clock.TickCount - compilerStartTime) * 10000&)
        Return span.Hours.ToString + "h " + span.Minutes.ToString + "m " + span.Seconds.ToString + "s " + span.Milliseconds.ToString + "ms"
    End Function

    Private Sub FuckThreadSub()
        While fuckThread.ThreadState = ThreadState.Running
            fuckEvent.WaitOne(Timeout.Infinite, True)

            isRunning = True
            UpdateUI()
            insPerSec = 0
            'txtCode.ReadOnly = True

            ReadCustomSettings()

            FuckIt(FindOpcode(0))

            'txtCode.ReadOnly = False
            dlgMemory.Refresh()
            isRunning = False
            UpdateUI()

            UpdateDisplay(True)
            ResumeTranslation()
        End While
    End Sub

    Private Sub ReadCustomSettings()
        Try
            Dim p As Integer
            Dim s As String = TextBoxCode.Text
            Dim params() As String = {"CellSize"}
            Dim v As String

            Do
                p = s.IndexOf("#", p)
                If p = -1 OrElse p + 1 >= s.Length Then Exit Do
                p += 1

                For Each param As String In params
                    If s.Substring(p, param.Length).ToLower = param.ToLower Then
                        p = s.IndexOf("=", p)
                        If p = -1 OrElse p + 3 >= s.Length Then Exit Do

                        v = ""
                        Select Case param
                            Case params(0) 'CellSize
                                For i As Integer = p + 1 To s.Length - 1
                                    If Char.IsNumber(CChar(s.Substring(i, 1))) Then
                                        v += s.Substring(i, 1)
                                    Else
                                        If v <> "" Then Exit For
                                    End If
                                Next

                                Select Case v
                                    Case "256" : SetMaxCellSize(256)
                                    Case "512" : SetMaxCellSize(512)
                                End Select
                        End Select
                    End If
                Next
            Loop
        Catch ex As Exception
        End Try
    End Sub

    Private Sub FilterOpcodes()
        Dim j As Integer = 0

        For i As Integer = 0 To prgLen
            If prg(i) = ocIncVal Or prg(i) = ocDecVal Or
                prg(i) = ocIncPtr Or prg(i) = ocDecPtr Or
                prg(i) = ocPrnChr Or prg(i) = ocInpChr Or
                prg(i) = ocWleStr Or prg(i) = ocWleEnd Then
                BFCodePointers(j) = i
                prg(j) = prg(i)
                j += 1
            End If
        Next i

        prgLen = j - 1
        ReDim Preserve prg(prgLen)
    End Sub

    Private Function FuckIt(i As Integer) As Integer
        Try
            Dim j As Integer
            For i = i To prgLen
                If forceAbort Then Exit Function
                insPerSec += 1

                If isDebugging OrElse liveTrace Then
                    If runToCursorPos = i Then
                        DoBreak()
                        runToCursorPos = -1
                    End If

                    If (Not (nextStepOut OrElse nextStepOver)) OrElse liveTrace Then
                        Select Case TabControlLanguages.TabPages(TabControlLanguages.SelectedIndex).Name
                            Case TabPageBF.Name ' Brainfuck
                                TextBoxCode.SelectionStart = BFCodePointers(i)
                                TextBoxCode.SelectionLength = 1
                                TextBoxCode.ScrollToCaret()
                            Case TabPageVBasic.Name ' VB
                                If IsTranslated Then
                                    If BasicCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        TextBoxBasic.SelectionLength = 0
                                        TextBoxBasic.SelectionStart = BasicCodePointers(i)
                                        TextBoxBasic.SelectionLength = InStr(TextBoxBasic.SelectionStart, TextBoxBasic.Text, vbCrLf) - TextBoxBasic.SelectionStart
                                        TextBoxBasic.ScrollToCaret()
                                    End If
                                End If
                            Case TabPageC.Name ' C
                                If IsTranslated Then
                                    If CCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        TextBoxC.SelectionLength = 0
                                        TextBoxC.SelectionStart = CCodePointers(i)
                                        TextBoxC.SelectionLength = InStr(TextBoxC.SelectionStart, TextBoxC.Text, vbCrLf) - TextBoxC.SelectionStart
                                        TextBoxC.ScrollToCaret()
                                    End If
                                End If
                            Case TabPageJS.Name ' Javascript
                                If IsTranslated Then
                                    If JSCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        TextBoxJS.SelectionLength = 0
                                        TextBoxJS.SelectionStart = JSCodePointers(i)
                                        TextBoxJS.SelectionLength = InStr(TextBoxJS.SelectionStart, TextBoxJS.Text, vbCrLf) - TextBoxJS.SelectionStart
                                        TextBoxJS.ScrollToCaret()
                                    End If
                                End If
                            Case TabPageBoF.Name ' Boolfuck
                                If IsTranslated Then
                                    If BoFCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        TextBoxBoF.SelectionLength = 0
                                        TextBoxBoF.SelectionStart = BoFCodePointers(i)
                                        TextBoxBoF.SelectionLength = InStr(TextBoxBoF.SelectionStart, TextBoxBoF.Text, vbCrLf) - TextBoxBoF.SelectionStart
                                        TextBoxBoF.ScrollToCaret()
                                    End If
                                End If
                        End Select
                        If dlgMemory.Visible Then dlgMemory.Invalidate()

                        If isDebugging Then
                            If Not nextStepOver AndAlso Not nextStepOut Then
                                Do
                                    debugEvent.WaitOne(1, False)
                                    Application.DoEvents()
                                Loop Until nextStep Or nextStepOver Or nextStepOut Or forceAbort
                                nextStep = False
                                If nextStepOver AndAlso prg(i) = ocWleStr Then
                                    nextStepPos = brq(i)
                                Else
                                    nextStepOver = False
                                End If
                            End If
                        End If
                    End If
                ElseIf Breakpoints(i) Then
                    DoBreak()
                End If

                Select Case prg(i)
                    Case ocIncPtr : If ptr < maxMem Then ptr += 1 Else ptr = 0
                    Case ocDecPtr : If ptr > 0 Then ptr -= 1 Else ptr = maxMem
                    Case ocIncVal : If mem(ptr) = maxCellSize Then mem(ptr) = 0 Else mem(ptr) += 1
                    Case ocDecVal : If mem(ptr) = 0 Then mem(ptr) = maxCellSize Else mem(ptr) -= 1
                    Case ocPrnChr
                        sOut += Chr(mem(ptr) And &HFF) ' EgoBomb
                        If isDebugging OrElse liveTrace Then refreshEvent.Set()
                    Case ocInpChr
                        refreshEvent.Set()
                        If Not DoInputChar(Me, ptr) Then DoBreak()
                    Case ocWleStr
                        j = brq(i)
                        Do Until mem(ptr) = 0 OrElse forceAbort
                            j = FuckIt(i + 1)
                        Loop
                        If isDebugging Then
                            If nextStepOver AndAlso j = nextStepPos Then nextStepOver = False
                            If nextStepOut Then
                                nextStepOut = False
                                nextStep = False
                            End If
                        End If
                        i = j
                    Case ocWleEnd
                        Exit For
                    Case Else
                        i = FindOpcode(i)
                End Select
            Next i

            Return i
        Catch ex As System.Exception
            MsgBox("This program has crashed the compiler. Try adjusting the cell size and try again...")
        End Try
    End Function

    Private Function FindOpcode(i As Integer) As Integer
        Dim c As String

        For i = i To prgLen
            If prg(i) = ocIncVal OrElse prg(i) = ocDecVal OrElse
                prg(i) = ocIncPtr OrElse prg(i) = ocDecPtr OrElse
                prg(i) = ocPrnChr OrElse prg(i) = ocInpChr OrElse
                prg(i) = ocWleStr OrElse prg(i) = ocWleEnd Then
                Exit For
            Else
                If prg(i) = CByte(vbCr) AndAlso prg(i + 1) = CByte(vbLf) Then
                    i += 1
                Else
                    c = prg(i).ToString()
                    If c = "'" OrElse c = "/" OrElse c = "*" Then
                        For k As Integer = i To prgLen
                            c = prg(k).ToString()
                            If c = vbCr OrElse c = vbLf Then
                                i = k
                                Exit For
                            End If
                        Next
                    End If
                End If
            End If
        Next

        Return If(i < 1, 0, i - 1)
    End Function

    Private Sub DlgMemory_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        If forceAbort Then
            dlgMemory = Nothing
        Else
            ToggleMemoryMap()
            e.Cancel = True
        End If
    End Sub

    Private Function FixEOL(ByVal s As String) As String
        s = Replace(s, vbCrLf, Chr(255))
        s = Replace(s, vbLf & vbCr, Chr(255))
        s = Replace(s, vbCr, Chr(255))
        s = Replace(s, vbLf, Chr(255))
        s = Replace(s, Chr(255), vbCrLf)

        FixEOL = s
    End Function

    Private Sub DoRun()
        SuspendTranslation()

        compilerStartTime = My.Computer.Clock.TickCount

        If isDebugging Then
            isDebugging = False
            nextStep = True
            UpdateUI()
        Else
            isDebugging = False
            Run(TextBoxCode.Text)
        End If
    End Sub

    Private Sub DoStop()
        forceAbort = True
        isDebugging = False

        Do While isRunning
            Application.DoEvents()
        Loop

        UpdateUI()
        ResumeTranslation()
    End Sub

    Private Sub DoBreak()
        If TextBoxCode.Text = "" Then Exit Sub

        isDebugging = True
        If Not dlgMemory.Visible Then ToggleMemoryMap()
        If Not isRunning Then Run(TextBoxCode.Text)
        ResumeTranslation()

        UpdateUI()
    End Sub

    Private Sub DoStep()
        If Not isRunning Then
            DoBreak()
        Else
            nextStep = True
        End If
    End Sub

    Private Sub SuspendTranslation()
        If Not translateThread Is Nothing Then
            If translateThread.ThreadState = ThreadState.Running Then
                translateThread.Suspend()
                StatusBarPanelTransStatus.Text = "(Translation Suspended)"
            End If
        End If
    End Sub

    Private Sub ResumeTranslation()
        If Not translateThread Is Nothing Then
            If translateThread.ThreadState = ThreadState.Suspended Then
                translateThread.Resume()
                StatusBarPanelTransStatus.Text = "Translating:"
            End If
        End If
    End Sub

    Private Sub DoStepOut()
        nextStepOut = True
    End Sub

    Private Sub DoStepOver()
        nextStepOver = True
        If Not isRunning Then
            DoBreak()
        End If
    End Sub

    Private Sub DoFileNew()
        CancelTranslateThread()
        pFileName = "Untitled.bf"
        Erase Breakpoints
        DoStop()
        prgLen = 0
        TextBoxCode.Text = ""
        TextBoxOut.Text = ""
        UpdateTitle()
    End Sub

    Private Sub UpdateTitle()
        Dim fn As String
        Dim p As Integer = pFileName.LastIndexOf("\")
        If p <> -1 Then
            fn = pFileName.Substring(p + 1)
        Else
            fn = pFileName
        End If

        Me.Text = "VBBrainFNET - " + fn
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'txtCode2.Visible = False
        'txtCode = New VBBFCTextBox
        'With txtCode
        '    .Parent = tbBF
        '    .Size = txtCode2.Size
        '    .Location = txtCode2.Location
        '    .Anchor = txtCode2.Anchor
        '    .BackColor = Color.FromKnownColor(KnownColor.Window)
        'End With

        TextBox.CheckForIllegalCrossThreadCalls = False

        refreshEvent = New AutoResetEvent(False)
        refreshDisplayThread = New Thread(New ThreadStart(AddressOf RefreshDisplaySub)) With {.Name = "refreshDisplay_thread"}
        refreshDisplayThread.Start()

        fuckEvent = New AutoResetEvent(False)
        fuckThread = New Thread(AddressOf FuckThreadSub) With {.Name = "FuckIt_thread"}
        fuckThread.Start()

        ipsEvent = New AutoResetEvent(False)
        ipsThread = New Thread(AddressOf IPSCounterSub) With {.Name = "ips_counter"}
        ipsThread.Start()

        debugEvent = New AutoResetEvent(False)

        dlgMemory = New FormMemory With {.Owner = Me}
        AddHandler dlgMemory.Closing, AddressOf DlgMemory_Closing

        AddHandler TextBoxCode.KeyUp, AddressOf SelectAll
        AddHandler TextBoxCode.TextChanged, AddressOf BFCodeChanged
        AddHandler TextBoxBasic.KeyUp, AddressOf SelectAll
        AddHandler TextBoxC.KeyUp, AddressOf SelectAll
        AddHandler TextBoxJS.KeyUp, AddressOf SelectAll
        AddHandler TextBoxBoF.KeyUp, AddressOf SelectAll

        TextBoxCode.Font = New Font("Consolas", 10, FontStyle.Regular)
        TextBoxBasic.Font = New Font("Consolas", 10, FontStyle.Regular)
        TextBoxC.Font = New Font("Consolas", 10, FontStyle.Regular)
        TextBoxJS.Font = New Font("Consolas", 10, FontStyle.Regular)
        TextBoxBoF.Font = New Font("Consolas", 10, FontStyle.Regular)
        TextBoxOut.Font = New Font("Consolas", 10, FontStyle.Regular)

        AddHandler MenuItemOTSLargest.Click, AddressOf ChangeOutTextSize
        AddHandler MenuItemOTSLarge.Click, AddressOf ChangeOutTextSize
        AddHandler MenuItemOTSNormal.Click, AddressOf ChangeOutTextSize
        AddHandler MenuItemOTSSmall.Click, AddressOf ChangeOutTextSize
        AddHandler MenuItemOTSSmallest.Click, AddressOf ChangeOutTextSize

        UpdateTitle()
        UpdateUI()
        InitProgram()

        LoadProgramSettings()

        StatusBarInfo.Progress = -1

        For Each arg As String In My.Application.CommandLineArgs
            If arg.IndexOf("\") <> -1 Then
                Dim f As IO.FileInfo = New IO.FileInfo(arg)
                OpenBFFile(f.FullName)
            End If
        Next
    End Sub

    Private Sub IPSCounterSub()
        Dim u As String
        Dim ips As Single

        While ipsThread.ThreadState = ThreadState.Running
            ipsEvent.WaitOne(1000, False)
            ips = insPerSec
            insPerSec = 0
            Select Case ips
                Case Is > 1000000
                    ips /= 1000000
                    u = "M"
                Case Is > 1000
                    ips /= 1000
                    u = "K"
                Case Else
                    u = ""
            End Select
            StatusBarPanelIPS.Text = (Math.Round(ips, 2)).ToString() + " " + u + "ips"
            StatusBarPanelElapsed.Text = CalcCompileTime()
        End While
    End Sub

    Private Sub FormMain_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        SaveProgramSettings()

        forceAbort = True
        ipsThread.Abort()
        CancelTranslateThread()
        refreshDisplayThread.Abort()
        fuckThread.Abort()
        If Not dlgMemory Is Nothing Then dlgMemory.Close()

        e.Cancel = False
    End Sub

    Private Sub MenuItemFileOpen_Click(sender As Object, e As EventArgs) Handles MenuItemFileOpen.Click
        Dim cDlg As OpenFileDialog = New OpenFileDialog

        DoFileNew()

        With cDlg
            .Title = "Select Program to Load"
            .Filter = "Brainfuck Program (*.b;*.bf)|*.b;*.bf|All Files (*.*)|*.*"
            .FilterIndex = 0
            .CheckFileExists = True
            .ShowDialog()
            If .FileName <> "" Then OpenBFFile(.FileName)
        End With
    End Sub

    Private Sub OpenBFFile(ByVal fileName As String)
        Try
            pFileName = fileName
            Using bs As New IO.StreamReader(pFileName)
                Dim fs As IO.Stream = bs.BaseStream
                Dim b(CInt(fs.Length)) As Byte
                fs.Read(b, 0, CInt(fs.Length))
                fs.Close()
                TextBoxCode.Text = FixEOL((New Text.UTF8Encoding).GetString(b))
            End Using

            UpdateTitle()

            TextBoxCode.SelectionStart = 0
            TextBoxCode.Focus()
        Catch ex As Exception
            MsgBox("Error opening " + fileName + vbCrLf + vbCrLf + ex.Message, MsgBoxStyle.OkOnly, "Error Opening Brainfuck Program")
        End Try
    End Sub

    Private Sub MenuItemNew_Click(sender As Object, e As EventArgs) Handles MenuItemNew.Click
        DoFileNew()
    End Sub

    Private Sub MenuItemFileExit_Click(sender As Object, e As EventArgs) Handles MenuItemFileExit.Click
        Me.Close()
    End Sub

    Private Sub MenuItemDebugRun_Click(sender As Object, e As EventArgs) Handles MenuItemDebugRun.Click
        DoRun()
    End Sub

    Private Sub MenuItemDebugBreak_Click(sender As Object, e As EventArgs) Handles MenuItemDebugBreak.Click
        DoBreak()
    End Sub

    Private Sub MenuItemDebugStepIntoInto_Click(sender As Object, e As EventArgs) Handles MenuItemDebugStepInto.Click
        DoStep()
    End Sub

    Private Sub MenuItemDebugStop_Click(sender As Object, e As EventArgs) Handles MenuItemDebugStop.Click
        DoStop()
    End Sub

    Private Sub MenuItemDebugLiveTrace_Click(sender As Object, e As EventArgs) Handles MenuItemDebugLiveTrace.Click
        MenuItemDebugLiveTrace.Checked = Not MenuItemDebugLiveTrace.Checked
        liveTrace = MenuItemDebugLiveTrace.Checked
    End Sub

    Private Sub MenuItemDebugFollowPointer_Click(sender As Object, e As EventArgs) Handles MenuItemDebugFollowPointer.Click
        MenuItemDebugFollowPointer.Checked = Not MenuItemDebugFollowPointer.Checked
        followPointer = MenuItemDebugFollowPointer.Checked
    End Sub

    Private Sub MenuItemViewMemoryMap_Click(sender As Object, e As EventArgs) Handles MenuItemViewMemoryMap.Click
        ToggleMemoryMap()
    End Sub

    Private Sub ToggleMemoryMap()
        MenuItemViewMemoryMap.Checked = Not MenuItemViewMemoryMap.Checked
        If MenuItemViewMemoryMap.Checked Then
            dlgMemory.Show()
            Application.DoEvents()
            Me.Focus()
        Else
            dlgMemory.Hide()
        End If
    End Sub

    Private Sub MenuItemDebugStepOver_Click(sender As Object, e As EventArgs) Handles MenuItemDebugStepOver.Click
        DoStepOver()
    End Sub

    Private Sub MenuItemDebugStepOut_Click(sender As Object, e As EventArgs) Handles MenuItemDebugStepOut.Click
        DoStepOut()
    End Sub

    Private Sub MenuItemDebugRun2Cursor_Click(sender As Object, e As EventArgs) Handles MenuItemDebugRun2Cursor.Click
        DoRun()
        runToCursorPos = TextBoxCode.SelectionStart
    End Sub

    Private Sub UpdateUI()
        If isRunning Then
            If isDebugging Then
                MenuItemDebugRun.Text = "Resume"
                MenuItemDebugRun.Enabled = True
                MenuItemDebugBreak.Enabled = False
                MenuItemDebugRun2Cursor.Enabled = True
                MenuItemDebugStepInto.Enabled = True
                MenuItemDebugStepOver.Enabled = True
                MenuItemDebugStepOut.Enabled = True

                StatusBarPanelStatus.Text = "Debugging"
            Else
                MenuItemDebugRun.Text = "Run"
                MenuItemDebugRun.Enabled = False
                MenuItemDebugBreak.Enabled = True
                MenuItemDebugRun2Cursor.Enabled = False
                MenuItemDebugStepInto.Enabled = False
                MenuItemDebugStepOver.Enabled = False
                MenuItemDebugStepOut.Enabled = False

                StatusBarPanelStatus.Text = "Running"
            End If
            MenuItemCellSize256.Enabled = False
            MenuItemCellSize512.Enabled = False
            MenuItemOptionsPrettify.Enabled = False

            MenuItemDebugStop.Enabled = True
        Else
            MenuItemFileOpen.Enabled = True

            MenuItemOptionsPrettify.Enabled = True
            MenuItemCellSize256.Enabled = True
            MenuItemCellSize512.Enabled = True

            MenuItemDebugRun.Text = "Run"
            MenuItemDebugRun.Enabled = True
            MenuItemDebugBreak.Enabled = False
            MenuItemDebugRun2Cursor.Enabled = True
            MenuItemDebugStepInto.Enabled = True
            MenuItemDebugStepOver.Enabled = True
            MenuItemDebugStepOut.Enabled = False
            MenuItemDebugStop.Enabled = False

            StatusBarPanelStatus.Text = "Idle"
        End If
        StatusBarPanelCellSize.Text = "Cell Size: " + (maxCellSize + 1).ToString
    End Sub

    Private Sub Translate()
        Dim bCode As String = ""
        Dim cCode As String = ""
        Dim jCode As String = ""
        Dim bofCode As String = ""
        Dim c As Byte
        Dim cn As Byte
        Dim nTabs As Integer = 1
        Dim j As Integer = 1
        Dim bs As String = ""
        Dim cs As String = ""
        Dim js As String = ""
        Dim bof As String = ""
        Dim genCode As Boolean = True
        Dim tabs As String
        Dim nullByte As Byte = CByte(0)

        IsTranslated = False
        If prgLen = 0 Then
            TextBoxBasic.Text = ""
            TextBoxC.Text = ""
            TextBoxJS.Text = ""
            TextBoxBoF.Text = ""
            StatusBarPanelProgSize.Text = "Size: 0 bytes"
            StatusBarInfo.ProgramSize = 0
            Exit Sub
        Else
            Dim pl As Integer = prgLen + 1
            Dim unit As String = ""
            Select Case pl
                Case Is > 1048576
                    pl \= 1048576
                    unit = "M"
                Case Is > 1024
                    pl \= 1024
                    unit = "K"
            End Select
            StatusBarPanelProgSize.Text = "Size: " + pl.ToString() + " " + unit + "bytes"
            StatusBarInfo.ProgramSize = (prgLen \ maxMem) * 100
            StatusBarPanelTransStatus.Text = "Translating"
        End If

        MenuItemFileSaveC.Enabled = False
        MenuItemFileSaveJS.Enabled = False
        MenuItemFileSaveVB.Enabled = False
        MenuItemFileSaveBoF.Enabled = False
        MenuItemFileCompileEXE.Enabled = False

        ReDim CCodePointers(prgLen)
        cCode = "#include <conio.h>" + vbCrLf + vbCrLf +
                "int main() {" + vbCrLf +
                vbTab + "char b[" + maxMem.ToString() + "];" + vbCrLf +
                vbTab + "for(int i = 0; i < sizeof(b); i++) b[i] = 0;" + vbCrLf +
                vbTab + "char *p = b;" + vbCrLf

        ReDim BasicCodePointers(prgLen)
        bCode = "Dim p As Integer" + vbCrLf +
                "Dim b(0 To 32767) As Byte" + vbCrLf + vbCrLf +
                "Public Sub Main()" + vbCrLf +
                vbTab + "p = 0" + vbCrLf

        ReDim JSCodePointers(prgLen)
        jCode = "var p = 0;" + vbCrLf +
                "var r = '';" + vbCrLf +
                "var b = new Array();" + vbCrLf +
                "for(var i=0; i<32768; i++) b[i] = 0;" + vbCrLf + vbCrLf +
                "function main() {" + vbCrLf

        ReDim BoFCodePointers(prgLen)
        bofCode = ""

        Try
            For i As Integer = 0 To prgLen
                If i Mod 100 = 0 Then
                    StatusBarInfo.Progress = CInt(((i + 1) / (prgLen + 1)) * 100)
                    TextBoxBasic.Text = "Translating: " + StatusBarInfo.Progress.ToString() + "%"
                    TextBoxC.Text = TextBoxBasic.Text
                    TextBoxJS.Text = TextBoxBasic.Text
                    TextBoxBoF.Text = TextBoxBasic.Text
                End If

                c = prg(i)
                If i = prgLen Then
                    cn = nullByte
                Else
                    cn = prg(i + 1)
                End If

                If (c = cn) AndAlso (c = ocIncPtr OrElse c = ocDecPtr OrElse c = ocIncVal OrElse c = ocDecVal) Then
                    j += 1
                    genCode = False
                Else
                    genCode = True
                End If
                genCode = genCode Or (i = prgLen)

                If genCode Then
                    Select Case prg(i)
                        Case ocIncPtr
                            bs = "incPtr(" + j.ToString() + ")"
                            cs = "p += " + j.ToString() + ";"
                            js = bs + ";"
                            bof = ">>>>>>>>>"
                        Case ocDecPtr
                            bs = "decPtr(" + j.ToString() + ")"
                            cs = "p -= " + j.ToString() + ";"
                            js = bs + ";"
                            bof = "<<<<<<<<<"
                        Case ocIncVal
                            bs = "incVal(" + j.ToString() + ")"
                            cs = "*p += " + j.ToString() + ";"
                            js = bs + ";"
                            bof = ">[>]+<[+<]>>>>>>>>>[+]<<<<<<<<<"
                        Case ocDecVal
                            bs = "decVal(" + j.ToString() + ")"
                            cs = "*p -= " + j.ToString() + ";"
                            js = bs + ";"
                            bof = ">>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]<<<<<<<<<"
                        Case ocPrnChr
                            bs = "Print Chr(b(p))"
                            cs = "_putch(*p);"
                            js = "echo(String.fromCharCode(b[p]));"
                            bof = ">;>;>;>;>;>;>;>;<<<<<<<<"
                        Case ocInpChr
                            bs = "b(p) = Asc(Input())"
                            cs = "*p = _getch();"
                            js = "b[p] = prompt('', '').charCodeAt(0);"
                            bof = ">,>,>,>,>,>,>,>,<<<<<<<<"
                        Case ocWleStr
                            bs = "Do While b(p) > 0"
                            cs = "while(*p) {"
                            js = "while(b[p]>0) {"
                            bof = ">>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]"
                        Case ocWleEnd
                            bs = "Loop"
                            cs = "}"
                            js = cs
                            bof = ">>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]"
                    End Select
                    If bs <> "" Then
                        If bs = "Loop" Then nTabs -= 1

                        BasicCodePointers(i - j + 1) = bCode.Length + nTabs
                        CCodePointers(i - j + 1) = cCode.Length + nTabs
                        JSCodePointers(i - j + 1) = jCode.Length + nTabs
                        BoFCodePointers(i - j + 1) = bofCode.Length + (nTabs - 1)

                        tabs = StrDup(nTabs, vbTab)

                        bCode += tabs + bs + vbCrLf
                        cCode += tabs + cs + vbCrLf
                        jCode += tabs + js + vbCrLf
                        bofCode += StrDup(nTabs - 1, vbTab) + bof + vbCrLf

                        If bs.Substring(0, 1) = "D" Then nTabs += 1
                        bs = ""
                    End If
                    j = 1
                End If
            Next i

            bCode = bCode +
                    "End Sub" + vbCrLf + vbCrLf +
                    "Private Sub incPtr(ByVal n As Integer)" + vbCrLf +
                    vbTab + "If p < " + maxMem.ToString() + " - n Then p += n Else p = 0" + vbCrLf +
                    "End Sub" + vbCrLf + vbCrLf +
                    "Private Sub decPtr(ByVal n As Integer)" + vbCrLf +
                    vbTab + "If p > n - 1 Then p -= n Else p = " + maxCellSize.ToString() + vbCrLf +
                    "End Sub" + vbCrLf + vbCrLf +
                    "Private Sub incVal(ByVal n As Integer)" + vbCrLf +
                    vbTab + "If b(p) < " + maxCellSize.ToString() + " - n Then b(p) += n Else b(p) = 0" + vbCrLf +
                    "End Sub" + vbCrLf + vbCrLf +
                    "Private Sub decVal(ByVal n As Integer)" + vbCrLf +
                    vbTab + "If b(p) > n - 1 Then b(p) -= n Else b(p) = " + maxCellSize.ToString() + vbCrLf +
                    "End Sub"

            cCode = cCode +
                    vbTab + "return 0;" + vbCrLf +
                    "}"

            jCode = jCode +
                    "}" + vbCrLf + vbCrLf +
                    "function incPtr(n) {" + vbCrLf +
                    vbTab + "if(p < " + maxMem.ToString() + " - n) p += n; else p = 0;" + vbCrLf +
                    "}" + vbCrLf + vbCrLf +
                    "function decPtr(n) {" + vbCrLf +
                    vbTab + "if(p > n - 1) p -= n; else p = " + maxCellSize.ToString() + ";" + vbCrLf +
                    "}" + vbCrLf + vbCrLf +
                    "function incVal(n) {" + vbCrLf +
                    vbTab + "if(b[p] < " + maxCellSize.ToString() + " - n) b[p] += n; else b[p] = 0;" + vbCrLf +
                    "}" + vbCrLf + vbCrLf +
                    "function decVal(n) {" + vbCrLf +
                    vbTab + "if(b[p] > n - 1) b[p] -= n; else b[p] = " + maxCellSize.ToString() + ";" + vbCrLf +
                    "}" + vbCrLf + vbCrLf +
                    "function echo(c) {" + vbCrLf +
                    vbTab + "r += c;" + vbCrLf +
                    "}" + vbCrLf +
                    "var isWS = (typeof(alert)=='undefined');" + vbCrLf +
                    "if(isWS) prompt = function() {WScript.Echo(""Unable to run this program.\n'prompt' is not supported under Windows Script Host!"");return """";};" + vbCrLf +
                    "main();" + vbCrLf +
                    "if(r!='') if(isWS) WScript.Echo(r); else alert(r);"
        Catch
            bCode = "Translation Failed!"
            cCode = bCode
            jCode = bCode
            bofCode = bCode
        End Try

        TextBoxBasic.Text = bCode
        TextBoxC.Text = cCode
        TextBoxJS.Text = jCode
        TextBoxBoF.Text = bofCode

        StatusBarPanelTransStatus.Text = ""
        StatusBarInfo.Progress = -1

        MenuItemFileSaveC.Enabled = True
        MenuItemFileSaveJS.Enabled = True
        MenuItemFileSaveVB.Enabled = True
        MenuItemFileSaveBoF.Enabled = True
        MenuItemFileCompileEXE.Enabled = True

        IsTranslated = True
    End Sub

    Private Sub BFCodeChanged(ByVal sender As Object, ByVal e As EventArgs)
        If Not isRunning Then
            GetProgram(TextBoxCode.Text)
            StartTranslation()
            DoPrettify()
        End If
    End Sub

    Private Sub StartTranslation()
        CancelTranslateThread()

        translateThread = New Thread(AddressOf Translate) With {.Name = "translate_thread"}
        translateThread.Start()
    End Sub

    Private Sub CancelTranslateThread()
        If Not translateThread Is Nothing Then
            If translateThread.ThreadState = ThreadState.Suspended Then translateThread.Resume()
            translateThread.Abort()
            translateThread = Nothing
            StatusBarPanelTransStatus.Text = ""
            StatusBarInfo.Progress = -1
        End If
    End Sub

    Private Sub PanelSplit_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSplit.MouseDown
        IsDragging = True
        PanelSplit.Tag = e.Y
    End Sub

    Private Sub PanelSplit_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSplit.MouseUp
        IsDragging = False
    End Sub

    Private Sub PanelSplit_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PanelSplit.MouseMove
        If IsDragging Then
            PanelSplit.Top = PanelSplit.Top + (e.Y - CType(PanelSplit.Tag, Integer))
            ResizeUI()
        End If
    End Sub

    Private Sub FormMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        ResizeUI()
    End Sub

    Private Sub ResizeUI()
        If Height - PanelSplit.Top < 125 Then PanelSplit.Top = Height - 125
        If PanelSplit.Top < 100 Then PanelSplit.Top = 100

        TabControlLanguages.Height = PanelSplit.Top - 10

        Dim n As Integer = PanelSplit.Top + PanelSplit.Height
        TextBoxOut.Height += (TextBoxOut.Top - n)
        TextBoxOut.Top = n
    End Sub

    Private Sub MenuItemFileSave_Click(sender As Object, e As EventArgs) Handles MenuItemFileSave.Click
        DoFileSave(False)
    End Sub

    Private Sub DoFileSave(ByVal IsSaveAs As Boolean)
        Dim cDlg As SaveFileDialog = New SaveFileDialog

        If Not IO.File.Exists(pFileName) Or IsSaveAs Then
            With cDlg
                .Title = "Save Program" + IIf(IsSaveAs, " As", "")
                .Filter = "Brainfuck Program (*.b;*.bf)|*.b;*.bf|All Files (*.*)|*.*"
                .FilterIndex = 0
                .CheckPathExists = True
                .FileName = pFileName
                .ShowDialog()
                If .FileName = "" Then
                    Exit Sub
                Else
                    pFileName = .FileName
                End If
            End With
        End If

        Dim fs As IO.FileStream = New IO.FileStream(pFileName, IO.FileMode.OpenOrCreate)
        fs.Write((New System.Text.ASCIIEncoding).GetBytes(TextBoxCode.Text), 0, TextBoxCode.Text.Length)
        fs.Close()

        UpdateTitle()
    End Sub

    Private Sub MenuItemFileSaveAs_Click(sender As Object, e As EventArgs) Handles MenuItemFileSaveAs.Click
        DoFileSave(True)
    End Sub

    Private Sub MenuItemCellSize256_Click(sender As Object, e As EventArgs) Handles MenuItemCellSize256.Click
        SetMaxCellSize(256)
    End Sub

    Private Sub MenuItemCellSize512_Click(sender As Object, e As EventArgs) Handles MenuItemCellSize512.Click
        SetMaxCellSize(512)
    End Sub

    Private Sub SetMaxCellSize(ByVal cs As Integer)
        If cs = maxCellSize + 1 Then Exit Sub
        Select Case cs
            Case 256
                MenuItemCellSize256.Checked = True
                MenuItemCellSize512.Checked = False
                maxCellSize = 256
            Case 512
                MenuItemCellSize256.Checked = False
                MenuItemCellSize512.Checked = True
                maxCellSize = 256 * 2
        End Select
        maxCellSize -= 1
        UpdateUI()
        StartTranslation()
    End Sub

    Private Sub SelectAll(ByVal sender As Object, ByVal e As KeyEventArgs)
        If e.KeyCode = Keys.A And e.Control Then
            If TypeOf (sender) Is TextBox Then
                Dim txt As TextBox = CType(sender, TextBox)
                txt.SelectionStart = 0
                txt.SelectionLength = txt.Text.Length
            End If
        End If
    End Sub

    Private Sub DoPrettify()
        If Not MenuItemOptionsPrettify.Checked OrElse TextBoxCode.Text.Length = 0 Then Exit Sub

        Dim i As Integer
        Dim ii As Integer
        Dim s As String = TextBoxCode.Text
        Dim nc As String = ""
        Dim c As String
        Dim lc As String
        Dim t As Integer = 0
        Dim selOpCode As Integer
        Dim selLen As Integer
        'Dim IsBracketOpen As Boolean
        Dim lineBreak As String
        Dim tabs As String = StrDup(t, vbTab)

        Const opCodes As String = "+-<>,.[]"

        lineBreak = vbCrLf

        MenuItemOptionsPrettify.Checked = False

        'If txtCode.SelectionStart = txtCode.Text.Length Then
        '    selOpCode = -1
        'Else
        '    For i = 1 To txtCode.SelectionStart
        '        If opCodes.IndexOf(s.Substring(i - 1, 1)) <> -1 Then selOpCode += 1
        '    Next i
        '    For i = i + 1 To i + txtCode.SelectionLength
        '        If opCodes.IndexOf(s.Substring(i - 1, 1)) <> -1 Then selLen += 1
        '    Next i
        'End If
        selOpCode = -1

        s = s.Replace(lineBreak, "").Replace(vbTab, "").Replace(" ", "")
        If s = "" Then Exit Sub
        StatusBarPanelTransStatus.Text = "Prettifying"

        lc = s.Substring(0, 1)
        For i = 1 To s.Length
            If i Mod 100 = 0 Then StatusBarInfo.Progress = CInt((i / s.Length) * 100)

            c = s.Substring(i - 1, 1)
            Select Case c
                Case "["
                    'findChar(nc)
                    nc += IIf(i > 1, lineBreak + tabs, "") + c
                    t += 1
                    tabs = StrDup(t, vbTab)
                    nc += lineBreak + tabs
                    If i < s.Length Then lc = s.Substring(i, 1)
                    'IsBracketOpen = True
                Case "]"
                    'findChar(nc)
                    t -= 1
                    tabs = StrDup(t, vbTab)
                    nc += lineBreak + tabs + c + lineBreak + tabs
                    If i < s.Length Then lc = s.Substring(i, 1)
                    'IsBracketOpen = False
                Case Else
                    For ii = i To s.Length
                        c = s.Substring(ii - 1, 1)
                        If c = "[" Or c = "]" Then Exit For
                        If lc <> c And opCodes.IndexOf(c) <> -1 Then
                            'findChar(nc)
                            nc += lineBreak + tabs
                            lc = c
                        End If
                        'If IsBracketOpen Then
                        '    If c = vbCr Or c = lineBreak Or c = vbTab Then
                        '        c = ""
                        '    Else
                        '        IsBracketOpen = False
                        '    End If
                        'End If
                        nc += c
                    Next ii
                    i = ii - 1
            End Select
        Next i

        If selOpCode = -1 Then
            i = nc.Length
            t = i
        Else
            If selOpCode > 0 Then
                t = 0
                For i = 1 To nc.Length
                    If InStr(opCodes, Mid(nc, i, 1)) > 0 Then
                        t += 1
                        If t = selOpCode Then Exit For
                    End If
                Next i
            Else
                i = 0
            End If
            If selLen > 0 Then
                For t = i + 1 To nc.Length
                    If opCodes.IndexOf(nc.Substring(t - 1, 1)) <> -1 Then ' If InStr(opCodes, Mid(nc, t, 1)) > 0 Then 
                        selLen -= 1
                        If selLen = 0 Then Exit For
                    End If
                Next t
            Else
                t = i
            End If
        End If

        TextBoxCode.Text = nc
        GetProgram(TextBoxCode.Text)
        For j As Integer = 0 To prgLen
            If Breakpoints(j) Then
                With TextBoxCode
                    .SelectionStart = BFCodePointers(j)
                    '.SelectionStyle(New Font(.Font.FontFamily, .Font.Size, FontStyle.Bold), Color.Red, .BackColor)
                End With
            End If
        Next

        With TextBoxCode
            .SelectionStart = i
            TextBoxCode.SelectionLength = t - i
            '.ScrollCaret()
        End With

        StatusBarPanelTransStatus.Text = ""
        StatusBarInfo.Progress = -1

        MenuItemOptionsPrettify.Checked = True
    End Sub

    Private Sub MenuItemOptionsPrettify_Click(sender As Object, e As EventArgs) Handles MenuItemOptionsPrettify.Click
        MenuItemOptionsPrettify.Checked = Not MenuItemOptionsPrettify.Checked
        DoPrettify()
    End Sub

    Private Sub MenuItemDebugToggleBreakpoint_Click(sender As Object, e As EventArgs) Handles MenuItemDebugToggleBreakpoint.Click
        DoToggleBreakpoint()
    End Sub

    Private Sub DoToggleBreakpoint()
        For i As Integer = 0 To prgLen
            If BFCodePointers(i) = TextBoxCode.SelectionStart Then
                Breakpoints(i) = Not Breakpoints(i)
                With TextBoxCode
                    If Breakpoints(i) Then
                        '.SelectionStyle(Color.White, Color.DarkRed, New Font(.Font.FontFamily, .Font.Size, FontStyle.Bold))
                    Else
                        '.SelectionStyle(.ForeColor, .BackColor, New Font(.Font.FontFamily, .Font.Size, FontStyle.Regular))
                    End If
                    Exit For
                End With
            End If
        Next i
    End Sub

    Private Sub MenuItemDebugClearAllBreakpoint_Click(sender As Object, e As EventArgs) Handles MenuItemDebugClearAllBreakpoint.Click
        Dim i As Integer
        Dim j As Integer
        For i = 0 To prgLen - 1
            Breakpoints(i) = False
        Next i

        With TextBoxCode
            i = .SelectionStart
            j = TextBoxCode.SelectionLength

            .SelectionStart = 0
            TextBoxCode.SelectionLength = .Text.Length
            '.SelectionStyle(.ForeColor, .BackColor, .Font)

            .SelectionStart = i
            TextBoxCode.SelectionLength = j
        End With
    End Sub

    Private Sub MenuItemFileSaveVB_Click(sender As Object, e As EventArgs) Handles MenuItemFileSaveVB.Click
        SaveAltLanguage(TextBoxBasic.Text, ".vb", "Visual Basic")
    End Sub

    Private Function SaveAltLanguage(ByVal c As String, ByVal ext As String, ByVal langName As String, Optional ByVal promptForFileName As Boolean = True) As String
        Dim cDlg As SaveFileDialog = New SaveFileDialog
        Dim f As IO.FileInfo = New IO.FileInfo(pFileName)
        Dim fileName As String = f.Directory.FullName + "\" + f.Name.Replace(f.Extension, "")

        If promptForFileName Then
            With cDlg
                .Title = "Save Program As " + langName
                .Filter = langName + " (*" + ext + ")|*" + ext + "|All Files (*.*)|*.*"
                .FilterIndex = 0
                .CheckPathExists = True
                .FileName = fileName
                .ShowDialog()
                If .FileName = "" Then
                    Return ""
                Else
                    fileName = .FileName
                End If
            End With
        Else
            fileName += ext
        End If

        IO.File.WriteAllBytes(fileName, (New System.Text.ASCIIEncoding).GetBytes(c))

        Return fileName
    End Function

    Private Sub MenuItemFileSaveC_Click(sender As Object, e As EventArgs) Handles MenuItemFileSaveC.Click
        SaveAsC()
    End Sub

    Private Sub MenuItemFileSaveJS_Click(sender As Object, e As EventArgs) Handles MenuItemFileSaveJS.Click
        SaveAltLanguage(TextBoxJS.Text, ".js", "JavaScript")
    End Sub

    Private Sub MenuItemFileSaveBoF_Click(sender As Object, e As EventArgs) Handles MenuItemFileSaveBoF.Click
        SaveAltLanguage(TextBoxBoF.Text, ".bof", "BoolFuck")
    End Sub

    Private Function SaveAsC(Optional ByVal promptForFileName As Boolean = True, Optional ByVal waitKeyStart As Boolean = False, Optional ByVal waitKeyEnd As Boolean = False) As String
        Dim c As String = TextBoxC.Text
        If waitKeyStart Then c = c.Replace("char *p = b;", "char *p = b;char m[]=""Press any key to start...\n\0"";_cputs(m);*m = _getch();")
        If waitKeyEnd Then c = c.Replace("return 0;", "*p = _getch();return 0;")
        Return SaveAltLanguage(c, ".cpp", "C", promptForFileName)
    End Function

    Private Sub MenuItemFileCompileEXE_Click(sender As Object, e As EventArgs) Handles MenuItemFileCompileEXE.Click
        Compile2EXE()
    End Sub

    Private Sub Compile2EXE()
        Try
            Dim vsBatchFile As String = "vsvars32.bat"

            Dim fi As IO.FileInfo = New IO.FileInfo(SaveAsC(False))
            Dim fileName As String = fi.Name
            Dim pathName As String = fi.DirectoryName
            Dim exeName As String = IO.Path.Combine(pathName, fi.FullName.Replace(".cpp", ".exe"))

            ' This used to work for older versions of VS
            Dim vsKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("VisualStudio")
            Dim vsvKey As Microsoft.Win32.RegistryKey
            Dim vsTmp As Microsoft.Win32.RegistryKey
            Dim clPath As String = ""
            Dim clFile As String
            Dim vsVars As String
            For Each kn As String In vsKey.GetSubKeyNames
                vsvKey = vsKey.OpenSubKey(kn)
                vsTmp = vsvKey.OpenSubKey("Setup")
                If Not vsTmp Is Nothing Then
                    vsTmp = vsTmp.OpenSubKey("VC")
                    If Not vsTmp Is Nothing Then
                        clPath = vsTmp.GetValue("ProductDir").ToString()
                        If clPath <> "" Then
                            clFile += "bin\cl.exe"
                            Exit For
                        End If
                    End If
                End If
            Next

            ' Support for VS 2017+
            ' https://devblogs.microsoft.com/cppblog/compiler-tools-layout-in-visual-studio-15/
            If clPath = "" Then
                Dim parentFolder As New IO.DirectoryInfo(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio"))
                Dim files() As IO.FileInfo = parentFolder.GetFiles("cl.exe", IO.SearchOption.AllDirectories)
                For Each file As IO.FileInfo In files
                    If file.DirectoryName.ToLower().Contains("bin\hostx86\x86") Then
                        vsBatchFile = "VsDevCmd.bat"
                        clPath = file.DirectoryName + "\..\..\..\..\..\..\"
                        clFile = file.FullName
                        Exit For
                    End If
                Next
            End If

            If clPath = "" Then Throw New IO.FileNotFoundException

            If IO.File.Exists(clFile) Then
                vsVars = String.Format("call ""{0}{1}""", clPath, $"..\Common7\Tools\{vsBatchFile}")
                clFile = String.Format("cl.exe /O& /G% /Fe""{0}"" ""{1}""", exeName, fi.FullName)

                Dim cOp As New FormCompilerOptions()
                With cOp
                    If .ShowDialog(Me) <> DialogResult.OK Then Exit Sub

                    SaveAsC(False, .CheckBoxWaitStart.Checked, .CheckBoxWaitEnd.Checked)

                    If .RadioButtonOp1.Checked Then clFile = clFile.Replace("&", "d")
                    If .RadioButtonOp2.Checked Then clFile = clFile.Replace("&", "1")
                    If .RadioButtonOp3.Checked Then clFile = clFile.Replace("&", "2")

                    If .RadioButtonCg1.Checked Then clFile = clFile.Replace("%", "B")
                    If .RadioButtonCg2.Checked Then clFile = clFile.Replace("%", "3")
                    If .RadioButtonCg3.Checked Then clFile = clFile.Replace("%", "4")
                    If .RadioButtonCg4.Checked Then clFile = clFile.Replace("%", "5")
                    If .RadioButtonCg5.Checked Then clFile = clFile.Replace("%", "6")
                    If .RadioButtonCg6.Checked Then clFile = clFile.Replace("%", "7")
                End With
                cOp.Dispose()

                Dim batFileName As String = fileName + ".bat"
                IO.File.WriteAllLines(batFileName, {vsVars, clFile})

                Shell(batFileName, AppWinStyle.Hide, True)

                IO.File.Delete(batFileName)
                IO.File.Delete(exeName.Replace(".exe", ".obj"))
                fi.Delete()

                Shell("explorer.exe " + pathName, AppWinStyle.NormalFocus, False)
            Else
                Throw New IO.FileNotFoundException
            End If
        Catch ex As IO.FileNotFoundException
            MsgBox("VBBrainFNET could not compile the program." + vbCrLf + vbCrLf +
                        "In order to be able to compile Brain Fuck programs make sure you have installed cl.exe." + vbCrLf +
                        "Check your Visual Studio .NET documentation for further information.", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Error Compiling")
        End Try
    End Sub

    Private Sub SaveProgramSettings()
        SaveSetting("VBBrainFNET", "Preferences", "WinX", Me.Left.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "WinY", Me.Top.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "WinW", Me.Width.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "WinH", Me.Height.ToString)

        SaveSetting("VBBrainFNET", "Preferences", "CellSize", (maxCellSize + 1).ToString)

        SaveSetting("VBBrainFNET", "Preferences", "MemX", dlgMemory.Left.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "MemY", dlgMemory.Top.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "MemH", dlgMemory.Height.ToString)
        SaveSetting("VBBrainFNET", "Preferences", "MemV", IIf(Of String)(dlgMemory.Visible, "1", "0"))

        SaveSetting("VBBrainFNET", "Preferences", "Prettify", IIf(Of String)(MenuItemOptionsPrettify.Checked, "1", "0"))
        SaveSetting("VBBrainFNET", "Preferences", "FollowPointer", IIf(Of String)(MenuItemDebugFollowPointer.Checked, "1", "0"))
        SaveSetting("VBBrainFNET", "Preferences", "LiveTrace", IIf(Of String)(MenuItemDebugLiveTrace.Checked, "1", "0"))

        SaveSetting("VBBrainFNET", "Preferences", "OutputFontSize", TextBoxOut.Font.Size.ToString)
    End Sub

    Private Sub LoadProgramSettings()
        Try
            If CInt(GetSetting("VBBrainFNET", "Preferences", "WinX", "-1")) = -1 Then Exit Sub

            Me.Left = CInt(GetSetting("VBBrainFNET", "Preferences", "WinX", "0"))
            Me.Top = CInt(GetSetting("VBBrainFNET", "Preferences", "WinY", "0"))
            Me.Width = CInt(GetSetting("VBBrainFNET", "Preferences", "WinW", CStr(Screen.PrimaryScreen.WorkingArea.Width / 2)))
            Me.Height = CInt(GetSetting("VBBrainFNET", "Preferences", "WinH", CStr(Screen.PrimaryScreen.WorkingArea.Height / 2)))

            SetMaxCellSize(CInt(GetSetting("VBBrainFNET", "Preferences", "CellSize", "256")))

            dlgMemory.Left = CInt(GetSetting("VBBrainFNET", "Preferences", "MemX", "0"))
            dlgMemory.Top = CInt(GetSetting("VBBrainFNET", "Preferences", "MemY", "0"))
            dlgMemory.Height = CInt(GetSetting("VBBrainFNET", "Preferences", "MemH", CStr(Me.Height / 2)))
            If CInt(GetSetting("VBBrainFNET", "Preferences", "MemV", "0")) = 1 Then ToggleMemoryMap()

            If CInt(GetSetting("VBBrainFNET", "Preferences", "Prettify", "0")) = 1 Then MenuItemOptionsPrettify_Click(Nothing, New System.EventArgs)
            If CInt(GetSetting("VBBrainFNET", "Preferences", "FollowPointer", "1")) = 0 Then MenuItemDebugFollowPointer_Click(Nothing, New System.EventArgs)
            If CInt(GetSetting("VBBrainFNET", "Preferences", "LiveTrace", "0")) = 1 Then MenuItemDebugLiveTrace_Click(Nothing, New System.EventArgs)

            Dim fontSize As Integer = CInt(GetSetting("VBBrainFNET", "Preferences", "OutputFontSize", "10"))
            ChangeOutTextSize(MenuItemOTSNormal, New EventArgs())
            For Each mItem As MenuItem In MenuItemOutTextSize.MenuItems
                If CType(mItem.Tag, Integer) = fontSize Then
                    ChangeOutTextSize(mItem, New EventArgs())
                End If
            Next
        Catch
        End Try
    End Sub

    Private Sub MenuItemHelpAbout_Click(sender As Object, e As EventArgs) Handles MenuItemHelpAbout.Click
        Dim dlgAbout As Form = New FormAbout
        dlgAbout.ShowDialog()
        dlgAbout.Dispose()
    End Sub

    Private Sub ChangeOutTextSize(sender As Object, e As EventArgs)
        Dim m As MenuItem = CType(sender, MenuItem)

        For Each mItem As MenuItem In m.Parent.MenuItems
            mItem.Checked = False
        Next
        m.RadioCheck = True
        m.Checked = True

        TextBoxOut.Font = New Font("Consolas", CType(m.Tag, Integer), FontStyle.Regular)
    End Sub

#Region "Windows Form Designer generated code "
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                dlgMemory?.Dispose()
                fuckEvent?.Dispose()
                refreshEvent?.Dispose()
                debugEvent?.Dispose()
                ipsEvent?.Dispose()
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Public WithEvents TextBoxOut As TextBox
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents MainMenuMenu As MainMenu
    Friend WithEvents MenuItem4 As MenuItem
    Friend WithEvents MenuItem5 As MenuItem
    Friend WithEvents MenuItem8 As MenuItem
    Friend WithEvents MenuItemFileOpen As MenuItem
    Friend WithEvents MainMenuFile As MenuItem
    Friend WithEvents MenuItemNew As MenuItem
    Friend WithEvents MenuItemFileSave As MenuItem
    Friend WithEvents MenuItemFileSaveAs As MenuItem
    Friend WithEvents MenuItemFileExit As MenuItem
    Friend WithEvents MainMenuDebug As MenuItem
    Friend WithEvents MenuItemDebugRun As MenuItem
    Friend WithEvents MenuItemDebugBreak As MenuItem
    Friend WithEvents MenuItemDebugStop As MenuItem
    Friend WithEvents MenuItem9 As MenuItem
    Friend WithEvents MenuItem10 As MenuItem
    Friend WithEvents MenuItem11 As MenuItem
    Friend WithEvents MenuItem1 As MenuItem
    Friend WithEvents MenuItemDebugLiveTrace As MenuItem
    Friend WithEvents MenuItemDebugFollowPointer As MenuItem
    Friend WithEvents MenuItemDebugStepInto As MenuItem
    Friend WithEvents MainMenuView As MenuItem
    Friend WithEvents MenuItemViewMemoryMap As MenuItem
    Friend WithEvents MenuItemDebugStepOver As MenuItem
    Friend WithEvents MenuItemDebugRun2Cursor As MenuItem
    Friend WithEvents TabPageBF As TabPage
    Friend WithEvents TabPageC As TabPage
    Friend WithEvents TextBoxBasic As TextBox
    Friend WithEvents TabControlLanguages As TabControl
    Friend WithEvents TextBoxC As TextBox
    Friend WithEvents PanelSplit As Panel
    Friend WithEvents TabPageJS As TabPage
    Friend WithEvents TextBoxJS As TextBox
    Friend WithEvents StatusBarPanelStatus As StatusBarPanel
    Friend WithEvents StatusBarPanelIPS As StatusBarPanel
    Friend WithEvents StatusBarPanelProgSize As StatusBarPanel
    Friend WithEvents StatusBarInfo As VBBFCStatusBar
    Friend WithEvents TabPageVBasic As TabPage
    Friend WithEvents MenuItemCellSize256 As MenuItem
    Friend WithEvents MenuItemCellSize512 As MenuItem
    Friend WithEvents MainMenuOptions As MenuItem
    Friend WithEvents MenuItemOptionsCellSize As MenuItem
    Friend WithEvents MenuItemOptionsPrettify As MenuItem
    Friend WithEvents MenuItemDebugStepOut As MenuItem
    Friend WithEvents StatusBarPanelTransStatus As StatusBarPanel
    Friend WithEvents MenuItemDebugClearAllBreakpoint As MenuItem
    Friend WithEvents MenuItem3 As MenuItem
    Friend WithEvents MenuItemDebugToggleBreakpoint As MenuItem
    Friend WithEvents MenuItem13 As MenuItem
    Friend WithEvents MenuItemSep As MenuItem
    Friend WithEvents MenuItemFileSaveVB As MenuItem
    Friend WithEvents MenuItemFileSaveC As MenuItem
    Friend WithEvents MenuItemFileSaveJS As MenuItem
    Friend WithEvents MenuItemFileCompileEXE As MenuItem
    Friend WithEvents StatusBarPanelCellSize As StatusBarPanel
    Friend WithEvents MainMenuHelp As MenuItem
    Friend WithEvents MenuItemHelpAbout As MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.TextBoxOut = New TextBox()
        Me.MainMenuMenu = New MainMenu(Me.components)
        Me.MainMenuFile = New MenuItem()
        Me.MenuItemNew = New MenuItem()
        Me.MenuItem4 = New MenuItem()
        Me.MenuItemFileOpen = New MenuItem()
        Me.MenuItem5 = New MenuItem()
        Me.MenuItemFileSave = New MenuItem()
        Me.MenuItemFileSaveAs = New MenuItem()
        Me.MenuItemSep = New MenuItem()
        Me.MenuItemFileSaveBoF = New MenuItem()
        Me.MenuItemFileSaveVB = New MenuItem()
        Me.MenuItemFileSaveC = New MenuItem()
        Me.MenuItemFileSaveJS = New MenuItem()
        Me.MenuItem8 = New MenuItem()
        Me.MenuItemFileCompileEXE = New MenuItem()
        Me.MenuItem13 = New MenuItem()
        Me.MenuItemFileExit = New MenuItem()
        Me.MainMenuView = New MenuItem()
        Me.MenuItemViewMemoryMap = New MenuItem()
        Me.MainMenuOptions = New MenuItem()
        Me.MenuItemOptionsCellSize = New MenuItem()
        Me.MenuItemCellSize256 = New MenuItem()
        Me.MenuItemCellSize512 = New MenuItem()
        Me.MenuItemOptionsPrettify = New MenuItem()
        Me.MenuItem2 = New MenuItem()
        Me.MenuItemOutTextSize = New MenuItem()
        Me.MenuItemOTSSmallest = New MenuItem()
        Me.MenuItemOTSSmall = New MenuItem()
        Me.MenuItemOTSNormal = New MenuItem()
        Me.MenuItemOTSLarge = New MenuItem()
        Me.MenuItemOTSLargest = New MenuItem()
        Me.MainMenuDebug = New MenuItem()
        Me.MenuItemDebugRun = New MenuItem()
        Me.MenuItemDebugRun2Cursor = New MenuItem()
        Me.MenuItem9 = New MenuItem()
        Me.MenuItemDebugBreak = New MenuItem()
        Me.MenuItem10 = New MenuItem()
        Me.MenuItemDebugStepInto = New MenuItem()
        Me.MenuItemDebugStepOver = New MenuItem()
        Me.MenuItemDebugStepOut = New MenuItem()
        Me.MenuItem11 = New MenuItem()
        Me.MenuItemDebugStop = New MenuItem()
        Me.MenuItem1 = New MenuItem()
        Me.MenuItemDebugToggleBreakpoint = New MenuItem()
        Me.MenuItemDebugClearAllBreakpoint = New MenuItem()
        Me.MenuItem3 = New MenuItem()
        Me.MenuItemDebugLiveTrace = New MenuItem()
        Me.MenuItemDebugFollowPointer = New MenuItem()
        Me.MainMenuHelp = New MenuItem()
        Me.MenuItemHelpAbout = New MenuItem()
        Me.TabControlLanguages = New TabControl()
        Me.TabPageBF = New TabPage()
        Me.TextBoxCode = New TextBox()
        Me.TabPageBoF = New TabPage()
        Me.TextBoxBoF = New TextBox()
        Me.TabPageVBasic = New TabPage()
        Me.TextBoxBasic = New TextBox()
        Me.TabPageC = New TabPage()
        Me.TextBoxC = New TextBox()
        Me.TabPageJS = New TabPage()
        Me.TextBoxJS = New TextBox()
        Me.PanelSplit = New Panel()
        Me.StatusBarInfo = New VBBrainFNET.VBBFCStatusBar()
        Me.StatusBarPanelProgSize = New StatusBarPanel()
        Me.StatusBarPanelStatus = New StatusBarPanel()
        Me.StatusBarPanelCellSize = New StatusBarPanel()
        Me.StatusBarPanelIPS = New StatusBarPanel()
        Me.StatusBarPanelElapsed = New StatusBarPanel()
        Me.StatusBarPanelTransStatus = New StatusBarPanel()
        Me.TabControlLanguages.SuspendLayout()
        Me.TabPageBF.SuspendLayout()
        Me.TabPageBoF.SuspendLayout()
        Me.TabPageVBasic.SuspendLayout()
        Me.TabPageC.SuspendLayout()
        Me.TabPageJS.SuspendLayout()
        CType(Me.StatusBarPanelProgSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanelStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanelCellSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanelIPS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanelElapsed, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanelTransStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TextBoxOut
        '
        Me.TextBoxOut.AcceptsReturn = True
        Me.TextBoxOut.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxOut.BackColor = System.Drawing.Color.Black
        Me.TextBoxOut.Cursor = Cursors.IBeam
        Me.TextBoxOut.Font = New System.Drawing.Font("Consolas", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxOut.ForeColor = System.Drawing.Color.Lime
        Me.TextBoxOut.HideSelection = False
        Me.TextBoxOut.Location = New System.Drawing.Point(0, 247)
        Me.TextBoxOut.MaxLength = 0
        Me.TextBoxOut.Multiline = True
        Me.TextBoxOut.Name = "TextBoxOut"
        Me.TextBoxOut.ReadOnly = True
        Me.TextBoxOut.RightToLeft = RightToLeft.No
        Me.TextBoxOut.ScrollBars = ScrollBars.Both
        Me.TextBoxOut.Size = New System.Drawing.Size(862, 382)
        Me.TextBoxOut.TabIndex = 1
        Me.TextBoxOut.WordWrap = False
        '
        'MainMenuMenu
        '
        Me.MainMenuMenu.MenuItems.AddRange(New MenuItem() {Me.MainMenuFile, Me.MainMenuView, Me.MainMenuOptions, Me.MainMenuDebug, Me.MainMenuHelp})
        '
        'MainMenuFile
        '
        Me.MainMenuFile.Index = 0
        Me.MainMenuFile.MenuItems.AddRange(New MenuItem() {Me.MenuItemNew, Me.MenuItem4, Me.MenuItemFileOpen, Me.MenuItem5, Me.MenuItemFileSave, Me.MenuItemFileSaveAs, Me.MenuItemSep, Me.MenuItemFileSaveBoF, Me.MenuItemFileSaveVB, Me.MenuItemFileSaveC, Me.MenuItemFileSaveJS, Me.MenuItem8, Me.MenuItemFileCompileEXE, Me.MenuItem13, Me.MenuItemFileExit})
        Me.MainMenuFile.Text = "File"
        '
        'MenuItemNew
        '
        Me.MenuItemNew.Index = 0
        Me.MenuItemNew.Shortcut = Shortcut.CtrlN
        Me.MenuItemNew.Text = "New"
        '
        'MenuItem4
        '
        Me.MenuItem4.Index = 1
        Me.MenuItem4.Text = "-"
        '
        'MenuItemFileOpen
        '
        Me.MenuItemFileOpen.Index = 2
        Me.MenuItemFileOpen.Shortcut = Shortcut.CtrlO
        Me.MenuItemFileOpen.Text = "Open..."
        '
        'MenuItem5
        '
        Me.MenuItem5.Index = 3
        Me.MenuItem5.Text = "-"
        '
        'MenuItemFileSave
        '
        Me.MenuItemFileSave.Index = 4
        Me.MenuItemFileSave.Shortcut = Shortcut.CtrlS
        Me.MenuItemFileSave.Text = "Save"
        '
        'MenuItemFileSaveAs
        '
        Me.MenuItemFileSaveAs.Index = 5
        Me.MenuItemFileSaveAs.Shortcut = Shortcut.CtrlShiftS
        Me.MenuItemFileSaveAs.Text = "Save As..."
        '
        'MenuItemSep
        '
        Me.MenuItemSep.Index = 6
        Me.MenuItemSep.Text = "-"
        '
        'MenuItemFileSaveBoF
        '
        Me.MenuItemFileSaveBoF.Index = 7
        Me.MenuItemFileSaveBoF.Text = "Save Boolfuck Code"
        '
        'MenuItemFileSaveVB
        '
        Me.MenuItemFileSaveVB.Index = 8
        Me.MenuItemFileSaveVB.Text = "Save Visual Basic Code"
        '
        'MenuItemFileSaveC
        '
        Me.MenuItemFileSaveC.Index = 9
        Me.MenuItemFileSaveC.Text = "Save C Code"
        '
        'MenuItemFileSaveJS
        '
        Me.MenuItemFileSaveJS.Index = 10
        Me.MenuItemFileSaveJS.Text = "Save JavaScript Code"
        '
        'MenuItem8
        '
        Me.MenuItem8.Index = 11
        Me.MenuItem8.Text = "-"
        '
        'MenuItemFileCompileEXE
        '
        Me.MenuItemFileCompileEXE.Index = 12
        Me.MenuItemFileCompileEXE.Text = "Compile to EXE"
        '
        'MenuItem13
        '
        Me.MenuItem13.Index = 13
        Me.MenuItem13.Text = "-"
        '
        'MenuItemFileExit
        '
        Me.MenuItemFileExit.Index = 14
        Me.MenuItemFileExit.Text = "Exit"
        '
        'MainMenuView
        '
        Me.MainMenuView.Index = 1
        Me.MainMenuView.MenuItems.AddRange(New MenuItem() {Me.MenuItemViewMemoryMap})
        Me.MainMenuView.Text = "View"
        '
        'MenuItemViewMemoryMap
        '
        Me.MenuItemViewMemoryMap.Index = 0
        Me.MenuItemViewMemoryMap.Shortcut = Shortcut.F2
        Me.MenuItemViewMemoryMap.Text = "Memory Map"
        '
        'MainMenuOptions
        '
        Me.MainMenuOptions.Index = 2
        Me.MainMenuOptions.MenuItems.AddRange(New MenuItem() {Me.MenuItemOptionsCellSize, Me.MenuItemOptionsPrettify, Me.MenuItem2, Me.MenuItemOutTextSize})
        Me.MainMenuOptions.Text = "Options"
        '
        'MenuItemOptionsCellSize
        '
        Me.MenuItemOptionsCellSize.Index = 0
        Me.MenuItemOptionsCellSize.MenuItems.AddRange(New MenuItem() {Me.MenuItemCellSize256, Me.MenuItemCellSize512})
        Me.MenuItemOptionsCellSize.Text = "Cell Size"
        '
        'MenuItemCellSize256
        '
        Me.MenuItemCellSize256.Checked = True
        Me.MenuItemCellSize256.Index = 0
        Me.MenuItemCellSize256.RadioCheck = True
        Me.MenuItemCellSize256.Text = "256"
        '
        'MenuItemCellSize512
        '
        Me.MenuItemCellSize512.Index = 1
        Me.MenuItemCellSize512.RadioCheck = True
        Me.MenuItemCellSize512.Text = "512"
        '
        'MenuItemOptionsPrettify
        '
        Me.MenuItemOptionsPrettify.Index = 1
        Me.MenuItemOptionsPrettify.Text = "Prettify"
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 2
        Me.MenuItem2.Text = "-"
        '
        'MenuItemOutTextSize
        '
        Me.MenuItemOutTextSize.Index = 3
        Me.MenuItemOutTextSize.MenuItems.AddRange(New MenuItem() {Me.MenuItemOTSSmallest, Me.MenuItemOTSSmall, Me.MenuItemOTSNormal, Me.MenuItemOTSLarge, Me.MenuItemOTSLargest})
        Me.MenuItemOutTextSize.Text = "Output Text Size"
        '
        'MenuItemOTSSmallest
        '
        Me.MenuItemOTSSmallest.Index = 0
        Me.MenuItemOTSSmallest.Tag = "8"
        Me.MenuItemOTSSmallest.Text = "Smallest"
        '
        'MenuItemOTSSmall
        '
        Me.MenuItemOTSSmall.Index = 1
        Me.MenuItemOTSSmall.Tag = "9"
        Me.MenuItemOTSSmall.Text = "Small"
        '
        'MenuItemOTSNormal
        '
        Me.MenuItemOTSNormal.Index = 2
        Me.MenuItemOTSNormal.Tag = "10"
        Me.MenuItemOTSNormal.Text = "Normal"
        '
        'MenuItemOTSLarge
        '
        Me.MenuItemOTSLarge.Index = 3
        Me.MenuItemOTSLarge.Tag = "11"
        Me.MenuItemOTSLarge.Text = "Large"
        '
        'MenuItemOTSLargest
        '
        Me.MenuItemOTSLargest.Index = 4
        Me.MenuItemOTSLargest.Tag = "12"
        Me.MenuItemOTSLargest.Text = "Largest"
        '
        'MainMenuDebug
        '
        Me.MainMenuDebug.Index = 3
        Me.MainMenuDebug.MenuItems.AddRange(New MenuItem() {Me.MenuItemDebugRun, Me.MenuItemDebugRun2Cursor, Me.MenuItem9, Me.MenuItemDebugBreak, Me.MenuItem10, Me.MenuItemDebugStepInto, Me.MenuItemDebugStepOver, Me.MenuItemDebugStepOut, Me.MenuItem11, Me.MenuItemDebugStop, Me.MenuItem1, Me.MenuItemDebugToggleBreakpoint, Me.MenuItemDebugClearAllBreakpoint, Me.MenuItem3, Me.MenuItemDebugLiveTrace, Me.MenuItemDebugFollowPointer})
        Me.MainMenuDebug.Text = "Debug"
        '
        'MenuItemDebugRun
        '
        Me.MenuItemDebugRun.Index = 0
        Me.MenuItemDebugRun.Shortcut = Shortcut.F5
        Me.MenuItemDebugRun.Text = "Run"
        '
        'MenuItemDebugRun2Cursor
        '
        Me.MenuItemDebugRun2Cursor.Index = 1
        Me.MenuItemDebugRun2Cursor.Shortcut = Shortcut.ShiftF5
        Me.MenuItemDebugRun2Cursor.Text = "Run to Cursor"
        '
        'MenuItem9
        '
        Me.MenuItem9.Index = 2
        Me.MenuItem9.Text = "-"
        '
        'MenuItemDebugBreak
        '
        Me.MenuItemDebugBreak.Index = 3
        Me.MenuItemDebugBreak.Shortcut = Shortcut.CtrlC
        Me.MenuItemDebugBreak.Text = "Break"
        '
        'MenuItem10
        '
        Me.MenuItem10.Index = 4
        Me.MenuItem10.Text = "-"
        '
        'MenuItemDebugStepInto
        '
        Me.MenuItemDebugStepInto.Index = 5
        Me.MenuItemDebugStepInto.Shortcut = Shortcut.F8
        Me.MenuItemDebugStepInto.Text = "Step Into"
        '
        'MenuItemDebugStepOver
        '
        Me.MenuItemDebugStepOver.Index = 6
        Me.MenuItemDebugStepOver.Shortcut = Shortcut.ShiftF8
        Me.MenuItemDebugStepOver.Text = "Step Over"
        '
        'MenuItemDebugStepOut
        '
        Me.MenuItemDebugStepOut.Index = 7
        Me.MenuItemDebugStepOut.Shortcut = Shortcut.CtrlShiftF8
        Me.MenuItemDebugStepOut.Text = "Step Out"
        '
        'MenuItem11
        '
        Me.MenuItem11.Index = 8
        Me.MenuItem11.Text = "-"
        '
        'MenuItemDebugStop
        '
        Me.MenuItemDebugStop.Index = 9
        Me.MenuItemDebugStop.Text = "Stop"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 10
        Me.MenuItem1.Text = "-"
        '
        'MenuItemDebugToggleBreakpoint
        '
        Me.MenuItemDebugToggleBreakpoint.Index = 11
        Me.MenuItemDebugToggleBreakpoint.Shortcut = Shortcut.F9
        Me.MenuItemDebugToggleBreakpoint.Text = "Toggle Breakpoint"
        '
        'MenuItemDebugClearAllBreakpoint
        '
        Me.MenuItemDebugClearAllBreakpoint.Index = 12
        Me.MenuItemDebugClearAllBreakpoint.Text = "Clear All Breakpoints"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 13
        Me.MenuItem3.Text = "-"
        '
        'MenuItemDebugLiveTrace
        '
        Me.MenuItemDebugLiveTrace.Index = 14
        Me.MenuItemDebugLiveTrace.Text = "Live Trace"
        '
        'MenuItemDebugFollowPointer
        '
        Me.MenuItemDebugFollowPointer.Checked = True
        Me.MenuItemDebugFollowPointer.Index = 15
        Me.MenuItemDebugFollowPointer.Text = "Follow Pointer"
        '
        'MainMenuHelp
        '
        Me.MainMenuHelp.Index = 4
        Me.MainMenuHelp.MenuItems.AddRange(New MenuItem() {Me.MenuItemHelpAbout})
        Me.MainMenuHelp.Text = "Help"
        '
        'MenuItemHelpAbout
        '
        Me.MenuItemHelpAbout.Index = 0
        Me.MenuItemHelpAbout.Text = "About..."
        '
        'TabControlLanguages
        '
        Me.TabControlLanguages.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TabControlLanguages.Controls.Add(Me.TabPageBF)
        Me.TabControlLanguages.Controls.Add(Me.TabPageBoF)
        Me.TabControlLanguages.Controls.Add(Me.TabPageVBasic)
        Me.TabControlLanguages.Controls.Add(Me.TabPageC)
        Me.TabControlLanguages.Controls.Add(Me.TabPageJS)
        Me.TabControlLanguages.Location = New System.Drawing.Point(0, 12)
        Me.TabControlLanguages.Name = "TabControlLanguages"
        Me.TabControlLanguages.SelectedIndex = 0
        Me.TabControlLanguages.Size = New System.Drawing.Size(862, 222)
        Me.TabControlLanguages.TabIndex = 6
        '
        'TabPageBF
        '
        Me.TabPageBF.Controls.Add(Me.TextBoxCode)
        Me.TabPageBF.Location = New System.Drawing.Point(4, 24)
        Me.TabPageBF.Name = "TabPageBF"
        Me.TabPageBF.Size = New System.Drawing.Size(854, 194)
        Me.TabPageBF.TabIndex = 0
        Me.TabPageBF.Text = "Brainfuck"
        '
        'TextBoxCode
        '
        Me.TextBoxCode.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxCode.Location = New System.Drawing.Point(5, 5)
        Me.TextBoxCode.Multiline = True
        Me.TextBoxCode.Name = "TextBoxCode"
        Me.TextBoxCode.ScrollBars = ScrollBars.Both
        Me.TextBoxCode.Size = New System.Drawing.Size(844, 175)
        Me.TextBoxCode.TabIndex = 0
        '
        'TabPageBoF
        '
        Me.TabPageBoF.BackColor = System.Drawing.SystemColors.Control
        Me.TabPageBoF.Controls.Add(Me.TextBoxBoF)
        Me.TabPageBoF.Location = New System.Drawing.Point(4, 22)
        Me.TabPageBoF.Name = "TabPageBoF"
        Me.TabPageBoF.Padding = New Padding(3)
        Me.TabPageBoF.Size = New System.Drawing.Size(854, 196)
        Me.TabPageBoF.TabIndex = 4
        Me.TabPageBoF.Text = "Boolfuck"
        '
        'TextBoxBoF
        '
        Me.TextBoxBoF.AcceptsReturn = True
        Me.TextBoxBoF.AcceptsTab = True
        Me.TextBoxBoF.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxBoF.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxBoF.Cursor = Cursors.IBeam
        Me.TextBoxBoF.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxBoF.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxBoF.HideSelection = False
        Me.TextBoxBoF.Location = New System.Drawing.Point(5, 5)
        Me.TextBoxBoF.MaxLength = 0
        Me.TextBoxBoF.Multiline = True
        Me.TextBoxBoF.Name = "TextBoxBoF"
        Me.TextBoxBoF.ReadOnly = True
        Me.TextBoxBoF.RightToLeft = RightToLeft.No
        Me.TextBoxBoF.ScrollBars = ScrollBars.Both
        Me.TextBoxBoF.Size = New System.Drawing.Size(637, 183)
        Me.TextBoxBoF.TabIndex = 10
        Me.TextBoxBoF.WordWrap = False
        '
        'TabPageVBasic
        '
        Me.TabPageVBasic.Controls.Add(Me.TextBoxBasic)
        Me.TabPageVBasic.Location = New System.Drawing.Point(4, 22)
        Me.TabPageVBasic.Name = "TabPageVBasic"
        Me.TabPageVBasic.Size = New System.Drawing.Size(854, 196)
        Me.TabPageVBasic.TabIndex = 1
        Me.TabPageVBasic.Text = "Visual Basic"
        '
        'TextBoxBasic
        '
        Me.TextBoxBasic.AcceptsReturn = True
        Me.TextBoxBasic.AcceptsTab = True
        Me.TextBoxBasic.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxBasic.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxBasic.Cursor = Cursors.IBeam
        Me.TextBoxBasic.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxBasic.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxBasic.HideSelection = False
        Me.TextBoxBasic.Location = New System.Drawing.Point(5, 5)
        Me.TextBoxBasic.MaxLength = 0
        Me.TextBoxBasic.Multiline = True
        Me.TextBoxBasic.Name = "TextBoxBasic"
        Me.TextBoxBasic.ReadOnly = True
        Me.TextBoxBasic.RightToLeft = RightToLeft.No
        Me.TextBoxBasic.ScrollBars = ScrollBars.Both
        Me.TextBoxBasic.Size = New System.Drawing.Size(637, 183)
        Me.TextBoxBasic.TabIndex = 8
        Me.TextBoxBasic.WordWrap = False
        '
        'TabPageC
        '
        Me.TabPageC.Controls.Add(Me.TextBoxC)
        Me.TabPageC.Location = New System.Drawing.Point(4, 22)
        Me.TabPageC.Name = "TabPageC"
        Me.TabPageC.Size = New System.Drawing.Size(854, 196)
        Me.TabPageC.TabIndex = 2
        Me.TabPageC.Text = "C"
        '
        'TextBoxC
        '
        Me.TextBoxC.AcceptsReturn = True
        Me.TextBoxC.AcceptsTab = True
        Me.TextBoxC.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxC.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxC.Cursor = Cursors.IBeam
        Me.TextBoxC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxC.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxC.HideSelection = False
        Me.TextBoxC.Location = New System.Drawing.Point(5, 5)
        Me.TextBoxC.MaxLength = 0
        Me.TextBoxC.Multiline = True
        Me.TextBoxC.Name = "TextBoxC"
        Me.TextBoxC.ReadOnly = True
        Me.TextBoxC.RightToLeft = RightToLeft.No
        Me.TextBoxC.ScrollBars = ScrollBars.Both
        Me.TextBoxC.Size = New System.Drawing.Size(637, 183)
        Me.TextBoxC.TabIndex = 8
        Me.TextBoxC.WordWrap = False
        '
        'TabPageJS
        '
        Me.TabPageJS.Controls.Add(Me.TextBoxJS)
        Me.TabPageJS.Location = New System.Drawing.Point(4, 22)
        Me.TabPageJS.Name = "TabPageJS"
        Me.TabPageJS.Size = New System.Drawing.Size(854, 196)
        Me.TabPageJS.TabIndex = 3
        Me.TabPageJS.Text = "JavaScript"
        '
        'TextBoxJS
        '
        Me.TextBoxJS.AcceptsReturn = True
        Me.TextBoxJS.AcceptsTab = True
        Me.TextBoxJS.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.TextBoxJS.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxJS.Cursor = Cursors.IBeam
        Me.TextBoxJS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxJS.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxJS.HideSelection = False
        Me.TextBoxJS.Location = New System.Drawing.Point(5, 5)
        Me.TextBoxJS.MaxLength = 0
        Me.TextBoxJS.Multiline = True
        Me.TextBoxJS.Name = "TextBoxJS"
        Me.TextBoxJS.ReadOnly = True
        Me.TextBoxJS.RightToLeft = RightToLeft.No
        Me.TextBoxJS.ScrollBars = ScrollBars.Both
        Me.TextBoxJS.Size = New System.Drawing.Size(637, 183)
        Me.TextBoxJS.TabIndex = 9
        Me.TextBoxJS.WordWrap = False
        '
        'PanelSplit
        '
        Me.PanelSplit.Anchor = CType((AnchorStyles.Left Or AnchorStyles.Right), AnchorStyles)
        Me.PanelSplit.Cursor = Cursors.NoMoveVert
        Me.PanelSplit.Location = New System.Drawing.Point(0, 356)
        Me.PanelSplit.Name = "PanelSplit"
        Me.PanelSplit.Size = New System.Drawing.Size(862, 7)
        Me.PanelSplit.TabIndex = 7
        '
        'StatusBarInfo
        '
        Me.StatusBarInfo.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusBarInfo.Location = New System.Drawing.Point(0, 635)
        Me.StatusBarInfo.Name = "StatusBarInfo"
        Me.StatusBarInfo.Panels.AddRange(New StatusBarPanel() {Me.StatusBarPanelProgSize, Me.StatusBarPanelStatus, Me.StatusBarPanelCellSize, Me.StatusBarPanelIPS, Me.StatusBarPanelElapsed, Me.StatusBarPanelTransStatus})
        Me.StatusBarInfo.ProgramSize = 0
        Me.StatusBarInfo.Progress = -1
        Me.StatusBarInfo.ShowPanels = True
        Me.StatusBarInfo.Size = New System.Drawing.Size(863, 26)
        Me.StatusBarInfo.TabIndex = 8
        '
        'StatusBarPanelProgSize
        '
        Me.StatusBarPanelProgSize.AutoSize = StatusBarPanelAutoSize.Contents
        Me.StatusBarPanelProgSize.Name = "sbpProgSize"
        Me.StatusBarPanelProgSize.Text = "Size: 0 bytes"
        Me.StatusBarPanelProgSize.Width = 81
        '
        'StatusBarPanelStatus
        '
        Me.StatusBarPanelStatus.AutoSize = StatusBarPanelAutoSize.Contents
        Me.StatusBarPanelStatus.MinWidth = 100
        Me.StatusBarPanelStatus.Name = "sbpStatus"
        '
        'StatusBarPanelCellSize
        '
        Me.StatusBarPanelCellSize.Name = "sbpCellSize"
        '
        'StatusBarPanelIPS
        '
        Me.StatusBarPanelIPS.AutoSize = StatusBarPanelAutoSize.Contents
        Me.StatusBarPanelIPS.Name = "sbpIPS"
        Me.StatusBarPanelIPS.Text = "0 ips"
        Me.StatusBarPanelIPS.Width = 40
        '
        'StatusBarPanelElapsed
        '
        Me.StatusBarPanelElapsed.AutoSize = StatusBarPanelAutoSize.Contents
        Me.StatusBarPanelElapsed.Name = "sbpElapsed"
        Me.StatusBarPanelElapsed.Text = "0h 0m 0s 0ms"
        Me.StatusBarPanelElapsed.Width = 90
        '
        'StatusBarPanelTransStatus
        '
        Me.StatusBarPanelTransStatus.AutoSize = StatusBarPanelAutoSize.Spring
        Me.StatusBarPanelTransStatus.Name = "sbpTransStatus"
        Me.StatusBarPanelTransStatus.Width = 435
        '
        'FormMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(863, 661)
        Me.Controls.Add(Me.StatusBarInfo)
        Me.Controls.Add(Me.TextBoxOut)
        Me.Controls.Add(Me.PanelSplit)
        Me.Controls.Add(Me.TabControlLanguages)
        Me.Cursor = Cursors.Default
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Location = New System.Drawing.Point(338, 230)
        Me.Menu = Me.MainMenuMenu
        Me.Name = "FormMain"
        Me.RightToLeft = RightToLeft.No
        Me.SizeGripStyle = SizeGripStyle.Show
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "VBBrainFNET"
        Me.TabControlLanguages.ResumeLayout(False)
        Me.TabPageBF.ResumeLayout(False)
        Me.TabPageBF.PerformLayout()
        Me.TabPageBoF.ResumeLayout(False)
        Me.TabPageBoF.PerformLayout()
        Me.TabPageVBasic.ResumeLayout(False)
        Me.TabPageVBasic.PerformLayout()
        Me.TabPageC.ResumeLayout(False)
        Me.TabPageC.PerformLayout()
        Me.TabPageJS.ResumeLayout(False)
        Me.TabPageJS.PerformLayout()
        CType(Me.StatusBarPanelProgSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanelStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanelCellSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanelIPS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanelElapsed, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanelTransStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region
End Class