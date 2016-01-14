Namespace Utils.Json
    Public Interface IReader
        Function Read() As Char
        Function Read(count As Integer) As String
        Function PeekToBuffer() As Char
        Function Current() As Char
        Sub ClearBuffer()
        ReadOnly Property Buffer As String
        ReadOnly Property BufferPeek As String
        ReadOnly Property BufferPreLastPeek As String
        Function Peek() As Char

        ReadOnly Property Position As Long

    End Interface
End NameSpace