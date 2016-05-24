Imports System.Reflection
Imports LazyFramework.CQRS.Security

Namespace Command
    Public Class Handling

        Private Shared ReadOnly PadLock As New Object
        Private Shared _handlers As ActionHandlerMapper
        Private Shared _commadList As Dictionary(Of String, Type)

        Public Shared Property UserAutoDiscoveryForHandlers As Boolean = True

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CommandList As Dictionary(Of String, Type)
            Get
                If _commadList Is Nothing Then
                    SyncLock PadLock
                        If _commadList Is Nothing Then
                            Dim temp As New Dictionary(Of String, Type)
                            For Each t In Reflection.FindAllClassesOfTypeInApplication(GetType(IAmACommand))
                                If t.IsAbstract Then Continue For 'Do not map abstract commands. 

                                Dim c As IAmACommand = CType(Activator.CreateInstance(t), IAmACommand)
                                temp.Add(c.ActionName, t)
                            Next
                            _commadList = temp
                        End If
                    End SyncLock
                End If

                Return _commadList
            End Get
        End Property


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property AllHandlers() As ActionHandlerMapper
            Get
                If _handlers Is Nothing Then
                    SyncLock PadLock
                        If _handlers Is Nothing Then
                            If UserAutoDiscoveryForHandlers Then
                                _handlers = New ActionHandlerMapper(
                                 Reflection.AllTypes.IsAssignableFrom(Of IHandleCommand).Union(Reflection.AllTypes.NameEndsWith("CommandHandler")).ToList().
                                 AllMethods.
                                 NameEndsWith("Handler").
                                 IsSub.
                                 SignatureIs(GetType(Object)).ToList, False)
                            Else
                                Throw New Exception("You must add handlers for your commands")
                            End If
                        End If
                    End SyncLock
                End If
                Return _handlers
            End Get
        End Property

        Public Shared Sub Add(Of TCommand)(run As Action(Of TCommand))

        End Sub


        Private Shared ReadOnly instanceLock As New Object
        Private Shared ReadOnly TypeInstanceCache As New Dictionary(Of Type, Object)


        ''' <summary>
        ''' Executes a command by finding the mapping to the type of command passed in. 
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks>Any command can have only 1 handler. An exception will be thrown if there is found more than one for any given command. </remarks>
        Public Shared Sub ExecuteCommand(profile As ExecutionProfile.IExecutionProfile, command As IAmACommand)

            If AllHandlers.ContainsKey(command.GetType) Then

                EntityResolver.Handling.ResolveEntity(profile, command)

                If Not Availability.Handler.CommandIsAvailable(profile, command) Then
                    profile.Publish(profile.User, New NoAccess(command))
                    Throw New ActionIsNotAvailableException(command, profile.User)
                End If

                If Not CanUserRunCommand(profile, CType(command, CommandBase)) Then
                    profile.Publish(profile.User, New NoAccess(command))
                    Throw New ActionSecurityAuthorizationFaildException(command, profile.User)
                End If

                Validation.Handling.ValidateAction(profile, command)

                Try
                    Dim methodInfo = AllHandlers(command.GetType)(0)

                    If Not TypeInstanceCache.ContainsKey(methodInfo.DeclaringType) Then
                        SyncLock instanceLock
                            If Not TypeInstanceCache.ContainsKey(methodInfo.DeclaringType) Then
                                TypeInstanceCache(methodInfo.DeclaringType) = Setup.ClassFactory.CreateInstance(methodInfo.DeclaringType)
                            End If
                        End SyncLock
                    End If

                    Dim temp = methodInfo.Invoke(TypeInstanceCache(methodInfo.DeclaringType), {command})
                    If temp IsNot Nothing Then
                        command.SetResult(Transform.Handling.TransformResult(profile, command, temp))
                    End If

                Catch ex As TargetInvocationException
                    Logging.Log.Error(command, ex)
                    Throw ex.InnerException
                Catch ex As Exception
                    Logging.Log.Error(command, ex)
                    Throw
                End Try
            Else
                Dim implementedException = New NotImplementedException(command.ActionName)
                Logging.Log.Error(command, implementedException)
                Throw implementedException
            End If

            command.ActionComplete()

        End Sub

        Public Shared Function IsCommandAvailable(profile As ExecutionProfile.IExecutionProfile, cmd As CommandBase) As Boolean
            Return Availability.Handler.CommandIsAvailable(profile, cmd)
        End Function

        Public Shared Function CanUserRunCommand(profile As ExecutionProfile.IExecutionProfile, cmd As CommandBase) As Boolean
            If cmd.GetInnerEntity Is Nothing Then
                Return Setup.ActionSecurity.UserCanRunThisAction(profile, cmd)
            Else
                Return Setup.ActionSecurity.UserCanRunThisAction(profile, cmd, cmd.GetInnerEntity)
            End If
        End Function


    End Class

End Namespace
