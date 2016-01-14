Namespace Utils.Json
    Public Class ObjectBuilder
        Inherits Builder

        Public Sub New(t As Type)
            MyBase.New(t)
        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            TokenAcceptors.WhiteSpace(nextChar)

            If TokenAcceptors.StartObjectOrNull(nextChar) Is Nothing Then
                Return Nothing
            End If

            Dim Result = Activator.CreateInstance(type)

            TokenAcceptors.Attributes(Result, nextChar)

            TokenAcceptors.EatUntil(TokenAcceptors.ObjectEnd, nextChar)

            Return Result
        End Function
    End Class
End Namespace