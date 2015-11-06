Namespace Utils

    Public Class ResponseThread
        Private ReadOnly _allMessages As New ThreadMessageCollection

        Public Sub New()
            Timer.DoLog = False
            'Logging.Logger.LogLevel = 0

        End Sub

        Public ReadOnly Property HasErrors() As Boolean
            Get
                Return Errors.Count > 0
            End Get
        End Property

        Public ReadOnly Property HasWarnings() As Boolean
            Get
                Return Warings.Count > 0
            End Get
        End Property

        Public ReadOnly Property Info() As IList(Of ThreadMessage)
            Get
                Return _allMessages.FindAll(Function(e) (e IsNot Nothing) AndAlso e.Severity = ThreadMessageSeverityEnum.Info)
            End Get
        End Property

        Public ReadOnly Property Warings() As IList(Of ThreadMessage)
            Get
                Return _allMessages.FindAll(Function(e) (e IsNot Nothing) AndAlso e.Severity = ThreadMessageSeverityEnum.Warning)
            End Get
        End Property

        Public ReadOnly Property Errors() As IList(Of ThreadMessage)
            Get
                Return _allMessages.FindAll(Function(e) (e IsNot Nothing) AndAlso e.Severity = ThreadMessageSeverityEnum.Error)
            End Get
        End Property

        Public ReadOnly Property AllMessages() As IEnumerable(Of ThreadMessage)
            Get
                Return _allMessages
            End Get
        End Property

        Public Sub Add(ByVal br As ThreadMessage)
            _allMessages.Add(br)
        End Sub


        Public ReadOnly Property Timer As Timing
            Get
                If Not ThreadHasKey(Timerforthread) Then
                    ThreadStore.Add(Timerforthread, New Timing)
                End If
                Return CType(ThreadStore(Timerforthread), Timing)
            End Get
        End Property
        

        Public Sub Clear()
            _allMessages.Clear()
            ThreadStore.Remove(Timerforthread)
        End Sub

        Public Sub ClearMessages()
            _allMessages.Clear()
        End Sub

        Private Const ResponseThreadSlot As String = "01F7441D-FCA0-48A5-AE29-CBD6E5F27C9C"
        Private Shared PadLock As New Object
        Private Const Timerforthread As String = "TimerForThread"

        Public Shared Property Current() As ResponseThread
            Get
                If Not ThreadHasKey(ResponseThreadSlot) Then
                    SyncLock PadLock
                        If Not ThreadHasKey(ResponseThreadSlot) Then
                            SetThreadValue(ResponseThreadSlot, New ResponseThread)
                        End If
                    End SyncLock
                End If
                Return GetThreadValue(Of ResponseThread)(ResponseThreadSlot)
            End Get
            Set(value As ResponseThread)
                SetThreadValue(ResponseThreadSlot, value)
            End Set
        End Property

        Private Const MyStoreName As String = "FC2ED2E8-47BA-4374-80C4-CD51ADE709E5"

        Public Shared Function GetThreadValue(Of TT)(ByVal name As String) As TT
            If Not ThreadStore.ContainsKey(name) Then Throw New KeyNotFoundException

            Return CType(ThreadStore(name), TT)

        End Function
        Public Shared Sub SetThreadValue(ByVal name As String, ByVal value As Object)
            ThreadStore(name) = value
        End Sub
        Public Shared Sub ClearThreadValues()
            ThreadStore.Clear()
        End Sub
        Public Shared Function ThreadHasKey(name As String) As Boolean
            Return ThreadStore.ContainsKey(name)
        End Function

        Private Shared Function ThreadStore() As IDictionary(Of String, Object)

            If Runtime.Context.Current Is Nothing Then
                Throw New NotConfiguredException("Please set LazyFramework.Runtime.Context.Current to a valid context")
            End If

            Return Runtime.Context.Current.Storage

        End Function

        Public Overrides Function ToString() As String
            Dim ret As String = ""
            For Each s In AllMessages

                ret += s.FieldName & ":"
                For Each i In s.Messages
                    ret += i & vbCrLf
                Next
            Next
            Return ret
        End Function

    End Class
End Namespace


