Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Command
    Public MustInherit Class CommandBase
        Inherits ActionBase
        Implements IAmACommand

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Sub SetInnerEntity(o As Object)
            InnerEntity = o
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Function Result() As Object Implements IAmACommand.Result
            Return InnerResult
        End Function

        Protected InnerEntity As Object
        Protected ReadOnly InnerEntityList As New List(Of Object)
        Protected InnerResult As Object

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Sub SetResult(o As Object) Implements IAmACommand.SetResult
            InnerResult = o
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function GetInnerEntity() As Object
            Return InnerEntity
        End Function

        Public Function GetEntityList() As List(Of Object)
            Return InnerEntityList
        End Function

    End Class

    Public MustInherit Class CommandBase(Of TEntity)
        Inherits CommandBase

        Private _Entity As TEntity
        Public Function Entity() As TEntity
            If _Entity Is Nothing AndAlso InnerEntity IsNot Nothing Then
                _Entity = CType(InnerEntity, TEntity)
            End If
            Return _Entity
        End Function
        
    End Class
End Namespace
