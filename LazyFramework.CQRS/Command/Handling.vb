Imports System.Reflection
Imports LazyFramework.CQRS.Security

Namespace Command
    Public Class Handling

        Private Shared _handlers As New Dictionary(Of Type, Func(Of Object, Object, Object))
        Private Shared _commadList As New Dictionary(Of String, Type)

        Public Shared Sub ClearMapping()
            _handlers = New Dictionary(Of Type, Func(Of Object, Object, Object))
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
        Private Shared ReadOnly Property AllHandlers() As Dictionary(Of Type, Func(Of Object, Object, Object))
            Get
                Return _handlers
            End Get
        End Property

        ''' <summary>
        ''' Adds a mapping to a function for the given command. 
        ''' </summary>
        ''' <typeparam name="TCommand"></typeparam>
        ''' <param name="run"></param>
        Public Shared Sub AddCommandHandler(Of TCommand As IAmACommand)(run As Action(Of Object, TCommand))
            Dim cmdInstance = Setup.ClassFactory.CreateInstance(Of TCommand)

            _handlers.Add(GetType(TCommand), New Func(Of Object, Object, Object)(Function(ctx As Object, cmd As Object)
                                                                                     run(ctx, CType(cmd, TCommand))
                                                                                     Return Nothing
                                                                                 End Function
                                                                 ))

        End Sub


        ''' <summary>
        ''' Executes a command by finding the mapping to the type of command passed in. 
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks>Any command can have only 1 handler. An exception will be thrown if there is found more than one for any given command. </remarks>
        Public Shared Sub ExecuteCommand(profile As Object, command As IAmACommand)


            EntityResolver.Handling.ResolveEntity(command)



            If Not Availability.Handler.CommandIsAvailable(profile, command) Then
                Throw New ActionIsNotAvailableException(command, profile)
            End If

            If Not CanUserRunCommand(profile, CType(command, CommandBase)) Then
                Throw New ActionSecurityAuthorizationFaildException(command, profile)
            End If

            Validation.Handling.ValidateAction(profile, command)

            Try
                Dim temp = AllHandlers(command.GetType)(profile, command)
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
            command.ActionComplete()
        End Sub

        Public Shared Function IsCommandAvailable(profile As Object, cmd As CommandBase) As Boolean
            Return Availability.Handler.CommandIsAvailable(profile, cmd)
        End Function

        Public Shared Function CanUserRunCommand(profile As Object, cmd As CommandBase) As Boolean
            If Setup.ActionSecurity Is Nothing Then
                Return True
            End If
            If cmd.GetInnerEntity Is Nothing Then
                Return Setup.ActionSecurity.UserCanRunThisAction(profile, cmd)
            Else
                Return Setup.ActionSecurity.UserCanRunThisAction(profile, cmd, cmd.GetInnerEntity)
            End If
        End Function


    End Class

End Namespace
