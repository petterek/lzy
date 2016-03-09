Imports System.Threading

Namespace Runtime
    Public Class Context

        Private Shared _current As IContext
        Public Shared Property Current As IContext
            Get
                If CtxStore.ContainsKey(Thread.CurrentThread.ManagedThreadId) Then
                    SyncLock StoreLock
                        If CtxStore.ContainsKey(Thread.CurrentThread.ManagedThreadId) Then
                            Return CtxStore(Thread.CurrentThread.ManagedThreadId).Peek
                        End If
                    End SyncLock
                End If
                Return _current
            End Get
            Set(value As IContext)
                _current = value
            End Set
        End Property

        Private Shared PadLock As New Object
        Private Shared _ctxStore As IDictionary(Of Integer, Stack(Of IContext))
        Private Shared ReadOnly Property CtxStore As IDictionary(Of Integer, Stack(Of IContext))
            Get
                If _ctxStore Is Nothing Then
                    SyncLock PadLock
                        If _ctxStore Is Nothing Then
                            _ctxStore = New Dictionary(Of Integer, Stack(Of IContext))
                        End If
                    End SyncLock
                End If
                Return _ctxStore
            End Get
        End Property

        Private Shared StoreLock As New Object
        Public Shared Sub AddOverrideContext(ctx As IContext)
            SyncLock StoreLock
                If Not CtxStore.ContainsKey(Thread.CurrentThread.ManagedThreadId) Then
                    CtxStore(Thread.CurrentThread.ManagedThreadId) = New Stack(Of IContext)
                End If
                CtxStore(Thread.CurrentThread.ManagedThreadId).Push(ctx)
            End SyncLock

        End Sub


        Public Shared Sub RemoveContext()
            SyncLock StoreLock
                CtxStore(Thread.CurrentThread.ManagedThreadId).Pop()
                If not CtxStore(Thread.CurrentThread.ManagedThreadId).Any then
                    CtxStore.Remove(Thread.CurrentThread.ManagedThreadId)
                End If
            End SyncLock
        End Sub


    End Class

End Namespace
