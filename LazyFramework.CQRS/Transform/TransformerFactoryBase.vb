Namespace Transform
    Public MustInherit Class TransformerFactoryBase(Of TAction, TEntity)
        Implements ITransformerFactory


        Friend Function GetTransformerInternal(action As IAmAnAction, ent As Object) As ITransformEntityToDto Implements ITransformerFactory.GetTransformer
            Dim transformEntityToDto As ITransformEntityToDto = GetTransformer(CType(action, TAction), CType(ent, TEntity))
            If transformEntityToDto Is Nothing Then Return Nothing

            transformEntityToDto.Action = action
            Return transformEntityToDto
        End Function

        Public MustOverride Function GetTransformer(action As TAction, ent As TEntity) As ITransformEntityToDto
        
        Public Overridable Function SortingFunc(action As IAmAnAction) As Comparison(Of Object) Implements ISortingFunction.SortingFunc
            Return Nothing
        End Function

        Public Property RunAsParallel As Boolean = true Implements ITransformerFactory.RunAsParallel
    End Class
End Namespace