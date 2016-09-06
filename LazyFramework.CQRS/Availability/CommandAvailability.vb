Imports LazyFramework.CQRS.Command

Namespace Availability
    Public MustInherit Class CommandAvailability(Of TCommand, TEntity)
        Implements ICommandAvailability

        Public Function IsAvailable(entity As Object) As Boolean Implements ICommandAvailability.IsAvailable
            Return IsAvailable(DirectCast(entity, TEntity))
        End Function

        Public MustOverride Function IsAvailable(entity As TEntity) As Boolean

    End Class
End Namespace