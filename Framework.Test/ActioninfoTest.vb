Imports LazyFramework.CQRS
Imports NUnit.Framework
Imports LazyFramework.Test.Cqrs

<TestFixture> Public Class ActioninfoTest

    <SetUp> Public Sub SetupFixture()

    End Sub

    <TearDown> Public Sub TearDown()

    End Sub


    <Test> Public Sub ActionsIsFoundForEntity()

        '  LazyFramework.CQRS.ActionInfo.AddActionForType(Of ActionEntity)

        'Assert.AreEqual(2, LazyFramework.CQRS.ActionInfo.GetAvailableActionsForEntity(New Object, New ActionEntity).Count)
        'Ikke en god test, men det får holde for nå..
        'Assert.IsInstanceOf(Of AcionForActionEntity)(ActionInfo.GetAvailableActionsForEntity(New Object, New ActionEntity)(1))

    End Sub

    <Test> Public Sub WiredActionIsFoundForEntity()
        'Assert.IsInstanceOf(Of MenuAction)(ActionInfo.GetAvailableActionsForEntity(New Object, New ActionEntity)(0))
    End Sub

End Class

Public Class SomeOtherActionBase(Of T)
    Inherits ActionBase


End Class

Public Class MenuAction
    Inherits SomeOtherActionBase(Of ActionEntity)

End Class


Public Class ActionEntity
    Public Id As Integer
End Class

Public Class AcionForActionEntity
    Inherits LazyFramework.CQRS.Command.CommandBase

End Class