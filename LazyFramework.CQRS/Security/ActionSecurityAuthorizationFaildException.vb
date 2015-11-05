Imports System.Security.Principal

Namespace Security


    Public Class ActionSecurityAuthorizationFaildException
        Inherits ActionValidationBaseException
        Public Sub New(action As IAmAnAction, user As IPrincipal)
            MyBase.New(action, user)
        End Sub

    End Class
End NameSpace