Namespace Utils.Json
    Friend Class IntegerParser
        Inherits Builder

        Public Sub New()
            MyBase.New(GetType(Integer))
        End Sub
        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim c  = (nextChar.Peek)
            While AscW( c) >= 44 And AscW(c) <=57
                nextChar.PeekToBuffer()
                c  = (nextChar.Peek)
            End While
            Return Integer.Parse(nextChar.Buffer)
        End Function

    End Class

    Friend Class LongParser
        Inherits Builder

        Public Sub New()
            MyBase.New(GetType(Integer))
        End Sub
        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim c  = (nextChar.Peek)
            While AscW( c) >= 44 And AscW(c) <=57
                nextChar.PeekToBuffer()
                c  = (nextChar.Peek)
            End While

            Return Long.Parse(nextChar.Buffer)

        End Function

    End Class


End Namespace