Imports System.Globalization

Namespace Utils.Json
    Public Class DoubleParser
        Inherits Builder
        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            TokenAcceptors.BufferLegalCharacters(nextChar, "0123456789.-")
            Return Double.Parse(nextChar.Buffer, CultureInfo.InvariantCulture.NumberFormat)
        End Function


        

    End Class





End Namespace