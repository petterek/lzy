Imports LazyFramework.CQRS.Logging
Namespace Security

    Public Class NoAccess
        Inherits Exception

        Private ReadOnly _AnAction As IActionBase

        Public Sub New(ByVal anAction As IActionBase)
            _AnAction = anAction
        End Sub

        Public ReadOnly Property AnAction As IActionBase
            Get
                Return _AnAction
            End Get
        End Property
    End Class
End NameSpace