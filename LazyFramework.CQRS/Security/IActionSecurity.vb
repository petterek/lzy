Namespace Security


    Public Interface IActionSecurity
        Function UserCanRunThisAction(profile As Object, action As IActionBase) As Boolean
        Function UserCanRunThisAction(profile As Object, action As IActionBase, entity As Object) As Boolean
        Function EntityIsAvailableForUser(profile As Object, ByVal action As IAmAnAction, ByVal entity As Object) As Boolean
        Function GetActionList(profile As Object, action As IActionBase, entity As Object) As List(Of IActionDescriptor)

    End Interface
End Namespace