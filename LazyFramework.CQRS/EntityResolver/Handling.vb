Imports LazyFramework.CQRS.Command

Namespace EntityResolver
    Public Class Handling

        Private Shared _entityResolvers As Dictionary(Of Type, IResolveEntity)
        Private Shared _padlock As new Object

        Private Shared ReadOnly Property EntityResolvers As Dictionary(Of Type, IResolveEntity)
            Get
                If _entityResolvers Is Nothing Then
                    SyncLock _padlock
                        If _entityResolvers Is Nothing Then
                            Dim temp As New Dictionary(Of Type, IResolveEntity)
                            For Each handler In Reflection.FindAllClassesOfTypeInApplication(GetType(IResolveEntity))
                                temp.Add(handler.BaseType.GetGenericArguments()(0), CType(Activator.CreateInstance(handler), IResolveEntity))
                            Next
                            _entityResolvers = temp
                        End If
                    End SyncLock
                End If
                Return _entityResolvers
            End Get
        End Property

        Public Shared Sub ResolveEntity(profile As ExecutionProfile.IExecutionProfile, action As Command.IAmACommand)
            If EntityResolvers.ContainsKey(action.GetType) Then
                EntityResolvers(action.GetType).Resolve(action)
            End If
        End Sub
    End Class

    Friend Interface IResolveEntity

        Sub Resolve(action As Command.IAmACommand)


    End Interface

    Public MustInherit Class ResolveEntityBase(Of T)
        Implements IResolveEntity

        Friend Sub Resolve(action As IAmACommand) Implements IResolveEntity.Resolve
            Resolve(DirectCast(action, T))
        End Sub

        Public MustOverride Sub Resolve(action As T)
    End Class


End Namespace