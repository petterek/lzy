﻿Imports LazyFramework.Data
Imports NUnit.Framework

<TestFixture> Public Class NewLzyTest

    Public Shared Connection As MSSqlServer.ServerConnectionInfo = New MSSqlServer.ServerConnectionInfo With {.Address = "10.151.46.52", .Database = "hr", .UserName = "loginFor_HR", .Password = "AsDfGhJkL12345"}


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

        Dim data As New FillStatus(Of DataObject)(New DataObject)

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

    <Test> Public Sub ExecNewScalar()

        Dim con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Dim cmd As New SqlClient.SqlCommand("select count(*) from Hrunit")
        Assert.AreNotEqual(0, Store.ExecScalar(Of Integer)(con, cmd))

    End Sub

    <Test> Public Sub ExecNewScalarThatReturnNullValue()

        Dim con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Dim cmd As New SqlClient.SqlCommand("select top 1  null from Hrunit")
        Assert.AreEqual(Nothing, Store.ExecScalar(Of String)(con, cmd))


    End Sub

    <Test> Public Sub ExecFillObjectNew()
        Dim con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Dim cmd As New SqlClient.SqlCommand("select * from Hrunit where id = @id")

        cmd.Parameters.Add(New SqlClient.SqlParameter("id", 3))
        Dim d = New DataObject
        Assert.DoesNotThrow(Sub() Store.Exec(con, cmd, d))
        Assert.AreEqual(3, d.Id)
    End Sub

    <Test> Public Sub TestNonQueryWithOpenConnection()

        Dim cmd As New SqlClient.SqlCommand("select * from Hrunit where id = @id")
        cmd.Parameters.AddWithValue("id", 3)
        Dim d = New DataObject
        Using con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
            con.Open()
            Store.Exec(con, cmd, d)
            Store.Exec(con, cmd, d)
        End Using

    End Sub
    <Test> Public Sub TestNonQuery()
        Dim con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Dim cmd As New SqlClient.SqlCommand("insert into HRunit (UnitTypeId,Name,Guid) values(1,'Test',@guid)")
        Dim d = New DataObject


        Dim guid = System.Guid.NewGuid()

        cmd.Parameters.Add(New SqlClient.SqlParameter("guid", guid))
        Assert.DoesNotThrow(Sub() Store.Exec(con, cmd))


        Dim cmd2 As New SqlClient.SqlCommand("select * from Hrunit where guid = @guid")
        cmd2.Parameters.Add(New SqlClient.SqlParameter("guid", guid))

        con = New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Assert.DoesNotThrow(Sub() Store.Exec(con, cmd2, d))
        Assert.AreEqual(guid, d.Guid)


        Dim cmd3 As New SqlClient.SqlCommand("delete from Hrunit where guid = @guid")
        cmd3.Parameters.Add(New SqlClient.SqlParameter("guid", guid))

        con = New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Assert.DoesNotThrow(Sub() Store.Exec(con, cmd3, d))
        Assert.AreEqual(guid, d.Guid)


    End Sub



    <Test> Public Sub ExecFillListObjectNew()
        Dim con As New SqlClient.SqlConnection(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", "10.151.46.52", "hr", "loginFor_HR", "AsDfGhJkL12345"))
        Dim cmd As New SqlClient.SqlCommand("select * from Hrunit")

        Dim d = New List(Of DataObject)
        Assert.DoesNotThrow(Sub() Store.Exec(con, cmd, d))
        Assert.Greater(d.Count, 0)
        Assert.IsNotEmpty(d(0).Name)
    End Sub


    <Test> Public Sub ExecCommandWithoutResult()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "Select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Update
        cmd2.Parameters.Add("Id", DbType.Int32, False, 1)

        Dim data As New DataObject

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd2))



    End Sub
    <Test> Public Sub ExecCommandWithoutResultAndEqSignInParam()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "Select * from Hrunit where name = @name"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        cmd2.Parameters.Add("name", DbType.String, False, "strange=stuff")

        Dim data As New DataObject

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd2))



    End Sub

    <Test> Public Sub ExecCommandWithoutEqInUpdateSignInParam()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "Update Hrunit set Name='iijj=ijijij'  where name = @name"
        cmd2.TypeOfCommand = CommandTypeEnum.Update
        cmd2.Parameters.Add("name", DbType.String, False, "strange = stuff")

        Dim data As New DataObject

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd2))



    End Sub


    <test> public Sub AddDataToHashSet
        Dim cmd2 As New CommandInfo
        cmd2.CommandText = "Select id from Hrunit"
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        
        Dim data As New HashSet(Of Integer)
        Store.Exec(Connection, cmd2,data,"Id")
        
        Assert.Greater(data.Count,0)

    End Sub

    <Test> Public Sub ProprtiesOfBaseClassIsFilledIfNotFOundOnInstanceClass()

        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "Select * from Hrunit where id = @Id"
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
        cmd2.CommandText = "Select * from HrFile where id = @Id"
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
        cmd2.CommandText = "Select count(*) from Hrunit "
        cmd2.TypeOfCommand = CommandTypeEnum.Read
        Dim ret As Integer

        Assert.DoesNotThrow(Sub() ret = CType(Store.ExecScalar(Connection, cmd2), Integer))
        Assert.AreNotEqual(0, ret)
    End Sub


    <Test> Public Sub DecomposeArrayToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "Select count(*) from Hrunit where Id In(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, {1, 2, 3})
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub

    <Test> Public Sub DecomposeListToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "Select count(*) from Hrunit where Id In(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, New List(Of Integer) From {1, 2, 3})
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub

    <Test> Public Sub DecomposeEmptyArrayToParams()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "Select count(*) from Hrunit where Id In(@Id) "
        cmd.TypeOfCommand = CommandTypeEnum.Read
        cmd.Parameters.AddExpandable("Id", DbType.Int32, New Integer() {})
        Dim ret As New List(Of DataObject)

        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, ret))
        Assert.AreEqual(ret.Count, 1)

    End Sub


    <Test> Public Sub FillListOfValueType()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "Select * FROM (VALUES ('4DD0D8B2-802D-4C1A-8D2B-135B014F362C'), ('4DD0D8B2-802D-4C1A-8D2B-135B014F362C')) AS X(a) "
        cmd.TypeOfCommand = CommandTypeEnum.Read

        Dim ret As New List(Of Guid)

        Store.Exec(Connection, cmd, ret)
        Assert.AreEqual(ret.Count, 2)
        Assert.AreEqual(ret(0), Guid.Parse("4DD0D8B2-802D-4C1A-8D2B-135B014F362C"))

    End Sub

    <Test> Public Sub FillListOfValueTypeNotInConverter()
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "SELECT * FROM (VALUES (1), (2)) AS X(a) "
        cmd.TypeOfCommand = CommandTypeEnum.Read

        Dim ret As New List(Of Integer)

        Store.Exec(Connection, cmd, ret)
        Assert.AreEqual(ret.Count, 2)
        Assert.AreEqual(ret(0), 1)

    End Sub

    <Test> Public Sub FillLilstOfGUIDS()
        Dim list As New List(Of Guid)
        Dim cmd As New Data.CommandInfo
        cmd.CommandText = "SELECT GUID FROM HRUnit"
        cmd.TypeOfCommand = CommandTypeEnum.Read
        Assert.DoesNotThrow(Sub() Store.Exec(Connection, cmd, list))

    End Sub

    <Test> Public Sub ReaderInstanceIsWorking()
        Dim instance = New MSSqlServer.ReaderInstance(Connection)
        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit where id = @Id"
        cmd2.TypeOfCommand = CommandTypeEnum.Create
        cmd2.Parameters.Add("Id", DbType.Int32, False, 3)

        Dim data As New DataObject
        instance.Read(cmd2, data)

        Assert.AreEqual(3, data.Id)

    End Sub

    <Test> Public Sub ReaderInstanceIsWorkingOnList()
        Dim instance = New MSSqlServer.ReaderInstance(Connection)
        Dim cmd2 As New Data.CommandInfo
        cmd2.CommandText = "select * from Hrunit "
        cmd2.TypeOfCommand = CommandTypeEnum.Create

        Dim data As New List(Of DataObject)
        instance.Read(cmd2, data)

        Assert.AreNotEqual(0, data.Count)

    End Sub

    <Test> Public Sub UnabeleToSetValueExceptionContainsBetterInfo()
        Dim instance = New MSSqlServer.ReaderInstance(Connection)
        Dim cmd2 As New Data.CommandInfo

        cmd2.CommandText = "select cast(Id as Char(5)) Id from Hrunit "
        cmd2.TypeOfCommand = CommandTypeEnum.Create

        Dim data As New List(Of DataObject)
        Try
            instance.Read(cmd2, data)
        Catch unable As Data.UnableToSetValueException
            Assert.AreEqual("Id", unable.Name)
            Assert.AreEqual("Unable to map Id to System.Int32 with value '3    '", unable.Message)
        Catch ex As Exception
            Throw
        End Try




    End Sub

End Class

Public Class StreamTo
    Inherits Data.WillDisposeThoseForU
    Public FileSize As Integer
    Public FileData As System.IO.Stream
End Class


Public Class DataObject
    Inherits Data.EntityBase

    Public Guid As Guid
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

    Public InstaceIsCalled As Boolean = False

    Public Overrides Sub Pre(context As DataModificationPluginContext)
        IsCalled = True
        InstaceIsCalled = True
    End Sub

    Public Overrides Sub Post(context As DataModificationPluginContext)
        MyBase.Post(context)
    End Sub


End Class


Public Class InheritedDataObject
    Inherits DataObject

    Public LastChanged As DateTime
End Class