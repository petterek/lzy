Imports System.Threading

Public Class ConfigStore
    Private _writerThread As Thread

    Public MustInherit Class ConfigId
        Public Id As Guid
        Public Sub New()
            Id = Guid.NewGuid()
        End Sub

    End Class

    Public Class WriteInfo
        Public Filename As String
        Public Data As ConfigId
    End Class
    Public Sub New()
        _writerThread = New System.Threading.Thread(Sub(e) FileWriter())
        _writerThread.Start()

    End Sub

    Public Function LoadInfo(Of T As ConfigId)(hookInfo As WebhookEvent) As IEnumerable(Of T)

    End Function

    Public Sub Addinfo(hookInfo As WebhookEvent, o As ConfigId)
        Dim toAdd As New WriteInfo() With {.Filename = hookInfo.GetType().Name & ".json", .Data = o}
        ToWrite.Enqueue(toAdd)
        _writerThread.Interrupt()
    End Sub

    Public Sub RemovoInfo(fileName As String, o As ConfigId)

    End Sub

    Private Cache As New Concurrent.ConcurrentDictionary(Of String, ConfigId)
    Private ToWrite As New Concurrent.ConcurrentQueue(Of WriteInfo)

    Private Sub FileWriter()
        Dim writeInfo As WriteInfo
        While AppRunnig
            While ToWrite.TryDequeue(writeInfo)
                'Load the file from disk.
                'Add to collection
                'Save to file
                'Swap the Cache
            End While
            
            Thread.Sleep(Timeout.Infinite)
        End While

    End Sub

    Public Property AppRunnig As Boolean

End Class