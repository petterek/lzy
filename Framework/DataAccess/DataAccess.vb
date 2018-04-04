
Imports System.Data.Common
Imports LazyFramework.Utils
Imports System.Reflection
Imports System.Xml

''' <summary>
'''
''' </summary>
''' <remarks></remarks>
Public Class DataAccessFactory
    ''' <summary>
    ''' Gives you the currently configured dataacces type.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DataAccess() As IDataAccess
        'Her kan vi legge inn logikk for å instansiere riktig type data klasse avhenging av fks verdier i appconfig.
        If ClassFactory.ContainsKey(Of IDataAccess)() Then
            Return ClassFactory.GetTypeInstance(Of IDataAccess)()
        Else
            If _dataAccess Is Nothing Then
                _dataAccess = CType(Activator.CreateInstance(Type.GetType(LazyFrameworkConfiguration.Current.DatastoreType)), IDataAccess)
            End If
            Return _dataAccess
        End If
    End Function


#Region "PreparedCommand"

    Private Shared Sub AddCount(ByVal dbName As String, ByVal c As CommandInfo)
        ResponseThread.Current.Querys.Add(dbName, c)
    End Sub

    Public Shared Function FillObject(ByVal sourceName As String, ByVal [object] As IORDataObject, ByVal command As CommandInfo, ByVal ParamArray parameters As Object()) As Boolean
        Dim w As New Stopwatch
        If parameters.Length <> 0 Then
            For x = 0 To parameters.Length - 1
                command.Parameters.Values(x).Value = parameters(x)
            Next
        End If
        w.Start()
        If DataAccess.FillObject(sourceName, [object], DataAccess.CreateCommand(command)) Then
            w.Stop()
            [object].Loaded = Now.Ticks
            command.CommandDuration = w.ElapsedTicks
            AddCount(sourceName, command)

            [object].OnFillComplete(New FillCompleteEventargs(command))
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function FillObject(ByVal sourceName As String, ByVal [object] As Queue(Of IORDataObject), ByVal command As CommandInfo, ByVal ParamArray parameters As Object()) As Boolean
        Dim w As New Stopwatch

        If parameters.Length <> 0 Then
            For x = 0 To parameters.Length - 1
                command.Parameters.Values(x).Value = parameters(x)
            Next
        End If

        w.Start()

        If DataAccess.FillObject(sourceName, [object], DataAccess.CreateCommand(command)) Then
            w.Stop()
            command.CommandDuration = w.ElapsedTicks
            AddCount(sourceName, command)
            For Each o As IORDataObject In [object]
                o.Loaded = Now.Ticks
                o.OnFillComplete(New FillCompleteEventargs(command))
            Next
            Return True
        Else
            Return False
        End If
    End Function


    Public Shared Function UpdateObject(ByVal sourceName As String, ByVal [object] As IORDataObject, ByVal command As CommandInfo) As Boolean
        Dim res As Boolean
        Try
            Dim w As New Stopwatch

            FirePlugins([object], command.TypeOfCommand, res, PluginExecutionPointEnum.Pre)

            w.Start()
            res = DataAccess.UpdateObject(sourceName, [object], DataAccess.CreateCommand(command))
            w.Stop()
            command.CommandDuration = w.ElapsedTicks
            If res Then
                [object].Loaded = Now.Ticks
                AddCount(sourceName, command)
                [object].FillResult = FillResultEnum.UpdateOK
                [object].OnFillComplete(New FillCompleteEventargs(command))
            End If
            FirePlugins([object], command.TypeOfCommand, res, PluginExecutionPointEnum.Post)
            Return res

        Catch ex As Exception
            ResponseThread.Current.Add(New ThreadMessage(ThreadMessageSeverityEnum.Error, "Database error", ex.Message & vbCrLf & command.CommandText))
            If Not LazyFrameworkConfiguration.Current.SupressErrors Then Throw
            Return False
        End Try

    End Function

    Public Shared Function ExecuteScalar(ByVal sourceName As String, ByVal command As CommandInfo, ByVal ParamArray params As Object()) As Object
        Dim w As New Stopwatch

        If params.Length <> 0 Then
            For x = 0 To params.Length - 1
                command.Parameters.Values(x).Value = params(x)
            Next
        End If

        w.Start()
        Dim ret = DataAccess.ExecuteScalar(sourceName, DataAccess.CreateCommand(command), params)
        w.Stop()
        command.CommandDuration = w.ElapsedTicks
        AddCount(sourceName, command)

        Return ret
    End Function

    Public Shared Function ExecuteXmlReader(ByVal sourceName As String, ByVal command As CommandInfo, ByVal ParamArray params As Object()) As XmlDocument
        Dim w As New Stopwatch

        If params.Length <> 0 Then
            For x = 0 To params.Length - 1
                command.Parameters.Values(x).Value = params(x)
            Next
        End If

        w.Start()
        Dim ret = DataAccess.ExecuteXmlReader(sourceName, DataAccess.CreateCommand(command))
        w.Stop()
        command.CommandDuration = w.ElapsedTicks
        AddCount(sourceName, command)

        Return ret
    End Function
    Public Shared Sub ExecuteScalarAsync(ByVal sourceName As String, ByVal command As CommandInfo)

        Dim exec As New ExecuteScalarDelegate(AddressOf DataAccess.ExecuteScalar)
        exec.BeginInvoke(sourceName, DataAccess.CreateCommand(command), Sub(r) r.AsyncWaitHandle.Close(), Nothing)
    End Sub

    Private Delegate Function ExecuteScalarDelegate(ByVal sourceName As String, ByVal command As DbCommand) As Object


    Public Shared Function ExecuteCommand(ByVal sourceName As String, ByVal command As CommandInfo, ByVal ParamArray params As Object()) As Boolean
        Dim w As New Stopwatch

        If params.Length <> 0 Then
            For x = 0 To params.Length - 1
                command.Parameters.Values(x).Value = params(x)
            Next
        End If

        w.Start()
        If DataAccess.ExecuteCommand(sourceName, DataAccess.CreateCommand(command), params) Then
            command.CommandDuration = w.ElapsedTicks
            AddCount(sourceName, command)
            Return True
        End If
        Return False
    End Function

