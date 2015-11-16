Imports LazyFramework.CQRS
Imports NUnit.Framework
Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile
Imports LazyFramework.CQRS.Security
Imports LazyFramework.Test.Cqrs

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

        Assert.AreEqual(2, LazyFramework.CQRS.ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity).Count)
        'Ikke en god test, men det får holde for nå..
        Assert.IsInstanceOf(Of AcionForActionEntity)(ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity)(1))

    End Sub

    <Test> Public Sub WiredActionIsFoundForEntity()
        Assert.IsInstanceOf(Of MenuAction)(ActionInfo.GetAvailableActionsForEntity(Nothing, New ActionEntity)(0))
    End Sub

End Class

Public Class SomeOtherActionBase(Of T)
    Implements IActionBase

    Private _profile as IExecutionProfile

    Public Function ActionName() As String Implements IActionBase.ActionName
        Return "m.m"
    End Function
    
    'Public Sub SetProfile(profile As IExecutionProfile) Implements IActionBase.SetProfile
    '    _profile = profile
    'End Sub

    'Public Function ExecutionProfile() As IExecutionProfile Implements IActionBase.ExecutionProfile
    '    Return _profile
    'End Function
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