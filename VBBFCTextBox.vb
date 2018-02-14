Imports System.Threading

Public Class VBBFCTextBox
    Inherits System.Windows.Forms.PictureBox

    Private cursorThread As Thread
    Private cursorEvent As AutoResetEvent

#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Me.TabStop = True
        Me.Cursor = Cursors.IBeam

        cursorEvent = New AutoResetEvent(False)
        cursorThread = New Thread(AddressOf renderCursorThread)
        cursorThread.Start()
    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
            cursorThread.Abort()
            cursorThread = Nothing
            cursorEvent = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#End Region

    Public Shadows Event TextChanged()

    Private Class sText
        Private mLines() As String
        Private mFont()() As Font
        Private mForeColor()() As Color
        Private mBackColor()() As Color
        Private mCustomStyle()() As Boolean
        Private mCharPos() As Point

        Private mDefFont As Font
        Private mDefForeColor As Color
        Private mDefBackColor As Color

        Private mText As String = ""

        Friend Structure RowCol
            Dim Row As Integer
            Dim Col As Integer
        End Structure

        Public Property charPos(ByVal pos As Integer) As Point
            Get
                Try
                    Return mCharPos(pos)
                Catch
                End Try
            End Get
            Set(ByVal Value As Point)
                mCharPos(pos) = Value
            End Set
        End Property

        Public ReadOnly Property Lines() As String()
            Get
                Return mLines
            End Get
        End Property

        Public Property Text() As String
            Get
                Return mText
            End Get
            Set(ByVal Value As String)
                ResetArrays()
                Value = Replace(Value, Chr(0), "")
                If Value <> "" Then AppendText(Value, 0)
            End Set
        End Property

        Friend Function RowCol2Pos(ByVal rc As RowCol) As Integer
            Return RowCol2Pos(rc.Row, rc.Col)
        End Function

        Friend Function RowCol2Pos(ByVal row As Integer, ByVal col As Integer) As Integer
            Dim pr As Integer = LengthToRow(row - 1)
            If mLines(row) Is Nothing Then
                col = 0
            Else
                If mLines(row).Length < col Then col = mLines(row).Length
            End If
            Return pr + col
        End Function

        Friend Function Pos2RowCol(ByVal pos As Integer) As RowCol
            Dim l As Integer = 0
            Dim rc As RowCol

            Try
                For rc.Row = 0 To mLines.Length
                    If l + mLines(rc.Row).Length + 2 > pos Then
                        rc.Col = pos - l
                        Exit For
                    End If
                    l = l + mLines(rc.Row).Length + 2
                Next rc.Row
            Catch
            End Try

            Return rc
        End Function

        Private Sub adjustRowBuffer(ByVal len As Integer)
            Dim plen As Integer = mLines.Length
            ReDim Preserve mLines(len)
            ReDim Preserve mFont(len)
            ReDim Preserve mForeColor(len)
            ReDim Preserve mBackColor(len)
            ReDim Preserve mCustomStyle(len)

            For plen = plen To len
                mLines(plen) = ""
            Next plen
        End Sub

        Private Sub adjustColBuffer(ByVal row As Integer)
            Dim rl As Integer = mLines(row).Length
            ReDim Preserve mFont(row)(rl)
            ReDim Preserve mForeColor(row)(rl)
            ReDim Preserve mBackColor(row)(rl)
            ReDim Preserve mCustomStyle(row)(rl)
        End Sub

        Public Sub AppendText(ByVal txt As String, ByVal pos As Integer)
            Dim i As Integer
            Dim rc As RowCol = Pos2RowCol(pos)
            If txt = vbCr Then
                adjustRowBuffer(mLines.Length)
                For i = mLines.Length - 1 To rc.Row + 2 Step -1
                    mLines(i) = mLines(i - 1)
                    adjustColBuffer(i)
                Next i
                mLines(rc.Row + 1) = mLines(rc.Row).Substring(rc.Col)
                adjustColBuffer(rc.Row + 1)
                mLines(rc.Row) = mLines(rc.Row).Substring(0, rc.Col)
                adjustColBuffer(rc.Row)
            Else
                Dim txtL() As String = Split(txt, vbCrLf)
                If txtL.Length > 1 Then
                    adjustRowBuffer(mLines.Length - 1 + txtL.Length - 1)

                    For i = 0 To txtL.Length - 1
                        If i > 0 Then rc.Col = 0
                        AppendText(txtL(i), RowCol2Pos(rc.Row + i, rc.Col))
                    Next
                Else
                    If mLines(rc.Row) Is Nothing Then
                        mLines(rc.Row) = txt
                    Else
                        mLines(rc.Row) = mLines(rc.Row).Substring(0, rc.Col) + txt + mLines(rc.Row).Substring(rc.Col)
                    End If

                    adjustColBuffer(rc.Row)

                    For c As Integer = rc.Col To rc.Col + txt.Length
                        mFont(rc.Row)(c) = mDefFont
                        mForeColor(rc.Row)(c) = mDefForeColor
                        mBackColor(rc.Row)(c) = mDefBackColor
                        mCustomStyle(rc.Row)(c) = False
                    Next c
                End If
            End If

            mText = Join(mLines, vbCrLf) & ""
            ReDim Preserve mCharPos(mText.Length)
        End Sub

        Public ReadOnly Property Length() As Integer
            Get
                Return Text.Length
            End Get
        End Property

        Friend Function LengthToRow(ByVal row As Integer, Optional ByVal IncludeCrLf As Boolean = True) As Integer
            Dim l As Integer
            For row = row To 0 Step -1
                l += mLines(row).Length + IIf(IncludeCrLf, 2, 0)
            Next row
            Return l
        End Function

        Public Sub SelStyle(ByVal SelFont As Font, ByVal selForeColor As Color, ByVal selBackColor As Color, ByVal selStart As Integer, ByVal selLength As Integer)
            Dim rc As RowCol
            For i As Integer = selStart To selStart + selLength - 1
                rc = Pos2RowCol(i)
                mFont(rc.Row)(rc.Col) = SelFont
                mForeColor(rc.Row)(rc.Col) = selForeColor
                mBackColor(rc.Row)(rc.Col) = selBackColor
                mCustomStyle(rc.Row)(rc.Col) = Not (SelFont Is mDefFont And eqColor(selForeColor, mDefForeColor) And eqColor(selBackColor, mDefBackColor))
            Next i
        End Sub

        Private Function eqColor(ByVal c1 As Color, ByVal c2 As Color) As Boolean
            eqColor = (c1.A = c2.A) And _
                        (c1.R = c2.R) And _
                        (c1.G = c2.G) And _
                        (c1.B = c2.B)
        End Function

        Public Sub SelDefStyle(ByVal SelFont As Font, ByVal selForeColor As Color, ByVal selBackColor As Color)
            mDefForeColor = selForeColor
            mDefBackColor = selBackColor
            mDefFont = SelFont
        End Sub

        Public ReadOnly Property charAt(ByVal pos As Integer) As String
            Get
                Dim rc As RowCol = Pos2RowCol(pos)
                Return Chr(mLines(rc.Row).Substring(rc.Col, 1))
            End Get
        End Property

        Public ReadOnly Property fontAt(ByVal row As Integer, ByVal col As Integer) As Font
            Get
                If mCustomStyle(row)(col) Then
                    Return mFont(row)(col)
                Else
                    Return mDefFont
                End If
            End Get
        End Property

        Public ReadOnly Property fontAt(ByVal pos As Integer) As Font
            Get
                Dim rc As RowCol = Pos2RowCol(pos)
                Return fontAt(rc.Row, rc.Col)
            End Get
        End Property

        Public ReadOnly Property foreColorAt(ByVal row As Integer, ByVal col As Integer) As Color
            Get
                Try
                    If mCustomStyle(row)(col) Then
                        Return mForeColor(row)(col)
                    Else
                        Return mDefForeColor
                    End If
                Catch
                    Return mDefForeColor
                End Try
            End Get
        End Property

        Public ReadOnly Property foreColorAt(ByVal pos As Integer) As Color
            Get
                Dim rc As RowCol = Pos2RowCol(pos)
                Return foreColorAt(rc.Row, rc.Col)
            End Get
        End Property

        Public ReadOnly Property backColorAt(ByVal row As Integer, ByVal col As Integer) As Color
            Get
                If mCustomStyle(row)(col) Then
                    Return mBackColor(row)(col)
                Else
                    Return mDefBackColor
                End If
            End Get
        End Property

        Public ReadOnly Property backColorAt(ByVal pos As Integer) As Color
            Get
                Dim rc As RowCol = Pos2RowCol(pos)
                Return backColorAt(rc.Row, rc.Col)
            End Get
        End Property

        Public Sub New()
            ResetArrays()
        End Sub

        Protected Overrides Sub Finalize()
            mLines = Nothing
            mFont = Nothing
            mForeColor = Nothing
            mBackColor = Nothing
            mCustomStyle = Nothing

            MyBase.Finalize()
        End Sub

        Private Sub ResetArrays()
            ReDim mLines(0) : mLines(0) = ""
            ReDim mFont(0)
            ReDim mCharPos(0)
            ReDim mForeColor(0)
            ReDim mBackColor(0)
            ReDim mCustomStyle(0)
        End Sub
    End Class

    Private mText As New sText
    Private mSelectionStart As Integer
    Private mSelectionEnd As Integer
    Private mReadOnly As Boolean
    Private handleKey As Boolean

    Private HasFocus As Boolean = False

    Private Sub renderCursorThread()
        Static cursorVisible As Boolean = True
        While True
            cursorEvent.WaitOne(IIf(cursorVisible, 500, 250), False)
            renderCursor(cursorVisible)
            cursorVisible = Not cursorVisible
        End While
    End Sub

    Private Sub renderCursor(ByVal cursorVisible As Boolean)
        If HasFocus Then
            Dim g As Graphics = Me.CreateGraphics
            Dim ss As Integer = SelectionEnd
            Dim row As Integer = mText.Pos2RowCol(ss).Row
            Dim pt2 As Point = New Point(mText.charPos(ss).X, mText.charPos(ss).Y + g.MeasureString("X", Font).Height - 2)
            Static prevRow As Integer

            If prevRow <> row Then
                renderBuffer(prevRow)
                prevRow = row
            End If
            If cursorVisible Then
                g.DrawLine(New Pen(Color.Black), mText.charPos(ss), pt2)
            Else
                renderBuffer(row)
            End If
            g.Dispose()
        End If
    End Sub

    Public Property [ReadOnly]() As Boolean
        Get
            Return mReadOnly
        End Get
        Set(ByVal Value As Boolean)
            mReadOnly = Value
        End Set
    End Property

    Public Property SelectionStart() As Integer
        Get
            Return mSelectionStart
        End Get
        Set(ByVal Value As Integer)
            If Value > mText.Length Then Value = mText.Length
            If Value < 0 Then Value = 0
            mSelectionStart = Value
            mSelectionEnd = mSelectionStart
            renderBuffer(mText.Pos2RowCol(mSelectionStart).Row)
        End Set
    End Property

    Public Property SelectionEnd() As Integer
        Get
            Return mSelectionEnd
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            If Value > mText.Length Then Value = mText.Length
            mSelectionEnd = Value
            Me.Refresh()
        End Set
    End Property

    Public Property SelectionLength() As Integer
        Get
            Return Math.Abs(mSelectionEnd - SelectionStart)
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            If Value + SelectionStart > mText.Length Then Value = mText.Length - SelectionStart
            mSelectionEnd = Value + SelectionStart
        End Set
    End Property

    Public Overrides Property Text() As String
        Get
            Return mText.Text
        End Get
        Set(ByVal Value As String)
            If mText.Text <> Value Then
                mText.Text = Value
                Me.Refresh()
                RaiseEvent TextChanged()
            End If
        End Set
    End Property

    Public Sub ScrollToCaret()

    End Sub

    Public Sub SelectionStyle(ByVal SelFont As Font, ByVal SelForeColor As Color, ByVal SelBackColor As Color)
        mText.SelStyle(SelFont, SelForeColor, SelBackColor, SelectionStart, SelectionLength)
    End Sub

    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
        For row As Integer = 0 To mText.Lines.Length - 1
            renderBuffer(row, pe.Graphics)
        Next row
    End Sub

    Private Sub renderBuffer(ByVal row As Integer, Optional ByRef g As Graphics = Nothing)
        Dim g2 As Graphics
        Dim b As Bitmap
        Dim x As Integer = 0
        Dim c As String = ""
        Static s As String
        Static lineHeight As Integer
        Dim col As Integer
        Dim doDispose As Boolean
        Dim drawSel As Boolean
        Dim cs As SizeF

        If row > mText.Lines.Length - 1 Then Exit Sub

        Dim ss As sText.RowCol = IIf(SelectionStart <= SelectionEnd, mText.Pos2RowCol(SelectionStart), mText.Pos2RowCol(SelectionEnd))
        Dim se As sText.RowCol = IIf(SelectionEnd >= SelectionStart, mText.Pos2RowCol(SelectionEnd), mText.Pos2RowCol(SelectionStart))

        If g Is Nothing Then
            g = Me.CreateGraphics
            doDispose = True
        End If
        lineHeight = g.MeasureString("X", Font).Height
        If s <> mText.Lines(row) Then
            Static cHeight As Integer
            s = mText.Lines(row)
            For col = 0 To s.Length - 1
                cHeight = g.MeasureString(s.Substring(col, 1), mText.fontAt(row, col)).Height
                If cHeight > lineHeight Then lineHeight = cHeight
            Next col
        End If

        b = New Bitmap(Width, lineHeight)
        g2 = Graphics.FromImage(b)
        g2.Clear(Me.BackColor)

        Try
            If Not s Is Nothing Then
                For col = 0 To s.Length - 1
                    mText.charPos(mText.RowCol2Pos(row, col)) = New Point(x + 1, lineHeight * row + 1)
                    c = s.Substring(col, 1)
                    cs = g.MeasureString(c, mText.fontAt(row, col))
                    If SelectionLength > 0 Then
                        drawSel = False
                        If (row >= ss.Row And row <= se.Row) Then
                            If ss.Row = se.Row Then
                                If col >= ss.Col And col < se.Col Then drawSel = True
                            Else
                                If row = ss.Row Then
                                    If col >= ss.Col Then drawSel = True
                                End If
                                If row = se.Row Then
                                    If col < se.Col Then drawSel = True
                                End If
                                If row > ss.Row And row < se.Row Then
                                    drawSel = True
                                End If
                            End If
                        End If
                        If drawSel Then
                            g2.FillRectangle(New SolidBrush(Color.FromKnownColor(KnownColor.Highlight)), x + 1, 0, cs.Width - 2, cs.Height)
                        End If
                    End If
                    Select Case c
                        Case vbCr
                            Exit For
                        Case vbTab
                            x += cs.Width * 4
                        Case " "
                            x += cs.Width
                        Case Else
                            Dim cc As Color = IIf(drawSel, Color.FromKnownColor(KnownColor.HighlightText), mText.foreColorAt(row, col))

                            g2.DrawString(c, Font, New SolidBrush(cc), x, 2)
                            x += cs.Width - 4
                    End Select
                Next col
                mText.charPos(mText.RowCol2Pos(row, col)) = New Point(x + 1, lineHeight * row + 1)
            End If
        Catch
        End Try

        g.DrawImageUnscaled(b, 0, lineHeight * row)
        If doDispose Then g.Dispose()
    End Sub

    Protected Overrides Sub OnForeColorChanged(ByVal e As System.EventArgs)
        mText.SelDefStyle(Font, ForeColor, BackColor)
    End Sub

    Protected Overrides Sub OnBackColorChanged(ByVal e As System.EventArgs)
        mText.SelDefStyle(Font, ForeColor, BackColor)
    End Sub

    Protected Overrides Sub OnFontChanged(ByVal e As System.EventArgs)
        mText.SelDefStyle(Font, ForeColor, BackColor)
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Dim rc As sText.RowCol = mText.Pos2RowCol(SelectionStart)
        Dim bSelStart As Integer = SelectionStart

        Select Case e.KeyCode
            Case Keys.Left
                If Not e.Shift And SelectionEnd <> 0 Then SelectionStart = SelectionEnd
                Dim np As Integer = IIf(e.Shift, SelectionEnd, SelectionStart)
                np -= 1
                If np < 0 Then np += 1
                If e.Shift Then
                    SelectionEnd = np
                Else
                    SelectionStart = np
                End If
            Case Keys.Right
                If Not e.Shift And SelectionEnd <> 0 Then SelectionStart = SelectionEnd
                Dim np As Integer = IIf(e.Shift, SelectionEnd, SelectionStart)
                np += 1
                If np < mText.Length - 2 Then
                    If mText.Text.Substring(np - 1, 2) = vbCrLf Then
                        np += 1
                    End If
                End If
                If e.Shift Then
                    SelectionEnd = np
                Else
                    SelectionStart = np
                End If
            Case Keys.Down
                If rc.Row < mText.Lines.Length - 1 Then
                    rc.Row += 1
                    If e.Shift Then
                        SelectionEnd = mText.RowCol2Pos(rc)
                    Else
                        SelectionStart = mText.RowCol2Pos(rc)
                    End If
                End If
            Case Keys.Up
                If rc.Row > 0 Then
                    rc.Row -= 1
                    If e.Shift Then
                        SelectionEnd = mText.RowCol2Pos(rc)
                    Else
                        SelectionStart = mText.RowCol2Pos(rc)
                    End If
                End If
            Case Keys.Home
                rc.Col = 0
                If e.Control Then rc.Row = 0
                If e.Shift Then
                    SelectionEnd = mText.RowCol2Pos(rc)
                Else
                    SelectionStart = mText.RowCol2Pos(rc)
                End If
            Case Keys.End
                If e.Control Then
                    rc.Col = 0
                    rc.Row = mText.Lines.Length
                Else
                    rc.Col = mText.Lines(rc.Row).Length
                End If
                If e.Shift Then
                    SelectionEnd = mText.RowCol2Pos(rc)
                Else
                    SelectionStart = mText.RowCol2Pos(rc)
                End If
            Case Keys.Delete
                Dim n As Integer = 1
                Dim ss As Integer = SelectionStart
                Dim sl As Integer = SelectionLength
                If sl > 0 Then sl -= 1
                With mText
                    If ss < .Text.Length Then
                        If .Text.Substring(ss, 1) = vbCr Then n = 2
                        .Text = .Text.Substring(0, ss) + .Text.Substring(ss + sl + n)
                        SelectionStart = ss
                    End If
                End With
            Case Keys.Back
                If SelectionStart > 0 Then
                    renderCursor(False)
                    Dim n As Integer = 1
                    With mText
                        If .Text.Substring(SelectionStart - 1, 1) = vbLf Then
                            n = 2
                        End If
                        .Text = .Text.Substring(0, SelectionStart - n) + .Text.Substring(SelectionStart)
                        SelectionStart -= 1
                    End With
                End If
            Case Keys.Enter
                mText.AppendText(vbCr, SelectionStart)
                rc.Col = 0
                rc.Row += 1
                SelectionStart = mText.RowCol2Pos(rc)
                rc.Row -= 1
                RaiseEvent TextChanged()
            Case Else
                If e.KeyValue > 30 Then handleKey = True
        End Select
    End Sub

    Private Function cursor2pos(ByVal x As Integer, ByVal y As Integer) As Integer
        Dim d As Integer
        Dim dmin As Integer
        Dim posOffset As Integer
        Dim rc As sText.RowCol
        Dim col As Integer

        dmin = Integer.MaxValue
        For i As Integer = 0 To mText.Text.Length - 1
            d = Math.Abs(mText.charPos(i).Y - y)
            If d <= dmin Then
                dmin = d
                rc = mText.Pos2RowCol(i)
                posOffset = mText.RowCol2Pos(rc.Row, 0)
            End If
        Next i

        dmin = Integer.MaxValue
        For rc.Col = 0 To mText.Lines(rc.Row).Length - 1
            d = Math.Abs(mText.charPos(rc.Col + posOffset).X - x)
            If d <= dmin Then
                dmin = d
                col = rc.Col
            End If
        Next

        rc.Col = col + IIf(dmin > 5, 1, 0)
        Return mText.RowCol2Pos(rc)
    End Function

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.Focus()
        SelectionStart = cursor2pos(e.X, e.Y)
        renderCursor(True)
    End Sub

    Protected Overrides Sub OnKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If handleKey Then
            mText.AppendText(e.KeyChar, SelectionStart)
            SelectionStart += 1
            handleKey = False
            RaiseEvent TextChanged()
        End If
    End Sub

    Protected Overrides Sub OnGotFocus(ByVal e As System.EventArgs)
        HasFocus = True
    End Sub

    Protected Overrides Sub OnLostFocus(ByVal e As System.EventArgs)
        renderCursor(False)
        HasFocus = False
    End Sub
End Class
