Namespace Utils.Json
    Public MustInherit Class Builder
        Protected type As Type

        Public Sub New(t As Type)
            type = t
        End Sub

        Public MustOverride Function Parse(nextChar As IReader) As Object
    End Class

End NameSpace