Imports System.Threading

Public Class frmMain
    Inherits System.Windows.Forms.Form

    Private dlgMemory As frmMemory = New frmMemory
    Private compilerStartTime As Integer

    Private Const ocIncVal As Byte = Asc("+")
    Private Const ocDecVal As Byte = Asc("-")
    Private Const ocIncPtr As Byte = Asc(">")
    Private Const ocDecPtr As Byte = Asc("<")
    Private Const ocPrnChr As Byte = Asc(".")
    Private Const ocInpChr As Byte = Asc(",")
    Private Const ocWleStr As Byte = Asc("[")
    Private Const ocWleEnd As Byte = Asc("]")

    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents mOutTextSize As System.Windows.Forms.MenuItem
    Friend WithEvents mOTSSmallest As System.Windows.Forms.MenuItem
    Friend WithEvents mOTSSmall As System.Windows.Forms.MenuItem
    Friend WithEvents mOTSNormal As System.Windows.Forms.MenuItem
    Friend WithEvents mOTSLarge As System.Windows.Forms.MenuItem
    Friend WithEvents mOTSLargest As System.Windows.Forms.MenuItem
    Friend WithEvents sbpElapsed As System.Windows.Forms.StatusBarPanel
    Friend WithEvents txtCode As System.Windows.Forms.TextBox

    Private IsTranslated As Boolean
    Friend WithEvents tpBoF As System.Windows.Forms.TabPage
    Friend WithEvents txtBoF As System.Windows.Forms.TextBox
    Friend WithEvents mFileSaveBoF As System.Windows.Forms.MenuItem
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

        txtOut.Text = vbNullString
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
        With txtOut
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

                        txtCode.SelectionStart = Len(txtCode.Text)
                        txtCode.SelectionLength = 0

                        txtBasic.SelectionStart = Len(txtBasic.Text)
                        txtBasic.SelectionLength = 0

                        txtC.SelectionStart = Len(txtC.Text)
                        txtC.SelectionLength = 0

                        txtJS.SelectionStart = Len(txtJS.Text)
                        txtJS.SelectionLength = 0

                        txtBoF.SelectionStart = Len(txtBoF.Text)
                        txtBoF.SelectionLength = 0
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
            Dim s As String = txtCode.Text
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
            If prg(i) = ocIncVal Or prg(i) = ocDecVal Or _
                prg(i) = ocIncPtr Or prg(i) = ocDecPtr Or _
                prg(i) = ocPrnChr Or prg(i) = ocInpChr Or _
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
                        Select Case tcLanguages.TabPages(tcLanguages.SelectedIndex).Name
                            Case tpBF.Name ' Brainfuck
                                txtCode.SelectionStart = BFCodePointers(i)
                                txtCode.SelectionLength = 1
                                txtCode.ScrollToCaret()
                            Case tpVBasic.Name ' VB
                                If IsTranslated Then
                                    If BasicCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        txtBasic.SelectionLength = 0
                                        txtBasic.SelectionStart = BasicCodePointers(i)
                                        txtBasic.SelectionLength = InStr(txtBasic.SelectionStart, txtBasic.Text, vbCrLf) - txtBasic.SelectionStart
                                        txtBasic.ScrollToCaret()
                                    End If
                                End If
                            Case tpC.Name ' C
                                If IsTranslated Then
                                    If CCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        txtC.SelectionLength = 0
                                        txtC.SelectionStart = CCodePointers(i)
                                        txtC.SelectionLength = InStr(txtC.SelectionStart, txtC.Text, vbCrLf) - txtC.SelectionStart
                                        txtC.ScrollToCaret()
                                    End If
                                End If
                            Case tpJS.Name ' Javascript
                                If IsTranslated Then
                                    If JSCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        txtJS.SelectionLength = 0
                                        txtJS.SelectionStart = JSCodePointers(i)
                                        txtJS.SelectionLength = InStr(txtJS.SelectionStart, txtJS.Text, vbCrLf) - txtJS.SelectionStart
                                        txtJS.ScrollToCaret()
                                    End If
                                End If
                            Case tpBoF.Name ' Boolfuck
                                If IsTranslated Then
                                    If BoFCodePointers(i) = 0 Then
                                        nextStep = True
                                    Else
                                        txtBoF.SelectionLength = 0
                                        txtBoF.SelectionStart = BoFCodePointers(i)
                                        txtBoF.SelectionLength = InStr(txtBoF.SelectionStart, txtBoF.Text, vbCrLf) - txtBoF.SelectionStart
                                        txtBoF.ScrollToCaret()
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

    Private Sub dlgMemory_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
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
            Run(txtCode.Text)
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
        If txtCode.Text = "" Then Exit Sub

        isDebugging = True
        If Not dlgMemory.Visible Then ToggleMemoryMap()
        If Not isRunning Then Run(txtCode.Text)
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
                sbpTransStatus.Text = "(Translation Suspended)"
            End If
        End If
    End Sub

    Private Sub ResumeTranslation()
        If Not translateThread Is Nothing Then
            If translateThread.ThreadState = ThreadState.Suspended Then
                translateThread.Resume()
                sbpTransStatus.Text = "Translating:"
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
        txtCode.Text = ""
        txtOut.Text = ""
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

    Private Sub frmMain_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
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
        refreshDisplayThread = New Thread(New ThreadStart(AddressOf RefreshDisplaySub))
        refreshDisplayThread.Name = "refreshDisplay_thread"
        refreshDisplayThread.Start()

        fuckEvent = New AutoResetEvent(False)
        fuckThread = New Thread(AddressOf FuckThreadSub)
        fuckThread.Name = "FuckIt_thread"
        fuckThread.Start()

        ipsEvent = New AutoResetEvent(False)
        ipsThread = New Thread(AddressOf IPSCounterSub)
        ipsThread.Name = "ips_counter"
        ipsThread.Start()

        debugEvent = New AutoResetEvent(False)

        dlgMemory = New frmMemory
        dlgMemory.Owner = Me
        AddHandler dlgMemory.Closing, AddressOf dlgMemory_Closing

        AddHandler txtCode.KeyUp, AddressOf SelectAll
        AddHandler txtCode.TextChanged, AddressOf BFCodeChanged
        AddHandler txtBasic.KeyUp, AddressOf SelectAll
        AddHandler txtC.KeyUp, AddressOf SelectAll
        AddHandler txtJS.KeyUp, AddressOf SelectAll
        AddHandler txtBoF.KeyUp, AddressOf SelectAll

        txtCode.Font = New Font("Consolas", 10, FontStyle.Regular)
        txtBasic.Font = New Font("Consolas", 10, FontStyle.Regular)
        txtC.Font = New Font("Consolas", 10, FontStyle.Regular)
        txtJS.Font = New Font("Consolas", 10, FontStyle.Regular)
        txtBoF.Font = New Font("Consolas", 10, FontStyle.Regular)
        txtOut.Font = New Font("Consolas", 10, FontStyle.Regular)

        AddHandler mOTSLargest.Click, AddressOf ChangeOutTextSize
        AddHandler mOTSLarge.Click, AddressOf ChangeOutTextSize
        AddHandler mOTSNormal.Click, AddressOf ChangeOutTextSize
        AddHandler mOTSSmall.Click, AddressOf ChangeOutTextSize
        AddHandler mOTSSmallest.Click, AddressOf ChangeOutTextSize

        UpdateTitle()
        UpdateUI()
        InitProgram()

        LoadProgramSettings()

        sbInfo.Progress = -1

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
            sbpIPS.Text = (Math.Round(ips, 2)).ToString() + " " + u + "ips"
            sbpElapsed.Text = CalcCompileTime()
        End While
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        SaveProgramSettings()

        forceAbort = True
        ipsThread.Abort()
        CancelTranslateThread()
        refreshDisplayThread.Abort()
        fuckThread.Abort()
        If Not dlgMemory Is Nothing Then dlgMemory.Close()

        e.Cancel = False
    End Sub

    Private Sub mFileOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileOpen.Click
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
            Dim fs As IO.Stream = New IO.StreamReader(pFileName).BaseStream
            Dim b(CInt(fs.Length)) As Byte
            fs.Read(b, 0, CInt(fs.Length))
            fs.Close()

            txtCode.Text = FixEOL((New System.Text.UTF8Encoding).GetString(b))
            UpdateTitle()

            txtCode.SelectionStart = 0
            txtCode.Focus()
        Catch ex As Exception
            MsgBox("Error opening " + fileName + vbCrLf + vbCrLf + ex.Message, MsgBoxStyle.OkOnly, "Error Opening Brainfuck Program")
        End Try
    End Sub

    Private Sub mNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mNew.Click
        DoFileNew()
    End Sub

    Private Sub mFileExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileExit.Click
        Me.Close()
    End Sub

    Private Sub mDebugRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugRun.Click
        DoRun()
    End Sub

    Private Sub mDebugBreak_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugBreak.Click
        DoBreak()
    End Sub

    Private Sub mDebugStepIntoInto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugStepInto.Click
        DoStep()
    End Sub

    Private Sub mDebugStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugStop.Click
        DoStop()
    End Sub

    Private Sub mDebugLiveTrace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugLiveTrace.Click
        mDebugLiveTrace.Checked = Not mDebugLiveTrace.Checked
        liveTrace = mDebugLiveTrace.Checked
    End Sub

    Private Sub mDebugFollowPointer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugFollowPointer.Click
        mDebugFollowPointer.Checked = Not mDebugFollowPointer.Checked
        followPointer = mDebugFollowPointer.Checked
    End Sub

    Private Sub mViewMemoryMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mViewMemoryMap.Click
        ToggleMemoryMap()
    End Sub

    Private Sub ToggleMemoryMap()
        mViewMemoryMap.Checked = Not mViewMemoryMap.Checked
        If mViewMemoryMap.Checked Then
            dlgMemory.Show()
            Application.DoEvents()
            Me.Focus()
        Else
            dlgMemory.Hide()
        End If
    End Sub

    Private Sub mDebugStepOver_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugStepOver.Click
        DoStepOver()
    End Sub

    Private Sub mDebugStepOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugStepOut.Click
        DoStepOut()
    End Sub

    Private Sub mDebugRun2Cursor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugRun2Cursor.Click
        DoRun()
        runToCursorPos = txtCode.SelectionStart
    End Sub

    Private Sub UpdateUI()
        If isRunning Then
            If isDebugging Then
                mDebugRun.Text = "Resume"
                mDebugRun.Enabled = True
                mDebugBreak.Enabled = False
                mDebugRun2Cursor.Enabled = True
                mDebugStepInto.Enabled = True
                mDebugStepOver.Enabled = True
                mDebugStepOut.Enabled = True

                sbpStatus.Text = "Debugging"
            Else
                mDebugRun.Text = "Run"
                mDebugRun.Enabled = False
                mDebugBreak.Enabled = True
                mDebugRun2Cursor.Enabled = False
                mDebugStepInto.Enabled = False
                mDebugStepOver.Enabled = False
                mDebugStepOut.Enabled = False

                sbpStatus.Text = "Running"
            End If
            mCellSize256.Enabled = False
            mCellSize512.Enabled = False
            mOptionsPrettify.Enabled = False

            mDebugStop.Enabled = True
        Else
            mFileOpen.Enabled = True

            mOptionsPrettify.Enabled = True
            mCellSize256.Enabled = True
            mCellSize512.Enabled = True

            mDebugRun.Text = "Run"
            mDebugRun.Enabled = True
            mDebugBreak.Enabled = False
            mDebugRun2Cursor.Enabled = True
            mDebugStepInto.Enabled = True
            mDebugStepOver.Enabled = True
            mDebugStepOut.Enabled = False
            mDebugStop.Enabled = False

            sbpStatus.Text = "Idle"
        End If
        sbpCellSize.Text = "Cell Size: " + (maxCellSize + 1).ToString
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
            txtBasic.Text = ""
            txtC.Text = ""
            txtJS.Text = ""
            txtBoF.Text = ""
            sbpProgSize.Text = "Size: 0 bytes"
            sbInfo.ProgramSize = 0
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
            sbpProgSize.Text = "Size: " + pl.ToString() + " " + unit + "bytes"
            sbInfo.ProgramSize = (prgLen \ maxMem) * 100
            sbpTransStatus.Text = "Translating"
        End If

        mFileSaveC.Enabled = False
        mFileSaveJS.Enabled = False
        mFileSaveVB.Enabled = False
        mFileSaveBoF.Enabled = False
        mFileCompileEXE.Enabled = False

        ReDim CCodePointers(prgLen)
        cCode = "#include <conio.h>" + vbCrLf + vbCrLf + _
                "int main() {" + vbCrLf + _
                vbTab + "char b[" + maxMem.ToString() + "];" + vbCrLf + _
                vbTab + "for(int i = 0; i < sizeof(b); i++) b[i] = 0;" + vbCrLf + _
                vbTab + "char *p = b;" + vbCrLf

        ReDim BasicCodePointers(prgLen)
        bCode = "Dim p As Integer" + vbCrLf + _
                "Dim b(0 To 32767) As Byte" + vbCrLf + vbCrLf + _
                "Public Sub Main()" + vbCrLf + _
                vbTab + "p = 0" + vbCrLf

        ReDim JSCodePointers(prgLen)
        jCode = "var p = 0;" + vbCrLf + _
                "var r = '';" + vbCrLf + _
                "var b = new Array();" + vbCrLf + _
                "for(var i=0; i<32768; i++) b[i] = 0;" + vbCrLf + vbCrLf + _
                "function main() {" + vbCrLf

        ReDim BoFCodePointers(prgLen)
        bofCode = ""

        Try
            For i As Integer = 0 To prgLen
                If i Mod 100 = 0 Then
                    sbInfo.Progress = CInt(((i + 1) / (prgLen + 1)) * 100)
                    txtBasic.Text = "Translating: " + sbInfo.Progress.ToString() + "%"
                    txtC.Text = txtBasic.Text
                    txtJS.Text = txtBasic.Text
                    txtBoF.Text = txtBasic.Text
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

            bCode = bCode + _
                    "End Sub" + vbCrLf + vbCrLf + _
                    "Private Sub incPtr(ByVal n As Integer)" + vbCrLf + _
                    vbTab + "If p < " + maxMem.ToString() + " - n Then p += n Else p = 0" + vbCrLf + _
                    "End Sub" + vbCrLf + vbCrLf + _
                    "Private Sub decPtr(ByVal n As Integer)" + vbCrLf + _
                    vbTab + "If p > n - 1 Then p -= n Else p = " + maxCellSize.ToString() + vbCrLf + _
                    "End Sub" + vbCrLf + vbCrLf + _
                    "Private Sub incVal(ByVal n As Integer)" + vbCrLf + _
                    vbTab + "If b(p) < " + maxCellSize.ToString() + " - n Then b(p) += n Else b(p) = 0" + vbCrLf + _
                    "End Sub" + vbCrLf + vbCrLf + _
                    "Private Sub decVal(ByVal n As Integer)" + vbCrLf + _
                    vbTab + "If b(p) > n - 1 Then b(p) -= n Else b(p) = " + maxCellSize.ToString() + vbCrLf + _
                    "End Sub"

            cCode = cCode + _
                    vbTab + "return 0;" + vbCrLf + _
                    "}"

            jCode = jCode + _
                    "}" + vbCrLf + vbCrLf + _
                    "function incPtr(n) {" + vbCrLf + _
                    vbTab + "if(p < " + maxMem.ToString() + " - n) p += n; else p = 0;" + vbCrLf + _
                    "}" + vbCrLf + vbCrLf + _
                    "function decPtr(n) {" + vbCrLf + _
                    vbTab + "if(p > n - 1) p -= n; else p = " + maxCellSize.ToString() + ";" + vbCrLf + _
                    "}" + vbCrLf + vbCrLf + _
                    "function incVal(n) {" + vbCrLf + _
                    vbTab + "if(b[p] < " + maxCellSize.ToString() + " - n) b[p] += n; else b[p] = 0;" + vbCrLf + _
                    "}" + vbCrLf + vbCrLf + _
                    "function decVal(n) {" + vbCrLf + _
                    vbTab + "if(b[p] > n - 1) b[p] -= n; else b[p] = " + maxCellSize.ToString() + ";" + vbCrLf + _
                    "}" + vbCrLf + vbCrLf + _
                    "function echo(c) {" + vbCrLf + _
                    vbTab + "r += c;" + vbCrLf + _
                    "}" + vbCrLf + _
                    "var isWS = (typeof(alert)=='undefined');" + vbCrLf + _
                    "if(isWS) prompt = function() {WScript.Echo(""Unable to run this program.\n'prompt' is not supported under Windows Script Host!"");return """";};" + vbCrLf + _
                    "main();" + vbCrLf + _
                    "if(r!='') if(isWS) WScript.Echo(r); else alert(r);"
        Catch
            bCode = "Translation Failed!"
            cCode = bCode
            jCode = bCode
            bofCode = bCode
        End Try

        txtBasic.Text = bCode
        txtC.Text = cCode
        txtJS.Text = jCode
        txtBoF.Text = bofCode

        sbpTransStatus.Text = ""
        sbInfo.Progress = -1

        mFileSaveC.Enabled = True
        mFileSaveJS.Enabled = True
        mFileSaveVB.Enabled = True
        mFileSaveBoF.Enabled = True
        mFileCompileEXE.Enabled = True

        IsTranslated = True
    End Sub

    Private Sub BFCodeChanged(ByVal sender As Object, ByVal e As EventArgs)
        If Not isRunning Then
            GetProgram(txtCode.Text)
            StartTranslation()
            DoPrettify()
        End If
    End Sub

    Private Sub StartTranslation()
        CancelTranslateThread()

        translateThread = New Thread(AddressOf Translate)
        translateThread.Name = "translate_thread"
        translateThread.Start()
    End Sub

    Private Sub CancelTranslateThread()
        If Not translateThread Is Nothing Then
            If translateThread.ThreadState = ThreadState.Suspended Then translateThread.Resume()
            translateThread.Abort()
            translateThread = Nothing
            sbpTransStatus.Text = ""
            sbInfo.Progress = -1
        End If
    End Sub

    Private Sub pSplit_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pSplit.MouseDown
        IsDragging = True
        pSplit.Tag = e.Y
    End Sub

    Private Sub pSplit_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pSplit.MouseUp
        IsDragging = False
    End Sub

    Private Sub pSplit_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pSplit.MouseMove
        If IsDragging Then
            pSplit.Top = pSplit.Top + (e.Y - CType(pSplit.Tag, Integer))
            ResizeUI()
        End If
    End Sub

    Private Sub frmMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        ResizeUI()
    End Sub

    Private Sub ResizeUI()
        If Height - pSplit.Top < 125 Then pSplit.Top = Height - 125
        If pSplit.Top < 100 Then pSplit.Top = 100

        tcLanguages.Height = pSplit.Top - 10

        Dim n As Integer = pSplit.Top + pSplit.Height
        txtOut.Height += (txtOut.Top - n)
        txtOut.Top = n
    End Sub

    Private Sub mFileSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileSave.Click
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
        fs.Write((New System.Text.ASCIIEncoding).GetBytes(txtCode.Text), 0, txtCode.Text.Length)
        fs.Close()

        UpdateTitle()
    End Sub

    Private Sub mFileSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileSaveAs.Click
        DoFileSave(True)
    End Sub

    Private Sub mCellSize256_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mCellSize256.Click
        SetMaxCellSize(256)
    End Sub

    Private Sub mCellSize512_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mCellSize512.Click
        SetMaxCellSize(512)
    End Sub

    Private Sub SetMaxCellSize(ByVal cs As Integer)
        If cs = maxCellSize + 1 Then Exit Sub
        Select Case cs
            Case 256
                mCellSize256.Checked = True
                mCellSize512.Checked = False
                maxCellSize = 256
            Case 512
                mCellSize256.Checked = False
                mCellSize512.Checked = True
                maxCellSize = 256 * 2
        End Select
        maxCellSize -= 1
        UpdateUI()
        StartTranslation()
    End Sub

    Private Sub SelectAll(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.A And e.Control Then
            If TypeOf (sender) Is TextBox Then
                Dim txt As TextBox = CType(sender, TextBox)
                txt.SelectionStart = 0
                txt.SelectionLength = txt.Text.Length
            End If
        End If
    End Sub

    Private Sub DoPrettify()
        If Not mOptionsPrettify.Checked OrElse txtCode.Text.Length = 0 Then Exit Sub

        Dim i As Integer
        Dim ii As Integer
        Dim s As String = txtCode.Text
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

        mOptionsPrettify.Checked = False

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
        sbpTransStatus.Text = "Prettifying"

        lc = s.Substring(0, 1)
        For i = 1 To s.Length
            If i Mod 100 = 0 Then sbInfo.Progress = CInt((i / s.Length) * 100)

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

        txtCode.Text = nc
        GetProgram(txtCode.Text)
        For j As Integer = 0 To prgLen
            If Breakpoints(j) Then
                With txtCode
                    .SelectionStart = BFCodePointers(j)
                    '.SelectionStyle(New Font(.Font.FontFamily, .Font.Size, FontStyle.Bold), Color.Red, .BackColor)
                End With
            End If
        Next

        With txtCode
            .SelectionStart = i
            txtCode.SelectionLength = t - i
            '.ScrollCaret()
        End With

        sbpTransStatus.Text = ""
        sbInfo.Progress = -1

        mOptionsPrettify.Checked = True
    End Sub

    Private Sub mOptionsPrettify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mOptionsPrettify.Click
        mOptionsPrettify.Checked = Not mOptionsPrettify.Checked
        DoPrettify()
    End Sub

    Private Sub mDebugToggleBreakpoint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugToggleBreakpoint.Click
        DoToggleBreakpoint()
    End Sub

    Private Sub DoToggleBreakpoint()
        For i As Integer = 0 To prgLen
            If BFCodePointers(i) = txtCode.SelectionStart Then
                Breakpoints(i) = Not Breakpoints(i)
                With txtCode
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

    Private Sub mDebugClearAllBreakpoint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mDebugClearAllBreakpoint.Click
        Dim i As Integer
        Dim j As Integer
        For i = 0 To prgLen - 1
            Breakpoints(i) = False
        Next i

        With txtCode
            i = .SelectionStart
            j = txtCode.SelectionLength

            .SelectionStart = 0
            txtCode.SelectionLength = .Text.Length
            '.SelectionStyle(.ForeColor, .BackColor, .Font)

            .SelectionStart = i
            txtCode.SelectionLength = j
        End With
    End Sub

    Private Sub mFileSaveVB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileSaveVB.Click
        SaveAltLanguage(txtBasic.Text, ".vb", "Visual Basic")
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

    Private Sub mFileSaveC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileSaveC.Click
        SaveAsC()
    End Sub

    Private Sub mFileSaveJS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileSaveJS.Click
        SaveAltLanguage(txtJS.Text, ".js", "JavaScript")
    End Sub

    Private Sub mFileSaveBoF_Click(sender As Object, e As EventArgs) Handles mFileSaveBoF.Click
        SaveAltLanguage(txtBoF.Text, ".bof", "BoolFuck")
    End Sub

    Private Function SaveAsC(Optional ByVal promptForFileName As Boolean = True, Optional ByVal waitKeyStart As Boolean = False, Optional ByVal waitKeyEnd As Boolean = False) As String
        Dim c As String = txtC.Text
        If waitKeyStart Then c = c.Replace("char *p = b;", "char *p = b;char m[]=""Press any key to start...\n\0"";_cputs(m);*m = _getch();")
        If waitKeyEnd Then c = c.Replace("return 0;", "*p = _getch();return 0;")
        Return SaveAltLanguage(c, ".cpp", "C", promptForFileName)
    End Function

    Private Sub mFileCompileEXE_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mFileCompileEXE.Click
        Compile2EXE()
    End Sub

    Private Sub Compile2EXE()
        Try
            Dim fi As IO.FileInfo = New IO.FileInfo(SaveAsC(False))
            Dim fileName As String = fi.Name
            Dim pathName As String = fi.DirectoryName
            Dim exeName As String = IO.Path.Combine(pathName, fi.FullName.Replace(".cpp", ".exe"))

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
                        If clPath <> "" Then Exit For
                    End If
                End If
            Next
            If clPath = "" Then Throw New IO.FileNotFoundException

            clFile = clPath + "bin\cl.exe"
            If IO.File.Exists(clFile) Then
                vsVars = String.Format("call ""{0}{1}""", clPath, "..\Common7\Tools\vsvars32.bat")
                clFile = String.Format("cl.exe /O& /G% /Fe""{0}"" ""{1}""", exeName, fi.FullName)

                Dim cOp As New frmCompilerOptions
                With cOp
                    If .ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then Exit Sub

                    SaveAsC(False, .chkWaitStart.Checked, .chkWaitEnd.Checked)

                    If .opOp1.Checked Then clFile = clFile.Replace("&", "d")
                    If .opOp2.Checked Then clFile = clFile.Replace("&", "1")
                    If .opOp3.Checked Then clFile = clFile.Replace("&", "2")

                    If .opCg1.Checked Then clFile = clFile.Replace("%", "B")
                    If .opCg2.Checked Then clFile = clFile.Replace("%", "3")
                    If .opCg3.Checked Then clFile = clFile.Replace("%", "4")
                    If .opCg4.Checked Then clFile = clFile.Replace("%", "5")
                    If .opCg5.Checked Then clFile = clFile.Replace("%", "6")
                    If .opCg6.Checked Then clFile = clFile.Replace("%", "7")
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
            MsgBox("VBBrainFNET could not compile the program." + vbCrLf + vbCrLf + _
                        "In order to be able to compile Brain Fuck programs make sure you have installed cl.exe." + vbCrLf + _
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

        SaveSetting("VBBrainFNET", "Preferences", "Prettify", IIf(Of String)(mOptionsPrettify.Checked, "1", "0"))
        SaveSetting("VBBrainFNET", "Preferences", "FollowPointer", IIf(Of String)(mDebugFollowPointer.Checked, "1", "0"))
        SaveSetting("VBBrainFNET", "Preferences", "LiveTrace", IIf(Of String)(mDebugLiveTrace.Checked, "1", "0"))

        SaveSetting("VBBrainFNET", "Preferences", "OutputFontSize", txtOut.Font.Size.ToString)
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

            If CInt(GetSetting("VBBrainFNET", "Preferences", "Prettify", "0")) = 1 Then mOptionsPrettify_Click(Nothing, New System.EventArgs)
            If CInt(GetSetting("VBBrainFNET", "Preferences", "FollowPointer", "1")) = 0 Then mDebugFollowPointer_Click(Nothing, New System.EventArgs)
            If CInt(GetSetting("VBBrainFNET", "Preferences", "LiveTrace", "0")) = 1 Then mDebugLiveTrace_Click(Nothing, New System.EventArgs)

            Dim fontSize As Integer = CInt(GetSetting("VBBrainFNET", "Preferences", "OutputFontSize", "10"))
            ChangeOutTextSize(mOTSNormal, New EventArgs())
            For Each mItem As MenuItem In mOutTextSize.MenuItems
                If CType(mItem.Tag, Integer) = fontSize Then
                    ChangeOutTextSize(mItem, New EventArgs())
                End If
            Next
        Catch
        End Try
    End Sub

    Private Sub mHelpAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mHelpAbout.Click
        Dim dlgAbout As Form = New frmAbout
        dlgAbout.ShowDialog()
        dlgAbout.Dispose()
    End Sub

    Private Sub ChangeOutTextSize(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim m As MenuItem = CType(sender, MenuItem)

        For Each mItem As MenuItem In m.Parent.MenuItems
            mItem.Checked = False
        Next
        m.RadioCheck = True
        m.Checked = True

        txtOut.Font = New Font("Consolas", CType(m.Tag, Integer), FontStyle.Regular)
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
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Public WithEvents txtOut As System.Windows.Forms.TextBox
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents mMenu As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem5 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem8 As System.Windows.Forms.MenuItem
    Friend WithEvents mFileOpen As System.Windows.Forms.MenuItem
    Friend WithEvents mFile As System.Windows.Forms.MenuItem
    Friend WithEvents mNew As System.Windows.Forms.MenuItem
    Friend WithEvents mFileSave As System.Windows.Forms.MenuItem
    Friend WithEvents mFileSaveAs As System.Windows.Forms.MenuItem
    Friend WithEvents mFileExit As System.Windows.Forms.MenuItem
    Friend WithEvents mDebug As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugRun As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugBreak As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugStop As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem9 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem10 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem11 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugLiveTrace As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugFollowPointer As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugStepInto As System.Windows.Forms.MenuItem
    Friend WithEvents mView As System.Windows.Forms.MenuItem
    Friend WithEvents mViewMemoryMap As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugStepOver As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugRun2Cursor As System.Windows.Forms.MenuItem
    Friend WithEvents tpBF As System.Windows.Forms.TabPage
    Friend WithEvents tpC As System.Windows.Forms.TabPage
    Friend WithEvents txtBasic As System.Windows.Forms.TextBox
    Friend WithEvents tcLanguages As System.Windows.Forms.TabControl
    Friend WithEvents txtC As System.Windows.Forms.TextBox
    Friend WithEvents pSplit As System.Windows.Forms.Panel
    Friend WithEvents tpJS As System.Windows.Forms.TabPage
    Friend WithEvents txtJS As System.Windows.Forms.TextBox
    Friend WithEvents sbpStatus As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbpIPS As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbpProgSize As System.Windows.Forms.StatusBarPanel
    Friend WithEvents sbInfo As VBBFCStatusBar
    Friend WithEvents tpVBasic As System.Windows.Forms.TabPage
    Friend WithEvents mCellSize256 As System.Windows.Forms.MenuItem
    Friend WithEvents mCellSize512 As System.Windows.Forms.MenuItem
    Friend WithEvents mOptions As System.Windows.Forms.MenuItem
    Friend WithEvents mOptionsCellSize As System.Windows.Forms.MenuItem
    Friend WithEvents mOptionsPrettify As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugStepOut As System.Windows.Forms.MenuItem
    Friend WithEvents sbpTransStatus As System.Windows.Forms.StatusBarPanel
    Friend WithEvents mDebugClearAllBreakpoint As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
    Friend WithEvents mDebugToggleBreakpoint As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem13 As System.Windows.Forms.MenuItem
    Friend WithEvents mSep As System.Windows.Forms.MenuItem
    Friend WithEvents mFileSaveVB As System.Windows.Forms.MenuItem
    Friend WithEvents mFileSaveC As System.Windows.Forms.MenuItem
    Friend WithEvents mFileSaveJS As System.Windows.Forms.MenuItem
    Friend WithEvents mFileCompileEXE As System.Windows.Forms.MenuItem
    Friend WithEvents sbpCellSize As System.Windows.Forms.StatusBarPanel
    Friend WithEvents mHelp As System.Windows.Forms.MenuItem
    Friend WithEvents mHelpAbout As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.txtOut = New System.Windows.Forms.TextBox()
        Me.mMenu = New System.Windows.Forms.MainMenu(Me.components)
        Me.mFile = New System.Windows.Forms.MenuItem()
        Me.mNew = New System.Windows.Forms.MenuItem()
        Me.MenuItem4 = New System.Windows.Forms.MenuItem()
        Me.mFileOpen = New System.Windows.Forms.MenuItem()
        Me.MenuItem5 = New System.Windows.Forms.MenuItem()
        Me.mFileSave = New System.Windows.Forms.MenuItem()
        Me.mFileSaveAs = New System.Windows.Forms.MenuItem()
        Me.mSep = New System.Windows.Forms.MenuItem()
        Me.mFileSaveBoF = New System.Windows.Forms.MenuItem()
        Me.mFileSaveVB = New System.Windows.Forms.MenuItem()
        Me.mFileSaveC = New System.Windows.Forms.MenuItem()
        Me.mFileSaveJS = New System.Windows.Forms.MenuItem()
        Me.MenuItem8 = New System.Windows.Forms.MenuItem()
        Me.mFileCompileEXE = New System.Windows.Forms.MenuItem()
        Me.MenuItem13 = New System.Windows.Forms.MenuItem()
        Me.mFileExit = New System.Windows.Forms.MenuItem()
        Me.mView = New System.Windows.Forms.MenuItem()
        Me.mViewMemoryMap = New System.Windows.Forms.MenuItem()
        Me.mOptions = New System.Windows.Forms.MenuItem()
        Me.mOptionsCellSize = New System.Windows.Forms.MenuItem()
        Me.mCellSize256 = New System.Windows.Forms.MenuItem()
        Me.mCellSize512 = New System.Windows.Forms.MenuItem()
        Me.mOptionsPrettify = New System.Windows.Forms.MenuItem()
        Me.MenuItem2 = New System.Windows.Forms.MenuItem()
        Me.mOutTextSize = New System.Windows.Forms.MenuItem()
        Me.mOTSSmallest = New System.Windows.Forms.MenuItem()
        Me.mOTSSmall = New System.Windows.Forms.MenuItem()
        Me.mOTSNormal = New System.Windows.Forms.MenuItem()
        Me.mOTSLarge = New System.Windows.Forms.MenuItem()
        Me.mOTSLargest = New System.Windows.Forms.MenuItem()
        Me.mDebug = New System.Windows.Forms.MenuItem()
        Me.mDebugRun = New System.Windows.Forms.MenuItem()
        Me.mDebugRun2Cursor = New System.Windows.Forms.MenuItem()
        Me.MenuItem9 = New System.Windows.Forms.MenuItem()
        Me.mDebugBreak = New System.Windows.Forms.MenuItem()
        Me.MenuItem10 = New System.Windows.Forms.MenuItem()
        Me.mDebugStepInto = New System.Windows.Forms.MenuItem()
        Me.mDebugStepOver = New System.Windows.Forms.MenuItem()
        Me.mDebugStepOut = New System.Windows.Forms.MenuItem()
        Me.MenuItem11 = New System.Windows.Forms.MenuItem()
        Me.mDebugStop = New System.Windows.Forms.MenuItem()
        Me.MenuItem1 = New System.Windows.Forms.MenuItem()
        Me.mDebugToggleBreakpoint = New System.Windows.Forms.MenuItem()
        Me.mDebugClearAllBreakpoint = New System.Windows.Forms.MenuItem()
        Me.MenuItem3 = New System.Windows.Forms.MenuItem()
        Me.mDebugLiveTrace = New System.Windows.Forms.MenuItem()
        Me.mDebugFollowPointer = New System.Windows.Forms.MenuItem()
        Me.mHelp = New System.Windows.Forms.MenuItem()
        Me.mHelpAbout = New System.Windows.Forms.MenuItem()
        Me.tcLanguages = New System.Windows.Forms.TabControl()
        Me.tpBF = New System.Windows.Forms.TabPage()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.tpBoF = New System.Windows.Forms.TabPage()
        Me.txtBoF = New System.Windows.Forms.TextBox()
        Me.tpVBasic = New System.Windows.Forms.TabPage()
        Me.txtBasic = New System.Windows.Forms.TextBox()
        Me.tpC = New System.Windows.Forms.TabPage()
        Me.txtC = New System.Windows.Forms.TextBox()
        Me.tpJS = New System.Windows.Forms.TabPage()
        Me.txtJS = New System.Windows.Forms.TextBox()
        Me.pSplit = New System.Windows.Forms.Panel()
        Me.sbInfo = New VBBrainFNET.VBBFCStatusBar()
        Me.sbpProgSize = New System.Windows.Forms.StatusBarPanel()
        Me.sbpStatus = New System.Windows.Forms.StatusBarPanel()
        Me.sbpCellSize = New System.Windows.Forms.StatusBarPanel()
        Me.sbpIPS = New System.Windows.Forms.StatusBarPanel()
        Me.sbpElapsed = New System.Windows.Forms.StatusBarPanel()
        Me.sbpTransStatus = New System.Windows.Forms.StatusBarPanel()
        Me.tcLanguages.SuspendLayout()
        Me.tpBF.SuspendLayout()
        Me.tpBoF.SuspendLayout()
        Me.tpVBasic.SuspendLayout()
        Me.tpC.SuspendLayout()
        Me.tpJS.SuspendLayout()
        CType(Me.sbpProgSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpCellSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpIPS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpElapsed, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sbpTransStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtOut
        '
        Me.txtOut.AcceptsReturn = True
        Me.txtOut.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOut.BackColor = System.Drawing.Color.Black
        Me.txtOut.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtOut.Font = New System.Drawing.Font("Consolas", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOut.ForeColor = System.Drawing.Color.Lime
        Me.txtOut.HideSelection = False
        Me.txtOut.Location = New System.Drawing.Point(0, 247)
        Me.txtOut.MaxLength = 0
        Me.txtOut.Multiline = True
        Me.txtOut.Name = "txtOut"
        Me.txtOut.ReadOnly = True
        Me.txtOut.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtOut.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtOut.Size = New System.Drawing.Size(655, 130)
        Me.txtOut.TabIndex = 1
        Me.txtOut.WordWrap = False
        '
        'mMenu
        '
        Me.mMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mFile, Me.mView, Me.mOptions, Me.mDebug, Me.mHelp})
        '
        'mFile
        '
        Me.mFile.Index = 0
        Me.mFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mNew, Me.MenuItem4, Me.mFileOpen, Me.MenuItem5, Me.mFileSave, Me.mFileSaveAs, Me.mSep, Me.mFileSaveBoF, Me.mFileSaveVB, Me.mFileSaveC, Me.mFileSaveJS, Me.MenuItem8, Me.mFileCompileEXE, Me.MenuItem13, Me.mFileExit})
        Me.mFile.Text = "File"
        '
        'mNew
        '
        Me.mNew.Index = 0
        Me.mNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN
        Me.mNew.Text = "New"
        '
        'MenuItem4
        '
        Me.MenuItem4.Index = 1
        Me.MenuItem4.Text = "-"
        '
        'mFileOpen
        '
        Me.mFileOpen.Index = 2
        Me.mFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO
        Me.mFileOpen.Text = "Open..."
        '
        'MenuItem5
        '
        Me.MenuItem5.Index = 3
        Me.MenuItem5.Text = "-"
        '
        'mFileSave
        '
        Me.mFileSave.Index = 4
        Me.mFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS
        Me.mFileSave.Text = "Save"
        '
        'mFileSaveAs
        '
        Me.mFileSaveAs.Index = 5
        Me.mFileSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS
        Me.mFileSaveAs.Text = "Save As..."
        '
        'mSep
        '
        Me.mSep.Index = 6
        Me.mSep.Text = "-"
        '
        'mFileSaveBoF
        '
        Me.mFileSaveBoF.Index = 7
        Me.mFileSaveBoF.Text = "Save Boolfuck Code"
        '
        'mFileSaveVB
        '
        Me.mFileSaveVB.Index = 8
        Me.mFileSaveVB.Text = "Save Visual Basic Code"
        '
        'mFileSaveC
        '
        Me.mFileSaveC.Index = 9
        Me.mFileSaveC.Text = "Save C Code"
        '
        'mFileSaveJS
        '
        Me.mFileSaveJS.Index = 10
        Me.mFileSaveJS.Text = "Save JavaScript Code"
        '
        'MenuItem8
        '
        Me.MenuItem8.Index = 11
        Me.MenuItem8.Text = "-"
        '
        'mFileCompileEXE
        '
        Me.mFileCompileEXE.Index = 12
        Me.mFileCompileEXE.Text = "Compile to EXE"
        '
        'MenuItem13
        '
        Me.MenuItem13.Index = 13
        Me.MenuItem13.Text = "-"
        '
        'mFileExit
        '
        Me.mFileExit.Index = 14
        Me.mFileExit.Text = "Exit"
        '
        'mView
        '
        Me.mView.Index = 1
        Me.mView.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mViewMemoryMap})
        Me.mView.Text = "View"
        '
        'mViewMemoryMap
        '
        Me.mViewMemoryMap.Index = 0
        Me.mViewMemoryMap.Shortcut = System.Windows.Forms.Shortcut.F2
        Me.mViewMemoryMap.Text = "Memory Map"
        '
        'mOptions
        '
        Me.mOptions.Index = 2
        Me.mOptions.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mOptionsCellSize, Me.mOptionsPrettify, Me.MenuItem2, Me.mOutTextSize})
        Me.mOptions.Text = "Options"
        '
        'mOptionsCellSize
        '
        Me.mOptionsCellSize.Index = 0
        Me.mOptionsCellSize.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mCellSize256, Me.mCellSize512})
        Me.mOptionsCellSize.Text = "Cell Size"
        '
        'mCellSize256
        '
        Me.mCellSize256.Checked = True
        Me.mCellSize256.Index = 0
        Me.mCellSize256.RadioCheck = True
        Me.mCellSize256.Text = "256"
        '
        'mCellSize512
        '
        Me.mCellSize512.Index = 1
        Me.mCellSize512.RadioCheck = True
        Me.mCellSize512.Text = "512"
        '
        'mOptionsPrettify
        '
        Me.mOptionsPrettify.Index = 1
        Me.mOptionsPrettify.Text = "Prettify"
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 2
        Me.MenuItem2.Text = "-"
        '
        'mOutTextSize
        '
        Me.mOutTextSize.Index = 3
        Me.mOutTextSize.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mOTSSmallest, Me.mOTSSmall, Me.mOTSNormal, Me.mOTSLarge, Me.mOTSLargest})
        Me.mOutTextSize.Text = "Output Text Size"
        '
        'mOTSSmallest
        '
        Me.mOTSSmallest.Index = 0
        Me.mOTSSmallest.Tag = "8"
        Me.mOTSSmallest.Text = "Smallest"
        '
        'mOTSSmall
        '
        Me.mOTSSmall.Index = 1
        Me.mOTSSmall.Tag = "9"
        Me.mOTSSmall.Text = "Small"
        '
        'mOTSNormal
        '
        Me.mOTSNormal.Index = 2
        Me.mOTSNormal.Tag = "10"
        Me.mOTSNormal.Text = "Normal"
        '
        'mOTSLarge
        '
        Me.mOTSLarge.Index = 3
        Me.mOTSLarge.Tag = "11"
        Me.mOTSLarge.Text = "Large"
        '
        'mOTSLargest
        '
        Me.mOTSLargest.Index = 4
        Me.mOTSLargest.Tag = "12"
        Me.mOTSLargest.Text = "Largest"
        '
        'mDebug
        '
        Me.mDebug.Index = 3
        Me.mDebug.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mDebugRun, Me.mDebugRun2Cursor, Me.MenuItem9, Me.mDebugBreak, Me.MenuItem10, Me.mDebugStepInto, Me.mDebugStepOver, Me.mDebugStepOut, Me.MenuItem11, Me.mDebugStop, Me.MenuItem1, Me.mDebugToggleBreakpoint, Me.mDebugClearAllBreakpoint, Me.MenuItem3, Me.mDebugLiveTrace, Me.mDebugFollowPointer})
        Me.mDebug.Text = "Debug"
        '
        'mDebugRun
        '
        Me.mDebugRun.Index = 0
        Me.mDebugRun.Shortcut = System.Windows.Forms.Shortcut.F5
        Me.mDebugRun.Text = "Run"
        '
        'mDebugRun2Cursor
        '
        Me.mDebugRun2Cursor.Index = 1
        Me.mDebugRun2Cursor.Shortcut = System.Windows.Forms.Shortcut.ShiftF5
        Me.mDebugRun2Cursor.Text = "Run to Cursor"
        '
        'MenuItem9
        '
        Me.MenuItem9.Index = 2
        Me.MenuItem9.Text = "-"
        '
        'mDebugBreak
        '
        Me.mDebugBreak.Index = 3
        Me.mDebugBreak.Shortcut = System.Windows.Forms.Shortcut.CtrlC
        Me.mDebugBreak.Text = "Break"
        '
        'MenuItem10
        '
        Me.MenuItem10.Index = 4
        Me.MenuItem10.Text = "-"
        '
        'mDebugStepInto
        '
        Me.mDebugStepInto.Index = 5
        Me.mDebugStepInto.Shortcut = System.Windows.Forms.Shortcut.F8
        Me.mDebugStepInto.Text = "Step Into"
        '
        'mDebugStepOver
        '
        Me.mDebugStepOver.Index = 6
        Me.mDebugStepOver.Shortcut = System.Windows.Forms.Shortcut.ShiftF8
        Me.mDebugStepOver.Text = "Step Over"
        '
        'mDebugStepOut
        '
        Me.mDebugStepOut.Index = 7
        Me.mDebugStepOut.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftF8
        Me.mDebugStepOut.Text = "Step Out"
        '
        'MenuItem11
        '
        Me.MenuItem11.Index = 8
        Me.MenuItem11.Text = "-"
        '
        'mDebugStop
        '
        Me.mDebugStop.Index = 9
        Me.mDebugStop.Text = "Stop"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 10
        Me.MenuItem1.Text = "-"
        '
        'mDebugToggleBreakpoint
        '
        Me.mDebugToggleBreakpoint.Index = 11
        Me.mDebugToggleBreakpoint.Shortcut = System.Windows.Forms.Shortcut.F9
        Me.mDebugToggleBreakpoint.Text = "Toggle Breakpoint"
        '
        'mDebugClearAllBreakpoint
        '
        Me.mDebugClearAllBreakpoint.Index = 12
        Me.mDebugClearAllBreakpoint.Text = "Clear All Breakpoints"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 13
        Me.MenuItem3.Text = "-"
        '
        'mDebugLiveTrace
        '
        Me.mDebugLiveTrace.Index = 14
        Me.mDebugLiveTrace.Text = "Live Trace"
        '
        'mDebugFollowPointer
        '
        Me.mDebugFollowPointer.Checked = True
        Me.mDebugFollowPointer.Index = 15
        Me.mDebugFollowPointer.Text = "Follow Pointer"
        '
        'mHelp
        '
        Me.mHelp.Index = 4
        Me.mHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mHelpAbout})
        Me.mHelp.Text = "Help"
        '
        'mHelpAbout
        '
        Me.mHelpAbout.Index = 0
        Me.mHelpAbout.Text = "About..."
        '
        'tcLanguages
        '
        Me.tcLanguages.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tcLanguages.Controls.Add(Me.tpBF)
        Me.tcLanguages.Controls.Add(Me.tpBoF)
        Me.tcLanguages.Controls.Add(Me.tpVBasic)
        Me.tcLanguages.Controls.Add(Me.tpC)
        Me.tcLanguages.Controls.Add(Me.tpJS)
        Me.tcLanguages.Location = New System.Drawing.Point(0, 12)
        Me.tcLanguages.Name = "tcLanguages"
        Me.tcLanguages.SelectedIndex = 0
        Me.tcLanguages.Size = New System.Drawing.Size(655, 222)
        Me.tcLanguages.TabIndex = 6
        '
        'tpBF
        '
        Me.tpBF.Controls.Add(Me.txtCode)
        Me.tpBF.Location = New System.Drawing.Point(4, 22)
        Me.tpBF.Name = "tpBF"
        Me.tpBF.Size = New System.Drawing.Size(647, 196)
        Me.tpBF.TabIndex = 0
        Me.tpBF.Text = "Brainfuck"
        '
        'txtCode
        '
        Me.txtCode.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCode.Location = New System.Drawing.Point(5, 5)
        Me.txtCode.Multiline = True
        Me.txtCode.Name = "txtCode"
        Me.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtCode.Size = New System.Drawing.Size(637, 185)
        Me.txtCode.TabIndex = 0
        '
        'tpBoF
        '
        Me.tpBoF.BackColor = System.Drawing.SystemColors.Control
        Me.tpBoF.Controls.Add(Me.txtBoF)
        Me.tpBoF.Location = New System.Drawing.Point(4, 22)
        Me.tpBoF.Name = "tpBoF"
        Me.tpBoF.Padding = New System.Windows.Forms.Padding(3)
        Me.tpBoF.Size = New System.Drawing.Size(647, 196)
        Me.tpBoF.TabIndex = 4
        Me.tpBoF.Text = "Boolfuck"
        '
        'txtBoF
        '
        Me.txtBoF.AcceptsReturn = True
        Me.txtBoF.AcceptsTab = True
        Me.txtBoF.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBoF.BackColor = System.Drawing.SystemColors.Window
        Me.txtBoF.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtBoF.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBoF.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtBoF.HideSelection = False
        Me.txtBoF.Location = New System.Drawing.Point(5, 5)
        Me.txtBoF.MaxLength = 0
        Me.txtBoF.Multiline = True
        Me.txtBoF.Name = "txtBoF"
        Me.txtBoF.ReadOnly = True
        Me.txtBoF.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtBoF.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtBoF.Size = New System.Drawing.Size(637, 185)
        Me.txtBoF.TabIndex = 10
        Me.txtBoF.WordWrap = False
        '
        'tpVBasic
        '
        Me.tpVBasic.Controls.Add(Me.txtBasic)
        Me.tpVBasic.Location = New System.Drawing.Point(4, 22)
        Me.tpVBasic.Name = "tpVBasic"
        Me.tpVBasic.Size = New System.Drawing.Size(647, 196)
        Me.tpVBasic.TabIndex = 1
        Me.tpVBasic.Text = "Visual Basic"
        '
        'txtBasic
        '
        Me.txtBasic.AcceptsReturn = True
        Me.txtBasic.AcceptsTab = True
        Me.txtBasic.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBasic.BackColor = System.Drawing.SystemColors.Window
        Me.txtBasic.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtBasic.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBasic.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtBasic.HideSelection = False
        Me.txtBasic.Location = New System.Drawing.Point(5, 5)
        Me.txtBasic.MaxLength = 0
        Me.txtBasic.Multiline = True
        Me.txtBasic.Name = "txtBasic"
        Me.txtBasic.ReadOnly = True
        Me.txtBasic.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtBasic.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtBasic.Size = New System.Drawing.Size(637, 185)
        Me.txtBasic.TabIndex = 8
        Me.txtBasic.WordWrap = False
        '
        'tpC
        '
        Me.tpC.Controls.Add(Me.txtC)
        Me.tpC.Location = New System.Drawing.Point(4, 22)
        Me.tpC.Name = "tpC"
        Me.tpC.Size = New System.Drawing.Size(647, 196)
        Me.tpC.TabIndex = 2
        Me.tpC.Text = "C"
        '
        'txtC
        '
        Me.txtC.AcceptsReturn = True
        Me.txtC.AcceptsTab = True
        Me.txtC.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtC.BackColor = System.Drawing.SystemColors.Window
        Me.txtC.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtC.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtC.HideSelection = False
        Me.txtC.Location = New System.Drawing.Point(5, 5)
        Me.txtC.MaxLength = 0
        Me.txtC.Multiline = True
        Me.txtC.Name = "txtC"
        Me.txtC.ReadOnly = True
        Me.txtC.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtC.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtC.Size = New System.Drawing.Size(637, 185)
        Me.txtC.TabIndex = 8
        Me.txtC.WordWrap = False
        '
        'tpJS
        '
        Me.tpJS.Controls.Add(Me.txtJS)
        Me.tpJS.Location = New System.Drawing.Point(4, 22)
        Me.tpJS.Name = "tpJS"
        Me.tpJS.Size = New System.Drawing.Size(647, 196)
        Me.tpJS.TabIndex = 3
        Me.tpJS.Text = "JavaScript"
        '
        'txtJS
        '
        Me.txtJS.AcceptsReturn = True
        Me.txtJS.AcceptsTab = True
        Me.txtJS.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtJS.BackColor = System.Drawing.SystemColors.Window
        Me.txtJS.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtJS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtJS.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtJS.HideSelection = False
        Me.txtJS.Location = New System.Drawing.Point(5, 5)
        Me.txtJS.MaxLength = 0
        Me.txtJS.Multiline = True
        Me.txtJS.Name = "txtJS"
        Me.txtJS.ReadOnly = True
        Me.txtJS.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtJS.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtJS.Size = New System.Drawing.Size(637, 185)
        Me.txtJS.TabIndex = 9
        Me.txtJS.WordWrap = False
        '
        'pSplit
        '
        Me.pSplit.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pSplit.Cursor = System.Windows.Forms.Cursors.NoMoveVert
        Me.pSplit.Location = New System.Drawing.Point(0, 224)
        Me.pSplit.Name = "pSplit"
        Me.pSplit.Size = New System.Drawing.Size(655, 7)
        Me.pSplit.TabIndex = 7
        '
        'sbInfo
        '
        Me.sbInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.sbInfo.Location = New System.Drawing.Point(0, 376)
        Me.sbInfo.Name = "sbInfo"
        Me.sbInfo.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.sbpProgSize, Me.sbpStatus, Me.sbpCellSize, Me.sbpIPS, Me.sbpElapsed, Me.sbpTransStatus})
        Me.sbInfo.ProgramSize = 0
        Me.sbInfo.Progress = -1
        Me.sbInfo.ShowPanels = True
        Me.sbInfo.Size = New System.Drawing.Size(656, 20)
        Me.sbInfo.TabIndex = 8
        '
        'sbpProgSize
        '
        Me.sbpProgSize.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpProgSize.Name = "sbpProgSize"
        Me.sbpProgSize.Text = "Size: 0 bytes"
        Me.sbpProgSize.Width = 79
        '
        'sbpStatus
        '
        Me.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpStatus.MinWidth = 100
        Me.sbpStatus.Name = "sbpStatus"
        '
        'sbpCellSize
        '
        Me.sbpCellSize.Name = "sbpCellSize"
        '
        'sbpIPS
        '
        Me.sbpIPS.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpIPS.Name = "sbpIPS"
        Me.sbpIPS.Text = "0 ips"
        Me.sbpIPS.Width = 38
        '
        'sbpElapsed
        '
        Me.sbpElapsed.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.sbpElapsed.Name = "sbpElapsed"
        Me.sbpElapsed.Text = "0h 0m 0s 0ms"
        Me.sbpElapsed.Width = 85
        '
        'sbpTransStatus
        '
        Me.sbpTransStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
        Me.sbpTransStatus.Name = "sbpTransStatus"
        Me.sbpTransStatus.Width = 237
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(656, 396)
        Me.Controls.Add(Me.sbInfo)
        Me.Controls.Add(Me.txtOut)
        Me.Controls.Add(Me.pSplit)
        Me.Controls.Add(Me.tcLanguages)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Location = New System.Drawing.Point(338, 230)
        Me.Menu = Me.mMenu
        Me.Name = "frmMain"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "VBBrainFNET"
        Me.tcLanguages.ResumeLayout(False)
        Me.tpBF.ResumeLayout(False)
        Me.tpBF.PerformLayout()
        Me.tpBoF.ResumeLayout(False)
        Me.tpBoF.PerformLayout()
        Me.tpVBasic.ResumeLayout(False)
        Me.tpVBasic.PerformLayout()
        Me.tpC.ResumeLayout(False)
        Me.tpC.PerformLayout()
        Me.tpJS.ResumeLayout(False)
        Me.tpJS.PerformLayout()
        CType(Me.sbpProgSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpCellSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpIPS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpElapsed, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sbpTransStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region
End Class