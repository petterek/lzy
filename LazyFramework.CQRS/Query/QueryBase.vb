Namespace Query
    Public MustInherit Class QueryBase
        Inherits ActionBase
        Implements IAmAQuery


    End Class


    ''' <summary>
    ''' Queries which result in a given entity type or list of entity type
    ''' Queries should never resolve the entity. That is always done through the handler. 
    ''' </summary>
    ''' <typeparam name="TResultEntity"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class QueryBase(Of TResultEntity)
        Inherits QueryBase


        Public Overridable Function IsActionAvailable(profile As Object) As Boolean
            Return True
        End Function
        Public Overridable Function IsActionAvailable(profile As Object, entity As TResultEntity) As Boolean
            Return True
        End Function

    End Class


End Namespace
