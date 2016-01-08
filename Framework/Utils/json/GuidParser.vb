Namespace Utils.Json
    Public Class GuidParser
        Inherits Builder

        Public sub New
            MyBase.New(GetType(Guid))
        End sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.Quote(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789ABCDEFabcdef-{}")
            Dim val = New Guid( nextChar.Buffer)
            TokenAcceptors.Quote(nextChar)
            Return val 

        End Function
    End Class
End NameSpace