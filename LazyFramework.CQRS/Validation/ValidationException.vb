Namespace Validation
    Public Class ValidationException
        Inherits Exception

        Private _Inner As New Dictionary(Of String, Exception)
        Public Property ExceptionList As Dictionary(Of String, Exception)
            Get
                Return _Inner
            End Get
            Set(value As Dictionary(Of String, Exception))
                _Inner = value
            End Set
        End Property
    End Class
End Namespace
