Imports System.Runtime.Serialization

Namespace Query
    <Serializable>
    Friend Class AllreadeyConfiguredException
        Inherits Exception

        Public Sub New(t As Type)
            MyBase.New(t.ToString)
        End Sub


    End Class
End Namespace
