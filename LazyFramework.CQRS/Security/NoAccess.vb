Imports LazyFramework.CQRS.Logging
Namespace Security

    Public Class NoAccess
        Inherits Exception

        Private ReadOnly _AnAction As IAmAnAction

        Public Sub New(ByVal anAction As IAmAnAction)
            _AnAction = anAction
        End Sub

        Public ReadOnly Property AnAction As IAmAnAction
            Get
                Return _AnAction
            End Get
        End Property
    End Class
End NameSpace