Public Class FormAbout
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
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents LinkXFX As System.Windows.Forms.LinkLabel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents LinkBF As System.Windows.Forms.LinkLabel
    Friend WithEvents LinkOWS As System.Windows.Forms.LinkLabel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAbout))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.LinkXFX = New System.Windows.Forms.LinkLabel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.LinkBF = New System.Windows.Forms.LinkLabel()
        Me.LinkOWS = New System.Windows.Forms.LinkLabel()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(417, 49)
        Me.Panel1.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.Label2.Location = New System.Drawing.Point(9, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 16)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "xFX JumpStart"
        Me.Label2.UseMnemonic = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.Label1.Location = New System.Drawing.Point(6, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(173, 30)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "VBBrainFNET 1.0"
        Me.Label1.UseMnemonic = False
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button1.Location = New System.Drawing.Point(344, 280)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(68, 30)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Close"
        '
        'LinkXFX
        '
        Me.LinkXFX.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LinkXFX.LinkArea = New System.Windows.Forms.LinkArea(22, 45)
        Me.LinkXFX.Location = New System.Drawing.Point(14, 232)
        Me.LinkXFX.Name = "LinkXFX"
        Me.LinkXFX.Size = New System.Drawing.Size(391, 16)
        Me.LinkXFX.TabIndex = 2
        Me.LinkXFX.TabStop = True
        Me.LinkXFX.Text = "VBBrainFNET Web Site: http://software.xfx.net/utilities/vbbfck/"
        Me.LinkXFX.UseCompatibleTextRendering = True
        Me.LinkXFX.UseMnemonic = False
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.Location = New System.Drawing.Point(14, 62)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(401, 66)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = resources.GetString("Label3.Text")
        '
        'Panel2
        '
        Me.Panel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel2.Location = New System.Drawing.Point(-11, 138)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(435, 4)
        Me.Panel2.TabIndex = 4
        '
        'LinkBF
        '
        Me.LinkBF.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LinkBF.LinkArea = New System.Windows.Forms.LinkArea(83, 122)
        Me.LinkBF.Location = New System.Drawing.Point(14, 154)
        Me.LinkBF.Name = "LinkBF"
        Me.LinkBF.Size = New System.Drawing.Size(379, 29)
        Me.LinkBF.TabIndex = 5
        Me.LinkBF.TabStop = True
        Me.LinkBF.Text = "Brainfuck is a creation of Urban Müller. Follow this link for further information" &
    ": http://en.wikipedia.org/wiki/Brainfuck"
        Me.LinkBF.UseCompatibleTextRendering = True
        Me.LinkBF.UseMnemonic = False
        '
        'LinkOWS
        '
        Me.LinkOWS.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LinkOWS.LinkArea = New System.Windows.Forms.LinkArea(38, 77)
        Me.LinkOWS.Location = New System.Drawing.Point(14, 193)
        Me.LinkOWS.Name = "LinkOWS"
        Me.LinkOWS.Size = New System.Drawing.Size(379, 29)
        Me.LinkOWS.TabIndex = 6
        Me.LinkOWS.TabStop = True
        Me.LinkOWS.Text = "(almost) Official Brainfuck Web Site: http://www.muppetlabs.com/~breadbox/bf/"
        Me.LinkOWS.UseCompatibleTextRendering = True
        '
        'FormAbout
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 16)
        Me.ClientSize = New System.Drawing.Size(417, 315)
        Me.Controls.Add(Me.LinkOWS)
        Me.Controls.Add(Me.LinkBF)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.LinkXFX)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormAbout"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "About VBBrainFNET"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub LinkBF_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkBF.LinkClicked
        With LinkBF
            System.Diagnostics.Process.Start(.Text.Substring(.LinkArea.Start))
        End With
    End Sub

    Private Sub LinkXFX_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkXFX.LinkClicked
        With LinkXFX
            System.Diagnostics.Process.Start(.Text.Substring(.LinkArea.Start))
        End With
    End Sub

    Private Sub LinkOWS_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkOWS.LinkClicked
        With LinkOWS
            System.Diagnostics.Process.Start(.Text.Substring(.LinkArea.Start))
        End With
    End Sub
End Class
