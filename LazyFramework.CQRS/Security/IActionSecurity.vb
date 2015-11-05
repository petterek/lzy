Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Security


    Public Interface IActionSecurity
        Function UserCanRunThisAction(profile As IExecutionProfile, action As IActionBase) As Boolean
        Function UserCanRunThisAction(profile As IExecutionProfile, action As IActionBase, entity As Object) As Boolean
        Function EntityIsAvailableForUser(profile As IExecutionProfile, ByVal action As IAmAnAction, ByVal entity As Object) As Boolean
        Function GetActionList(profile As IExecutionProfile, action As IActionBase, entity As Object) As List(Of IActionDescriptor)

    End Interface
End NameSpace