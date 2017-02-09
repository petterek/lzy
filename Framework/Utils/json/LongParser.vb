Namespace Utils.Json
    Friend Class LongParser
        Inherits Builder

        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789-")
            Return Long.Parse(nextChar.Buffer)

        End Function

    End Class
End NameSpace