

Namespace Transform

    Public Interface ITransformerFactory
        Property RunAsParallel As Boolean
        Function GetTransformer(ent As Object) As ITransformEntityToDto

        Property ObjectComparer As Comparison(Of Object)
    End Interface
End Namespace
