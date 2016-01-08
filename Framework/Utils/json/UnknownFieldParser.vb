Namespace Utils.Json
    Public Class UnknownFieldParser
        Inherits Builder

        Public Sub New()
            MyBase.New(GetType(UnknownFieldParser))
        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim value As Object

            Dim peek = nextChar.Peek

            Select Case ascw(peek)
                Case = 34 'This is a " start of string pass on to stringparser... 
                    value = TokenAcceptors.TypeParserMapper(GetType(String)).Parse(nextChar)
                Case =  45,46,48,49,50,51,52,53,54,55,56,57 'This is a number of some kind..
                    value = TokenAcceptors.TypeParserMapper(GetType(Long)).Parse(nextChar)
                Case = 70,84 ' T or F -> boolean
                    value = TokenAcceptors.TypeParserMapper(GetType(BoolanParser)).Parse(nextChar)
                Case = 78,44 'N null or , means no value.
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