#End Region

#Region "Non IORDataobject"
    Public Shared Function FillObjectGeneric(Of T As {New})(sourceName As String, cmd As CommandInfo, res As List(Of T)) As Boolean
        Using dataCommand = DataAccess.CreateCommand(cmd)
            Using reader = DataAccess.ExecuteDatareader(sourceName, dataCommand)
                Dim props As New Dictionary(Of String, PropertyInfo)()
                Dim tType = GetType(T)

                If reader.HasRows() Then
                    Dim n As String
                    For x = 0 To reader.FieldCount - 1
                        n = reader.GetName(x)
                        props.Add(n, tType.GetProperty(n, BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Instance))
                    Next

                    While reader.Read
                        Dim value As New T
                        For Each s In props
                            If Not s.Value Is Nothing Then
                                s.Value.SetValue(value, If(TypeOf (reader(s.Key)) Is DBNull, Nothing, reader(s.Key)), Nothing)
                            ElseIf value.GetType().IsPrimitive
                                value = CType(reader(s.Key), T)
                            Else
                                Continue For
                            End If
                        Next
                        If tType.IsAssignableFrom(GetType(IORDataObject)) Then
                            CType(value, IORDataObject).FillResult = FillResultEnum.DataFound
                            CType(value, IORDataObject).Loaded = Now.Ticks
                        End If
                        res.Add(value)
                    End While
                End If
            End Using
            dataCommand.Connection.Close()
        End Using
        Return True
    End Function
#End Region

    Private Shared _plugInCollection As Dictionary(Of String, IDataModificationPlugin)
    Private Shared ReadOnly O As New Object
    Private Shared _dataAccess As IDataAccess

    Private Shared Sub FirePlugins(ByVal obj As IORDataObject, ByVal commandType As CommandInfoCommandTypeEnum, ByVal result As Boolean, ByVal point As PluginExecutionPointEnum)
        Dim plugIn As IDataModificationPlugin
        Dim t As Type

        If _plugInCollection Is Nothing Then
            SyncLock O
                _plugInCollection = New Dictionary(Of String, IDataModificationPlugin)
                For Each p As ModificationObjectPluginElement In LazyFrameworkConfiguration.Current.DataModificationPlugins
                    t = Type.GetType(p.Type)
                    If t Is Nothing Then
                        Dim e As New PluginNotFoundException()
                        e.PluginType = p.Type
                    End If
                    plugIn = CType(Activator.CreateInstance(t), IDataModificationPlugin)
                    plugIn.ExecutionPoint = p.ExecutionPoint
                    plugIn.PluginName = p.Name
                    Try
                        plugIn.InitPlugin(p.params)
                        _plugInCollection.Add(p.Name, plugIn)
                    Catch ex As Exception
                    End Try
                Next
            End SyncLock
        End If

        If TypeOf (obj) Is IDoNotFirePlugin Then Return

        For Each plugIn In _plugInCollection.Values
            If (plugIn.ExecutionPoint And point) <> 0 Then
                Try
                    plugIn.ExecuteTask(obj, commandType, result)
                Catch ex As Exception
                    ResponseThread.Current.Add(New ThreadMessage(ThreadMessageSeverityEnum.Error, "Plugin:" & plugIn.PluginName, ex.Message))
                    If Not LazyFrameworkConfiguration.Current.SupressErrors Then Throw
                End Try
            End If
        Next
    End Sub


    Public Shared Function GetDataTable(ByVal sourceName As String, ByVal sqlQuery As String, ByVal commandType As CommandType, ByVal ParamArray parameters As Object()) As DataTable
        Return DataAccess.GetDataTable(sourceName, sqlQuery, commandType, parameters)
    End Function

    Public Shared Function ExecuteCommand(ByVal sourceName As String, ByVal command As DbCommand, ByVal ParamArray params As Object()) As Boolean
        DataAccess.ExecuteCommand(sourceName, command, params)
    End Function

