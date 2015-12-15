Imports System.Data.SqlClient
Imports LazyFramework.Data

Public Class DataProvider
    Implements IDataAccessProvider

    Public Function CreateCommand(cmd As data.CommandInfo) As IDbCommand Implements IDataAccessProvider.CreateCommand
        Dim ret As New SqlCommand()

        ret.CommandText = cmd.CommandText
        ret.CommandType = cmd.CommandType

        Return ret
    End Function

    Public Function CreateConnection(connectionInfo As Data.ServerConnectionInfo) As IDbConnection Implements IDataAccessProvider.CreateConnection
        Return New SqlConnection(String.Format(ConnectionStringTemplate, connectionInfo.Address, connectionInfo.Database, connectionInfo.UserName, connectionInfo.Password, connectionInfo.Pooling) )
    End Function
    
    Public Shared ConnectionStringTemplate As String = "server={0};Database={1};User ID={2};Password={3};pooling={4};" ' ApplicationIntent=ReadOnly"

    Public Shared Sub BulkInsert(Of T)(connectionInfo As Data.ServerConnectionInfo, tableName As String, list As IEnumerable(Of T))
        Dim decorator As New BulkCopyDecorator(Of T)(list)

        Using con = New SqlConnection(String.Format(ConnectionStringTemplate, connectionInfo.Address, connectionInfo.Database, connectionInfo.UserName, connectionInfo.Password, connectionInfo.Pooling))
            con.Open()
            Using bc As New SqlBulkCopy(con)
                bc.DestinationTableName = tableName
                bc.WriteToServer(decorator)
            End Using
        End Using

    End Sub

End Class

