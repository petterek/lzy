Namespace Utils.Json
    Public Class UnExpectedTokenException
        Inherits Exception

        Public Sub New(ByVal current As Char)
            MyBase.New(current)
        End Sub
    End Class
End NameSpace