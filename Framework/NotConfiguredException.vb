Imports LazyFramework

Public Class NotConfiguredException
    Inherits ApplicationException

    Public Sub New(msg As String)
        MyBase.New(msg)
    End Sub
End Class