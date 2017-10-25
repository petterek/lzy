Namespace Utils.Json
    Public Class UnknownFieldParser
        Inherits Builder


        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)
            Dim value As Object

            Dim peek = nextChar.Peek

            Select Case ascw(peek)
                Case = 34 'This is a " start of string pass on to stringparser... 
                    value = TokenAcceptors.TypeParserMapper(GetType(String)).Parse(nextChar, GetType(String))
                Case = 45, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 'This is a number of some kind..
                    value = TokenAcceptors.TypeParserMapper(GetType(Long)).Parse(nextChar, GetType(Double))
                Case = 70, 84, 116, 102 ' T, F, t, f -> boolean
                    value = TokenAcceptors.TypeParserMapper(GetType(Boolean)).Parse(nextChar, GetType(BoolanParser))
                Case 44
                    value = Nothing
                Case = 78, 110 '[N,n]ull or , means no value.
                    TokenAcceptors.TypeParserMapper(GetType(NullableParser)).Parse(nextChar, GetType(NullableParser))
                    value = Nothing
                Case = 123, 91 ' { [
                    'Must do generic push/pop on stack....
                    PushPopStack(nextChar, nextChar.Read(), New Stack(Of Char))
                    'Throw New NotSupportedException("Cannot parse unknown objct types")
                Case Else
                    value = Nothing
            End Select

            Return Value

        End Function



        Private Sub PushPopStack(nextChar As IReader, start As Char, stack As Stack(Of Char))
            Dim stackCount = stack.Count
            stack.Push(start)

            Dim endToken As Char
            Dim currToken As Char
            If (start = "["c) Then endToken = "]"c
            If (start = "{"c) Then endToken = "}"c

            While stack.Count <> stackCount
                currToken = nextChar.Read()
                If currToken = endToken Then
                    stack.Pop()
                End If
                If currToken = "["c Or currToken = "{"c Then
                    PushPopStack(nextChar, currToken, stack)
                End If
            End While
        End Sub
    End Class
End Namespace