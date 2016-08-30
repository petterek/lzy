Imports LazyFramework.CQRS.Command

Namespace Availability
    Public Interface ICommandAvailability
        Function IsAvailable(profile As ExecutionProfile.IExecutionProfile, cmd As IAmACommand, entity As Object) As Boolean
    End Interface
End Namespace