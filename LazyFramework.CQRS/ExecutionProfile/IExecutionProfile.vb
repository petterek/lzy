Imports System.Security.Principal

Namespace ExecutionProfile

    Public Interface IExecutionProfile
        Function User() As IPrincipal
        Function Application As IApplicationInfo
    End Interface
End NameSpace