Imports System.IO

Namespace Utils.Json
    Public Class ReadStream
        Implements IReader

        Private ReadOnly _StreamReader As StreamReader
        Private ReadOnly internaleBuffer As New Queue(Of Char)

        Public Sub New(s As StreamReader)
            _StreamReader = s
        End Sub

        Public Function Peek() As Char Implements IReader.Peek
            If internaleBuffer.Count > 0 Then
                Return internaleBuffer(internaleBuffer.Count - 1)
            Else
                Return ChrW(_StreamReader.Peek())
            End If
        End Function

        Public Function PeekToBuffer() As Char Implements IReader.PeekToBuffer
            Dim ret = ChrW(_StreamReader.Read)
            internaleBuffer.Enqueue(ret)
            Return ret
        End Function

        Public Function Read() As Char Implements IReader.Read
            If internaleBuffer.Count = 0 Then
                Return ChrW(_StreamReader.Read)
            Else
                Return internaleBuffer.Dequeue
            End If
        End Function

        Public ReadOnly Property Buffer As String Implements IReader.Buffer
            Get
                Dim ret As New Text.StringBuilder
                While internaleBuffer.Count > 1
                    ret.Append(internaleBuffer.Dequeue)
                End While
                Return ret.ToString
            End Get
        End Property

        Public ReadOnly Property BufferPeek As String Implements IReader.BufferPeek
            Get
                Return New String(internaleBuffer.ToArray)
            End Get
        End Property

        Public Function Current() As Char Implements IReader.Current
            If internaleBuffer.Count = 0 Then
                Return ChrW(0)
            End If
            Return internaleBuffer(internaleBuffer.Count - 1)
        End Function

        Public Sub ClearBuffer() Implements IReader.ClearBuffer
            While internaleBuffer.Count > 1
                internaleBuffer.Dequeue()
            End While
        End Sub

        Public ReadOnly Property BufferPreLastPeek As String Implements IReader.BufferPreLastPeek
            Get

                If internaleBuffer.Count < 2 Then
                    Return ""
                End If
                Dim ret As New Text.StringBuilder
                For x = 0 To internaleBuffer.Count - 2
                    ret.Append(internaleBuffer(x))
                Next
                
                Return ret.ToString

            End Get
        End Property
    End Class
End NameSpace