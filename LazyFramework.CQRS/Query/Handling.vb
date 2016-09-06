Imports System.Reflection
Imports LazyFramework.CQRS.Monitor
Imports LazyFramework.CQRS.Security

Namespace Query
    Public Class Handling

        Private Shared _queryList As New Dictionary(Of String, Type)
        Public Shared ReadOnly Property QueryList As Dictionary(Of String, Type)
            Get
                Return _queryList
            End Get
        End Property


        ''Public Shared Sub AddQueryHandler(Of T As IAmAQuery)(handler As Func(Of T, Object))

        Public Shared Sub AddQueryHandler(Of T As IAmAQuery)(handler As Func(Of Object, T, ExecutionProfile))

            If _queryList.ContainsKey(GetType(T).FullName) Then
                Throw New AllreadeyConfiguredException(GetType(T))
            Else
                _queryList(GetType(T).FullName) = GetType(T)
            End If

            _handlers.Add(GetType(T), New Func(Of Object, IAmAQuery, ExecutionProfile)(Function(o, a) handler(o, CType(a, T))))

            'New Func(Of Object, Object)(Function(q) handler(CType(q, T)))
        End Sub

        Public Shared Sub ClearHandlers()
            _queryList = New Dictionary(Of String, Type)
            _handlers = New Dictionary(Of Type, Func(Of Object, IAmAQuery, ExecutionProfile))
        End Sub


        Private Delegate Function InternalHandler(query As Object) As Object

        Private Shared _handlers As New Dictionary(Of Type, Func(Of Object, IAmAQuery, ExecutionProfile))
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property Handlers() As Dictionary(Of Type, Func(Of Object, IAmAQuery, ExecutionProfile))
            Get
                Return _handlers
            End Get
        End Property



        Public Shared Function ExecuteQuery(profile As Object, q As IAmAQuery) As Object

            Dim handler As Func(Of Object, IAmAQuery, ExecutionProfile) = Nothing
            'Standard queryhandling. 1->1 mapping 
            If _handlers.TryGetValue(q.GetType, handler) Then

                Dim context = handler(profile, q)

                If context.ActionIsAvailable IsNot Nothing AndAlso Not context.ActionIsAvailable.IsAvailable(q) Then
                    Throw New ActionIsNotAvailableException(q, profile)
                End If

                If context.ActionSecurity IsNot Nothing Then
                    If Not context.ActionSecurity.UserCanRunThisAction() Then
                        Dim actionSecurityAuthorizationFaildException As ActionSecurityAuthorizationFaildException = New ActionSecurityAuthorizationFaildException(q, profile)
                        Logging.Log.Error(q, actionSecurityAuthorizationFaildException)
                        Throw actionSecurityAuthorizationFaildException
                    End If
                End If


                If context.ValidateAction IsNot Nothing Then context.ValidateAction.InternalValidate(q)

                Try
                        q.HandlerStart()
                        Dim result As Object = context.ActionHandler(q)

                        result = Transform.Handling.TransformResult(context, result)
                        Sorting.Handler.SortResult(q, result)

                        q.ActionComplete()

                        Return result

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



    End Class
End Namespace
