Module ModuleGlobals
    Public maxMem As Integer = 32768 - 1
    Public maxCellSize As Integer = 256 - 1

    Public followPointer As Boolean = True
    Public isRunning As Boolean
    Public isDebugging As Boolean
    Public forceAbort As Boolean

    Public mem() As Integer
    Public prg() As Byte
    Public ptr As Integer

    Public Structure InputCharDef
        Dim ASCII As Integer
        Dim IsValid As Boolean
        Dim DoContinue As Boolean
        Dim DoDebug As Boolean
        Dim DoStop As Boolean
        Dim Buffer As String
    End Structure
    Public inputChar As InputCharDef

    Public Function DoInputChar(ByVal ownerForm As Form, ByVal p As Integer, Optional ByVal InitValue As Integer = -1) As Boolean
        If inputChar.Buffer <> "" Then
            inputChar.ASCII = Asc(inputChar.Buffer.Substring(0, 1))
            inputChar.Buffer = inputChar.Buffer.Substring(1)
        Else
            Dim dlgInput As Form = New FormInput

            inputChar.ASCII = InitValue
            ownerForm.Enabled = False
            dlgInput.ShowDialog(ownerForm)
            dlgInput.Dispose()
            ownerForm.Enabled = True

            If inputChar.DoDebug And Not isDebugging Then Return False
            If inputChar.DoStop Then forceAbort = True
        End If

        mem(p) = inputChar.ASCII

        Return True
    End Function

    Public Function IIf(Of T)(ByVal c As Boolean, ByVal truePart As T, ByVal falsePart As T) As T
        If c Then
            Return truePart
        Else
            Return falsePart
        End If
    End Function
End Module