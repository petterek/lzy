Imports System.Linq
Imports System.Reflection
Imports System.Security.Principal
Imports System.Threading


''' <summary>
''' 
''' </summary>
''' <remarks></remarks>

Public Class EventHub
    Private Shared _handlers As New Dictionary(Of Type, List(Of Action(Of Object, Object)))

    Private Shared ReadOnly PadLock As New Object

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property AllHandlers() As Dictionary(Of Type, List(Of Action(Of Object, Object)))
        Get
            Return _handlers
        End Get
    End Property

    Public Shared Sub RegisterHandler(Of T)(handler As Action(Of Object, T))
        Dim handlerLIst As List(Of Action(Of Object, Object))
        If Not _handlers.ContainsKey(GetType(T)) Then
            _handlers.Add(GetType(T), New List(Of Action(Of Object, Object)))
        End If
        handlerLIst = _handlers(GetType(T))
        handlerLIst.Add(Sub(sender, e) handler(sender, DirectCast(e, T)))
    End Sub



    Public Shared Sub Publish(sender As Object, [event] As Object)
        Dim key As System.Type = [event].GetType
        While key IsNot Nothing
            If AllHandlers.ContainsKey(key) Then
                For Each MethodInfo In AllHandlers(key)
                    Try
                        MethodInfo(sender, [event])
                    Catch ex As Exception

                    End Try
                Next
            End If
            key = key.BaseType
        End While
    End Sub

    Private Shared Sub WriteToEventLog(ByVal message As String)

        If Not EventLog.SourceExists("Lazyframework") Then
            EventLog.CreateEventSource("LazyFramework", "Application")
        End If
        Dim ELog As New EventLog("Application", ".", "LazyFramework")
        ELog.WriteEntry(message, EventLogEntryType.Error, 0, 1S)



    End Sub

End Class

