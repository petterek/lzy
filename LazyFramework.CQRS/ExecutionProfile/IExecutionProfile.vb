Imports System.Security.Principal

Namespace ExecutionProfile

    Public Interface IExecutionProfile
        Function User() As IPrincipal
        Function Application As IApplicationInfo

        ReadOnly Property EventBus As IEventPublisher

    End Interface
End NameSpace