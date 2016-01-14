Namespace Utils.Json
    Friend Class DictionaryBuilder
        Inherits Builder
        Public Sub New(type As Type)
            MyBase.New(type)


        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object

            Dim res As IDictionary = Nothing
            'this is a dictionary... 

            TokenAcceptors.WhiteSpace(nextChar)

            If TokenAcceptors.StartObjectOrNull(nextChar) IsNot Nothing Then
                Dim key As Object
                Dim value As Object
                Dim getGenericArguments = Me.type.GetGenericArguments
                Dim typeOfValue As Type

                If getGenericArguments.Count = 2 Then 'Name value
                    If Not getGenericArguments(0) Is GetType(String) Then
                        Throw New UnsupportedDictionaryException()
                    End If
                    typeOfValue = getGenericArguments(1)
                Else
                    Throw New UnsupportedDictionaryException()
                End If

                res = CType(Activator.CreateInstance(Me.type), IDictionary)
                TokenAcceptors.WhiteSpace(nextChar)
                If nextChar.Current = Chr(34) Then
                    Do
                        key = TokenAcceptors.Attribute(nextChar)
                        TokenAcceptors.EatUntil(TokenAcceptors.Qualifier, nextChar)
                        value = TokenAcceptors.ParseValue(typeOfValue, nextChar)
                        res.Add(key, value)
                    Loop While TokenAcceptors.CanFindValueSeparator(nextChar)
                End If
            End If
            'Cleaning out whitespace, check for " to ensure not empty object




            Return res
        End Function
    End Class

    Friend Class UnsupportedDictionaryException
        Inherits Exception

    End Class
End Namespace