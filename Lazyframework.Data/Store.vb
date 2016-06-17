Imports System.Linq
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports LazyFramework.Data

Public Class Store

    Private Shared Plugins As New List(Of Type)
    Public Shared Sub RegisterPlugin(Of T As DataModificationPluginBase)()
        Plugins.Add(GetType(T))
    End Sub

    Public Shared Sub Exec(Of T As New)(connectionInfo As ServerConnectionInfo, command As CommandInfo, data As List(Of T))
        ExecReader(connectionInfo, command, New FillStatus(Of List(Of T))(data), CommandBehavior.SingleResult, AddressOf New ListFiller().FillList, GetType(T))
    End Sub

    Public Shared Sub Exec(Of T As New)(connectionInfo As ServerConnectionInfo, command As CommandInfo, data As FillStatus(Of T))
        ExecReader(connectionInfo, command, data, CommandBehavior.SingleResult Or CommandBehavior.SingleRow, AddressOf ReadOne, GetType(T))
    End Sub

    Public Shared Sub Exec(connectionInfo As ServerConnectionInfo, command As CommandInfo, data As Object)
        ExecReader(connectionInfo, command, New FillStatus(Of Object)(data), CommandBehavior.SingleResult Or CommandBehavior.SingleRow, AddressOf ReadOne, data.GetType)
    End Sub

    Public Shared Sub Exec(connectionInfo As ServerConnectionInfo, command As CommandInfo)
        ExecReader(Of Object)(connectionInfo, command, New FillStatus(Of Object)(Nothing), CommandBehavior.SingleResult, Nothing, Nothing)
    End Sub

    Public Shared Sub Exec(Of T As Structure)(connectionInfo As ServerConnectionInfo, command As CommandInfo, data As ICollection(Of T), colName As String)
        ExecReader(Of T)(connectionInfo, command, data, CommandBehavior.SingleResult, AddressOf New ListFiller(colName).FillListForValueType)
    End Sub


    Public Shared Sub GetStream(Of T As {New, WillDisposeThoseForU})(connectionInfo As ServerConnectionInfo, command As CommandInfo, data As T)
        ExecReaderWithStream(Of T)(connectionInfo, command, New FillStatus(Of T)(data), CommandBehavior.SingleResult Or CommandBehavior.SingleRow, AddressOf ReadOne(Of T), data.GetType)
    End Sub



