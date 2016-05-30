Imports LazyFramework.CQRS.Command

Namespace EntityResolver
    Public Class Handling

        Private Shared _entityResolvers As New Dictionary(Of Type, IResolveEntity)
        Private Shared _padlock As New Object

        Private Shared ReadOnly Property EntityResolvers As Dictionary(Of Type, IResolveEntity)
            Get
                Return _entityResolvers
            End Get
        End Property

        Public Sub AddResolver(Of TCommand As IAmACommand)(resolver As IResolveEntity)
            _entityResolvers.Add(GetType(TCommand), resolver)
        End Sub

        Public Shared Sub ResolveEntity(action As IAmACommand)
            If EntityResolvers.ContainsKey(action.GetType) Then
                EntityResolvers(action.GetType).Resolve(action)
            End If
        End Sub
    End Class

    Public Interface IResolveEntity

        Sub Resolve(action As IAmACommand)

    End Interface

    'Public MustInherit Class ResolveEntityBase(Of T)
    '    Implements IResolveEntity

    '    Friend Sub Resolve(action As IAmACommand) Implements IResolveEntity.Resolve
    '        Resolve(DirectCast(action, T))
    '    End Sub

    '    Public MustOverride Sub Resolve(action As T)
    'End Class


End Namespace