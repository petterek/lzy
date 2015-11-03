


Public MustInherit Class WebhookEvent
End Class


Public MustInherit Class Subscriber

    Public Id As Guid
    Public Url As String
    Public Headers As Dictionary(Of String, String)

End Class

Public Class WebHookSubscriber
    Inherits Subscriber
    Public Type As String = System.Net.WebRequestMethods.Http.Post

End Class


Public Class WebhookServer
    Public Shared Sub AddHook(e As WebhookEvent, subscriber As Subscriber)
        'Write Subscriber info to the e.GetType().Name() & "subscriberinfo.json"


    End Sub

    Public Shared Sub RemoveSubscriber(e As WebhookEvent, subscriber As Subscriber)

    End Sub

    Public Shared Sub Trigger(e As WebhookEvent)
        'load file..


    End Sub


    Public Shared Function AllEvents() As IEnumerable(Of WebhookEvent)

    End Function

End Class