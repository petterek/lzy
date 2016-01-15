Namespace Utils.Json
    Public Class DateParser
        Inherits Builder

    
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            
            If TokenAcceptors.QuoteOrNull(nextChar) Is Nothing Then
                Return Nothing
            End If
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789.:T+Z- ")

            Dim bufferVal = nextChar.Buffer()

            TokenAcceptors.Quote(nextChar)

            Return Date.Parse(bufferVal)
            
        End Function
    End Class
End NameSpace