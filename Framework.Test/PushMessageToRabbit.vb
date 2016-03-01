Imports LazyFramework.EventHandling
Imports NUnit.Framework

<testfixture> Public Class PushMessageToRabbit

    <Test> Public sub MessageIsSendtToQ

        LazyFramework.Runtime.Context.Current = New LazyFramework.Runtime.WinThread
        LazyFramework.EventHandling.EventHub.Publish(Runtime.Context.Current.CurrentUser,New TestEvent With {.MyMessage = "klshjdkfjsdkfj"})
   End sub
    
    Public Class TestEvent
        Inherits LazyFramework.EventHandling.EventBase
        
        Public MyMessage As String

    End Class

    Public Class SomeEventHandler 
        Implements LazyFramework.EventHandling.IHandleEvent

        Public Shared sub EventHandler(e As TestEvent)
            
        End sub

    End Class
End Class