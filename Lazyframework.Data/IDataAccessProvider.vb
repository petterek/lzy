Public Interface IDataAccessProvider
        Function CreateCommand(cmd As CommandInfo) As IDbCommand
        Function CreateConnection(connectionInfo As ServerConnectionInfo) As IDbConnection
    End Interface
