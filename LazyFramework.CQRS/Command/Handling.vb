Imports System.Reflection
Imports LazyFramework.CQRS.Security
Imports LazyFramework.EventHandling
Imports LazyFramework.Logging
Imports LazyFramework.Pipeline
Imports LazyFramework.Utils

Namespace Command
    Public Class Handling
        Implements IPublishEvent

        Private Shared ReadOnly PadLock As New Object
        Private Shared _handlers As Dictionary(Of Type, List(Of MethodInfo))
        Private Shared _commadList As Dictionary(Of String, Type)


        Private Shared ReadOnly Property PipeLine As CommandPipeLine
            Get
                Static pipe As New CommandPipeLine
                Return pipe
            End Get
        End Property



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
        Private Shared ReadOnly Property AllHandlers() As Dictionary(Of Type, List(Of MethodInfo))
            Get
                If _handlers Is Nothing Then
                    Dim temp As Dictionary(Of Type, List(Of MethodInfo)) = Nothing
                    SyncLock PadLock
                        If _handlers Is Nothing Then
                            temp = FindHandlers.FindAllHandlerDelegates(Of IHandleCommand, IAmACommand)(False)
                        End If
                        _handlers = temp
                    End SyncLock
                End If
                Return _handlers
            End Get
        End Property

        ''' <summary>
        ''' Executes a command by finding the mapping to the type of command passed in. 
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks>Any command can have only 1 handler. An exception will be thrown if there is found more than one for any given command. </remarks>
        Public Shared Sub ExecuteCommand(command As IAmACommand)

            If AllHandlers.ContainsKey(command.GetType) Then

                If command.ExecutionProfile Is Nothing then
                    command.SetProfile(LazyFramework.ClassFactory.GetTypeInstance(Of LazyFramework.CQRS.ExecutionProfile.IExecutionProfileProvider).GetExecutionProfile)
                End If
                PipeLine.Execute(Of IAmACommand, Object)(Function()
                                                             If Not CanUserRunCommand(CType(command, CommandBase)) Then
                                                                 EventHub.Publish(New NoAccess(command))
                                                                 Throw New ActionSecurityAuthorizationFaildException(command, command.ExecutionProfile.User)
                                                             End If

                                                             Validation.Handling.ValidateAction(command)

                                                             Try
                                                                 Dim temp = AllHandlers(command.GetType)(0).Invoke(Nothing, {command})
                                                                 If temp IsNot Nothing Then
                                                                     command.SetResult(Transform.Handling.TransformResult(command, temp))
                                                                 End If

                                                                 'If TypeOf (command) Is ActionBase Then
                                                                 '    DirectCast(command, ActionBase).OnActionComplete()
                                                                 'End If

                                                             Catch ex As TargetInvocationException
                                                                 Logging.Log.Error(command, ex)
                                                                 Throw ex.InnerException
                                                             Catch ex As Exception
                                                                 Logging.Log.Error(command, ex)
                                                                 Throw
                                                             End Try
                                                             Return Nothing
                                                         End Function,command)
            Else
                Dim notImplementedException = New NotImplementedException(command.ActionName)
                Logging.Log.Error(command, notImplementedException)
                Throw notImplementedException
            End If

            command.ActionComplete()

            Log.Write(command,LogLevelEnum.System)
        End Sub

        Public Shared Function IsCommandAvailable(cmd As CommandBase) As Boolean
            Return cmd.IsAvailable()
        End Function

        Public Shared Function CanUserRunCommand(cmd As CommandBase) As Boolean
            If cmd.GetInnerEntity Is Nothing Then
                Return ActionSecurity.Current.UserCanRunThisAction(cmd.ExecutionProfile, cmd)
            Else
                Return ActionSecurity.Current.UserCanRunThisAction(cmd.ExecutionProfile, cmd, cmd.GetInnerEntity)
            End If
        End Function


    End Class

    Friend Class CommandPipeLine
        Inherits Pipeline.Base

        Public Sub New()
            Me.AddPreExecuteStep(New IsCommandAvailableStep)
        End Sub

    End Class

    Friend Class IsCommandAvailableStep
        Implements IPipelineStep
        
        Public Sub ExecuteStep(Of TContext)(context As TContext) Implements IPipelineStep.ExecuteStep
            If TypeOf (context) Is CommandBase Then
                Dim command = DirectCast(context,IAmACommand)
                If Not command.IsAvailable Then
                    EventHub.Publish(New NoAccess(command))
                    Throw New ActionIsNotAvailableException(command, command.ExecutionProfile.User)
                End If
            End If
        End Sub
    End Class
End Namespace
