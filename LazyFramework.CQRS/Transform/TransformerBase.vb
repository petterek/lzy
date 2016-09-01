

Namespace Transform
    Public MustInherit Class TransformerBase(Of TEntity, TDto)
        Implements ITransformEntityToDto

        Friend Function TransformEntity(ctx As Object, ByVal ent As Object) As Object Implements ITransformEntityToDto.TransformEntity
            Return TransformToDto(ctx, CType(ent, TEntity))
        End Function

        Public MustOverride Function TransformToDto(ctx As Object, ent As TEntity) As TDto

        Public Property Action As IAmAnAction Implements ITransformEntityToDto.Action

    End Class
End Namespace
