Imports System.Security.Principal
Imports LazyFramework.EventHandling

Namespace ExecutionProfile

    Public Interface IExecutionProfile
        Function User() As IPrincipal
        Function Application As IApplicationInfo

        Sub Publish(currentUser As IPrincipal, [event] As IAmAnEvent)
    End Interface
End Namespace