Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports LazyFramework.Utils
Imports LazyFramework.CQRS.Monitor
Imports LazyFramework.CQRS.Security
Imports LazyFramework.EventHandling

Namespace Query


    Public Class QueryMonitorData
        Implements IMonitorData

        Public Property HandlerName As String Implements IMonitorData.Name
        Public ActionName As String

        Public Sub New()
            StartTime = Now.Ticks
        End Sub

        Public Params As Object

        Public ReadOnly Property Took() As Long Implements IMonitorData.Took
            Get
                Return New TimeSpan(EndTime - StartTime).Milliseconds
            End Get
        End Property

        Public Property EndTime As Long Implements IMonitorData.EndTime
        Public Property StartTime As Long Implements IMonitorData.StartTime

        Public Property User As String Implements IMonitorData.User
    End Class

    Public Class Handling

        Private Shared ReadOnly PadLock As New Object


        Private Shared _queryList As Dictionary(Of String, Type)
        Public Shared ReadOnly Property QueryList As Dictionary(Of String, Type)
            Get
                If _queryList Is Nothing Then
                    SyncLock PadLock
                        If _queryList Is Nothing Then
                            Dim temp As New Dictionary(Of String, Type)
                            For Each t In Reflection.FindAllClassesOfTypeInApplication(GetType(IAmAQuery))
                                If t.IsAbstract Then Continue For 'Do not map abstract queries. 

                                Dim c As IAmAQuery = CType(Activator.CreateInstance(t), IAmAQuery)
                                temp.Add(c.ActionName, t)
                            Next
                            _queryList = temp
                        End If
                    End SyncLock
                End If

                Return _queryList
            End Get
        End Property



        Private Shared _handlers As ActionHandlerMapper
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property Handlers() As ActionHandlerMapper
            Get
                If _handlers Is Nothing Then
                    SyncLock PadLock
                        If _handlers Is Nothing Then
                            Dim list = LazyFramework.Reflection.AllTypes.IsAssignableFrom(Of IHandleQuery).Union(Reflection.AllTypes.NameEndsWith("QueryHandler"))
                            Dim temp As ActionHandlerMapper = FindHandlers.FindAllHandlerDelegates(list,GetType(IAmAQuery),False)
                            _handlers = temp
                        End If
                    End SyncLock
                End If
                Return _handlers
            End Get
        End Property



        Private Shared ReadOnly InstanceLock As New Object
        Private Shared ReadOnly TypeInstanceCache As New Dictionary(Of Type, Object)


        Public Shared Function ExecuteQuery(profile As ExecutionProfile.IExecutionProfile, q As IAmAQuery) As Object


            If Not ActionSecurity.Current.UserCanRunThisAction(profile, q) Then
                Dim actionSecurityAuthorizationFaildException As ActionSecurityAuthorizationFaildException = New ActionSecurityAuthorizationFaildException(q, profile.User)
                Logging.Log.Error(q, actionSecurityAuthorizationFaildException)
                Throw actionSecurityAuthorizationFaildException
            End If

            Validation.Handling.ValidateAction(profile, q)
            Dim handler As MethodInfo = Nothing

            'Standard queryhandling. 1->1 mapping 
            If Handlers.TryFindHandler(q.GetType, handler) Then
                Try
                    Dim ctx = ExecutionContext.GetContextForAction(q)
                    If ctx IsNot Nothing Then
                        ctx.StartSession(q)
                    End If

                    q.HandlerStart()

                    If not TypeInstanceCache.ContainsKey(handler.DeclaringType) Then
                        SyncLock instanceLock
                            If not TypeInstanceCache.ContainsKey(handler.DeclaringType) Then
                                TypeInstanceCache(handler.DeclaringType) = ClassFactory.Construct(handler.DeclaringType)
                            End If
                        End SyncLock
                    End If
                    Dim invoke As Object = handler.Invoke(TypeInstanceCache(handler.DeclaringType), {q})

                    EventHub.Publish(New QueryExecuted(q))


                    invoke = Transform.Handling.TransformResult(profile, q, invoke)
                    Sorting.Handler.SortResult(q, invoke)

                    q.ActionComplete()

                    If ctx IsNot Nothing Then
                        ctx.EndSession()
                    End If

                    Dim info = CType(Attribute.GetCustomAttribute(q.GetType, GetType(Monitor.MonitorMaxTimeAttribute)), MonitorMaxTimeAttribute)
                    If info IsNot Nothing AndAlso New TimeSpan(q.EndTimeStamp - q.HandlerStartTimeStamp).Milliseconds >= info.MaxTimeInMs Then
                        Dim mon As New QueryMonitorData
                        mon.StartTime = q.HandlerStartTimeStamp
                        mon.EndTime = q.EndTimeStamp
                        mon.HandlerName = Handlers(q.GetType)(0).Name
                        mon.ActionName = q.GetType().FullName
                        mon.Params = q
                        mon.User() = profile.User.Identity.Name
                        Monitor.Handling.AddToQueue(mon)
                    End If

                    Return invoke

                Catch ex As TargetInvocationException
                    Logging.Log.Error(q, ex)
                    Throw ex.InnerException
                Catch ex As AggregateException
                    Logging.Log.Error(q, ex)
                    Throw
                Catch ex As Exception
                    Logging.Log.Error(q, ex)
                    Throw
                End Try
            End If

            Throw New NotSupportedException("Query handler not found")

        End Function



        Private Shared _multihandlers As Dictionary(Of Type, FindHandlers.MethodList)

        Public Shared ReadOnly Property MultiHandlers() As Dictionary(Of Type, FindHandlers.MethodList)
            Get
                If _multihandlers Is Nothing Then
                    _multihandlers = FindHandlers.FindAllMultiHandlers(Of IParalellQuery, IAmAQuery)()
                End If
                Return _multihandlers
            End Get
        End Property
    End Class
End Namespace
