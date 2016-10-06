Imports LazyFramework.CQRS.Command

Namespace Availability
    Public Interface ICommandAvailability
        Function IsAvailable(entity As Object) As Boolean
    End Interface
End Namespace