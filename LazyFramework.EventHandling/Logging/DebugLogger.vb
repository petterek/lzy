Imports System.Reflection

Namespace Logging


    Public Class DebugLogger
        Implements IDoEventLogging

        Public Sub LogSuccessEvent(e As IAmAnEvent, target As MethodInfo) Implements IDoEventLogging.LogSuccessEvent
            Debug.WriteLine(String.Format("{0}-{1}-{2}-{3}-{4}", e.GetType.ToString, e.Guid, e.TimeStamp, e.RunAsync, target.Module.Name & "." & target.Name))
        End Sub

    End Class


    'Friend Class Logger
    '    Public Shared Sub Write(ByVal e As IAmAnEvent)
    '        LazyFramework.Logging.Log.Write(Of IAmAnEvent)( e,LazyFramework.Logging.LogLevelEnum.Info Or LazyFramework.Logging.LogLevelEnum.Verbose)
    '    End Sub
    'End Class


End NameSpace