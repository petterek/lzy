Namespace Dto
    Public Interface ISupportActionList
        ReadOnly Property Actions() As List(Of IActionDescriptor)
    End Interface
    Public Interface ISupportActionNameList
        ReadOnly Property Actions() As List(Of String)
    End Interface
End Namespace
