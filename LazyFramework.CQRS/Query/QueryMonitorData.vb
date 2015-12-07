Imports LazyFramework.CQRS.Monitor

Namespace Query
    Public Class QueryMonitorData
        Implements IMonitorData

        Public Property HandlerName As String Implements IMonitorData.Name
        Public ActionName As String

        Public Sub New()
            StartTime = Now.Ticks
        End Sub

        Public Params As Object

        Public ReadOnly Property Took() As Long Implements IMonitorData.Took
            Get
                Return New TimeSpan(EndTime - StartTime).Milliseconds
            End Get
        End Property

        Public Property EndTime As Long Implements IMonitorData.EndTime
        Public Property StartTime As Long Implements IMonitorData.StartTime

        Public Property User As String Implements IMonitorData.User
    End Class
End NameSpace