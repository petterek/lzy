

Namespace Transform
    Public MustInherit Class TransformerBase(Of TEntity, TDto)
        Implements ITransformEntityToDto

        Public context As ExecutionProfile

        Public Sub New()

        End Sub

        Public Sub New(context As ExecutionProfile, action As IAmAnAction)
            If context Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(context))
            End If
            Me.context = context
            Me.Action = action
        End Sub




        Friend Function TransformEntity(ByVal ent As Object) As Object Implements ITransformEntityToDto.TransformEntity
            Return TransformToDto(CType(ent, TEntity))
        End Function

        Public MustOverride Function TransformToDto(ent As TEntity) As TDto

        Public Property Action As IAmAnAction Implements ITransformEntityToDto.Action

    End Class
End Namespace
