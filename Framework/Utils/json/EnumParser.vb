Namespace Utils.Json
    Friend Class EnumParser
        Inherits Builder
        
        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim value As Object


            If nextChar.Current = TokenAcceptors.Hyphen Then
                value = TokenAcceptors.TypeParserMapper(GetType(string)).Parse(nextChar,GetType(string))
            Else
                value = TokenAcceptors.TypeParserMapper(t.GetEnumUnderlyingType).Parse(nextChar,t.GetEnumUnderlyingType)
            End If
            
            Return [Enum].Parse(t, value.ToString)
            
        End Function
    End Class
End NameSpace