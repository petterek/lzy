Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Availability
    Public MustInherit Class CommandAvailabilityBase(Of T)
        Implements ICommandAvailability

        Public MustOverride Function IsAvailable(profile As Object, action As T, entity As Object) As Boolean

        Public Function IsAvailable(profile As Object, cmd As IAmACommand, entity As Object) As Boolean Implements ICommandAvailability.IsAvailable
            Return IsAvailable(profile, DirectCast(cmd, T), entity)
        End Function
    End Class
End Namespace