Namespace Utils.Json
    Public Class DateParser
        Inherits Builder

        Public Sub New()
            MyBase.New(GetType(Date))
        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.Quote(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789.:T+Z- ")

            Dim bufferVal = nextChar.Buffer()

            TokenAcceptors.Quote(nextChar)

            Return Date.Parse(bufferVal)


        End Function
    End Class
End NameSpace