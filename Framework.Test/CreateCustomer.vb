Imports System.Security.Principal
Imports LazyFramework.CQRS.Command
Imports LazyFramework.EventHandling
Imports LazyFramework.Logging


Public Class CustomerCommandBase
    Inherits CommandBase(Of Customer)

    Public Id As Guid

   
End Class

Public Class CreateCustomerCommand
    Inherits CustomerCommandBase

    Public Name As String
    Public Address As String
End Class

Public Class UpdateCustomerNameCommand
    Inherits CustomerCommandBase

    Public NewName As String

End Class


Public Class Customer

    Property Id As Guid
    Property Name As String
    Property Address As String

    Public Phone As String
    Public Country As String
    Public CustomerLevel As Integer

End Class

Public Class CustomerCreatedEvent
    Inherits EventHandling.EventBase

    Public Id As Guid
    Public Name As String
    Public Address As String
    Public CustomerLevel As Integer

    Public Sub New(customer As Customer)
        Id = customer.Id
        Name = customer.Name
        Address = customer.Address
        CustomerLevel = customer.CustomerLevel
    End Sub


End Class


Public Class CustomerAlteredEvent
    Inherits EventHandling.EventBase

    Public ReadOnly ID As Guid

    Public Sub New(id As Guid)
        Me.ID = id
    End Sub

End Class

Public Class HandleCustomerEvents
    Implements EventHandling.IHandleEvent

    Public Shared Sub CustomerCreated(e As CustomerCreatedEvent)

    End Sub
End Class

Public Class CommandHandler
    Implements LazyFramework.CQRS.Command.IHandleCOmmand, EventHandling.IPublishEvent

    <ThreadStatic> Public Shared CustomerRepository As New Dictionary(Of Guid, Customer)

    <EventHandling.PublishesEventOfType(GetType(CustomerCreatedEvent))>
    Public Shared Sub CreateCustomer(cust As CreateCustomerCommand)
        'Save customer to databse

        Dim customer As New Customer
        customer.Id = cust.Id
        customer.Name = cust.Name
        customer.Address = cust.Address
        customer.CustomerLevel = 0

        CommandHandler.CustomerRepository.Add(customer.Id, customer)

        EventHandling.EventHub.Publish(Runtime.Context.Current.CurrentUser, New CustomerCreatedEvent(customer))

    End Sub

    Public Shared Sub AlterName(cmd As UpdateCustomerNameCommand)
        CustomerRepository(cmd.Id).Name = cmd.NewName
        EventHandling.EventHub.Publish(Runtime.Context.Current.CurrentUser, New CustomerAlteredEvent(cmd.Id))
    End Sub

End Class


Public Class EventHandler
    Implements IHandleEvent

    Public Shared Sub SaveCustomer(custom As CustomerCreatedEvent)

    End Sub


End Class


Public Class Persistdata
    Implements ILogWriter

    Public Sub Write(info As LogInfo) Implements ILogWriter.Write
        Throw New NotImplementedException()
    End Sub

    Public Function Level() As LogLevelEnum Implements ILogWriter.Level
        Return LogLevelEnum.Verbose
    End Function

    Public Property EventList As New List(Of LazyFramework.CQRS.Command.IAmACommand)

End Class
