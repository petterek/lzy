Imports LazyFramework.Data
Imports LazyFramework.Logging
Imports NUnit.Framework

<TestFixture> Public Class NewLzyTest

    Public Shared Connection As New MSSqlServer.ServerConnectionInfo With {.Address = "13-testsql", .Database = "hr", .UserName = "sa", .Password = "supermann"}

    Private _MemoryLogger As MemoryWriter


    <SetUp> Public Sub SetUp()
        Runtime.Context.Current = New Runtime.WinThread
        LazyFramework.ClassFactory.Clear()

        Dim lazyFrameworkConfiguration As LazyFrameworkConfiguration = New LazyFrameworkConfiguration
        LazyFramework.ClassFactory.SetTypeInstance(Of ILazyFrameworkConfiguration)(lazyFrameworkConfiguration)

        lazyFrameworkConfiguration.LogLevel = 100
        lazyFrameworkConfiguration.EnableTiming = True


        _MemoryLogger = New LazyFramework.Logging.MemoryWriter
        Log.AddWriter(Of Object)(_MemoryLogger)

    End Sub

    <TearDown> Public Sub Tear()
        LazyFramework.ClassFactory.Clear()
    End Sub

    <Test> Public Sub FillObject()

        Dim cmd As New data.CommandInfo

        Dim a = New With {.a = 123}

        cmd.CommandText = "select * from Hrunit where id = @Id"
        cmd.TypeOfCommand = data.CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32)



        Dim ret As New DataObject
        ret.Id = 27
        Store.Exec(Connection, cmd, ret)
        Assert.AreEqual("Sigurd Brekkesen", ret.Name)

        Debug.Print(LazyFramework.Utils.ResponseThread.Current.Timer.Timings.Count.ToString)
        For Each t In LazyFramework.Utils.ResponseThread.Current.Timer.Timings
            Debug.Print(t.Key & t.Value.List(0))
        Next


    End Sub


    <Test> Public Sub SelectMany()

        Dim cmd As New data.CommandInfo
        cmd.CommandText = "select Id,Name,Inactive from Hrunit"
        cmd.TypeOfCommand = commandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, 1)

        Dim ret As New List(Of DataObject)

        data.Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
        'Assert.AreEqual("Petter Ekrann", ret.Name)
    End Sub

    <Test> Public Sub InheritedGenericListIsWorking()
        Dim cmd As New LazyFramework.Data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, 1)

        Dim ret As New DataObjectList

        Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
    End Sub


    <Test> Public Sub ListWithObjectsOfEntityBaseGetCorrectFillStatus()
        Dim cmd As New data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, 1)

        Dim ret As New DataObjectList

        Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
        Assert.AreEqual(ret(0).FillResult, FillResultEnum.DataFound)


    End Sub



    Public Class DataObjectList
        Inherits List(Of DataObject)

    End Class


    <Test> Public Sub FillerIsReused()

        Store.Fillers.Clear()

        'Hent mange
        Dim cmd As New data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        Dim ret As New List(Of DataObject)


        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, 1)

        Dim data As New DataObject
        Store.Exec(Connection, cmd, ret)
        Store.Exec(Connection, cmd2, data)

        Assert.AreEqual(1, Store.Fillers.Count)

    End Sub

    <Test> Public Sub UseExpression()
        Dim cmd As New CommandInfo
        cmd.TypeOfCommand = CommandTypeEnum.Read

        'cmd.CommandQuery 

        Dim ret As New List(Of DataObject)

    End Sub


    <Test> Public Sub TestNullables()

        Dim o = New Nullable(Of Integer)(4)
        Dim o2 As Integer?
        Dim o3 As Object = New Nullable(Of Integer)
        Dim o4 As Integer? = 4


        'Debug.Print(o2.GetValueOrDefault.ToString)

        Assert.AreNotEqual(o, Nothing)
        Assert.AreEqual(o2, Nothing)
        Assert.AreEqual(o3, Nothing)
        Assert.AreNotEqual(o4, Nothing)

        Assert.IsTrue(o.GetType.IsValueType)
        Assert.IsFalse(o4.GetType.IsGenericType)



    End Sub



    <Test> Public Sub UseFillStatus()

        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, 27)

        Dim data As New FillStatus(Of DataObject)

        Store.Exec(Connection, cmd2, data)

        Assert.AreEqual(FillResultEnum.DataFound, data.FillResult)



    End Sub

    <Test> Public Sub TestingLog()
        
        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, 1)


        Dim data As New DataObject
        Store.Exec(Connection, cmd2, data)

    End Sub


    <Test> Public Sub PluginIsFired()
        Store.RegisterPlugin(Of TestPlugin)()
        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Create
        cmd2.Parameters.Add("Id", DbType.Int32, 1)

        Dim data As New DataObject
        Store.Exec(Connection, cmd2, data)

        Assert.IsTrue(TestPlugin.IsCalled)

    End Sub

    <Test> Public Sub ExecCommandWithoutResult()

        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Update
        cmd2.Parameters.Add("Id", DbType.Int32, 1)

        Dim data As New DataObject

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd2))



    End Sub


    <Test> Public Sub ProprtiesOfBaseClassIsFilledIfNotFOundOnInstanceClass

        Dim cmd2 As New data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, 27)

        Dim data As New InheritedDataObject

        LazyFramework.Data.Store.Exec(Connection, cmd2,data)
        Assert.IsNotNull(data.LastChanged)
        Assert.AreEqual(27, data.Id)
    End Sub


    <Test> Public Sub ReadStreamFromTable()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from HrFile where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, 2375)

        Using data As New StreamTo
            Dim mem As New System.IO.MemoryStream
            Store.GetStream(Of StreamTo)(Connection, cmd2, data)
            Assert.Greater(data.FileSize, 0)
            Assert.IsInstanceOf(Of System.IO.Stream)(data.FileData)
            data.FileData.CopyTo(mem)
            Assert.AreEqual(mem.Length, data.FileSize)
        End Using

    End Sub

End Class

Public Class StreamTo
    Inherits Data.WillDisposeThoseForU
    Public FileSize As Integer
    Public FileData As System.IO.Stream
End Class


Public Class DataObject
    Inherits data.EntityBase


    Property Id As Integer
    Property Name As String
    Property Age As Integer
    Property BirthDay As DateTime

    Property Mail As List(Of Mail)
    Property IsSet As Boolean?


End Class

Public Class Mail
    Property PersonId As Integer
    Property Address As String
End Class


Public Class TestPlugin
    Inherits Data.DataModificationPluginBase

    Public Shared IsCalled As Boolean = False

    Public Overrides Sub Pre(context As DataModificationPluginContext)
        IsCalled = True
    End Sub

    Public Overrides Sub Post(context As DataModificationPluginContext)
        MyBase.Post(context)
    End Sub


End Class


Public Class InheritedDataObject
    Inherits DataObject

    Public LastChanged As DateTime
End Class