End Class


Public Class CommandInfo
    Private _CommandText As String

    Public Overridable Property CommandText() As String
        Get
            Return _CommandText
        End Get
        Set(ByVal value As String)
            _CommandText = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Overridable Sub AddParameters()

    End Sub

    Public Overridable ReadOnly Property CommandType() As CommandType
        Get
            Return CommandType.Text
        End Get
    End Property

    Private _TypeOfCommand As CommandInfoCommandTypeEnum

    Public Overridable Property TypeOfCommand() As CommandInfoCommandTypeEnum
        Get
            Return _TypeOfCommand
        End Get
        Set(ByVal value As CommandInfoCommandTypeEnum)
            _TypeOfCommand = value
        End Set
    End Property

    Private _Parameters As ParmeterInfoCollection

    Public ReadOnly Property Parameters() As ParmeterInfoCollection
        Get
            If _Parameters Is Nothing Then
                _Parameters = New ParmeterInfoCollection
            End If
            Return _Parameters
        End Get
    End Property


    Private _DataFormatters As Dictionary(Of String, IValueFormatter)

    Public ReadOnly Property DataFormatters() As Dictionary(Of String, IValueFormatter)
        Get
            If _DataFormatters Is Nothing Then
                _DataFormatters = New Dictionary(Of String, IValueFormatter)
            End If
            Return _DataFormatters
        End Get
    End Property

    Private _CommandDuration As Long
    Public Property CommandDuration() As Long
        Get
            Return _CommandDuration
        End Get
        Set(ByVal value As Long)
            _CommandDuration = value
        End Set
    End Property

    Private _Count As Long = 1
    Public Property Count() As Long
        Get
            Return _Count
        End Get
        Set(ByVal value As Long)
            _Count = value
        End Set
    End Property

    Public Property CommandQuery() As Linq.Expressions.Expression


    Public Overrides Function ToString() As String
        Dim res As New System.Text.StringBuilder

        For Each p In Me.Parameters
            res.AppendLine(String.Format("Declare @{0}  {1} = {2}", p.Value.Name, p.Value.DbType.ToString, p.Value.Value))
        Next

        res.AppendLine(CommandText)
        Return res.ToString

    End Function

End Class

Public Class ParameterInfo
    Private _Name As String
    Private _DBType As DbType
    Private _Size As Integer
    Private _IsNullable As Boolean
    Private _Value As Object

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property DbType() As DbType
        Get
            Return _DBType
        End Get
        Set(ByVal value As DbType)
            _DBType = value
        End Set
    End Property

    Public Property Size() As Integer
        Get
            Return _Size
        End Get
        Set(ByVal value As Integer)
            _Size = value
        End Set
    End Property

    Public Property IsNullable() As Boolean
        Get
            Return _IsNullable
        End Get
        Set(ByVal value As Boolean)
            _IsNullable = value
        End Set
    End Property

    Public Property Value() As Object
        Get
            Return _Value
        End Get
        Set(ByVal value As Object)
            _Value = value
        End Set
    End Property

End Class

Public Enum CommandInfoCommandTypeEnum As Integer
    Unknown = 0
    Read = 1
    Create = 2
    Update = 4
    Delete = 8
End Enum

Public Interface IValueFormatter
    Function FormatValue(ByVal value As Object) As Object
End Interface

