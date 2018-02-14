Public Class VBBFCStatusBar
    Inherits Windows.Forms.StatusBar

    Private mProgress As Integer = -1
    Private mProgSize As Integer = 0

    Public Sub New()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.UpdateStyles()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim r As Rectangle = New Rectangle(0, 0, 0, 0)
        Dim txtSize As Integer
        Dim itemIdx As Integer = 1

        For Each p As StatusBarPanel In Panels
            'txtSize = g.MeasureString(p.Text, Me.Font)
            txtSize = p.Text.Length * 7
            r = New Rectangle( _
                                r.Left, _
                                0, _
                                IIf(p.AutoSize = StatusBarPanelAutoSize.Spring, _
                                    CInt(Me.Width - r.Left - 1), _
                                    CInt(txtSize + 24)), _
                                Me.Height - 1)
            OnDrawItem(New StatusBarDrawItemEventArgs( _
                            e.Graphics, Me.Font, _
                            r, _
                            itemIdx, _
                            DrawItemState.Default, p))

            itemIdx += 1
            r.X = r.Right
        Next p
    End Sub

    Protected Overrides Sub OnDrawItem(ByVal sbdievent As System.Windows.Forms.StatusBarDrawItemEventArgs)
        With sbdievent
            Dim pTxt As String = .Panel.Text
            Dim txtH As Integer = 12 ' CInt(.Graphics.MeasureString(pTxt, Font).Height)
            Dim sb As SolidBrush = New SolidBrush(Color.FromKnownColor(KnownColor.ControlText))

            .Graphics.FillRectangle(New SolidBrush(Me.BackColor), .Bounds)
            .Graphics.DrawRectangle(New Pen(Color.FromKnownColor(KnownColor.DarkGray)), .Bounds)

            Select Case .Index
                Case 1
                    .Graphics.FillRectangle(New SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)), _
                                        .Bounds.X + 1, _
                                        .Bounds.Y + 1, _
                                        CInt(.Bounds.Width * mProgSize / 100) - 1, _
                                        .Bounds.Height - 1)
                Case 6
                    If mProgress <> -1 Then
                        .Graphics.FillRectangle(New SolidBrush(Color.FromKnownColor(KnownColor.Highlight)), _
                                        .Bounds.X + 1, _
                                        .Bounds.Y + 1, _
                                        CInt(.Bounds.Width * mProgress / 100) - 1, _
                                        .Bounds.Height - 1)
                        pTxt += " " + CStr(mProgress) + "%"
                        sb = New SolidBrush(Color.FromKnownColor(KnownColor.HighlightText))
                    End If
            End Select

            If pTxt <> "" Then
                .Graphics.DrawString(pTxt, Font, sb, .Bounds.X + 4, .Bounds.Y + (.Bounds.Height - txtH) \ 2 + 1)
            End If

            sb.Dispose()
        End With
    End Sub

    Public Property Progress() As Integer
        Get
            Return mProgress
        End Get
        Set(ByVal Value As Integer)
            mProgress = Value
            Me.Invalidate()
        End Set
    End Property

    Public Property ProgramSize() As Integer
        Get
            Return mProgSize
        End Get
        Set(ByVal Value As Integer)
            mProgSize = Value
        End Set
    End Property
End Class