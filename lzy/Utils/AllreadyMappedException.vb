Namespace Utils
    <Serializable> Friend Class AllreadyMappedException
        Inherits Exception

        Public Sub New(ByVal s As String)
            MyBase.New(s)
        End Sub
    End Class
End NameSpace
