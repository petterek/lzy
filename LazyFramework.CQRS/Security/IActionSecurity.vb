Namespace Security
    Public Interface IActionSecurity
        Function UserCanRunThisAction() As Boolean
        Function EntityIsAvailableForUser(ByVal entity As Object) As Boolean
        Function GetActionList(entity As Object) As List(Of IActionDescriptor)
    End Interface
End Namespace