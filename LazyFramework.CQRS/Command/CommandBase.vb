Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Command
    Public MustInherit Class CommandBase
        Inherits ActionBase
        Implements IAmACommand
        
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overridable Function ResolveEntity() As Object
            Return Nothing
        End Function

        ''' <summary>
        ''' Override this method to fill the InnerEntityList
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Sub FillEntityList()

        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Sub SetInnerEntity(o As Object)
            InnerEntity = o
            IsResolved = True
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Function Result() As Object Implements IAmACommand.Result
            Return InnerResult
        End Function

        Protected InnerEntity As Object
        Protected ReadOnly InnerEntityList As New List(Of Object)
        Protected InnerResult As Object
        Protected IsResolved As Boolean

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Sub SetResult(o As Object) Implements IAmACommand.SetResult
            InnerResult = o
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function GetInnerEntity() As Object
            If Not IsResolved Then
                InnerEntity = ResolveEntity()
                IsResolved = True
            End If
            Return InnerEntity
        End Function

        Public Function GetEntityList() As List(Of Object)
            If Not IsResolved Then
                InnerEntity = ResolveEntity()
                FillEntityList()
            End If
            Return InnerEntityList
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overrides Function IsAvailable() As Boolean
            If InnerEntity Is Nothing Then
                InnerEntity = ResolveEntity()
                FillEntityList()
                IsResolved = True
            End If
            Return IsAvailable(_profile, InnerEntity)
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overrides Function IsAvailable(user As IExecutionProfile, o As Object) As Boolean
            SetProfile(user)
            SetInnerEntity(o)
            Return True
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overrides Function IsAvailable(user As IExecutionProfile) As Boolean
            SetProfile(user)
            Return True
        End Function
        
    End Class

    Public MustInherit Class CommandBase(Of TEntity)
        Inherits CommandBase

        Private _Entity As TEntity
        Public Function Entity() As TEntity
            If _Entity Is Nothing Then
                If IsResolved Then
                    _Entity = CType(InnerEntity, TEntity)
                Else
                    _Entity = CType(ResolveEntity(), TEntity)
                    SetInnerEntity(_Entity)
                End If

            End If
            Return _Entity

        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overrides Function IsAvailable(profile As IExecutionProfile, o As Object) As Boolean
            Return IsActionAvailable(profile, CType(o, TEntity))
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overrides Function IsAvailable(profile As IExecutionProfile) As Boolean
            Return IsActionAvailable(profile)
        End Function
        
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overridable Function IsActionAvailable(profile As IExecutionProfile) As Boolean
            Return True
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overridable Function IsActionAvailable(profile As IExecutionProfile, entity As TEntity) As Boolean
            Return True
        End Function

    End Class
End Namespace
