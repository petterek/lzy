Imports LazyFramework.Pipeline
Imports NUnit.Framework

<TestFixture> Public Class PipelineTest


    <Test> Public Sub FunctionIsExecuted()
        Dim changeThis As Boolean = False
        Dim pipe = New LazyFramework.Pipeline.Base

        changeThis = pipe.Execute(Of Object, Boolean)(Function(ctx As Object)
                                                          Console.WriteLine("Executing function")
                                                          Return True
                                                      End Function, Nothing)

        Assert.IsTrue(changeThis)

    End Sub

    <Test> Public Sub AddTryCatchWrapper()

        Dim pipe As New Pipeline.Base
        pipe.AddExecuteWrapperStep(New TryCatchHandler)

        Assert.DoesNotThrow(Sub()
                                pipe.Execute(Of Object, Object)(Function()
                                                                    Throw New NotImplementedException
                                                                    Return Nothing
                                                                End Function, Nothing)
                            End Sub)

    End Sub


    <Test> Public Sub MultipleWrappersGetCalled()
        Dim pipe As New Pipeline.Base

        pipe.AddExecuteWrapperStep(New TryCatchHandler)
        pipe.AddExecuteWrapperStep(New TimerWrapper)

        Dim changeThis As Boolean = False

        changeThis = pipe.Execute(Of Object, Boolean)(Function()
                                                          Console.WriteLine("Executing function")
                                                          Return True
                                                      End Function, Nothing)
        Assert.IsTrue(changeThis)
    End Sub




    Public Class TimerWrapper
        Implements Pipeline.IExecuteWrapper
        
        Public Function Execute(Of TContext , TResult)(f As Func(Of TContext, TResult), context As TContext) As TResult Implements IExecuteWrapper.Execute
            Dim result As TResult
            Console.WriteLine("TIMER START:" & Now.Ticks)
            result = f(context)
            Console.WriteLine("TIMER END:" & Now.Ticks)
            Return result
        End Function
    End Class


    Public Class TryCatchHandler
        Implements Pipeline.IExecuteWrapper


        Public Function Execute(Of TContext, TResult)(f As Func(Of TContext, TResult), context As TContext) As TResult Implements IExecuteWrapper.Execute
            Dim result As TResult
            Console.WriteLine("Exceptionhandling start")
            Try
                result = f(context)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
            Console.WriteLine("Exceptionhandling end")
            Return result
        End Function
    End Class

End Class
