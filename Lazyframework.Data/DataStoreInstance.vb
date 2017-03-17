<Obsolete("Use Reader or Writer instance instead")> Public Class DataStoreInstance
    Implements IDataStore

    Private connectionInfo As ServerConnectionInfo
    Public Sub New(connectionInfo As ServerConnectionInfo)
        If connectionInfo Is Nothing Then
            Throw New System.ArgumentNullException(NameOf(connectionInfo))
        End If

        Me.connectionInfo = connectionInfo

    End Sub

    Public Sub Exec(command As CommandInfo) Implements IDataStore.Exec
        Store.Exec(connectionInfo, command)
    End Sub

    Public Sub Exec(command As CommandInfo, data As Object) Implements IDataStore.Exec
        Store.Exec(connectionInfo, command, data)
    End Sub

    Public Sub Exec(Of T As New)(command As CommandInfo, data As FillStatus(Of T)) Implements IDataStore.Exec
        Store.Exec(connectionInfo, command, data)
    End Sub

    Public Sub Exec(Of T As New)(command As CommandInfo, data As List(Of T)) Implements IDataStore.Exec
        Store.Exec(connectionInfo, command, data)
    End Sub

    Public Sub Exec(Of T As Structure)(command As CommandInfo, data As ICollection(Of T), colName As String) Implements IDataStore.Exec
        Store.Exec(connectionInfo, command, data, colName)
    End Sub

    Public Sub GetStream(Of T As {WillDisposeThoseForU, New})(command As CommandInfo, data As T) Implements IDataStore.GetStream
        Store.GetStream(connectionInfo, command, data)
    End Sub

    Public Function ExecScalar(Of T)(command As CommandInfo) As T Implements IDataStore.ExecScalar
        Dim ret = Store.ExecScalar(connectionInfo, command)

        Return CType(ret, T)

    End Function
End Class