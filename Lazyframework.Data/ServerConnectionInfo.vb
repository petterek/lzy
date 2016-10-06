
Public MustInherit Class ServerConnectionInfo
    Public Address As String
    Public UserName As String
    Public Password As String
    Public Database As String
    Public Pooling As Boolean = True
    Public ReadOnly ConnectionStrig As String

    Public MustOverride Function GetProvider() As IDataAccessProvider

    Public Sub New()
    End Sub

    Protected Sub New(conStr As String)
        ConnectionStrig = conStr
    End Sub

End Class


Public Interface IConnectionInfoProvider
    Function ConnectionInfo() As LazyFramework.Data.ServerConnectionInfo
End Interface

