Namespace Utils.Json
    Public Class ObjectBuilder
        Inherits Builder


        Public Overrides Function Parse(nextChar As IReader, t As Type) As Object
            TokenAcceptors.WhiteSpace(nextChar)

            If TokenAcceptors.StartObjectOrNull(nextChar) Is Nothing Then
                Return Nothing
            End If

            Dim Result = Activator.CreateInstance(t)

            TokenAcceptors.Attributes(Result, nextChar)

            TokenAcceptors.EatUntil(TokenAcceptors.ObjectEnd, nextChar)

            Return Result
        End Function
    End Class
End Namespace