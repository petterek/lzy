Imports System.Linq
Imports System.Reflection
Imports System.Security.Principal
Imports System.Threading
Imports LazyFramework.Utils

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>

Public Class EventHub
    Private Shared _handlers As Dictionary(Of Type, List(Of MethodInfo))
    Private Shared _publishers As Dictionary(Of Type, List(Of MethodInfo))

    Private Shared ReadOnly PadLock As New Object

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property AllHandlers() As Dictionary(Of Type, List(Of MethodInfo))
        Get
            If _handlers Is Nothing Then
                SyncLock PadLock
                    If _handlers Is Nothing Then
                        _handlers = New ActionHandlerMapper(Reflection.AllTypes.IsAssignableFrom(Of IHandleEvent).
                                                              Union(Reflection.AllTypes.NameEndsWith("EventHandler")).ToList().
                                                              AllMethods.
                                                              NameEndsWith("EventHandler").
                                                              IsSub.SignatureIs(GetType(Object)), True)
                    End If
                End SyncLock
            End If
            Return _handlers
        End Get
    End Property



    Public Shared Function Publishers(Of T)() As IEnumerable(Of MethodInfo)

        If _publishers Is Nothing Then
            SyncLock PadLock
                If _publishers Is Nothing Then
                    Dim temp As New Dictionary(Of Type, List(Of MethodInfo))
                    For Each [class] In Reflection.FindAllClassesOfTypeInApplication(GetType(IPublishEvent))
                        For Each method In [class].GetMethods
                            Dim getCustomAttribute = method.GetCustomAttribute(Of PublishesEventOfTypeAttribute)()
                            If getCustomAttribute IsNot Nothing Then
                                For Each t In getCustomAttribute.Types
                                    If Not temp.ContainsKey(t) Then
                                        temp(t) = New List(Of MethodInfo)
                                    End If
                                    temp(t).Add(method)
                                Next
                            End If
                        Next
                    Next
                    _publishers = temp
                End If
            End SyncLock
        End If

        If _publishers.ContainsKey(GetType(T)) Then
            Return _publishers(GetType(T))
        Else
            Return New List(Of MethodInfo)
        End If

    End Function


    ''' <summary>
    ''' Publish an event to the application. 
    ''' </summary>
    ''' <param name="event"></param>
    ''' <remarks></remarks>
    Public Shared Sub Publish([event] As IAmAnEvent, Optional runAsync As Boolean = False)
        Dim key As System.Type = [event].GetType
        While key IsNot Nothing
            If AllHandlers.ContainsKey(key) Then
                For Each MethodInfo In AllHandlers(key)
                    Try
                        WrapAndFire(MethodInfo, [event], runAsync)
                    Catch ex As Exception

                    End Try
                Next
            End If
            key = key.BaseType
        End While
        [event].ActionComplete()

        Logging.Logger.Write([event])

    End Sub

    Private Shared Sub WriteToEventLog(ByVal message As String)

        If Not EventLog.SourceExists("Lazyframework") Then
            EventLog.CreateEventSource("LazyFramework", "Application")
        End If
        Dim ELog As New EventLog("Application", ".", "LazyFramework")
        ELog.WriteEntry(message, EventLogEntryType.Error, 0, 1S)



    End Sub

    Private Shared Sub WrapAndFire(ByVal methodInfo As MethodInfo, ByVal [event] As IAmAnEvent, ByVal runAsync As Boolean)
        'Wrapper den alltid.
        'ikke nødvendig, men da blir det i hvertfall likt.. :) 
        Dim doAsync As Boolean = False

        If methodInfo.GetCustomAttribute(GetType(RunAsyncAttribute)) IsNot Nothing Then
            doAsync = True
        Else
            If runAsync Then
                doAsync = True
            End If
        End If

        Dim w = New ThreadWrapper(methodInfo, [event], Runtime.Context.Current.CurrentUser)
        If doAsync Then
            Dim waitCallback As WaitCallback = New WaitCallback(AddressOf w.Start)
            ThreadPool.QueueUserWorkItem(waitCallback)
        Else
            w.Start(Nothing)
        End If
    End Sub


    Private Class ThreadWrapper
        Private ReadOnly _M As MethodInfo
        Private ReadOnly _E As IAmAnEvent
        Private ReadOnly _Principal As IPrincipal

        Public Sub New(m As MethodInfo, e As IAmAnEvent, principal As IPrincipal)
            _M = m
            _E = e
            _Principal = principal
        End Sub

        Public Sub Start(stateInfo As Object)
            If Not _Principal Is Nothing Then
                Thread.CurrentPrincipal = _Principal
            End If
            Try
                _M.Invoke(Nothing, {_E})
            Catch ex As Exception
                WriteToEventLog(ex.Message) 'Writes to the server log of the machine. Eventhandling failuers
                'Logging.Log.Error(_E, ex)
                Throw
            End Try
        End Sub
    End Class

End Class

Public Class RunAsyncAttribute
    Inherits Attribute

End Class
