
Imports System.Text
Imports RabbitMQ.Client

Public Class ReciveEventHandler
    Implements IHandleEvent
    
    Public Shared Sub HandleAllEventHandler(e As Object)

        Dim message = "Hello world"
        Dim factory As New RabbitMQ.Client.ConnectionFactory With {.HostName = "localhost"}


        Using con = factory.CreateConnection()
            Using channel = con.CreateModel()
                Dim body = Encoding.UTF8.GetBytes(message)
                Dim properties = channel.CreateBasicProperties()
                properties.Persistent = True
                channel.BasicPublish("","task_queue", properties, body)
            End Using
        End Using    
    End Sub
    



End Class
