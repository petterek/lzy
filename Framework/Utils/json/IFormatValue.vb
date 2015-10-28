Namespace Utils.Json
    Public Interface IFormatValue(Of T)
        Function ToStringValue(value As T) As String
        Function ToInstance(value As String) As T
    End Interface
End NameSpace