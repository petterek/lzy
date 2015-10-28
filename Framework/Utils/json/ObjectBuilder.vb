Namespace Utils.Json
    Public Class ObjectBuilder
        Inherits Builder

        Public Sub New(t As Type)
            MyBase.New(t)
        End Sub

        Public Overrides Function Parse(nextChar As IReader) As Object
            Try
                TokenAcceptors.EatUntil(TokenAcceptors.ObjectStart, nextChar)
            Catch ex As MissingTokenException
                Return Nothing
            End Try

            Dim Result = Activator.CreateInstance(type)

            TokenAcceptors.Attributes(Result, nextChar)

            TokenAcceptors.EatUntil(TokenAcceptors.ObjectEnd, nextChar)

            Return Result
        End Function
    End Class
End NameSpace