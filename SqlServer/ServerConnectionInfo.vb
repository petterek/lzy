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

    Public Overrides Function GetProvider() As IDataAccessProvider
        Return New DataProvider
    End Function
End Class