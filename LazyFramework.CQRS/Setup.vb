Public Class Setup

    Public Shared ActionSecurity As Security.IActionSecurity
    Public Shared ClassFactory As IClassFactory
    Public Shared ChickenMode As Boolean

End Class

Public Interface IClassFactory
    Function CreateInstance(type As System.Type) As Object
    Function CreateInstance(Of T)() As T
End Interface
