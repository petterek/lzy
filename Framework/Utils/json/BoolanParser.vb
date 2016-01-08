Namespace Utils.Json
    Public Class BoolanParser
        Inherits Builder

        Public sub New()
            MyBase.New(GetType(Boolean))

        End sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "TrueFals")
            Dim bufferVal = nextChar.Buffer()
            

            Select Case bufferVal
                Case "True"
                    Return True
                Case "False"
                    Return False
                Case Else
                    Throw New InvalidCastException(bufferVal & " is not a boolean. True/False")
            End Select


        End Function
    End Class
End NameSpace