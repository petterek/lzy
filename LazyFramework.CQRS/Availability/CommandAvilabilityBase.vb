Imports LazyFramework.CQRS.Command

Namespace Availability
    Public MustInherit Class CommandAvailabilityBase(Of T)
        Implements ICommandAvailability

        Public MustOverride Function IsAvailable(entity As Object) As Boolean Implements ICommandAvailability.IsAvailable

        Public Function IsAvailable(entity As T) As Boolean
            Return IsAvailable(entity)
        End Function

    End Class
End Namespace