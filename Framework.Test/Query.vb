Imports LazyFramework.CQRS
Imports NUnit.Framework
Imports LazyFramework.CQRS.Transform
Imports LazyFramework.Test.Cqrs

Public Class DebugLogger
    Implements LazyFramework.CQRS.Monitor.IMonitorWriter


    Public Sub Write(list As IEnumerable(Of Monitor.IMonitorData)) Implements Monitor.IMonitorWriter.Write
        For Each e In list
            Debug.Print(Now().TimeOfDay.ToString & ":" & Newtonsoft.Json.JsonConvert.SerializeObject(e))
        Next
    End Sub

    Public Property IsSuspended As Boolean Implements Monitor.IMonitorWriter.IsSuspended
End Class

Public Class ClassFactoryImpl
    Implements LazyFramework.CQRS.IClassFactory

    Public Function CreateInstance(type As Type) As Object Implements IClassFactory.CreateInstance
        Return LazyFramework.ClassFactory.Construct(type)
    End Function

    Public Function CreateInstance(Of T)() As T Implements IClassFactory.CreateInstance
        Return LazyFramework.ClassFactory.Construct(Of T)()
    End Function
End Class

<TestFixture> Public Class Query

    <SetUp> Public Sub First()

        LazyFramework.CQRS.Setup.ActionSecurity = New TestSecurity
        LazyFramework.CQRS.Setup.ClassFactory = New ClassFactoryImpl

        LazyFramework.CQRS.Query.Handling.ClearHandlers()

        'Debug.Print(Now.Ticks.ToString)
        'LazyFramework.CQRS.Monitor.Handling.StartMonitoring()
        'Monitor.Logger.Loggers.Add(New DebugLogger)
    End Sub

    '<SetUp> Public Sub SetUp()

    'End Sub

    <TearDown> Public Sub TearDown()
        '  LazyFramework.CQRS.Monitor.Handling.StopMonitor()
    End Sub

    <Test> Public Sub QueryFlowIsCorrect()
        Dim q As New TestQuery With {.Id = 1}
        Dim res As Object

        LazyFramework.CQRS.Query.Handling.AddQueryHandler(Of TestQuery)(AddressOf New QueryHandler(New SomeInfoClass).DummyQueryHandler)

        res = LazyFramework.CQRS.Query.Handling.ExecuteQuery(New Object, q)

        Assert.IsInstanceOf(Of QueryResultDto)(res)

    End Sub


    <Test> Public Sub ListIsConvertedCorrectlyAndSorted()

        Dim q As New TestQuery2 With {.Id = 1, .Startdate = Now}
        LazyFramework.CQRS.Query.Handling.AddQueryHandler(Of TestQuery2)(AddressOf New QueryHandler(New SomeInfoClass).Dummy2QueryHandler)
        LazyFramework.CQRS.Transform.EntityTransformerProvider.AddFactory(Of TestQuery2)(New TransformFactory)
        LazyFramework.CQRS.Sorting.Handler.AddSorter(Of TestQuery2)(New SortResult)

        Dim res = LazyFramework.CQRS.Query.Handling.ExecuteQuery(New Object, q)

        Assert.IsInstanceOf(Of QueryResultDto)(CType(res, IEnumerable)(0))

        Assert.AreEqual(4, CType(CType(res, IEnumerable)(0), QueryResultDto).Id)
    End Sub



    <Test> Public Sub ContextSetupIsFound()
        Dim q As New TestQuery With {.Id = 1}
        LazyFramework.CQRS.Query.Handling.AddQueryHandler(Of TestQuery)(AddressOf New QueryHandler(New SomeInfoClass).DummyQueryHandler)
        LazyFramework.CQRS.Transform.EntityTransformerProvider.AddFactory(Of TestQuery)(New TransformFactory)


        Dim res As QueryResultDto = CType(LazyFramework.CQRS.Query.Handling.ExecuteQuery(New Object, q), QueryResultDto)

        Assert.AreEqual(1, res.Id)
        StringAssert.StartsWith("jhjhhjk", res.NameAndDate)


    End Sub
End Class



Public Class ValidateTestQuery
    Inherits LazyFramework.CQRS.Validation.ValidateActionBase(Of TestQuery)


End Class

Public Class ValidateTestQuery3
    Inherits LazyFramework.CQRS.Validation.ValidateActionBase(Of TestQuery3)

End Class

<Monitor.MonitorMaxTime(0)> Public Class TestQuery
    Inherits LazyFramework.CQRS.Query.QueryBase

    Public Id As Integer


End Class


Public Class TestQuery2
    Inherits TestQuery
    Public Startdate As DateTime

End Class

Public Class TestQuery3
    Inherits TestQuery2

End Class

Public Class SomeInfoClass
    Public A As String
End Class

Public Class QueryHandler
    Implements LazyFramework.CQRS.Query.IHandleQuery

    Private ReadOnly _someExternalInjection As SomeInfoClass

    Public Sub New(someExternalInjection As SomeInfoClass)
        _someExternalInjection = someExternalInjection
        _someExternalInjection.A = "jhjhhjk"
    End Sub

    Public Function DummyQueryHandler(q As TestQuery) As QueryResult
        Return New QueryResult With {.Id = 1, .Name = _someExternalInjection.A, .SomeDate = New Date(1986, 7, 24)}
    End Function

    Public Function Dummy2QueryHandler(q As TestQuery2) As List(Of QueryResult)

        Return New List(Of QueryResult) From {
            New QueryResult With {.Id = 1, .Name = "Espen", .SomeDate = New Date(1986, 7, 24)},
            New QueryResult With {.Id = 4, .Name = "Først", .SomeDate = New Date(1986, 7, 22)}}
    End Function

End Class

Public Class SortResult
    Inherits LazyFramework.CQRS.Sorting.SortResultBase(Of TestQuery2, QueryResultDto)

    Public Overrides Function Compare(x As QueryResultDto, y As QueryResultDto) As Integer
        Return y.Id - x.Id
    End Function
End Class

Public Class QueryResult
    Public Id As Integer
    Public Name As String
    Public SomeDate As DateTime
End Class

Public Class QueryResultDto
    Public Id As Integer
    Public NameAndDate As String

End Class


Public Class TransformFactory
    Inherits TransformerFactoryBase(Of TestQuery, QueryResult)

    Dim trans As New Transformers

    Public Overrides Function GetTransformer(action As TestQuery, ent As QueryResult) As ITransformEntityToDto
        Return trans
    End Function
End Class

Friend Class Transformers
    Inherits TransformerBase(Of QueryResult, QueryResultDto)

    Public Overrides Function TransformToDto(profile As Object, ent As QueryResult) As QueryResultDto

        Dim ret As New QueryResultDto
        ret.Id = ent.Id
        ret.NameAndDate = String.Format("{0} har bursdag på {1}", ent.Name, ent.SomeDate.ToShortDateString)
        Return ret
    End Function
End Class




