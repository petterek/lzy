Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Availability
    Public MustInherit Class CommandAvailability(Of TCommand, TEntity)
        Implements ICommandAvailability

        Public Function IsAvailable(profile As ExecutionProfile.IExecutionProfile, command As IAmACommand, entity As Object) As Boolean Implements ICommandAvailability.IsAvailable
            Return IsAvailable(profile, DirectCast(command, TCommand), DirectCast(entity, TEntity))
        End Function

        Public MustOverride Function IsAvailable(profile As ExecutionProfile.IExecutionProfile, command As TCommand, entity As TEntity) As Boolean

    End Class
End Namespace