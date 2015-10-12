Namespace Pipeline

    Public Class ExtendedWExecuteWrapper

        Public Current As IExecuteWrapper
        Public [Next] As ExtendedWExecuteWrapper

    End Class

    Public Class Base
        Implements IPipeline

        Protected Pre As New List(Of IPipelineStep)
        Protected Post As New List(Of IPipelineStep)

        Protected Wrapped As New List(Of IExecuteWrapper)

        Public Sub New()

        End Sub

        Public Sub AddExecuteWrapperStep(stepToExecute As IExecuteWrapper) Implements IPipeline.AddExecuteWrapperStep
            Wrapped.Add(stepToExecute)
        End Sub

        Public Sub AddPostExecuteStep(stepToExecute As IPipelineStep) Implements IPipeline.AddPostExecuteStep
            Post.Add(stepToExecute)
        End Sub

        Public Sub AddPreExecuteStep(stepToExecute As IPipelineStep) Implements IPipeline.AddPreExecuteStep
            Pre.Add(stepToExecute)
        End Sub

        Public Function Execute(Of TContext As Class,TResult)(toExecute As Func(Of TContext,TResult), context As TContext) As TResult Implements IPipeline.Execute

            For Each executeStep In Pre
                executeStep.ExecuteStep(context)
            Next
            
            Dim res = CallWrappers(Of TContext,TResult)(0, Wrapped, toExecute, context)
            
            For Each executeStep In Post
                executeStep.ExecuteStep(context)
            Next

            Return res

        End Function

        Private Function CallWrappers(Of TContext As Class,TResult)(i As Integer, executeWrappers As List(Of IExecuteWrapper), toExecute As Func(Of TContext,TResult),context As TContext) As TResult

            If i >= executeWrappers.Count Then
                Return toExecute(context)
            End If

            Return Wrapped(i).Execute(Of  TContext ,TResult)(Function()
                                                      Return CallWrappers(i + 1, executeWrappers, toExecute, context)
                                                  End Function,context)

        End Function
        
    End Class

End Namespace