
Imports System.Security.Principal
Imports LazyFramework.CQRS
Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile
Imports LazyFramework.CQRS.Security
Imports NUnit.Framework

Namespace Cqrs

    <TestFixture> Public Class CommandHub

        <SetUp> Public Sub SetupFixture()
            Runtime.Context.Current = New Runtime.WinThread
            LazyFramework.ClassFactory.Clear()
            LazyFramework.ClassFactory.SetTypeInstance(Of IActionSecurity)(New TestSecurity)
            LazyFramework.ClassFactory.SetTypeInstance(Of IExecutionProfileProvider)(New TestExecutionProfileProvider)
        End Sub

        <TearDown> Public Sub TearDown()
            LazyFramework.ClassFactory.Clear()
        End Sub

        <Test> Public Sub CommandIsLogged()

            Handling.ExecuteCommand(New TestCommand)
            'Assert.IsTrue(_TestLogger.LogIsCalled)

        End Sub


        <Test> Public Sub CommandIsRunned()

            Handling.ExecuteCommand(New TestCommand)
            Handling.ExecuteCommand(New AnotherCommand)
            Assert.IsTrue(HandleCommands.Found)

        End Sub


        <Test> Public Sub ExecptionIsHandledCorrectly()

            Assert.Throws(Of InnerException)(Sub() Handling.ExecuteCommand(New ExceptionIsThrownCommand))

        End Sub

        <Test> Public Sub ByRefparemSubIsCalled()
            Dim byrefCommand As ByrefCommand = New ByrefCommand
            Handling.ExecuteCommand(byrefCommand)

            Assert.IsTrue(byrefCommand.Called)

        End Sub

        <Test> Public Sub CommandIsMappedToName()
            Dim toTest As New CommandForA

            Assert.AreEqual(toTest.ActionName, Handling.CommandList(toTest.ActionName).FullName.Replace("."c, ""))

        End Sub


        <Test> Public Sub ReadOnlyPropertiesIsNotSerialized()

            Dim a As New CommandForA

            Dim res = Newtonsoft.Json.JsonConvert.SerializeObject(a)

            Debug.Print(res)

        End Sub



    End Class


    Public Class Entity
        Public Property A As Integer
    End Class


    Public MustInherit Class BaseCommand(Of T)
        Inherits CommandBase(Of T)


    End Class

    Public Class CommandForA
        Inherits BaseCommand(Of Entity)

        Public MyParam As Integer = 1

    End Class

    Public Class AnotherCommandForA
        Inherits CommandForA

    End Class





    Public Class CalculateKm
        Inherits CommandBase

        Public KmDrive As Integer
        Public DateDriven As DateTime

        Public Overrides Function ActionName() As String

            Return "Kalkuler km sats"

        End Function
    End Class

    Public Class TestSecurity
        Implements IActionSecurity

        Public Function EntityIsAvailableForUser(profile As IExecutionProfile, action As IAmAnAction, entity As Object) As Boolean Implements IActionSecurity.EntityIsAvailableForUser
            Return True
        End Function

        Public Function GetActionList(profile As IExecutionProfile, action As IActionBase, entity As Object) As List(Of IActionDescriptor) Implements IActionSecurity.GetActionList
            Return New List(Of IActionDescriptor)
        End Function

        Public Function UserCanRunThisAction(profile As IExecutionProfile, c As IActionBase) As Boolean Implements IActionSecurity.UserCanRunThisAction
            Return True
        End Function

        Public Function UserCanRunThisAction(profile As IExecutionProfile, action As IActionBase, entity As Object) As Boolean Implements IActionSecurity.UserCanRunThisAction
            Return True
        End Function
    End Class


    Public Class HandleCommands
        Implements IHandleCommand

        Public Shared Found As Boolean = False



        Public Shared Sub Handle(cmd As CalculateKm)

            Dim res As Single

            res = CType((cmd.KmDrive * 4.14), Single)

            'EventHandling.EventHub.Publish()


        End Sub

        Public Shared Sub Handle(command As TestCommand)
            Found = True
        End Sub

    End Class

    Public Class InnerException
        Inherits Exception

    End Class

    Public Class ExceptionIsThrownCommand
        Inherits CommandBase

        Public Overrides Function ActionName() As String

            Return "Exception"

        End Function
    End Class

    Public Class AnotherCommand
        Inherits CommandBase

        Public Overrides Function ActionName() As String
            Return "jbjkbkjb"
        End Function
    End Class

    Public Class ByrefCommand
        Inherits CommandBase

        Public Called As Boolean = False

        Public Overrides Function ActionName() As String

            Return ""

        End Function
    End Class

    Public Class TestCommand
        Inherits CommandBase

        Public Overrides Function ActionName() As String

            Return "Name"

        End Function


    End Class


    Public Class Another
        Implements IHandleCommand

        Public Shared Sub HandleSomethingElse(cmd As AnotherCommand)
            Dim a = cmd.ActionName
        End Sub


        Public Shared IsCalled As Boolean = False

        Public Shared Sub ParamIsByRef(ByRef cmd As ByrefCommand)
            cmd.Called = True
        End Sub

        Public Shared Sub ExceptionIsThrown(cmd As ExceptionIsThrownCommand)

            Throw New InnerException

        End Sub

    End Class
End Namespace