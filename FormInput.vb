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
	Public WithEvents txtASCII As System.Windows.Forms.TextBox
	Public WithEvents txtChar As System.Windows.Forms.TextBox
    Public WithEvents cmdCont As System.Windows.Forms.Button
    'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.
	'Do not modify it using the code editor.
    Friend WithEvents rbChar As System.Windows.Forms.RadioButton
    Friend WithEvents rbASCII As System.Windows.Forms.RadioButton
    Public WithEvents cmdDebug As System.Windows.Forms.Button
    Public WithEvents cmdStop As System.Windows.Forms.Button
    Public WithEvents txtBuffer As System.Windows.Forms.TextBox
    Friend WithEvents rbBuffer As System.Windows.Forms.RadioButton
    Friend WithEvents btnLf As System.Windows.Forms.Button
    Friend WithEvents btnCr As System.Windows.Forms.Button
    Friend WithEvents btnNull As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormInput))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtASCII = New System.Windows.Forms.TextBox()
        Me.txtChar = New System.Windows.Forms.TextBox()
        Me.cmdDebug = New System.Windows.Forms.Button()
        Me.cmdStop = New System.Windows.Forms.Button()
        Me.cmdCont = New System.Windows.Forms.Button()
        Me.rbChar = New System.Windows.Forms.RadioButton()
        Me.rbASCII = New System.Windows.Forms.RadioButton()
        Me.txtBuffer = New System.Windows.Forms.TextBox()
        Me.rbBuffer = New System.Windows.Forms.RadioButton()
        Me.btnLf = New System.Windows.Forms.Button()
        Me.btnCr = New System.Windows.Forms.Button()
        Me.btnNull = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtASCII
        '
        Me.txtASCII.AcceptsReturn = True
        Me.txtASCII.BackColor = System.Drawing.SystemColors.Window
        Me.txtASCII.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtASCII.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtASCII.Location = New System.Drawing.Point(86, 45)
        Me.txtASCII.MaxLength = 0
        Me.txtASCII.Name = "txtASCII"
        Me.txtASCII.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtASCII.Size = New System.Drawing.Size(40, 23)
        Me.txtASCII.TabIndex = 1
        Me.txtASCII.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtChar
        '
        Me.txtChar.AcceptsReturn = True
        Me.txtChar.BackColor = System.Drawing.SystemColors.Window
        Me.txtChar.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtChar.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtChar.Location = New System.Drawing.Point(86, 15)
        Me.txtChar.MaxLength = 0
        Me.txtChar.Name = "txtChar"
        Me.txtChar.ReadOnly = True
        Me.txtChar.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtChar.Size = New System.Drawing.Size(23, 23)
        Me.txtChar.TabIndex = 0
        Me.txtChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cmdDebug
        '
        Me.cmdDebug.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDebug.BackColor = System.Drawing.SystemColors.Control
        Me.cmdDebug.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdDebug.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDebug.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdDebug.Location = New System.Drawing.Point(160, 103)
        Me.cmdDebug.Name = "cmdDebug"
        Me.cmdDebug.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdDebug.Size = New System.Drawing.Size(68, 37)
        Me.cmdDebug.TabIndex = 7
        Me.cmdDebug.Text = "Debug (F8)"
        Me.cmdDebug.UseVisualStyleBackColor = True
        '
        'cmdStop
        '
        Me.cmdStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStop.BackColor = System.Drawing.SystemColors.Control
        Me.cmdStop.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdStop.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdStop.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdStop.Location = New System.Drawing.Point(234, 103)
        Me.cmdStop.Name = "cmdStop"
        Me.cmdStop.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdStop.Size = New System.Drawing.Size(68, 37)
        Me.cmdStop.TabIndex = 8
        Me.cmdStop.Text = "Stop (CTRL+C)"
        Me.cmdStop.UseVisualStyleBackColor = True
        '
        'cmdCont
        '
        Me.cmdCont.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCont.BackColor = System.Drawing.SystemColors.Control
        Me.cmdCont.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCont.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCont.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCont.Location = New System.Drawing.Point(86, 103)
        Me.cmdCont.Name = "cmdCont"
        Me.cmdCont.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCont.Size = New System.Drawing.Size(68, 37)
        Me.cmdCont.TabIndex = 6
        Me.cmdCont.Text = "Continue   (F5)"
        Me.cmdCont.UseVisualStyleBackColor = True
        '
        'rbChar
        '
        Me.rbChar.Checked = True
        Me.rbChar.Location = New System.Drawing.Point(12, 17)
        Me.rbChar.Name = "rbChar"
        Me.rbChar.Size = New System.Drawing.Size(76, 19)
        Me.rbChar.TabIndex = 9
        Me.rbChar.TabStop = True
        Me.rbChar.Text = "Character"
        '
        'rbASCII
        '
        Me.rbASCII.Location = New System.Drawing.Point(12, 47)
        Me.rbASCII.Name = "rbASCII"
        Me.rbASCII.Size = New System.Drawing.Size(76, 19)
        Me.rbASCII.TabIndex = 10
        Me.rbASCII.Text = "ASCII"
        '
        'txtBuffer
        '
        Me.txtBuffer.AcceptsReturn = True
        Me.txtBuffer.BackColor = System.Drawing.SystemColors.Window
        Me.txtBuffer.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtBuffer.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtBuffer.Location = New System.Drawing.Point(86, 74)
        Me.txtBuffer.MaxLength = 0
        Me.txtBuffer.Name = "txtBuffer"
        Me.txtBuffer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtBuffer.Size = New System.Drawing.Size(216, 23)
        Me.txtBuffer.TabIndex = 5
        '
        'rbBuffer
        '
        Me.rbBuffer.Location = New System.Drawing.Point(12, 76)
        Me.rbBuffer.Name = "rbBuffer"
        Me.rbBuffer.Size = New System.Drawing.Size(76, 19)
        Me.rbBuffer.TabIndex = 11
        Me.rbBuffer.Text = "Buffer"
        '
        'btnLf
        '
        Me.btnLf.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLf.Location = New System.Drawing.Point(132, 43)
        Me.btnLf.Name = "btnLf"
        Me.btnLf.Size = New System.Drawing.Size(52, 26)
        Me.btnLf.TabIndex = 2
        Me.btnLf.Text = "Lf/10"
        '
        'btnCr
        '
        Me.btnCr.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCr.Location = New System.Drawing.Point(191, 43)
        Me.btnCr.Name = "btnCr"
        Me.btnCr.Size = New System.Drawing.Size(52, 26)
        Me.btnCr.TabIndex = 3
        Me.btnCr.Text = "Cr/13"
        '
        'btnNull
        '
        Me.btnNull.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNull.Location = New System.Drawing.Point(250, 43)
        Me.btnNull.Name = "btnNull"
        Me.btnNull.Size = New System.Drawing.Size(52, 26)
        Me.btnNull.TabIndex = 4
        Me.btnNull.Text = "Null/0"
        '
        'FormInput
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(312, 150)
        Me.Controls.Add(Me.btnNull)
        Me.Controls.Add(Me.btnCr)
        Me.Controls.Add(Me.btnLf)
        Me.Controls.Add(Me.txtBuffer)
        Me.Controls.Add(Me.rbBuffer)
        Me.Controls.Add(Me.txtASCII)
        Me.Controls.Add(Me.txtChar)
        Me.Controls.Add(Me.rbChar)
        Me.Controls.Add(Me.rbASCII)
        Me.Controls.Add(Me.cmdDebug)
        Me.Controls.Add(Me.cmdStop)
        Me.Controls.Add(Me.cmdCont)
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
    Private Sub cmdCont_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCont.Click
        DoContinue()
    End Sub

    Private Sub mDebugBreak_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdDebug.Click
        DoDebug()
    End Sub

    Private Sub mDebugStop_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdStop.Click
        DoStop()
    End Sub

    Private Sub frmInput_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)

        If rbChar.Checked Then txtChar.Text = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub frmInput_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        With inputChar
            .DoContinue = False
            .DoDebug = False
            .DoStop = False

            If .ASCII > 0 Then txtASCII.Text = CStr(.ASCII)
            .ASCII = 0

            txtBuffer.Text = .Buffer
        End With

        cmdStop.Enabled = isRunning

        If isDebugging Then
            cmdCont.Text = "Resume (F5)"
            cmdDebug.Text = "Step (F8)"
        Else
            cmdCont.Text = "Run" + vbCrLf + "(F5)"
            cmdDebug.Text = "Break (F8)"
        End If
    End Sub

    Private Sub txtASCII_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtASCII.TextChanged
        If Val(txtASCII.Text) >= 0 And Val(txtASCII.Text) <= 255 Then
            txtChar.Text = Chr(Val(txtASCII.Text))
        End If
    End Sub

    Private Sub txtASCII_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles txtASCII.KeyDown
        Dim KeyCode As Short = eventArgs.KeyCode
        Dim Shift As Short = eventArgs.KeyData \ &H10000

        If Not (KeyCode >= Asc("0") And KeyCode <= CDbl("9") Or KeyCode = System.Windows.Forms.Keys.Left Or KeyCode = System.Windows.Forms.Keys.Right Or KeyCode = System.Windows.Forms.Keys.Delete) Then
            KeyCode = 0
            Exit Sub
        End If
    End Sub

    Private Sub txtChar_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtChar.TextChanged
        If txtChar.Text <> "" Then txtASCII.Text = CStr(Asc(txtChar.Text))
    End Sub

    Private Sub frmInput_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If txtChar.Text <> "" Then
            inputChar.ASCII = Asc(txtChar.Text)
        End If
        e.Cancel = False
    End Sub

    Private Sub DoContinue()
        If rbBuffer.Checked Then
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

    Private Sub frmInput_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        Select Case e.KeyCode
            Case Keys.F5 : DoContinue()
            Case Keys.F8 : DoDebug()
            Case Keys.C : If e.Control Then DoStop()
        End Select
    End Sub

    Private Sub btnLf_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLf.Click
        txtASCII.Focus()
        txtASCII.Text = "10"
    End Sub

    Private Sub btnCr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCr.Click
        txtASCII.Focus()
        txtASCII.Text = "13"
    End Sub

    Private Sub btnNull_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNull.Click
        txtASCII.Focus()
        txtASCII.Text = "0"
    End Sub

    Private Sub txtBuffer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBuffer.TextChanged
        inputChar.Buffer = txtBuffer.Text
    End Sub

    Private Sub rbBuffer_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbBuffer.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            txtBuffer.Focus()
        End If
    End Sub

    Private Sub rbChar_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbChar.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            txtChar.Focus()
        End If
    End Sub

    Private Sub rbASCII_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbASCII.CheckedChanged
        If cancelBubbling Then
            cancelBubbling = False
        Else
            txtASCII.Focus()
        End If
    End Sub

    Private Sub txtChar_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtChar.Enter
        cancelBubbling = True
        rbChar.Checked = True
    End Sub

    Private Sub txtASCII_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtASCII.Enter
        cancelBubbling = True
        rbASCII.Checked = True
    End Sub

    Private Sub txtBuffer_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBuffer.Enter
        cancelBubbling = True
        rbBuffer.Checked = True
    End Sub
End Class