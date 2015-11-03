

Public Class Log
    Private Shared ReadOnly PadLock As New Object
    Private Shared _writers As Dictionary(Of Type, List(Of ILogWriter))

    Public Shared sub AddWriter (of T)(writer As ILogWriter)
        If Not Writers.ContainsKey(gettype(T)) Then
            Writers.Add(GetType(t), New List(Of ILogWriter))
        End If
        Writers(GetType(t)).Add(writer)
    End sub

    Public Shared ReadOnly Property Writers As Dictionary(Of Type, List(Of ILogWriter))
        Get
            If _writers Is Nothing Then
                SyncLock PadLock
                    If _writers Is Nothing Then
                        _writers = New Dictionary(Of Type, List(Of ILogWriter))
                    End If
                End SyncLock
            End If
            Return _writers
        End Get
    End Property


    Public Shared Sub Write (of T )(data As T, Optional level As LogLevelEnum = LogLevelEnum.Error)
        Dim loginfo As New LogInfo
        loginfo.LogData = data
        loginfo.DataType = GetType(T).FullName

        For Each writerType In Writers.Keys
            If writerType.IsAssignableFrom(GetType(t)) Then
                For Each w In Writers(writerType)
                    If (w.Level And level) <> 0 Then
                        w.Write(loginfo)
                    End If
                Next
            End If
        Next
    End Sub

End Class

Public Enum LogLevelEnum
    [Verbose] = -1
    [Error] = 1
    [Warning] = 2
    [Info] = 4
    [System] = 8
End Enum