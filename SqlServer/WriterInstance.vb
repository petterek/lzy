Imports LazyFramework.Data

Public Class WriterInstance
    Implements IWriterInstance

    Private ReadOnly connectionInfo As ServerConnectionInfo

    Public Sub New(connectionInfo As ServerConnectionInfo)
        If connectionInfo Is Nothing Then
            Throw New System.ArgumentNullException(NameOf(connectionInfo))
        End If

        Me.connectionInfo = connectionInfo
    End Sub

    Public Sub Create(command As CommandInfo) Implements IWriterInstance.Create
        command.TypeOfCommand = CommandTypeEnum.Create
        Store.Exec(connectionInfo, command)
    End Sub

    Public Sub Delete(command As CommandInfo) Implements IWriterInstance.Delete
        command.TypeOfCommand = CommandTypeEnum.Delete
        Store.Exec(connectionInfo, command)
    End Sub

    Public Sub Update(command As CommandInfo) Implements IWriterInstance.Update
        command.TypeOfCommand = CommandTypeEnum.Update
        Store.Exec(connectionInfo, command)
    End Sub

    Public Function Create(Of T)(command As CommandInfo) As T Implements IWriterInstance.Create
        command.TypeOfCommand = CommandTypeEnum.Create
        Return CType(Store.ExecScalar(connectionInfo, command), T)
    End Function
End Class
