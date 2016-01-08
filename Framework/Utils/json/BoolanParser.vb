Namespace Utils.Json
    Public Class BoolanParser
        Inherits Builder

        Public sub New()
            MyBase.New(GetType(Boolean))

        End sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "TtrueFfals")
            Dim bufferVal = nextChar.Buffer()
            
            Dim res As Boolean
            if not Boolean.TryParse(bufferVal, res) Then
                        Throw New InvalidCastException(bufferVal & " is not a boolean. True/False")
            End If
            Return res
        End Function
    End Class
End NameSpace