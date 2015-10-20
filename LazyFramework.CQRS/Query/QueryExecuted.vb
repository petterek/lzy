Imports LazyFramework.CQRS.Logging

Namespace Query
    Public Class QueryExecuted
        Inherits BaseLogEvent

        Private ReadOnly _Query As IAmAQuery

        Public Sub New(ByVal query As IAmAQuery)
            _Query = query
        End Sub

        Public ReadOnly Property Query As IAmAQuery
            Get
                Return _Query
            End Get
        End Property
    End Class
End Namespace
