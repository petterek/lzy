Namespace Utils.Json
    Friend Class LongParser
        Inherits Builder

        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            While IsNumeric(nextChar.Peek)
                nextChar.PeekToBuffer()
            End While
            Return Long.Parse(nextChar.Buffer)

        End Function

    End Class
End NameSpace