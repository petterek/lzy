Namespace Pipeline
    Public Interface IPipelineStep
        Sub ExecuteStep(of TContext )(context As TContext)
    End Interface


    Public Interface IExecuteWrapper
        Function Execute(Of TContext ,TResult)(f As Func(of TContext,TResult),context As TContext) As TResult 
    End Interface

End Namespace