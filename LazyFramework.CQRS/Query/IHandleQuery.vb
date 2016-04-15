Namespace Query
    Public Interface IHandleQuery

    End Interface

    Public Interface IHandleQuery(Of TQuery,TResult)
        Function Handle(query As TQuery) As TResult
    End Interface

End Namespace
