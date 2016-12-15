Imports LazyFramework.Data
Imports NUnit.Framework

<TestFixture> Public Class NewLzyTest

    Public Shared Connection As Data.ServerConnectionInfo = New MSSqlServer.ServerConnectionInfo With {.Address = "10.151.46.52", .Database = "hr", .UserName = "loginFor_HR", .Password = "AsDfGhJkL12345"}


    <SetUp> Public Sub SetUp()
        Runtime.Context.Current = New Runtime.WinThread
        LazyFramework.ClassFactory.Clear()
    End Sub

    <TearDown> Public Sub Tear()
        LazyFramework.ClassFactory.Clear()
    End Sub

    <Test> Public Sub FillObject()

        Dim cmd As New Data.CommandInfo
        Dim a = New With {.a = 123}

        cmd.CommandText = "select * from Hrunit where id = @Id"
        cmd.TypeOfCommand = Data.CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, False, 27)

        Dim ret As New DataObject
        Store.Exec(Connection, cmd, ret)
        Assert.AreEqual(27, ret.id)

        Debug.Print(LazyFramework.Utils.ResponseThread.Current.Timer.Timings.Count.ToString)
        For Each t In LazyFramework.Utils.ResponseThread.Current.Timer.Timings
            Debug.Print(t.Key & t.Value.List(0))
        Next

    End Sub

    
    <Test> Public Sub FillGenericListObject()

        Dim cmd As New Data.CommandInfo

        cmd.CommandText = "select * from Hrunit where id = @Id"
        cmd.TypeOfCommand = Data.CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, False, 27)

        Dim ret As New DataObjectList

        Store.Exec(Connection, cmd, ret)
        Assert.AreNotEqual(0, ret.Count)

        Debug.Print(LazyFramework.Utils.ResponseThread.Current.Timer.Timings.Count.ToString)
        For Each t In LazyFramework.Utils.ResponseThread.Current.Timer.Timings
            Debug.Print(t.Key & t.Value.List(0))
        Next


    End Sub


    <Test> Public Sub SelectMany()

        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select Id,Name,Inactive from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim ret As New List(Of DataObject)

        Data.Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
        'Assert.AreEqual("Petter Ekrann", ret.Name)
    End Sub

    <Test> Public Sub InheritedGenericListIsWorking()
        Dim cmd As New LazyFramework.Data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim ret As New DataObjectList

        Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
    End Sub


    <Test> Public Sub ListWithObjectsOfEntityBaseGetCorrectFillStatus()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim ret As New DataObjectList

        Store.Exec(Connection, cmd, ret)

        Assert.AreNotEqual(0, ret.Count)
        Assert.AreEqual(ret(0).FillResult, FillResultEnum.DataFound)


    End Sub


    <Test> Public Sub ReadonlyFieldsIsFilled()

        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        Dim ret As New List(Of UnitWithReadOnly)


        Store.Exec(Connection, cmd, ret)


    End Sub


    Public Class UnitWithReadOnly

        Private _name As String

        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property
    End Class


    Public Class DataObjectList
        Inherits List(Of DataObject)

    End Class


    <Test> Public Sub FillerIsReused()

        Store.Fillers.Clear()

        'Hent mange
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select * from Hrunit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        Dim ret As New List(Of DataObject)


        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, False, 1)

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

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, False, 27)

        Dim data As New FillStatus(Of DataObject)

        Store.Exec(Connection, cmd2, data)

        Assert.AreEqual(FillResultEnum.DataFound, data.FillResult)



    End Sub

    <Test> Public Sub TestingLog()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, False, 1)


        Dim data As New DataObject
        Store.Exec(Connection, cmd2, data)

    End Sub


    <Test> Public Sub PluginIsFired()
        Store.RegisterPlugin(Of TestPlugin)()
        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Create
        cmd2.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim data As New DataObject
        Store.Exec(Connection, cmd2, data)

        Assert.IsTrue(TestPlugin.IsCalled)

    End Sub

    <Test> Public Sub ExecCommandWithoutResult()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Update
        cmd2.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim data As New DataObject

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd2))



    End Sub
    

     <test> public Sub AddDataToHashSet
        Dim cmd2 As New CommandInfo
        cmd2.CommandText = "select id from Hrunit"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        
        Dim data As New HashSet(Of Integer)
        Store.Exec(Connection, cmd2,data,"Id")
        
        Assert.Greater(data.Count,0)

    End Sub

    <Test> Public Sub ProprtiesOfBaseClassIsFilledIfNotFOundOnInstanceClass()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, False)

        Dim data As New InheritedDataObject
        data.Id = 27
        LazyFramework.Data.Store.Exec(Connection, cmd2, data)
        Assert.IsNotNull(data.LastChanged)
        Assert.AreEqual(27, data.Id)
    End Sub


    <Test, Ignore> Public Sub ReadStreamFromTable()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from HrFile where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("Id", DbType.Int32, False, 19)

        Using data As New StreamTo
            Dim mem As New System.IO.MemoryStream
            Store.GetStream(Of StreamTo)(Connection, cmd2, data)
            Assert.Greater(data.FileSize, 0)
            Assert.IsInstanceOf(Of System.IO.Stream)(data.FileData)
            data.FileData.CopyTo(mem)
            Assert.AreEqual(mem.Length, data.FileSize)
        End Using

    End Sub

    <Test> Public Sub ExecScalar()
        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select count(*) from Hrunit "
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        Dim ret As Integer

        Assert.DoesNotThrow(Sub() ret = CType(Store.ExecScalar(Connection, cmd2), Integer))
        Assert.AreNotEqual(0, ret)
    End Sub


    <Test> Public Sub DecomposeArrayToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select count(*) from Hrunit where Id in(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, {1, 2, 3})
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub

    <Test> Public Sub DecomposeListToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select count(*) from Hrunit where Id in(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, New List(Of Integer) From {1, 2, 3})
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub

    <Test> Public Sub DecomposeEmptyArrayToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "select count(*) from Hrunit where Id in(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, New Integer(){} )
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub

End Class

Public Class StreamTo
    Inherits Data.WillDisposeThoseForU
    Public FileSize As Integer
    Public FileData As System.IO.Stream
End Class


Public Class DataObject
    Inherits Data.EntityBase

    Private _id As Integer
    Property Id As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property
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