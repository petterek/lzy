Imports System.Globalization

Namespace Utils.Json
    Public Class DoubleParser
        Inherits Builder
        Public Sub New()
            MyBase.New(GetType(Double))
        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789.")
            Return Double.Parse(nextChar.Buffer, CultureInfo.InvariantCulture.NumberFormat)
        End Function


        

    End Class





End Namespace