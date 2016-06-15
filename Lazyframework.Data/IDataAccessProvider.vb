Public Interface IDataAccessProvider
    Function CreateCommand(cmd As CommandInfo) As IDbCommand
    Function CreateConnection(connectionInfo As ServerConnectionInfo) As IDbConnection
    Function CreateConnection(connectionInfo As String) As IDbConnection
End Interface
