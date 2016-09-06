Namespace Transform
    Public Interface ITransformEntityToDto
        Function TransformEntity(ByVal ent As Object) As Object
        Property Action As IAmAnAction
    End Interface
End Namespace
