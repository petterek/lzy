Namespace Utils.Json
    Public MustInherit Class Builder
        Public MustOverride Function Parse(nextChar As IReader, t As Type) As Object
    End Class

End NameSpace