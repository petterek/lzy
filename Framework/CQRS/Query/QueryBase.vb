Imports System.Security.Principal

Namespace CQRS.Query
    Public MustInherit Class QueryBase
        Inherits CQRS.ActionBase
        Implements IAmAQuery
        
        Public Overrides Function IsAvailable() As Boolean
            Return IsAvailable(User)
        End Function

        Public Overrides Function IsAvailable(user As IPrincipal) As Boolean
            Return True
        End Function

        Public Overrides Function IsAvailable(user As IPrincipal, o As Object) As Boolean
            Return True
        End Function
    End Class


    ''' <summary>
    ''' Queries which result in a given entity type or list of entity type
    ''' Queries should never resolve the entity. That is always done through the handler. 
    ''' </summary>
    ''' <typeparam name="TResultEntity"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class QueryBase(Of TResultEntity)
        Inherits QueryBase

        Public Overrides Function IsAvailable(user As IPrincipal, o As Object) As Boolean
            Return IsActionAvailable(user, CType(o, TResultEntity))
        End Function
        Public Overrides Function IsAvailable(user As IPrincipal) As Boolean
            Return IsActionAvailable(user)
        End Function



        Public Overridable Function IsActionAvailable(user As IPrincipal) As Boolean
            Return True
        End Function
        Public Overridable Function IsActionAvailable(user As IPrincipal, entity As TResultEntity) As Boolean
            Return True
        End Function

    End Class


End Namespace
