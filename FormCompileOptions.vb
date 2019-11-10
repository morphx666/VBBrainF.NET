Public Class FormCompilerOptions
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
    Friend WithEvents ButtonCompile As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButtonCg1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonCg6 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonCg5 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonCg4 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonCg3 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonCg2 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonOp1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonOp3 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonOp2 As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBoxWaitStart As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxWaitEnd As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCompilerOptions))
        Me.ButtonCompile = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioButtonCg1 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonCg6 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonCg5 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonCg4 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonCg3 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonCg2 = New System.Windows.Forms.RadioButton()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.RadioButtonOp1 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonOp3 = New System.Windows.Forms.RadioButton()
        Me.RadioButtonOp2 = New System.Windows.Forms.RadioButton()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.CheckBoxWaitEnd = New System.Windows.Forms.CheckBox()
        Me.CheckBoxWaitStart = New System.Windows.Forms.CheckBox()
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
        'ButtonCompile
        '
        Me.ButtonCompile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCompile.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonCompile.Location = New System.Drawing.Point(391, 225)
        Me.ButtonCompile.Name = "ButtonCompile"
        Me.ButtonCompile.Size = New System.Drawing.Size(70, 25)
        Me.ButtonCompile.TabIndex = 7
        Me.ButtonCompile.Text = "Compile"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg1)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg6)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg5)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg4)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg3)
        Me.GroupBox1.Controls.Add(Me.RadioButtonCg2)
        Me.GroupBox1.Location = New System.Drawing.Point(224, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(237, 142)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Code Generation"
        '
        'RadioButtonCg1
        '
        Me.RadioButtonCg1.AutoSize = True
        Me.RadioButtonCg1.Checked = True
        Me.RadioButtonCg1.Location = New System.Drawing.Point(10, 18)
        Me.RadioButtonCg1.Name = "RadioButtonCg1"
        Me.RadioButtonCg1.Size = New System.Drawing.Size(204, 19)
        Me.RadioButtonCg1.TabIndex = 12
        Me.RadioButtonCg1.TabStop = True
        Me.RadioButtonCg1.Text = "Optimize blended model (default)"
        '
        'RadioButtonCg6
        '
        Me.RadioButtonCg6.AutoSize = True
        Me.RadioButtonCg6.Location = New System.Drawing.Point(10, 108)
        Me.RadioButtonCg6.Name = "RadioButtonCg6"
        Me.RadioButtonCg6.Size = New System.Drawing.Size(201, 19)
        Me.RadioButtonCg6.TabIndex = 11
        Me.RadioButtonCg6.Text = "Optimize for Pentium 4 or Athlon"
        '
        'RadioButtonCg5
        '
        Me.RadioButtonCg5.AutoSize = True
        Me.RadioButtonCg5.Location = New System.Drawing.Point(10, 90)
        Me.RadioButtonCg5.Name = "RadioButtonCg5"
        Me.RadioButtonCg5.Size = New System.Drawing.Size(170, 19)
        Me.RadioButtonCg5.TabIndex = 10
        Me.RadioButtonCg5.Text = "Optimize for PPro, P-II, P-III"
        '
        'RadioButtonCg4
        '
        Me.RadioButtonCg4.AutoSize = True
        Me.RadioButtonCg4.Location = New System.Drawing.Point(10, 72)
        Me.RadioButtonCg4.Name = "RadioButtonCg4"
        Me.RadioButtonCg4.Size = New System.Drawing.Size(139, 19)
        Me.RadioButtonCg4.TabIndex = 9
        Me.RadioButtonCg4.Text = "Optimize for Pentium"
        '
        'RadioButtonCg3
        '
        Me.RadioButtonCg3.AutoSize = True
        Me.RadioButtonCg3.Location = New System.Drawing.Point(10, 54)
        Me.RadioButtonCg3.Name = "RadioButtonCg3"
        Me.RadioButtonCg3.Size = New System.Drawing.Size(124, 19)
        Me.RadioButtonCg3.TabIndex = 8
        Me.RadioButtonCg3.Text = "Optimize for 80486"
        '
        'RadioButtonCg2
        '
        Me.RadioButtonCg2.AutoSize = True
        Me.RadioButtonCg2.Location = New System.Drawing.Point(10, 36)
        Me.RadioButtonCg2.Name = "RadioButtonCg2"
        Me.RadioButtonCg2.Size = New System.Drawing.Size(124, 19)
        Me.RadioButtonCg2.TabIndex = 7
        Me.RadioButtonCg2.Text = "Optimize for 80386"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.RadioButtonOp1)
        Me.GroupBox2.Controls.Add(Me.RadioButtonOp3)
        Me.GroupBox2.Controls.Add(Me.RadioButtonOp2)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(210, 78)
        Me.GroupBox2.TabIndex = 9
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Optimization"
        '
        'RadioButtonOp1
        '
        Me.RadioButtonOp1.AutoSize = True
        Me.RadioButtonOp1.Checked = True
        Me.RadioButtonOp1.Location = New System.Drawing.Point(10, 18)
        Me.RadioButtonOp1.Name = "RadioButtonOp1"
        Me.RadioButtonOp1.Size = New System.Drawing.Size(188, 19)
        Me.RadioButtonOp1.TabIndex = 15
        Me.RadioButtonOp1.TabStop = True
        Me.RadioButtonOp1.Text = "Disable Optimizations (default)"
        '
        'RadioButtonOp3
        '
        Me.RadioButtonOp3.AutoSize = True
        Me.RadioButtonOp3.Location = New System.Drawing.Point(10, 54)
        Me.RadioButtonOp3.Name = "RadioButtonOp3"
        Me.RadioButtonOp3.Size = New System.Drawing.Size(111, 19)
        Me.RadioButtonOp3.TabIndex = 14
        Me.RadioButtonOp3.Text = "Maximize Speed"
        '
        'RadioButtonOp2
        '
        Me.RadioButtonOp2.AutoSize = True
        Me.RadioButtonOp2.Location = New System.Drawing.Point(10, 36)
        Me.RadioButtonOp2.Name = "RadioButtonOp2"
        Me.RadioButtonOp2.Size = New System.Drawing.Size(108, 19)
        Me.RadioButtonOp2.TabIndex = 13
        Me.RadioButtonOp2.Text = "Minimize Space"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.CheckBoxWaitEnd)
        Me.GroupBox3.Controls.Add(Me.CheckBoxWaitStart)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 86)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(210, 60)
        Me.GroupBox3.TabIndex = 10
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Miscellaneous"
        '
        'CheckBoxWaitEnd
        '
        Me.CheckBoxWaitEnd.AutoSize = True
        Me.CheckBoxWaitEnd.Checked = True
        Me.CheckBoxWaitEnd.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxWaitEnd.Location = New System.Drawing.Point(10, 34)
        Me.CheckBoxWaitEnd.Name = "CheckBoxWaitEnd"
        Me.CheckBoxWaitEnd.Size = New System.Drawing.Size(129, 19)
        Me.CheckBoxWaitEnd.TabIndex = 1
        Me.CheckBoxWaitEnd.Text = "Wait for key on end"
        '
        'CheckBoxWaitStart
        '
        Me.CheckBoxWaitStart.AutoSize = True
        Me.CheckBoxWaitStart.Location = New System.Drawing.Point(10, 18)
        Me.CheckBoxWaitStart.Name = "CheckBoxWaitStart"
        Me.CheckBoxWaitStart.Size = New System.Drawing.Size(132, 19)
        Me.CheckBoxWaitStart.TabIndex = 0
        Me.CheckBoxWaitStart.Text = "Wait for key on start"
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.SystemColors.Info
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Location = New System.Drawing.Point(6, 154)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(455, 63)
        Me.Panel1.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Location = New System.Drawing.Point(52, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(391, 47)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "The program will freeze while the compiler is generating the executable. Be patie" &
    "nt and wait for the compiler to finish. As soon as it finishes, the folder conta" &
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
        'FormCompilerOptions
        '
        Me.AcceptButton = Me.ButtonCompile
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        Me.ClientSize = New System.Drawing.Size(468, 256)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ButtonCompile)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormCompilerOptions"
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

    Private Sub ButtonCompile_Click(sender As Object, e As EventArgs) Handles ButtonCompile.Click
        SaveCtrlsState(Me)
        Me.DialogResult = DialogResult.OK
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

    Private Sub FormCompilerOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
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