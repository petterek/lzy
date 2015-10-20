Imports LazyFramework.CQRS.Logging


Public Class HandlerNotFound
    Inherits BaseLogEvent

    Private ReadOnly _AmAnAction As IAmAnAction
    Public Sub New(ByVal amAnAction As IAmAnAction)
        _AmAnAction = amAnAction
    End Sub

    Public ReadOnly Property AmAnAction As IAmAnAction
        Get
            Return _AmAnAction
        End Get
    End Property
End Class
