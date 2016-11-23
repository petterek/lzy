Imports LazyFramework.CQRS
Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.Security
Imports NUnit.Framework

Namespace Cqrs

    <TestFixture> Public Class CommandHub

        <SetUp> Public Sub SetupFixture()
            LazyFramework.CQRS.Command.Handling.ClearMapping()
        End Sub

        <TearDown> Public Sub TearDown()
            LazyFramework.ClassFactory.Clear()
        End Sub

        <Test> Public Sub CommandIsLogged()

            'Handling.ExecuteCommand(New TestExecutionProfileProvider().GetExecutionProfile, New TestCommand)
            'Assert.IsTrue(_TestLogger.LogIsCalled)

        End Sub


        <Test> Public Sub CommandIsRunned()

            Handling.AddCommandHandler(Of TestCommand)(Function(o, a) New CommandExecutionProfile(Of TestCommand, Boolean, String)(AddressOf New CommandHandler().CommandHandler))

            'LazyFramework.CQRS.Command.Handling.AddCommandHandler(Of AnotherCommand)(AddressOf Another.HandleSomethingElseCommandHandler)


            Assert.IsTrue(CBool(Handling.ExecuteCommand(New Object, New TestCommand)))

            'Handling.ExecuteCommand(New Object, New AnotherCommand)



        End Sub


        <Test> Public Sub AddCommandWithoutReturnValue()

            Handling.AddCommandHandler(Of CommandWithoutRetValue)(Function(o, a) New CommandExecutionProfile(Of CommandWithoutRetValue, Object)(AddressOf New CommandHandler().HandleCommandWithoutRetValue))

        End Sub





        <Test> Public Sub ExecptionIsHandledCorrectly()
            LazyFramework.CQRS.Command.Handling.AddCommandHandler(Of ExceptionIsThrownCommand)(Function(o, a) New CommandExecutionProfile(Of ExceptionIsThrownCommand, Object, Object)(AddressOf New Another().ExceptionIsThrownCommandHandler))
            Assert.Throws(Of InnerException)(Sub() Handling.ExecuteCommand(New Object, New ExceptionIsThrownCommand))

        End Sub


        <Test> Public Sub CommandIsMappedToName()
            Dim toTest As New CommandForA
            LazyFramework.CQRS.Command.Handling.AddCommandHandler(Of CommandForA)(Function(a, c) Nothing)

            Assert.AreEqual(toTest.GetType.FullName, Handling.CommandList(toTest.GetType.FullName).FullName)

        End Sub


        <Test> Public Sub ReadOnlyPropertiesIsNotSerialized()

            Dim a As New CommandForA

            Dim res = Newtonsoft.Json.JsonConvert.SerializeObject(a)

            Debug.Print(res)

        End Sub

        '<Test> Public Sub NotAvailableCommandIsStopped()
        '    LazyFramework.CQRS.Command.Handling.AddCommandHandler(Of ThisCommandIsNotAvailableIfIdIs0)(AddressOf New CommandHandler().CommandHandler)
        '    LazyFramework.CQRS.Availability.Handler.AvailabilityList.Add(GetType(ThisCommandIsNotAvailableIfIdIs0), New CommandAvaialability)
        '    Assert.Throws(Of ActionIsNotAvailableException)(Sub() Handling.ExecuteCommand(New Object, New ThisCommandIsNotAvailableIfIdIs0 With {.Id = 0}))

        'End Sub

        <Test> Public Sub CommandAvailabilityIsCalled()

            LazyFramework.CQRS.Command.Handling.AddCommandHandler(Of ThisCommandIsNotAvailableIfIdIs0)(Function(o, a)
                                                                                                           Dim ret As New CommandExecutionProfile(Of ThisCommandIsNotAvailableIfIdIs0, Object, Object)(AddressOf New CommandHandler().CommandHandler)

                                                                                                           Return ret
                                                                                                       End Function)

            Assert.DoesNotThrow(Sub() Handling.ExecuteCommand(New Object, New ThisCommandIsNotAvailableIfIdIs0 With {.Id = 1}))

        End Sub



    End Class


    Public Class CommandAvaialability
        Inherits LazyFramework.CQRS.Availability.CommandAvailability(Of ThisCommandIsNotAvailableIfIdIs0, Entity)

        Private ReadOnly command As ThisCommandIsNotAvailableIfIdIs0

        Public Sub New(command As ThisCommandIsNotAvailableIfIdIs0)
            If command Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(command))
            End If
            Me.command = command
        End Sub

        Public Overrides Function IsAvailable(entity As Entity) As Boolean
            Return command.Id <> 0
        End Function
    End Class

    Public Class ThisCommandIsNotAvailableIfIdIs0
        Inherits CommandBase

        Public Property Id As Integer


    End Class


    Public Class Entity
        Public Property A As Integer
    End Class



    Public Class CommandForA
        Inherits CommandBase

        Public MyParam As Integer = 1


    End Class

    Public Class AnotherCommandForA
        Inherits CommandForA

    End Class


    Public Class CalculateKm
        Inherits CommandBase

        Public KmDrive As Integer
        Public DateDriven As DateTime




    End Class

    Public Class TestSecurity
        Implements IActionSecurity

        Public Function EntityIsAvailableForUser(entity As Object) As Boolean Implements IActionSecurity.EntityIsAvailableForUser
            Return True
        End Function

        Public Function GetActionList(entity As Object) As List(Of IActionDescriptor) Implements IActionSecurity.GetActionList
            Return New List(Of IActionDescriptor)
        End Function

        Public Function GetActionNameList(entity As Object) As List(Of String) Implements IActionSecurity.GetActionNameList
            Throw New NotImplementedException()
        End Function

        Public Function UserCanRunThisAction(action As IActionBase) As Boolean Implements IActionSecurity.UserCanRunThisAction
            Throw New NotImplementedException()
        End Function

        Public Function UserCanRunThisAction(action As IActionBase, entity As Object) As Boolean Implements IActionSecurity.UserCanRunThisAction
            Throw New NotImplementedException()
        End Function
    End Class


    Public Class CommandWithoutRetValue
        Inherits CommandBase
    End Class


    Public Class CommandHandler
        Public Function CommandHandler(cmd As ThisCommandIsNotAvailableIfIdIs0) As Object

        End Function

        Public Sub HandleCommandWithoutRetValue(cmd As CommandWithoutRetValue)

        End Sub

        Public Function CommandHandler(cmd As CalculateKm) As Single

            Return CType((cmd.KmDrive * 4.14), Single)

        End Function

        Public Function CommandHandler(command As TestCommand) As Boolean
            Return True
        End Function

    End Class

    Public Class InnerException
        Inherits Exception

    End Class

    Public Class ExceptionIsThrownCommand
        Inherits CommandBase




    End Class

    Public Class AnotherCommand
        Inherits CommandBase



    End Class

    Public Class ByrefCommand
        Inherits CommandBase

        Public Called As Boolean = False



    End Class

    Public Class TestCommand
        Inherits CommandBase




    End Class


    Public Class Another

        Public Function HandleSomethingElseCommandHandler(cmd As AnotherCommand) As Object
            Return Nothing
        End Function


        Public Shared IsCalled As Boolean = False

        Public Function ParamIsByRefCommandHandler(ByRef cmd As ByrefCommand) As Object
            cmd.Called = True
            Return Nothing
        End Function

        Public Function ExceptionIsThrownCommandHandler(cmd As ExceptionIsThrownCommand) As Object

            Throw New InnerException

        End Function

    End Class
End Namespace