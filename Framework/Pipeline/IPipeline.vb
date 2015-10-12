Namespace Pipeline
    Public Interface IPipeline

        Sub AddPreExecuteStep(stepToExecute As IPipelineStep)
        Sub AddPostExecuteStep(stepToExecute As IPipelineStep)
        Sub AddExecuteWrapperStep(stepToExecute As IExecuteWrapper)

        Function Execute(Of TContext As Class,TResult)(toExecute As Func(Of TContext,TResult),context As TContext) As TResult

    End Interface
End NameSpace