#Region "Privates"

    Private Shared Sub ExecReaderWithStream(Of T As {New, WillDisposeThoseForU})(ByVal connectionInfo As ServerConnectionInfo, ByVal command As CommandInfo, data As FillStatus(Of T), readerOptions As CommandBehavior, handler As HandleReader(Of T), dataObjectType As Type)
        Dim pluginCollection As List(Of DataModificationPluginBase) = Nothing
        Dim provider = connectionInfo.GetProvider

        Dim cmd = provider.CreateCommand(command)
        data.Value.DisposeThis(cmd)

        FillParameters(provider, command, dataObjectType, data.Value, cmd)
        FirePlugin(pluginCollection, PluginExecutionPointEnum.Pre, connectionInfo, command, data.Value)

        Dim conn = provider.CreateConnection(connectionInfo)
        data.Value.DisposeThis(conn)

        cmd.Connection = conn
        conn.Open()

        Dim filler As FillObject = Nothing
        Dim reader As IDataReader = Nothing

        reader = cmd.ExecuteReader(readerOptions Or CommandBehavior.CloseConnection Or CommandBehavior.SequentialAccess)

        If dataObjectType IsNot Nothing Then
            filler = GetFiller(command, reader, dataObjectType)
            handler(filler, reader, data)
        End If

        FirePlugin(pluginCollection, PluginExecutionPointEnum.Post, connectionInfo, command, data.Value)

    End Sub

    Private Shared Sub ExecReader(Of T As Structure)(ByVal connectionInfo As ServerConnectionInfo, ByVal command As CommandInfo, data As ICollection(Of T), readerOptions As CommandBehavior, handler As HandleReaderForValueType(Of T))
        Dim pluginCollection As List(Of DataModificationPluginBase) = Nothing
        Dim provider = connectionInfo.GetProvider

        Using cmd = provider.CreateCommand(command)
            FirePlugin(pluginCollection, PluginExecutionPointEnum.Pre, connectionInfo, command, data)
            Using conn = provider.CreateConnection(connectionInfo)
                cmd.Connection = conn
                conn.Open()
                Dim filler As FillObject = Nothing
                Dim reader As IDataReader = Nothing
                reader = cmd.ExecuteReader(readerOptions Or CommandBehavior.CloseConnection Or CommandBehavior.SequentialAccess)
                handler(reader, data)
            End Using

            FirePlugin(pluginCollection, PluginExecutionPointEnum.Post, connectionInfo, command, data)

        End Using
    End Sub

    Public Shared Function ExecScalar(connectionInfo As ServerConnectionInfo, command As CommandInfo) As Object
        Dim provider = connectionInfo.GetProvider
        Dim sw As New Stopwatch
        sw.Start()
        Dim executeScalar As Object

        Try
            Using cmd = provider.CreateCommand(command)
                Using conn = provider.CreateConnection(connectionInfo)
                    cmd.Connection = conn
                    conn.Open()
                    executeScalar = cmd.ExecuteScalar()
                End Using
            End Using
            sw.Stop()

            'Dim loginfo As New DbRequestOkLog With {.DbName = connectionInfo, .Command = command, .Took = sw.ElapsedMilliseconds}
            'LazyFramework.Logging.Log.Write(Of DbRequestLog)(loginfo, Logging.LogLevelEnum.Info)
        Catch ex As Exception
            sw.Stop()
            'Dim loginfo As New DbRequestFaildLog With {.DbName = connectionInfo, .Command = command, .Took = sw.ElapsedMilliseconds, .Error = ex}
            'LazyFramework.Logging.Log.Write(Of DbRequestFaildLog)(loginfo, Logging.LogLevelEnum.Info)
            Throw
        End Try

        Return executeScalar
    End Function


    Private Shared Sub ExecReader(Of T As New)(ByVal connectionInfo As ServerConnectionInfo, ByVal command As CommandInfo, data As FillStatus(Of T), readerOptions As CommandBehavior, handler As HandleReader(Of T), dataObjectType As Type)
        Dim pluginCollection As List(Of DataModificationPluginBase) = Nothing
        Dim provider = connectionInfo.GetProvider

        Using cmd = provider.CreateCommand(command)
            FillParameters(provider, command, dataObjectType, data.Value, cmd)

            FirePlugin(pluginCollection, PluginExecutionPointEnum.Pre, connectionInfo, command, data.Value)

            Using conn = provider.CreateConnection(connectionInfo)

                cmd.Connection = conn
                conn.Open()
                Dim filler As FillObject = Nothing
                Dim reader As IDataReader = Nothing

                reader = cmd.ExecuteReader(readerOptions Or CommandBehavior.CloseConnection Or CommandBehavior.SequentialAccess)
                If dataObjectType IsNot Nothing Then
                    filler = GetFiller(command, reader, dataObjectType)
                    handler(filler, reader, data)
                End If

            End Using

            FirePlugin(pluginCollection, PluginExecutionPointEnum.Post, connectionInfo, command, data.Value)

        End Using
    End Sub

    Private Shared Sub FirePlugin(<Out> ByRef pluginCollection As List(Of DataModificationPluginBase), point As PluginExecutionPointEnum, connectionInfo As ServerConnectionInfo, command As CommandInfo, data As Object)
        If (command.TypeOfCommand And (CommandTypeEnum.Create Or CommandTypeEnum.Delete Or CommandTypeEnum.Update)) <> 0 Then 'Only fire plugins for CUD operations
            If pluginCollection Is Nothing Then
                pluginCollection = (From t In Plugins Select DirectCast(Activator.CreateInstance(t), DataModificationPluginBase)).ToList()
            End If

            For Each p In pluginCollection
                Try
                    Dim dmCtx = New DataModificationPluginContext(connectionInfo, command, data)
                    If point = PluginExecutionPointEnum.Pre Then
                        p.Pre(dmCtx)
                    ElseIf point = PluginExecutionPointEnum.Post Then
                        p.Post(dmCtx)
                    End If

                Catch ex As Exception
                    'SWOLLOW
                    'CQRS.EventHandling.EventHub.Publish(New ExceptionInPluginEvent(point.ToString, ex))
                End Try
            Next
        End If
    End Sub



    Private Shared Sub FillParameters(ByVal provider As IDataAccessProvider, ByVal command As CommandInfo, ByVal dataObjectType As Type, ByVal data As Object, ByVal cmd As IDbCommand)
        Dim p As IDbDataParameter

        For Each pi As ParameterInfo In command.Parameters.Values
            p = cmd.CreateParameter
            p.DbType = pi.DbType
            p.ParameterName = pi.Name
            If pi.Size <> 0 Then
                p.Size = pi.Size
            End If

            If pi.Value IsNot Nothing Then
                p.Value = pi.Value
            Else
                'Read the value from the object... 
                'Logger.Log(1000, New DataLog("Finding value for param:" & pi.Name))
                Dim f = dataObjectType.GetField("_" & pi.Name, BindingFlags.IgnoreCase Or BindingFlags.NonPublic Or BindingFlags.Instance)
                If f Is Nothing Then
                    'Logger.Log(1000, New DataLog("Field not found:" & pi.Name))
                    Throw New MissingFieldException("_" & pi.Name)
                Else
                    Dim value As Object = f.GetValue(data)
                    If value IsNot Nothing Then
                        p.Value = value
                    Else
                        If pi.AllowNull Then
                            p.Value = DBNull.Value
                        End If
                    End If
                End If
            End If

            cmd.Parameters.Add(p)
        Next
    End Sub



    Private Shared Sub ReadOne(Of T As New)(filler As FillObject, reader As IDataReader, data As FillStatus(Of T))
        If reader.Read Then
            FillData(reader, filler, data.Value)
            data.FillResult = FillResultEnum.DataFound
            data.Timestamp = Now.Ticks
            If TypeOf (data.Value) Is IEntityBase Then
                CType(data.Value, IEntityBase).Loaded = Now.Ticks
                CType(data.Value, IEntityBase).FillResult = FillResultEnum.DataFound
            End If
        End If
    End Sub

    Friend Shared Sub FillData(ByVal reader As IDataReader, ByVal filler As FillObject, ByVal data As Object)
        filler(reader, data)
    End Sub

    Private Shared ReadOnly PadLock As New Object
    Public Shared ReadOnly Fillers As New Dictionary(Of Integer, DataFiller)
    Private Shared ReadOnly Match As New System.Text.RegularExpressions.Regex("^.*?(?:(?= from(?!.*?(?=from)))|$)", Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Singleline Or RegexOptions.Compiled) 'Match anything all the way to the last FROM. 

    Private Shared Function GetFiller(ByVal commandInfo As CommandInfo, ByVal dataReader As IDataReader, ByVal t As Type) As FillObject

        Dim key = GetHashCodeForCommand(commandInfo, t, dataReader)
        If Not Fillers.ContainsKey(key) Then
            SyncLock PadLock
                If Not Fillers.ContainsKey(key) Then
                    Fillers(key) = New DataFiller(dataReader, t, commandInfo.CommandText.Contains("*"c))
                End If
            End SyncLock
        End If
        Return AddressOf Fillers(key).FillObject
    End Function

    Private Shared Function GetHashCodeForCommand(ByVal commandInfo As CommandInfo, ByVal t As Type, ByVal dataReader As IDataReader) As Integer
        Dim cmd = Match.Match(commandInfo.CommandText.ToLower).Value.Replace(" ", "")
        Return (cmd & t.ToString & "-" & dataReader.FieldCount).GetHashCode
    End Function

    Friend Delegate Sub FillObject(reader As IDataReader, data As Object)
    Private Delegate Sub HandleReader(Of T As New)(filler As FillObject, reader As IDataReader, data As FillStatus(Of T))
    Private Delegate Sub HandleReaderForValueType(Of T As Structure)(reader As IDataReader, data As ICollection(Of T))
#End Region

End Class

Public Interface IDataStore
    Sub Exec(Of T As New)(command As CommandInfo, data As List(Of T))
    Sub Exec(Of T As New)(command As CommandInfo, data As FillStatus(Of T))
    Sub Exec(command As CommandInfo, data As Object)
    Sub Exec(command As CommandInfo)
    Sub Exec(Of T As Structure)(command As CommandInfo, data As ICollection(Of T), colName As String)
    Sub GetStream(Of T As {New, WillDisposeThoseForU})(command As CommandInfo, data As T)
End Interface

Public Class DataStoreInstance
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
End Class