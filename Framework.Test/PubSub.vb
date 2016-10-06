Imports LazyFramework.EventHandling
Imports NUnit.Framework

<TestFixture> Public Class PubSub

    <Test> Public Sub PublishedEventIsRecived()

        LazyFramework.EventHandling.EventHub.RegisterHandler(Of PersonGotNewName)(AddressOf ClassThatHandlesEventHandler.NewNameEventHandler)
        LazyFramework.EventHandling.EventHub.RegisterHandler(Of PersonGotNewName)(AddressOf ClassThatHandlesEventHandler.HandlesTooEventHandler)


        EventPublisher.PublishEvent()

        Assert.IsNotNull(ClassThatHandlesEventHandler.Hmm)
        Assert.AreEqual(ClassThatHandlesEventHandler.Hmm.Name, "Petter")
        Assert.AreEqual(2, ClassThatHandlesEventHandler.Counter)

    End Sub


End Class

Public Class EventPublisher
    Implements IPublishEvent

    Public Shared Sub PublishEvent()

        EventHub.Publish(Nothing, New PersonGotNewName With {.Name = "Petter"})

    End Sub
End Class


Public Class PersonGotNewName
    Inherits EventBase

    Public Property Name() As String

End Class

Public Class AnotherHandler
    Implements IHandleEvent

    Public Shared Sub jsdklfj(p As PersonGotNewName)

    End Sub

End Class


Public Class ClassThatHandlesEventHandler
    
    Public Shared Hmm As PersonGotNewName
    Public Shared Counter As Integer = 0

    Public Shared Sub NewNameEventHandler(sender As Object, param As PersonGotNewName)
        Hmm = param
        Counter += 1
    End Sub

    Public Shared Sub HandlesTooEventHandler(sender As Object, param As PersonGotNewName)
        Hmm = param
        Counter += 1
    End Sub


End Class
