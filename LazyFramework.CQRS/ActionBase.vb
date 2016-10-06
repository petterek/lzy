Imports System.Security.Principal
Imports LazyFramework.CQRS

Public MustInherit Class ActionBase
    Implements IActionBase

    Public Sub New()
        Guid = System.Guid.NewGuid
    End Sub


    Public ReadOnly Property Guid As Guid Implements IActionBase.Guid

End Class