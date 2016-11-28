Namespace Utils.Json
    Public Class UnknownFieldParser
        Inherits Builder

        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim value As Object

            Dim peek = nextChar.Peek

            Select Case ascw(peek)
                Case = 34 'This is a " start of string pass on to stringparser... 
                    value = TokenAcceptors.TypeParserMapper(GetType(String)).Parse(nextChar, GetType(string))
                Case =  45,46,48,49,50,51,52,53,54,55,56,57 'This is a number of some kind..
                    value = TokenAcceptors.TypeParserMapper(GetType(Long)).Parse(nextChar, GetType(Double))
                Case = 70,84 ' T or F -> boolean
                    value = TokenAcceptors.TypeParserMapper(GetType(BoolanParser)).Parse(nextChar, GetType(BoolanParser))
                Case 44
                    value = Nothing
                Case = 78, 110 '[N,n]ull or , means no value.
                    TokenAcceptors.TypeParserMapper(GetType(NullableParser)).Parse(nextChar, GetType(NullableParser))
                    value = Nothing
                Case = 123 ' {
                    Throw new NotSupportedException("Cannot parse unknown objct types")
                Case Else
                    value = Nothing
            End Select
            
            Return Value

        End Function
    End Class
End Namespace