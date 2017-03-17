
<Obsolete("Use the one in MSSQLServer instead")> Public Class ReaderInstance
    Implements IDataReaderStore

    Private ReadOnly connectionInfo As ServerConnectionInfo

    Public Sub New(connectionInfo As ServerConnectionInfo)
        If connectionInfo Is Nothing Then
            Throw New System.ArgumentNullException(NameOf(connectionInfo))
        End If

        Me.connectionInfo = connectionInfo

    End Sub

    Public Sub Read(Of T As New)(command As CommandInfo, data As List(Of T)) Implements IDataReaderStore.Read
        Store.Exec(connectionInfo, command, data)
    End Sub

    Public Sub Read(Of T)(command As CommandInfo, data As T) Implements IDataReaderStore.Read
        Store.Exec(connectionInfo, command, data)
    End Sub
    Public Function Read(Of T)(command As CommandInfo) As T Implements IDataReaderStore.Read
        Return CType(Store.ExecScalar(connectionInfo, command), T)
    End Function

    Public Function CreateCommand(commandText As String) As IDbCommand Implements IDataReaderStore.CreateCommand
        Return New SqlClient.SqlCommand
    End Function

    Public Function CreateParam(name As String, type As DbType, nullable As Boolean) As IDbDataParameter Implements IDataReaderStore.CreateParam
        Throw New NotImplementedException()
    End Function
End Class
