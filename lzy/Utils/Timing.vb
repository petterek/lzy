Namespace Utils

    Public Class TimingInfos
        Inherits Dictionary(Of String, TimingInfo)

        Private padLock As New Object
        Private padLock2 As New Object

        Public Overloads Sub Add(ti As TimingInfo)
            If Not ContainsKey(ti.Key) Then
                SyncLock padLock
                    If Not ContainsKey(ti.Key) Then
                        MyBase.Add(ti.Key, ti)
                    End If
                End SyncLock
            Else
                For Each l In ti.List
                    SyncLock padLock2
                        If Not Me(ti.Key).List.Contains(l)
                            Me(ti.Key).List.Add(l)
                        End If
                    End SyncLock
                Next
            End If
        End Sub

    End Class

    Public Class Timing

        Public Timings As New TimingInfos

        Public Property DoLog As Boolean = True

    End Class

    Public Class InlineTimer
        Implements IDisposable

        Private _Result As TimingInfo
        Private ReadOnly _Stopwatch As Stopwatch

        Private ReadOnly _Key As String
        Private ReadOnly _TimingInfos As TimingInfos

        Public Sub New(ByVal key As String, ByVal timingInfos As TimingInfos)
            _Stopwatch = New Stopwatch
            _Stopwatch.Start()

            _Key = key
            _TimingInfos = timingInfos
            _Result = New TimingInfo(_Key)
        End Sub



#Region "IDisposable Support"
        Private _DisposedValue As Boolean

        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._DisposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                _Stopwatch.Stop()
                _Result.List.Add(_Stopwatch.ElapsedTicks)
                _TimingInfos.Add(_Result)

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me._DisposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    Public Class TimingInfo
        Public Key As String
        Public List As IList(Of Long)

        Public Sub New(ByVal k As String)
            Key = k
            List = New List(Of Long)
        End Sub
    End Class
End Namespace
