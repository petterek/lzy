Namespace Utils.Json
    Public Interface IReader
        Function Read() As Char
        Function PeekToBuffer() As Char
        Function Current() As Char
        Sub ClearBuffer()
        ReadOnly Property Buffer As String
        ReadOnly Property BufferPeek As String
        ReadOnly Property BufferPreLastPeek As String
        Function Peek() As Char
    End Interface
End NameSpace