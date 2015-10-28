Namespace Utils.Json
    Friend Class IntegerParser
        Inherits Builder

        Public Sub New()
            MyBase.New(GetType(Integer))
        End Sub
        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            While IsNumeric(nextChar.Peek)
                nextChar.PeekToBuffer()
            End While
            Return Integer.Parse(nextChar.Buffer)
        End Function

    End Class
End Namespace