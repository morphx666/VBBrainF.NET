Public Class FormMemory
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents vsbScrollBar As System.Windows.Forms.VScrollBar
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMemory))
        Me.vsbScrollBar = New System.Windows.Forms.VScrollBar
        Me.SuspendLayout()
        '
        'vsbScrollBar
        '
        Me.vsbScrollBar.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.vsbScrollBar.Dock = System.Windows.Forms.DockStyle.Right
        Me.vsbScrollBar.Location = New System.Drawing.Point(271, 0)
        Me.vsbScrollBar.Name = "vsbScrollBar"
        Me.vsbScrollBar.Size = New System.Drawing.Size(21, 270)
        Me.vsbScrollBar.TabIndex = 0
        '
        'frmMemory
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(8, 16)
        Me.ClientSize = New System.Drawing.Size(292, 270)
        Me.Controls.Add(Me.vsbScrollBar)
        Me.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "frmMemory"
        Me.Text = "Memory"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private sIndex As Integer
    Private lh As Single
    Private b As Brush = New SolidBrush(Color.Black)
    Private b2 As Brush = New SolidBrush(Color.Beige)
    Private colX(2) As Integer

    Private ctrlWidth As Integer
    Private ctrlHeight As Integer

    Private mouseIsOver As Boolean
    Private freezeSel As Boolean

    Private selIdx As Integer
    Private Const Space4 As String = "    "
    Private p As Pen
    Private p2 As Pen = New Pen(Color.LightGray)

    Private sf As StringFormat = New StringFormat(StringFormatFlags.MeasureTrailingSpaces Or StringFormatFlags.FitBlackBox)

    Private Sub frmMemory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Font = New Font("Consolas", 10, FontStyle.Regular)

        Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.DoubleBuffered = True
        Me.UpdateStyles()

        With vsbScrollBar
            .Minimum = 0
            .Maximum = maxMem
        End With

        lh = Me.CreateGraphics.MeasureString("8", Me.Font).Height
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        With e.Graphics
            .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
        End With
        RenderMemory(e.Graphics)
    End Sub

    Friend Sub RenderMemory(ByVal g As Graphics)
        Dim y As Single = 6
        Dim x As Single = 2
        Dim s As String = ""
        Dim h As Integer = 0

        p = New Pen(CType(IIf(isDebugging, Color.Blue, Color.Gray), Color))

        If followPointer And isRunning Then
            h = CInt(ctrlHeight / (lh + 6))
            sIndex = ptr
            sIndex = sIndex - h \ 2
            If sIndex < 0 Then sIndex = 0
            vsbScrollBar.Value = sIndex
        End If

        For i As Integer = sIndex To maxMem
            Select Case maxCellSize
                Case 255
                    s = padZeros(Hex(i)) + Space4 + _
                        padZeros(mem(i)) + Space4 + _
                        padZeros(Hex(mem(i))) + Space4 + _
                        Chr(mem(i))
                Case 511
                    s = padZeros(Hex(i)) + Space4 + _
                        padZeros(mem(i)) + Space4 + _
                        padZeros(Hex(mem(i))) + Space4 + _
                        ChrW(mem(i))
            End Select

            If i = ptr Then g.FillRectangle(b2, 0, y, ctrlWidth, lh + 4)
            If i = selIdx And mouseIsOver Then g.DrawRectangle(p, 0, y, ctrlWidth, lh + 4)

            g.DrawString(s, Me.Font, b, x, y + 3, sf)

            y += (lh + 6)
            If y >= ctrlHeight Then Exit For
        Next i

        g.DrawLine(p2, colX(0), 0, colX(0), Height)
        g.DrawLine(p2, colX(1), 0, colX(1), Height)
        g.DrawLine(p2, colX(2), 0, colX(2), Height)
        p.Dispose()
    End Sub

    Private Function padZeros(ByVal s As String) As String
        Return StrDup(4 - s.Length, "0") + s
    End Function

    Private Function padZeros(ByVal v As Integer) As String
        Dim s As String = CStr(v)
        Return Microsoft.VisualBasic.Strings.StrDup(4 - s.Length, "0") + s
    End Function

    Private Sub vsbScrollBar_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles vsbScrollBar.Scroll
        sIndex = vsbScrollBar.Value
        Me.Refresh()
    End Sub

    Private Sub frmMemory_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        If freezeSel Then
            Me.Refresh()
        Else
            If e.Y >= 6 And e.X < vsbScrollBar.Left Then
                selIdx = CInt((e.Y + 6) / (lh + 6) - 1 + sIndex)
                Me.Refresh()
                Me.Cursor = IIf(isRunning, Cursors.Hand, Cursors.Default)
            Else
                Me.Cursor = Cursors.Default
            End If
        End If
    End Sub

    Private Sub frmMemory_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseWheel
        If freezeSel Then Exit Sub
        sIndex -= e.Delta \ 10
        If sIndex < 0 Then
            sIndex = 0
        ElseIf sIndex > maxMem Then
            sIndex = maxMem
        End If
        vsbScrollBar.Value = sIndex
        Me.Refresh()
    End Sub

    Private Sub frmMemory_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            Dim g As Graphics = Me.CreateGraphics

            colX(0) = CInt(g.MeasureString(StrDup(4, "8 ") + "88", Me.Font, 0, sf).Width)
            colX(1) = CInt(g.MeasureString(StrDup(12, "8 ") + "88", Me.Font, 0, sf).Width)
            colX(2) = CInt(g.MeasureString(StrDup(20, "8 ") + "88", Me.Font, 0, sf).Width)

            Width = CInt(g.MeasureString(StrDup(32, "8 "), Me.Font).Width + vsbScrollBar.Width * 2)

            ctrlWidth = vsbScrollBar.Left - 2
            ctrlHeight = CInt(g.VisibleClipBounds.Height - (lh + 6))

            Me.MaximumSize = New Size(Width, Screen.PrimaryScreen.WorkingArea.Height)

            g.Dispose()
        Catch
        End Try
    End Sub

    Private Sub frmMemory_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.MouseEnter
        mouseIsOver = True
        Me.Refresh()
    End Sub

    Private Sub frmMemory_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.MouseLeave
        mouseIsOver = False
        Me.Refresh()
    End Sub

    Private Sub frmMemory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Click
        If isDebugging Then
            freezeSel = True
            vsbScrollBar.Enabled = False

            DoInputChar(Me, selIdx, mem(selIdx))

            vsbScrollBar.Enabled = True
            freezeSel = False

            Me.Refresh()
        End If
    End Sub
End Class
