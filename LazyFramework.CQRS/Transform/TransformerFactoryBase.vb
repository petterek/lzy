Namespace Transform
    Public MustInherit Class TransformerFactoryBase(Of TEntity, TDto)
        Implements ITransformerFactory

        Friend Function GetTransformerInternal(ent As Object) As ITransformEntityToDto Implements ITransformerFactory.GetTransformer
            Dim transformEntityToDto As ITransformEntityToDto = GetTransformer(CType(ent, TEntity))
            If transformEntityToDto Is Nothing Then Return Nothing

            'transformEntityToDto.Action = Action
            Return transformEntityToDto
        End Function

        Public MustOverride Function GetTransformer(ent As TEntity) As ITransformEntityToDto

        Public Property RunAsParallel As Boolean = True Implements ITransformerFactory.RunAsParallel

        Friend Property ObjectComparer As Comparison(Of Object) Implements ITransformerFactory.ObjectComparer

        Public WriteOnly Property Sorting As Comparison(Of TDto)
            Set(value As Comparison(Of TDto))
                ObjectComparer = New Comparison(Of Object)(Function(o1, o2) value(CType(o1, TDto), CType(o2, TDto)))
            End Set
        End Property

    End Class
End Namespace