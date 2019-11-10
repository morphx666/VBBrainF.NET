Option Strict Off
Option Explicit On
Friend Class FormInput
    Inherits System.Windows.Forms.Form

    Private cancelBubbling As Boolean

#Region "Windows Form Designer generated code "
	Public Sub New()
		MyBase.New()
        'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents TextBoxASCII As System.Windows.Forms.TextBox
	Public WithEvents TextBoxChar As System.Windows.Forms.TextBox
    Public WithEvents ButtonCont As System.Windows.Forms.Button
    'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.
	'Do not modify it using the code editor.
    Friend WithEvents RadioButtonChar As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonASCII As System.Windows.Forms.RadioButton
    Public WithEvents ButtonDebug As System.Windows.Forms.Button
    Public WithEvents ButtonStop As System.Windows.Forms.Button
    Public WithEvents TextBoxBuffer As System.Windows.Forms.TextBox
    Friend WithEvents RadioButtonBuffer As System.Windows.Forms.RadioButton
    Friend WithEvents ButtonLf As System.Windows.Forms.Button
    Friend WithEvents ButtonCr As System.Windows.Forms.Button
    Friend WithEvents ButtonNull As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormInput))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TextBoxASCII = New System.Windows.Forms.TextBox()
        Me.TextBoxChar = New System.Windows.Forms.TextBox()
        Me.ButtonDebug = New System.Windows.Forms.Button()
        Me.ButtonStop = New System.Windows.Forms.Button()
        Me.ButtonCont = New System.Windows.Forms.Button()
        Me.RadioButtonChar = New System.Windows.Forms.RadioButton()
        Me.RadioButtonASCII = New System.Windows.Forms.RadioButton()
        Me.TextBoxBuffer = New System.Windows.Forms.TextBox()
        Me.RadioButtonBuffer = New System.Windows.Forms.RadioButton()
        Me.ButtonLf = New System.Windows.Forms.Button()
        Me.ButtonCr = New System.Windows.Forms.Button()
        Me.ButtonNull = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'TextBoxASCII
        '
        Me.TextBoxASCII.AcceptsReturn = True
        Me.TextBoxASCII.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxASCII.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TextBoxASCII.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxASCII.Location = New System.Drawing.Point(86, 45)
        Me.TextBoxASCII.MaxLength = 0
        Me.TextBoxASCII.Name = "TextBoxASCII"
        Me.TextBoxASCII.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.TextBoxASCII.Size = New System.Drawing.Size(40, 23)
        Me.TextBoxASCII.TabIndex = 1
        Me.TextBoxASCII.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBoxChar
        '
        Me.TextBoxChar.AcceptsReturn = True
        Me.TextBoxChar.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxChar.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TextBoxChar.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxChar.Location = New System.Drawing.Point(86, 15)
        Me.TextBoxChar.MaxLength = 0
        Me.TextBoxChar.Name = "TextBoxChar"
        Me.TextBoxChar.ReadOnly = True
        Me.TextBoxChar.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.TextBoxChar.Size = New System.Drawing.Size(23, 23)
        Me.TextBoxChar.TabIndex = 0
        Me.TextBoxChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ButtonDebug
        '
        Me.ButtonDebug.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonDebug.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonDebug.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonDebug.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonDebug.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonDebug.Location = New System.Drawing.Point(160, 103)
        Me.ButtonDebug.Name = "ButtonDebug"
        Me.ButtonDebug.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonDebug.Size = New System.Drawing.Size(68, 37)
        Me.ButtonDebug.TabIndex = 7
        Me.ButtonDebug.Text = "Debug (F8)"
        Me.ButtonDebug.UseVisualStyleBackColor = True
        '
        'ButtonStop
        '
        Me.ButtonStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonStop.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonStop.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonStop.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonStop.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonStop.Location = New System.Drawing.Point(234, 103)
        Me.ButtonStop.Name = "ButtonStop"
        Me.ButtonStop.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonStop.Size = New System.Drawing.Size(68, 37)
        Me.ButtonStop.TabIndex = 8
        Me.ButtonStop.Text = "Stop (CTRL+C)"
        Me.ButtonStop.UseVisualStyleBackColor = True
        '
        'ButtonCont
        '
        Me.ButtonCont.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCont.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonCont.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonCont.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonCont.ForeColor = System.Drawing.SystemColors.ControlText
        Me.ButtonCont.Location = New System.Drawing.Point(86, 103)
        Me.ButtonCont.Name = "ButtonCont"
        Me.ButtonCont.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ButtonCont.Size = New System.Drawing.Size(68, 37)
        Me.ButtonCont.TabIndex = 6
        Me.ButtonCont.Text = "Continue   (F5)"
        Me.ButtonCont.UseVisualStyleBackColor = True
        '
        'RadioButtonChar
        '
        Me.RadioButtonChar.Checked = True
        Me.RadioButtonChar.Location = New System.Drawing.Point(12, 17)
        Me.RadioButtonChar.Name = "RadioButtonChar"
        Me.RadioButtonChar.Size = New System.Drawing.Size(76, 19)
        Me.RadioButtonChar.TabIndex = 9
        Me.RadioButtonChar.TabStop = True
        Me.RadioButtonChar.Text = "Character"
        '
        'RadioButtonASCII
        '
        Me.RadioButtonASCII.Location = New System.Drawing.Point(12, 47)
        Me.RadioButtonASCII.Name = "RadioButtonASCII"
        Me.RadioButtonASCII.Size = New System.Drawing.Size(76, 19)
        Me.RadioButtonASCII.TabIndex = 10
        Me.RadioButtonASCII.Text = "ASCII"
        '
        'TextBoxBuffer
        '
        Me.TextBoxBuffer.AcceptsReturn = True
        Me.TextBoxBuffer.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxBuffer.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TextBoxBuffer.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TextBoxBuffer.Location = New System.Drawing.Point(86, 74)
        Me.TextBoxBuffer.MaxLength = 0
        Me.TextBoxBuffer.Name = "TextBoxBuffer"
        Me.TextBoxBuffer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.TextBoxBuffer.Size = New System.Drawing.Size(216, 23)
        Me.TextBoxBuffer.TabIndex = 5
        '
        'RadioButtonBuffer
        '
        Me.RadioButtonBuffer.Location = New System.Drawing.Point(12, 76)
        Me.RadioButtonBuffer.Name = "RadioButtonBuffer"
        Me.RadioButtonBuffer.Size = New System.Drawing.Size(76, 19)
        Me.RadioButtonBuffer.TabIndex = 11
        Me.RadioButtonBuffer.Text = "Buffer"
        '
        'ButtonLf
        '
        Me.ButtonLf.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonLf.Location = New System.Drawing.Point(132, 43)
        Me.ButtonLf.Name = "ButtonLf"
        Me.ButtonLf.Size = New System.Drawing.Size(52, 26)
        Me.ButtonLf.TabIndex = 2
        Me.ButtonLf.Text = "Lf/10"
        '
        'ButtonCr
        '
        Me.ButtonCr.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonCr.Location = New System.Drawing.Point(191, 43)
        Me.ButtonCr.Name = "ButtonCr"
        Me.ButtonCr.Size = New System.Drawing.Size(52, 26)
        Me.ButtonCr.TabIndex = 3
        Me.ButtonCr.Text = "Cr/13"
        '
        'ButtonNull
        '
        Me.ButtonNull.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonNull.Location = New System.Drawing.Point(250, 43)
        Me.ButtonNull.Name = "ButtonNull"
        Me.ButtonNull.Size = New System.Drawing.Size(52, 26)
        Me.ButtonNull.TabIndex = 4
        Me.ButtonNull.Text = "Null/0"
        '
        'FormInput
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(312, 150)
        Me.Controls.Add(Me.ButtonNull)
        Me.Controls.Add(Me.ButtonCr)
        Me.Controls.Add(Me.ButtonLf)
        Me.Controls.Add(Me.TextBoxBuffer)
        Me.Controls.Add(Me.RadioButtonBuffer)
        Me.Controls.Add(Me.TextBoxASCII)
        Me.Controls.Add(Me.TextBoxChar)
        Me.Controls.Add(Me.RadioButtonChar)
        Me.Controls.Add(Me.RadioButtonASCII)
        Me.Controls.Add(Me.ButtonDebug)
        Me.Controls.Add(Me.ButtonStop)
        Me.Controls.Add(Me.ButtonCont)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Location = New System.Drawing.Point(480, 485)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormInput"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Input"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region
    Private Sub ButtonCont_Click(sender As Object, e As EventArgs) Handles ButtonCont.Click
        DoContinue()
    End Sub

    Private Sub ButtonDebugBreak_Click(sender As Object, e As EventArgs) Handles ButtonDebug.Click
        DoDebug()
    End Sub

    Private Sub ButtonDebugStop_Click(sender As Object, e As EventArgs) Handles ButtonStop.Click
        DoStop()
    End Sub

    Private Sub FormInput_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)

        If RadioButtonChar.Checked Then TextBoxChar.Text = Chr(KeyAscii)
        If KeyAscii = 0 Then e.Handled = True
    End Sub

    Private Sub FormInput_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With inputChar
            .DoContinue = False
            .DoDebug = False
            .DoStop = False

            If .ASCII > 0 Then TextBoxASCII.Text = CStr(.ASCII)
            .ASCII = 0

            TextBoxBuffer.Text = .Buffer
        End With

        ButtonStop.Enabled = isRunning

        If isDebugging Then
            ButtonCont.Text = "Resume (F5)"
            ButtonDebug.Text = "Step (F8)"
        Else
            ButtonCont.Text = "Run" + vbCrLf + "(F5)"
            ButtonDebug.Text = "Break (F8)"
        End If
    End Sub

    Private Sub TextBoxASCII_TextChanged(sender As Object, e As EventArgs) Handles TextBoxASCII.TextChanged
        If Val(TextBoxASCII.Text) >= 0 And Val(TextBoxASCII.Text) <= 255 Then
            TextBoxChar.Text = Chr(Val(TextBoxASCII.Text))
        End If
    End Sub

    Private Sub TextBoxASCII_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxASCII.KeyDown
        Dim KeyCode As Short = e.KeyCode
        Dim Shift As Short = e.KeyData \ &H10000

        If Not (KeyCode >= Asc("0") And KeyCode <= CDbl("9") Or KeyCode = System.Windows.Forms.Keys.Left Or KeyCode = System.Windows.Forms.Keys.Right Or KeyCode = System.Windows.Forms.Keys.Delete) Then
            KeyCode = 0
            Exit Sub
        End If
    End Sub

    Private Sub TextBoxChar_TextChanged(sender As Object, e As EventArgs) Handles TextBoxChar.TextChanged
        If TextBoxChar.Text <> "" Then TextBoxASCII.Text = CStr(Asc(TextBoxChar.Text))
    End Sub

    Private Sub FormInput_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If TextBoxChar.Text <> "" Then
            inputChar.ASCII = Asc(TextBoxChar.Text)
        End If
        e.Cancel = False
    End Sub

    Private Sub DoContinue()
        If RadioButtonBuffer.Checked Then
            inputChar.ASCII = Asc(inputChar.Buffer.Substring(0, 1))
            inputChar.Buffer = inputChar.Buffer.Substring(1)
        End If
        inputChar.DoContinue = True
        Me.Close()
    End Sub

    Private Sub DoDebug()
        inputChar.DoDebug = True
        Me.Close()
    End Sub

    Private Sub DoStop()
        inputChar.DoStop = True
        Me.Close()
    End Sub

    Private Sub FormInput_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        Select Case e.KeyCode
            Case Keys.F5 : DoContinue()
            Case Keys.F8 : DoDebug()
            Case Keys.C : If e.Control Then DoStop()
        End Select
    End Sub

    Private Sub ButtonLf_Click(sender As Object, e As EventArgs) Handles ButtonLf.Click
        TextBoxASCII.Focus()
        TextBoxASCII.Text = "10"
    End Sub

    Private Sub ButtonCr_Click(sender As Object, e As EventArgs) Handles ButtonCr.Click
        TextBoxASCII.Focus()
        TextBoxASCII.Text = "13"
    End Sub

    Private Sub ButtonNull_Click(sender As Object, e As EventArgs) Handles ButtonNull.Click
        TextBoxASCII.Focus()
        TextBoxASCII.Text = "0"
    End Sub

    Private Sub TextBoxBuffer_TextChanged(sender As Object, e As EventArgs) Handles TextBoxBuffer.TextChanged
        inputChar.Buffer = TextBoxBuffer.Text
    End Sub

    Private Sub RadioButtonBuffer_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonBuffer.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            TextBoxBuffer.Focus()
        End If
    End Sub

    Private Sub RadioButtonChar_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonChar.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            TextBoxChar.Focus()
        End If
    End Sub

    Private Sub RadioButtonASCII_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonASCII.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            TextBoxASCII.Focus()
        End If
    End Sub

    Private Sub TextBoxChar_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBoxChar.Enter
        cancelBubbling = True
        RadioButtonChar.Checked = True
    End Sub

    Private Sub TextBoxASCII_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBoxASCII.Enter
        cancelBubbling = True
        RadioButtonASCII.Checked = True
    End Sub

    Private Sub TextBoxBuffer_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBoxBuffer.Enter
        cancelBubbling = True
        RadioButtonBuffer.Checked = True
    End Sub
End Class