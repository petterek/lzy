Namespace CQRS.Validation
    Friend Class DetailedValidationException
        Inherits System.Exception

        Private ReadOnly _exList As Dictionary(Of String,Exception)

        Public Sub New(exList As Dictionary(Of String,Exception))
            _exList = exList
        End Sub

        Public ReadOnly Property ExceptionList as Dictionary(Of String,Exception)
            Get
                return _exList
            End Get
        End Property
    End Class
End NameSpace