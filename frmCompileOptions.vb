Public Class frmCompilerOptions
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
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents opCg1 As System.Windows.Forms.RadioButton
    Friend WithEvents opCg6 As System.Windows.Forms.RadioButton
    Friend WithEvents opCg5 As System.Windows.Forms.RadioButton
    Friend WithEvents opCg4 As System.Windows.Forms.RadioButton
    Friend WithEvents opCg3 As System.Windows.Forms.RadioButton
    Friend WithEvents opCg2 As System.Windows.Forms.RadioButton
    Friend WithEvents opOp1 As System.Windows.Forms.RadioButton
    Friend WithEvents opOp3 As System.Windows.Forms.RadioButton
    Friend WithEvents opOp2 As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents chkWaitStart As System.Windows.Forms.CheckBox
    Friend WithEvents chkWaitEnd As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCompilerOptions))
        Me.btnOK = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.opCg1 = New System.Windows.Forms.RadioButton()
        Me.opCg6 = New System.Windows.Forms.RadioButton()
        Me.opCg5 = New System.Windows.Forms.RadioButton()
        Me.opCg4 = New System.Windows.Forms.RadioButton()
        Me.opCg3 = New System.Windows.Forms.RadioButton()
        Me.opCg2 = New System.Windows.Forms.RadioButton()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.opOp1 = New System.Windows.Forms.RadioButton()
        Me.opOp3 = New System.Windows.Forms.RadioButton()
        Me.opOp2 = New System.Windows.Forms.RadioButton()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.chkWaitEnd = New System.Windows.Forms.CheckBox()
        Me.chkWaitStart = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(370, 218)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(63, 25)
        Me.btnOK.TabIndex = 7
        Me.btnOK.Text = "Compile"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.opCg1)
        Me.GroupBox1.Controls.Add(Me.opCg6)
        Me.GroupBox1.Controls.Add(Me.opCg5)
        Me.GroupBox1.Controls.Add(Me.opCg4)
        Me.GroupBox1.Controls.Add(Me.opCg3)
        Me.GroupBox1.Controls.Add(Me.opCg2)
        Me.GroupBox1.Location = New System.Drawing.Point(224, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(210, 142)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Code Generation"
        '
        'opCg1
        '
        Me.opCg1.AutoSize = True
        Me.opCg1.Checked = True
        Me.opCg1.Location = New System.Drawing.Point(10, 18)
        Me.opCg1.Name = "opCg1"
        Me.opCg1.Size = New System.Drawing.Size(178, 17)
        Me.opCg1.TabIndex = 12
        Me.opCg1.TabStop = True
        Me.opCg1.Text = "Optimize blended model (default)"
        '
        'opCg6
        '
        Me.opCg6.AutoSize = True
        Me.opCg6.Location = New System.Drawing.Point(10, 108)
        Me.opCg6.Name = "opCg6"
        Me.opCg6.Size = New System.Drawing.Size(175, 17)
        Me.opCg6.TabIndex = 11
        Me.opCg6.Text = "Optimize for Pentium 4 or Athlon"
        '
        'opCg5
        '
        Me.opCg5.AutoSize = True
        Me.opCg5.Location = New System.Drawing.Point(10, 90)
        Me.opCg5.Name = "opCg5"
        Me.opCg5.Size = New System.Drawing.Size(153, 17)
        Me.opCg5.TabIndex = 10
        Me.opCg5.Text = "Optimize for PPro, P-II, P-III"
        '
        'opCg4
        '
        Me.opCg4.AutoSize = True
        Me.opCg4.Location = New System.Drawing.Point(10, 72)
        Me.opCg4.Name = "opCg4"
        Me.opCg4.Size = New System.Drawing.Size(121, 17)
        Me.opCg4.TabIndex = 9
        Me.opCg4.Text = "Optimize for Pentium"
        '
        'opCg3
        '
        Me.opCg3.AutoSize = True
        Me.opCg3.Location = New System.Drawing.Point(10, 54)
        Me.opCg3.Name = "opCg3"
        Me.opCg3.Size = New System.Drawing.Size(113, 17)
        Me.opCg3.TabIndex = 8
        Me.opCg3.Text = "Optimize for 80486"
        '
        'opCg2
        '
        Me.opCg2.AutoSize = True
        Me.opCg2.Location = New System.Drawing.Point(10, 36)
        Me.opCg2.Name = "opCg2"
        Me.opCg2.Size = New System.Drawing.Size(113, 17)
        Me.opCg2.TabIndex = 7
        Me.opCg2.Text = "Optimize for 80386"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.opOp1)
        Me.GroupBox2.Controls.Add(Me.opOp3)
        Me.GroupBox2.Controls.Add(Me.opOp2)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(210, 78)
        Me.GroupBox2.TabIndex = 9
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Optimization"
        '
        'opOp1
        '
        Me.opOp1.AutoSize = True
        Me.opOp1.Checked = True
        Me.opOp1.Location = New System.Drawing.Point(10, 18)
        Me.opOp1.Name = "opOp1"
        Me.opOp1.Size = New System.Drawing.Size(166, 17)
        Me.opOp1.TabIndex = 15
        Me.opOp1.TabStop = True
        Me.opOp1.Text = "Disable Optimizations (default)"
        '
        'opOp3
        '
        Me.opOp3.AutoSize = True
        Me.opOp3.Location = New System.Drawing.Point(10, 54)
        Me.opOp3.Name = "opOp3"
        Me.opOp3.Size = New System.Drawing.Size(102, 17)
        Me.opOp3.TabIndex = 14
        Me.opOp3.Text = "Maximize Speed"
        '
        'opOp2
        '
        Me.opOp2.AutoSize = True
        Me.opOp2.Location = New System.Drawing.Point(10, 36)
        Me.opOp2.Name = "opOp2"
        Me.opOp2.Size = New System.Drawing.Size(99, 17)
        Me.opOp2.TabIndex = 13
        Me.opOp2.Text = "Minimize Space"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.chkWaitEnd)
        Me.GroupBox3.Controls.Add(Me.chkWaitStart)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 86)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(210, 60)
        Me.GroupBox3.TabIndex = 10
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Miscellaneous"
        '
        'chkWaitEnd
        '
        Me.chkWaitEnd.AutoSize = True
        Me.chkWaitEnd.Checked = True
        Me.chkWaitEnd.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkWaitEnd.Location = New System.Drawing.Point(10, 34)
        Me.chkWaitEnd.Name = "chkWaitEnd"
        Me.chkWaitEnd.Size = New System.Drawing.Size(119, 17)
        Me.chkWaitEnd.TabIndex = 1
        Me.chkWaitEnd.Text = "Wait for key on end"
        '
        'chkWaitStart
        '
        Me.chkWaitStart.AutoSize = True
        Me.chkWaitStart.Location = New System.Drawing.Point(10, 18)
        Me.chkWaitStart.Name = "chkWaitStart"
        Me.chkWaitStart.Size = New System.Drawing.Size(121, 17)
        Me.chkWaitStart.TabIndex = 0
        Me.chkWaitStart.Text = "Wait for key on start"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Info
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Location = New System.Drawing.Point(6, 154)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(427, 56)
        Me.Panel1.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(52, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(363, 40)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "The program will freeze while the compiler is generating the executable. Be patie" & _
            "nt and wait for the compiler to finish. As soon as it finishes, the folder conta" & _
            "ining the executable will be displayed."
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(10, 11)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(32, 32)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 12
        Me.PictureBox1.TabStop = False
        '
        'frmCompilerOptions
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(440, 249)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCompilerOptions"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Compiler Options"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        SaveCtrlsState(Me)
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub SaveCtrlsState(ByVal cc As Control)
        For Each c As Control In cc.Controls
            If c.Controls.Count > 0 Then
                SaveCtrlsState(c)
            Else
                If TypeOf c Is CheckBox Then SaveSetting("VBBrainFNET", "CompilerOptions", c.Name, IIf(Of String)(CType(c, CheckBox).Checked, "1", "0"))
                If TypeOf c Is RadioButton Then SaveSetting("VBBrainFNET", "CompilerOptions", c.Name, IIf(Of String)(CType(c, RadioButton).Checked, "1", "0"))
            End If
        Next
    End Sub

    Private Sub frmCompilerOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadCtrlsState(Me)
    End Sub

    Private Sub LoadCtrlsState(ByVal cc As Control)
        For Each c As Control In cc.Controls
            If c.Controls.Count > 0 Then
                LoadCtrlsState(c)
            Else
                If TypeOf c Is CheckBox Then CType(c, CheckBox).Checked = GetSetting("VBBrainFNET", "CompilerOptions", c.Name, IIf(Of String)(CType(c, CheckBox).Checked, "1", "0")) = "1"
                If TypeOf c Is RadioButton Then CType(c, RadioButton).Checked = GetSetting("VBBrainFNET", "CompilerOptions", c.Name, IIf(Of String)(CType(c, RadioButton).Checked, "1", "0")) = "1"
            End If
        Next
    End Sub
End Class
