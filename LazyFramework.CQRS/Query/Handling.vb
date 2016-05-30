Imports System.Reflection
Imports LazyFramework.CQRS.Monitor
Imports LazyFramework.CQRS.Security

Namespace Query
    Public Class Handling

        Private Shared ReadOnly PadLock As New Object

        Private Shared _queryList As New Dictionary(Of String, Type)
        Public Shared ReadOnly Property QueryList As Dictionary(Of String, Type)
            Get
                'If _queryList Is Nothing Then
                '    SyncLock PadLock
                '        If _queryList Is Nothing Then
                '            Dim temp As New Dictionary(Of String, Type)
                '            For Each t In Reflection.FindAllClassesOfTypeInApplication(GetType(IAmAQuery))
                '                If t.IsAbstract Then Continue For 'Do not map abstract queries. 

                '                Dim c As IAmAQuery = CType(Setup.ClassFactory.CreateInstance(t), IAmAQuery)
                '                temp.Add(c.ActionName, t)
                '            Next
                '            _queryList = temp
                '        End If
                '    End SyncLock
                'End If

                Return _queryList
            End Get
        End Property


        Public Shared Sub AddQueryHandler(Of T As IAmAQuery)(handler As Func(Of T, Object))
            Dim c As IAmAQuery = CType(Setup.ClassFactory.CreateInstance(Of T), IAmAQuery)
            If _queryList.ContainsKey(c.ActionName) Then
                Throw New AllreadeyConfiguredException(GetType(T))
            Else
                _queryList(c.ActionName) = c.GetType()
            End If

            _handlers.Add(GetType(T), New Func(Of Object, Object)(Function(q) handler(CType(q, T))))

        End Sub

        Public Shared Sub ClearHandlers()
            _queryList = New Dictionary(Of String, Type)
            _handlers = New Dictionary(Of Type, Func(Of Object, Object))
        End Sub


        Private Delegate Function InternalHandler(query As Object) As Object

        Private Shared _handlers As New Dictionary(Of Type, Func(Of Object, Object))
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property Handlers() As Dictionary(Of Type, Func(Of Object, Object))
            Get
                Return _handlers
            End Get
        End Property



        Private Shared ReadOnly InstanceLock As New Object
        Private Shared ReadOnly TypeInstanceCache As New Dictionary(Of Type, Object)


        Public Shared Function ExecuteQuery(profile As ExecutionProfile.IExecutionProfile, q As IAmAQuery) As Object

            If Not Availability.Handler.ActionIsAvailable(profile, q) Then
                profile.Publish(profile.User, New NoAccess(q))
                Throw New ActionIsNotAvailableException(q, profile.User)
            End If

            If Setup.ActionSecurity IsNot Nothing Then
                If Not Setup.ActionSecurity.UserCanRunThisAction(profile, q) Then
                    Dim actionSecurityAuthorizationFaildException As ActionSecurityAuthorizationFaildException = New ActionSecurityAuthorizationFaildException(q, profile.User)
                    Logging.Log.Error(q, actionSecurityAuthorizationFaildException)
                    Throw actionSecurityAuthorizationFaildException
                End If
            End If


            Validation.Handling.ValidateAction(profile, q)
            Dim handler As Func(Of Object, Object) = Nothing

            'Standard queryhandling. 1->1 mapping 
            If _handlers.TryGetValue(q.GetType, handler) Then
                Try
                    q.HandlerStart()
                    Dim result As Object = handler(q)

                    result = Transform.Handling.TransformResult(profile, q, result)
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
