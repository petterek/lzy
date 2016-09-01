Imports LazyFramework.CQRS.Command

Namespace Availability
    Public Interface ICommandAvilability
        Function IsAvailable(profile As Object, cmd As IAmACommand, entity As Object) As Boolean
        Function IsAvailable(profile As Object, cmd As IAmACommand) As Boolean
    End Interface
End NameSpace