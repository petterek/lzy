Imports System.Reflection.Emit
Imports NUnit.Framework



<TestFixture> Public Class ClassFactory

    <SetUp> Public Sub SetUp()
        LazyFramework.ClassFactory.Clear()
        LazyFramework.ClassFactory.LogToDebug = True

        LazyFramework.Runtime.Context.Current = New Runtime.WinThread
    End Sub

    <TearDown> Public Sub TearDown()
    End Sub

    <Test> Public Sub ExpectErrorWhenInsertingNonIsessionAwareIntoSession()
        Assert.Throws(Of NotSupportedException)(Sub() LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, NoneSessionsAwareTest)(True))
    End Sub

    <Test> Public Sub InsertIsessionAwareIntoSession()
        Using New LazyFramework.ClassFactory.SessionInstance
            Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True))
        End Using
    End Sub

    <Test> Public Sub TreadInstanceIsPersisted()
        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5

            Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
        End Using

    End Sub

    <Test> Public Sub TreadInstanceIsNotPresisted()
        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(False)

            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5
            Assert.AreNotEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
            Assert.AreEqual(0, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
        End Using
    End Sub

    <Test> Public Sub TypeIsRemovedWhenSessionIsCompleted()

        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)()
            LazyFramework.ClassFactory.SessionStart()
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest)()
            LazyFramework.ClassFactory.SessionComplete()
        End Using

        Assert.Throws(Of LazyFramework.ClassFactory.NotConfiguredException)(Sub() LazyFramework.ClassFactory.GetTypeInstance(Of ITest)())


    End Sub


    <Test> Public Sub SettingInstanceFromMoq()
        Dim m = Substitute.For(Of ITest)()

        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)(m)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5
            Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
        End Using

    End Sub


    <Test> Public Sub NewSessionSetup()

        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 1
            Using New LazyFramework.ClassFactory.SessionInstance
                LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True)
                LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5
                Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
            End Using
            Assert.AreEqual(1, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)

        End Using

    End Sub

    <Test> Public Sub InstanceIsInheritedFromParentSession()
        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 1
            Using New LazyFramework.ClassFactory.SessionInstance
                Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5)
            End Using
            Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
        End Using
    End Sub

    <Test> Public Sub InstanceAndValueIsInheritedFromParentSession()
        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest, SessionAwareTest)(True)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 1
            Using New LazyFramework.ClassFactory.SessionInstance
                LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5
                Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
            End Using
            Assert.AreEqual(5, LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter)
        End Using
    End Sub


    <Test> Public Sub InsertInstanceToFactory()
        Dim instance As New NoneSessionsAwareTest

        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)(instance)

    End Sub

    <Test> Public Sub TryToInsertNoneInterfaceToFactoryExpectException()

        Assert.Throws(Of NotSupportedException)(Sub() LazyFramework.ClassFactory.SetTypeInstance(Of NoneSessionsAwareTest, NoneSessionsAwareTest)())
        Assert.Throws(Of NotSupportedException)(Sub() LazyFramework.ClassFactory.SetTypeInstance(Of NoneSessionsAwareTest)(New NoneSessionsAwareTest))

    End Sub

    <Test> Public Sub InsertMissmatchingDefaultTypeExpectException()
        Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.SetTypeInstance(Of ITest, NoneSessionsAwareTest)())
    End Sub

    <Test> Public Sub SetInstanceInFactoryReturnsCorrectType()
        Dim m = Substitute.For(Of ITest)()
        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)(m)

        Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.GetTypeInstance(Of ITest)())

    End Sub


    <Test> Public Sub SetInstanceInFactoryReturnsCorrectTypeWhenDefaultTypeIsSet()
        Dim m = Substitute.For(Of ITest)()

        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)(m)

        Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.GetTypeInstance(Of ITest, NoneSessionsAwareTest)())

    End Sub

    <Test> public Sub vetikkehelt()
        Using New LazyFramework.ClassFactory.SessionInstance
            Dim sub1 = Substitute.For(Of ITest)()
            sub1.TestValue.Returns(Function()
                                       Return "From mock"
                                   End Function)

            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)(sub1)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 5
            Assert.AreEqual("From mock", LazyFramework.ClassFactory.GetTypeInstance(Of ITest).TestValue)
        End Using

        Using New LazyFramework.ClassFactory.SessionInstance
            Dim sub1 = Substitute.For(Of ITest)()
            sub1.TestValue.Returns(Function()
                                       Return "From mock2"
                                   End Function)
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)(sub1)
            LazyFramework.ClassFactory.GetTypeInstance(Of ITest).Counter = 10
            Assert.AreEqual("From mock2", LazyFramework.ClassFactory.GetTypeInstance(Of ITest).TestValue)
        End Using
    End Sub

    <Test> Public Sub GettingDefaultTypeWithoutSessionDoesNotFail()

        'Dim o As ITest = LazyFramework.ClassFactory.GetTypeInstance(Of ITest, NoneSessionsAwareTest)()

    End Sub
    <Test> Public Sub GettingSessionInstanceWhenSameTypeIsSetOutsideSession()

        LazyFramework.ClassFactory.SetTypeInstance(Of ITest, NoneSessionsAwareTest)()
        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)(New SessionAwareTest)
            Assert.IsInstanceOf(GetType(SessionAwareTest), LazyFramework.ClassFactory.GetTypeInstance(Of ITest))
        End Using

        Assert.IsInstanceOf(GetType(NoneSessionsAwareTest), LazyFramework.ClassFactory.GetTypeInstance(Of ITest))

    End Sub

    <Test> Public Sub AskForKeyInFactoryDoesNotFailWhenSessionIsNotSet()
        Assert.DoesNotThrow(Sub() LazyFramework.ClassFactory.ContainsKey(Of Object)())
    End Sub


    Public Class NoneSessionsAwareTest
        Implements ITest
        Public Property Counter As Integer Implements ITest.Counter
        Public Function TestValue() As String Implements ITest.TestValue
            Return "noSession"
        End Function
    End Class

    Public Class SessionAwareTest
        Implements ITest
        Implements LazyFramework.ClassFactory.ISessionAware

        Public Property Counter As Integer Implements ITest.Counter

        Public Sub SessionStart() Implements LazyFramework.ClassFactory.ISessionAware.SessionStart

        End Sub

        Public Sub SessionEnd() Implements LazyFramework.ClassFactory.ISessionAware.SessionEnd

        End Sub

        Public Function TestValue() As String Implements ITest.TestValue
            Return "Session"
        End Function

        
    End Class

    Public Interface ITest
        Function TestValue() As String
        Property Counter As Integer
    End Interface


    <Test>
    Public Sub CallDelegateThatCallsSomethingOnMock()
        Dim toChange = 0

        Dim toCall As T

        Runtime.Context.Current = New Runtime.TestContext
        With New LazyFramework.ClassFactory.SessionInstance

            Dim toBeMocked As IToBeMocked = Substitute.For(Of IToBeMocked)()
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of IToBeMocked)(toBeMocked)

            toBeMocked.When(Sub(m) m.T(2)).Do(Sub(p)
                                                  toChange = CType(p(0), Integer)
                                              End Sub)

            toCall = Sub(i)
                         LazyFramework.ClassFactory.GetTypeInstance(Of IToBeMocked).T(i)
                     End Sub

            toCall.BeginInvoke(2, Nothing, Nothing)
        End With

        While toChange = 0

        End While
        Assert.AreEqual(2, toChange)
        

    End Sub

    Public Delegate Sub T(i As Integer)
    Public Interface IToBeMocked
        Sub T(i As Integer)
    End Interface


    <test>
    Public Sub InsertMultipleTypesOfInterfaceWithKeys()

        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)("bla", New SessionAwareTest)
        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)("ukebla", New NoneSessionsAwareTest)


        Assert.IsInstanceOf(Of SessionAwareTest)(LazyFramework.ClassFactory.GetTypeInstance(Of ITest)("bla"))


    End Sub

    <Test> Public Sub InsertTypesWithKey()
        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)("bla", GetType(SessionAwareTest))
        LazyFramework.ClassFactory.SetTypeInstance(Of ITest)("ukebla", GetType(NoneSessionsAwareTest))
        
        Assert.IsInstanceOf(Of SessionAwareTest)(LazyFramework.ClassFactory.GetTypeInstance(Of ITest)("bla"))
        Assert.IsInstanceOf(Of NoneSessionsAwareTest)(LazyFramework.ClassFactory.GetTypeInstance(Of ITest)("ukebla"))

    End Sub

    <Test>
    Public Sub InsertMultipleTypesOfInterfaceWithKeysInSession()

        Using New LazyFramework.ClassFactory.SessionInstance
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)("bla", New SessionAwareTest)
            LazyFramework.ClassFactory.SetTypeInstanceForSession(Of ITest)("ukebla", New NoneSessionsAwareTest)


            Assert.IsInstanceOf(Of SessionAwareTest)(LazyFramework.ClassFactory.GetTypeInstance(Of ITest)("bla"))
        End Using

    End Sub



    Public Class TestType
        Public Sub New()
        End Sub
    End Class

    Private Delegate Function CreateNew() As TestType

    <Test> Public Sub SpeedTest()

        Dim _Constructor As CreateNew
        Dim timings As New LazyFramework.Utils.TimingInfos

        Dim ty = GetType(TestType)
        Dim firs As New List(Of TestType)

        Using New LazyFramework.Utils.InlineTimer("Create First", timings)
            Dim ctor = ty.GetConstructor({})
            Dim method As New DynamicMethod(String.Empty, ty, {})
            Dim gen = method.GetILGenerator()
            gen.Emit(OpCodes.Newobj, ctor)
            gen.Emit(OpCodes.Ret)
            _Constructor = CType(method.CreateDelegate(GetType(CreateNew)), CreateNew)
            Dim result As TestType = _Constructor.Invoke()
        End Using

        Using New Utils.InlineTimer("CreateMany", timings)
            For x = 0 To 1000
                firs.Add(_Constructor())
            Next
        End Using


        Using New Utils.InlineTimer("Activator", timings)
            Dim first = Activator.CreateInstance(ty)
        End Using
        
        Using New Utils.InlineTimer("ActivatorMany", timings)
            For x = 0 To 1000
                firs.Add(CType(Activator.CreateInstance(ty), TestType))
            Next
        End Using


        Using New Utils.InlineTimer("Direct", timings)
            For x = 0 To 1000
                firs.Add(New TestType)
            Next
        End Using

        For Each t In timings
            Debug.Print(t.Key & ":" & t.Value.List(0))
        Next


    End Sub



End Class
