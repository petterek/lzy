Imports LazyFramework.Data

Public Class ServerConnectionInfo
    Inherits LazyFramework.Data.ServerConnectionInfo

    Public Sub New()
    End Sub

    Public Sub New(connString As String)
        MyBase.New(connString)
        If connString Is Nothing Then
            Throw New System.ArgumentNullException(NameOf(connString))
        End If

    End Sub
    Public Sub New(userName As String, password As String, server As String, initalCatalog As String)
        MyBase.New(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", server, initalCatalog, userName, password))
    End Sub

    Public Overrides Function GetProvider() As IDataAccessProvider
        Return New DataProvider
    End Function
End Class