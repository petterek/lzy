Imports System.Reflection

Namespace Utils.Json
    Public Class TokenAcceptors
        Public Const ListStart = "["c
        Public Const ListEnd = "]"c
        Public Const ObjectStart = "{"c
        Public Const ObjectEnd = "}"c
        Public Const Qualifier = ":"c
        Public Const Separator = ","c
        Public Const Hyphen = Chr(34)

        Public Shared BuilderFactory As Type = GetType(ObjectBuilder)

        Public Shared Sub EatUntil(c As Char, nextChar As IReader)
            WhiteSpace(nextChar)
            If nextChar.Current <> c Then
                Throw New MissingTokenException(c, nextChar)
            End If
            nextChar.Read()
        End Sub

        Public Shared Sub EatUntil(c As String, nextChar As IReader)
            While nextChar.BufferPeek.Length < c.Length
                nextChar.PeekToBuffer()
            End While
            While nextChar.BufferPeek <> c
                nextChar.Read()
                nextChar.PeekToBuffer()
            End While
            nextChar.ClearBuffer()
            nextChar.Read() 'Dump end of buffer
        End Sub

        Public Shared Sub WhiteSpace(nextchar As IReader)
            If AscW(nextchar.Current) > 32 Then
                Return
            End If
            While AscW(nextchar.Peek) <= 32
                nextchar.Read()
            End While
            ConsumeComment(nextchar)
        End Sub

        Private Shared Sub ConsumeComment(ByVal nextchar As IReader)
            nextchar.PeekToBuffer()
            If nextchar.Current = "/" Then 'Start of single or multiline comment
                nextchar.PeekToBuffer()
                If nextchar.BufferPeek = "//" Then
                    nextchar.ClearBuffer()
                    EatUntil(vbCrLf, nextchar)
                End If
                If nextchar.BufferPeek = "/*" Then
                    nextchar.ClearBuffer()
                    EatUntil("*/", nextchar)
                End If
                WhiteSpace(nextchar)
            End If
        End Sub


        Public Shared Function QuoteOrNull(nextChar As IReader) As Object
            If Quote(nextChar) Then
                Return New Object
            End If

            TokenAcceptors.BufferLegalCharacters(nextChar, "NULnul")
            If nextChar.Buffer.ToLower = "null" Then
                nextChar.ClearBuffer()
                Return Nothing
            End If

            Return Nothing

        End Function

        Public Shared Function StartObjectOrNull(nextChar As IReader) As Object

            If nextChar.Current = ObjectStart Then
                nextChar.Read()
                Return New Object
            Else
                TokenAcceptors.BufferLegalCharacters(nextChar, "NULnul")
                If nextChar.Buffer.ToLower = "null" Then
                    nextChar.ClearBuffer()
                    Return Nothing
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function Quote(nextChar As IReader) As Boolean
            If nextChar.Current = Chr(34) Then
                nextChar.Read() 'Dump quote
                Return True
            End If
        End Function

        Public Shared Function Attribute(nextChar As IReader) As String
            'Dim buffer As New StringBuilder
            WhiteSpace(nextChar)
            Quote(nextChar)
            Dim w = AscW(nextChar.PeekToBuffer)

            'TokenAcceptors.BufferLegalCharacters(nextChar, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-")

            While (w > 64 AndAlso w < 91) OrElse (w > 96 AndAlso w < 123) OrElse (w > 47 AndAlso w < 58) OrElse w = 45 'This is A-Z a-z  The only characters allowed in attribute names.
                w = AscW(nextChar.PeekToBuffer)
            End While

            Dim ret = nextChar.Buffer
            Quote(nextChar)
            Return ret
        End Function

        Public Shared Sub Attributes(ByVal result As Object, ByVal nextChar As IReader)
            WhiteSpace(nextChar)
            'Cleaning out whitespace, check for " to ensure not empty object
            If nextChar.Current = Chr(34) Then
                Do
                    Dim name = Attribute(nextChar)
                    EatUntil(Qualifier, nextChar)
                    CreateAttributeValue(nextChar, result, name)
                Loop While CanFindValueSeparator(nextChar)
            End If

        End Sub

        Private Shared Sub CreateAttributeValue(ByVal nextChar As IReader, ByVal result As Object, ByVal name As String)
            Dim fType As Type = Nothing
            Dim parserType As Type = Nothing

            Dim fInfo As MemberInfo = Reflection.SearchForSetterInfo(result.GetType, name)
            If fInfo IsNot Nothing Then
                fType = CType(fInfo, PropertyInfo).PropertyType
            Else
                fInfo = Reflection.SearchForFieldInfo(result.GetType, name)
                If fInfo IsNot Nothing Then
                    fType = CType(fInfo, FieldInfo).FieldType
                End If
            End If

            If fInfo Is Nothing Then
                fType = GetType(UnknownFieldParser)
            End If

            If fType.IsEnum Then
                parserType = GetType(EnumParser)
            End If


            
            Dim value As Object = ParseValue(fType, nextChar, parserType)
            Dim parsedValue = value
            If fInfo IsNot Nothing Then
                SetterCache.GetInfo(fInfo).Setter()(result, parsedValue)
            End If


            'fInfo.SetValue(result, value)
        End Sub

        Public Shared Function ParseValue(t As Type, nextChar As IReader, Optional parserType As Type = Nothing) As Object
            Dim value As Object
            Dim useParser As Type = t

            If parserType Isnot Nothing Then
                useParser = parserType
            End If

            If TypeParserMapper.ContainsKey(useParser) Then
                value = TypeParserMapper(useParser).Parse(nextChar, t)
            Else
                value = Reader.StringToObject(nextChar, t)
            End If
            Return value
        End Function

        Public Shared Sub BufferLegalCharacters(nextChar As IReader, leagal As String)
            Dim toArray = leagal.ToArray
            While toArray.Contains(nextChar.Peek)
                nextChar.PeekToBuffer()
            End While
        End Sub


        Public Shared TypeParserMapper As New Dictionary(Of Type, Builder) From {
                                                                            {GetType(String), New StringParser},
                                                                            {GetType(Integer), New IntegerParser},
                                                                            {GetType(Int64), New LongParser},
                                                                            {GetType(Int16), New IntegerParser},
                                                                            {GetType(Date), New DateParser},
                                                                            {GetType(Double), New DoubleParser},
                                                                            {GetType(Guid), New GuidParser},
                                                                            {GetType(Boolean), New BoolanParser},
                                                                            {GetType(UnknownFieldParser), New UnknownFieldParser},
                                                                            {GetType(EnumParser), New EnumParser}
                                                                        }

        Friend Shared Function CanFindValueSeparator(ByVal nextChar As IReader) As Boolean
            WhiteSpace(nextChar)
            If nextChar.Current = TokenAcceptors.Separator Then
                nextChar.Read()
                Return True
            End If
            Return False
        End Function
    End Class

    <Serializable> Friend Class UnknownAttributeException
        Inherits Exception
        Public Sub New(ByVal name As String)
            MyBase.New(name)
        End Sub
    End Class

    Public Class NotCompleteException
        Inherits Exception

    End Class
End Namespace