Imports LazyFramework.CQRS
Imports NUnit.Framework
Imports System.Security.Principal

<TestFixture> Public Class ActioninfoTest

    <SetUp> Public Sub SetupFixture()
        Runtime.Context.Current = New Runtime.WinThread
        LazyFramework.ClassFactory.Clear()
        LazyFramework.ClassFactory.SetTypeInstance(Of IActionSecurity)(New TestSecurity)
    End Sub

    <TearDown> Public Sub TearDown()
        LazyFramework.ClassFactory.Clear()
    End Sub


    <Test> Public Sub ActionsIsFoundForEntity()

        Assert.AreEqual(2, CQRS.ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity,New MenuAction).Count)
        'Ikke en god test, men det får holde for nå..
        Assert.IsInstanceOf(Of AcionForActionEntity)(ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity,New MenuAction)(1))

    End Sub

    <Test> Public Sub WiredActionIsFoundForEntity()
        Assert.IsInstanceOf(Of MenuAction)(ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity,New MenuAction)(0))
    End Sub

End Class

Public Class SomeOtherActionBase(Of T)
    Implements IActionBase



    Public Function ActionName() As String Implements IActionBase.ActionName
        Return "m.m"
    End Function

    Public Function IsAvailable(user As IPrincipal, o As Object) As Boolean Implements IActionBase.IsAvailable
        Return True
    End Function

    Public Function IsAvailable(user As IPrincipal) As Boolean Implements IActionBase.IsAvailable
        Return True
    End Function

    Public Function IsAvailable() As Boolean Implements IActionBase.IsAvailable
        Return True
    End Function
End Class

Public Class MenuAction
    Inherits SomeOtherActionBase(Of ActionEntity)

End Class


Public Class ActionEntity
    Public Id As Integer
End Class

Public Class AcionForActionEntity
    Inherits BaseCommand(Of ActionEntity)
End Class