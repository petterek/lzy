

Namespace Transform
    Public Interface ITransformerFactory
        Function GetTransformer(ByVal action As IAmAnAction, ByVal ent As Object) As ITransformEntityToDto
    End Interface
End Namespace
