Imports System.Reflection
Imports LazyFramework.CQRS.Command

Namespace Logging
    Public Class Log
        Public Shared Sub [Error](action As IAmAnAction, ex As Exception)
            Dim input As New ErrorInfo

            input.ActionType = If(TypeOf (action) Is IAmACommand, "Command","Query")
            input.Source = action.ActionName
            input.SourceGuid = action.Guid
            input.Message = ex.Message
            input.Type = ex.GetType.FullName
            input.Params = action

            LazyFramework.Logging.Log.Write(Of ErrorInfo)(input,LazyFramework.Logging.LogLevelEnum.Error)

        End Sub

        Public Shared Sub Query(query As Query.IAmAQuery)
            Dim input As New QueryInfo
            
        End Sub

    End Class
End NameSpace