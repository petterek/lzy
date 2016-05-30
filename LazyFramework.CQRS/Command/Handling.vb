Imports System.Reflection
Imports LazyFramework.CQRS.Security

Namespace Command
    Public Class Handling

        Private Shared ReadOnly PadLock As New Object
        Private Shared _handlers As New Dictionary(Of Type, Func(Of Object, Object))
        Private Shared _commadList As New Dictionary(Of String, Type)

        Public Shared Property UserAutoDiscoveryForHandlers As Boolean = True

        Public Shared Sub ClearMapping()
            _handlers = New Dictionary(Of Type, Func(Of Object, Object))
            _commadList = New Dictionary(Of String, Type)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property CommandList As Dictionary(Of String, Type)
            Get
                Return _commadList
            End Get
        End Property


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property AllHandlers() As Dictionary(Of Type, Func(Of Object, Object))
            Get
                Return _handlers
            End Get
        End Property

        Public Shared Sub AddCommandHandler(Of TCommand As IAmACommand)(run As Action(Of TCommand))
            Dim cmdInstance = Setup.ClassFactory.CreateInstance(Of TCommand)

            If _commadList.ContainsKey(cmdInstance.ActionName) Then
                Throw New CommandAllreadyMappedExcpetion(GetType(TCommand))
            End If
            _commadList.Add(cmdInstance.ActionName(), cmdInstance.GetType())

            _handlers.Add(GetType(TCommand), New Func(Of Object, Object)(Function(cmd As Object)
                                                                             run(CType(cmd, TCommand))
                                                                             Return Nothing
                                                                         End Function
                                                                 ))

        End Sub
        Public Shared Sub AddCommandHandler(Of TCommand As IAmACommand)(run As Func(Of TCommand, Object))
            Dim cmdInstance = Setup.ClassFactory.CreateInstance(Of TCommand)

            If _commadList.ContainsKey(cmdInstance.ActionName) Then
                Throw New CommandAllreadyMappedExcpetion(GetType(TCommand))
            End If
            _commadList.Add(cmdInstance.ActionName(), cmdInstance.GetType())

            _handlers.Add(GetType(TCommand), New Func(Of Object, Object)(Function(cmd As Object) run(CType(cmd, TCommand))))

        End Sub

        ''' <summary>
        ''' Executes a command by finding the mapping to the type of command passed in. 
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks>Any command can have only 1 handler. An exception will be thrown if there is found more than one for any given command. </remarks>
        Public Shared Sub ExecuteCommand(profile As ExecutionProfile.IExecutionProfile, command As IAmACommand)

            If AllHandlers.ContainsKey(command.GetType) Then

                EntityResolver.Handling.ResolveEntity(command)

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
                    Dim temp = AllHandlers(command.GetType)(command)
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
