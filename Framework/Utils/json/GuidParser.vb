Namespace Utils.Json
    Public Class GuidParser
        Inherits Builder

        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            If TokenAcceptors.QuoteOrNull(nextChar) Is Nothing Then
                Return Nothing
            End If
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789ABCDEFabcdef-{}")
            Dim val = New Guid( nextChar.Buffer)
            TokenAcceptors.Quote(nextChar)
            Return val 

        End Function
    End Class
End NameSpace