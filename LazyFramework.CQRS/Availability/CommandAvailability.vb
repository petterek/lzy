Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Availability
    Public MustInherit Class CommandAvailability(Of TCommand, TEntity)
        Implements ICommandAvilability

        Friend Function IsAvailable(profile As ExecutionProfile.IExecutionProfile, command As IAmACommand, entity As Object) As Boolean Implements ICommandAvilability.IsAvailable
            Return IsAvailable(profile, DirectCast(command, TCommand),DirectCast(entity, TEntity))            
        End Function
        
        Public Function IsAvailable(profile As IExecutionProfile, command As IAmACommand) As Boolean Implements ICommandAvilability.IsAvailable
            Return IsAvailable(profile, DirectCast(command, TCommand))
        End Function

        Public MustOverride Function IsAvailable(profile As ExecutionProfile.IExecutionProfile,command As TCommand) As Boolean
        Public MustOverride Function IsAvailable(profile As ExecutionProfile.IExecutionProfile,command As TCommand, entity As TEntity) As Boolean

    End Class
End NameSpace