Namespace Security
    Public Interface IActionSecurity

        Function UserCanRunThisAction(action As IActionBase) As Boolean
        Function UserCanRunThisAction(action As IActionBase, entity As Object) As Boolean
        Function EntityIsAvailableForUser(ByVal entity As Object) As Boolean
        Function GetActionList(entity As Object) As List(Of IActionDescriptor)
    End Interface
End Namespace