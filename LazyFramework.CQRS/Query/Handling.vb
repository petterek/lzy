Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports LazyFramework.Utils
Imports LazyFramework.CQRS.Monitor
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

    <Extension> Public Module Extensions

        <Extension> Public Function Execute(Of T)(obj As IAmAQuery) As T

            Return CType(Handling.ExecuteQuery(obj), T)

        End Function

    End Module

    Public Class Handling

        Private Shared ReadOnly PadLock As New Object
        Private Shared _handlers As Dictionary(Of Type, List(Of MethodInfo))


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



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property Handlers() As Dictionary(Of Type, List(Of MethodInfo))
            Get
                If _handlers Is Nothing Then
                    SyncLock PadLock
                        If _handlers Is Nothing Then
                            Dim temp As Dictionary(Of Type, List(Of MethodInfo)) = FindHandlers.FindAllHandlerDelegates(Of IHandleQuery, IAmAQuery)(False)
                            _handlers = temp
                        End If
                    End SyncLock
                End If
                Return _handlers
            End Get
        End Property

        Public Shared Function ExecuteQuery(q As IAmAQuery) As Object

            If Not ActionSecurity.Current.UserCanRunThisAction(q.User, q) Then
                Dim actionSecurityAuthorizationFaildException As ActionSecurityAuthorizationFaildException = New ActionSecurityAuthorizationFaildException(q, q.User)
                Logging.Log.Error(q,actionSecurityAuthorizationFaildException )
                Throw actionSecurityAuthorizationFaildException
            End If

            Validation.Handling.ValidateAction(q)

            'Standard queryhandling. 1->1 mapping
            If Handlers.ContainsKey(q.GetType) Then
                Try
                    Dim ctx = ExecutionContext.GetContextForAction(q)
                    If ctx IsNot Nothing Then
                        ctx.StartSession(q)
                    End If

                    q.HandlerStart()
                    Dim invoke As Object = Handlers(q.GetType)(0).Invoke(Nothing, {q})

                    EventHub.Publish(New QueryExecuted(q))

                    Dim transformResult As Object = Nothing
                    If invoke IsNot Nothing Then
                        transformResult = Transform.Handling.TransformResult(q, invoke)
                    End If

                    'If TypeOf (q) Is ActionBase Then
                    '    DirectCast(q, ActionBase).OnActionComplete()
                    'End If

                    q.ActionComplete()

                    If ctx IsNot Nothing Then
                        ctx.EndSession
                    End If

                    Dim info As Monitor.MonitorMaxTimeAttribute = CType(Attribute.GetCustomAttribute(q.GetType, GetType(Monitor.MonitorMaxTimeAttribute)), MonitorMaxTimeAttribute)
                    If info Is Nothing OrElse New TimeSpan(q.EndTimeStamp - q.HandlerStartTimeStamp).Milliseconds >= info.MaxTimeInMs Then
                        Dim mon As New QueryMonitorData
                        mon.StartTime = q.HandlerStartTimeStamp
                        mon.EndTime = q.EndTimeStamp
                        mon.HandlerName = Handlers(q.GetType)(0).Name
                        mon.ActionName = q.GetType().FullName
                        mon.Params = q
                        mon.User() = q.User.Identity.Name
                        Monitor.Handling.AddToQueue(mon)
                    End If

                    

                    Return transformResult

                Catch ex As TargetInvocationException
                    Logging.Log.Error(q, ex)
                    Throw ex.InnerException
                Catch ex As AggregateException

                Catch ex As Exception
                    Logging.Log.Error(q, ex)
                    Throw
                End Try

                'Else
                'If MultiHandlers.ContainsKey(q.GetType) Then
                '    Dim handler = MultiHandlers(q.GetType)
                '    Dim target As Global.LazyFramework.CQRS.Query.IParalellQuery = CType(handler.CreateInstance, IParalellQuery)
                '    target.InnerQuery = q

                '    Dim p = Runtime.Context.Current.CurrentUser
                '    Dim s = Runtime.Context.Current.Storage

                '    handler.Methods.AsParallel.ForAll(Sub(m)
                '                                          Threading.Thread.CurrentPrincipal = p
                '                                          Dim ldss As LocalDataStoreSlot = Thread.GetNamedDataSlot(Runtime.Constants.StoreName)
                '                                          Thread.SetData(ldss, s)
                '                                          m.Invoke(target, {})
                '                                          Thread.CurrentPrincipal = Nothing
                '                                          Thread.FreeNamedDataSlot(Runtime.Constants.StoreName)
                '                                      End Sub)
                '    Return target.InnerResult
                'End If
                'EventHub.Publish(New HandlerNotFound(q))
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
