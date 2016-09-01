Imports System.Security.Principal


Public MustInherit Class ActionValidationBaseException
    Inherits Exception
    Private ReadOnly _Action As IAmAnAction
    Private ReadOnly _User As Object

    Public Sub New(action As IAmAnAction, user As Object)
        _Action = action
        _User = user
    End Sub

    Public ReadOnly Property Action As IAmAnAction
        Get
            Return _Action
        End Get
    End Property

    Public ReadOnly Property User As Object
        Get
            Return _User
        End Get
    End Property
End Class
