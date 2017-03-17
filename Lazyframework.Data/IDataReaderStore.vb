Public Interface IDataReaderStore
    Sub Read(Of T)(command As CommandInfo, data As T)
    Sub Read(Of T As New)(command As CommandInfo, data As List(Of T))
    Function Read(Of T)(command As CommandInfo) As T

    Function CreateCommand(commandText As String) As IDbCommand
    Function CreateParam(name As String, type As DbType, nullable As Boolean) As IDbDataParameter

End Interface
