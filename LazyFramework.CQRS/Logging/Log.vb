Imports System.Reflection
Imports LazyFramework.CQRS.Command

Namespace Logging
    Public Class Log

        Public Shared Logger As ILogger = New DevNullLogger()

        Public Shared Sub [Error](ctx As ExecutionProfile, ex As Exception)
            Dim input As New ErrorInfo

            input.ActionType = If(TypeOf (ctx.Action) Is IAmACommand, "Command", "Query")
            input.Source = ctx.Action.GetType.FullName()
            input.SourceGuid = ctx.Action.Guid
            input.Message = ex.Message
            input.Type = ex.GetType.FullName
            input.Params = ctx.Action

            Logger.Error(input)

        End Sub

        Public Shared Sub Context(ctx As ExecutionProfile)
            If ctx Is Nothing Then

            End If
            Logger.Trace(ctx)
        End Sub
    End Class

    Friend Class DevNullLogger
        Implements ILogger

        Public Sub Debug(data As Object) Implements ILogger.Debug

        End Sub

        Public Sub [Error](data As Object) Implements ILogger.Error

        End Sub

        Public Sub Info(data As Object) Implements ILogger.Info

        End Sub

        Public Sub Trace(data As Object) Implements ILogger.Trace

        End Sub

        Public Sub Warn(data As Object) Implements ILogger.Warn

        End Sub
    End Class

    Public Interface ILogger
        Sub [Error](data As Object)
        Sub Debug(data As Object)
        Sub Info(data As Object)
        Sub Trace(data As Object)
        Sub Warn(data As Object)

    End Interface
End Namespace