
Partial Class ClassFactory

    <Serializable>
    Public Class NotConfiguredException
        Inherits ApplicationException

        Public Sub New(msg As String)
            MyBase.New(msg)
        End Sub
    End Class
End Class
