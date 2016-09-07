Imports System.Security.Principal


Public Class ActionIsNotAvailableException
    Inherits ActionValidationBaseException
    Public Sub New(action As IActionBase, user As Object)
        MyBase.New(action, user)
    End Sub


End Class

