Imports LazyFramework.Data

Public Class ServerConnectionInfo
    Inherits LazyFramework.Data.ServerConnectionInfo

    Public Overrides Function GetProvider() As IDataAccessProvider
        Return New DataProvider
    End Function
End Class