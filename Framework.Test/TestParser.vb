Imports LazyFramework.Utils.Json
Imports NUnit.Framework

<TestFixture> Public Class TestParser
    <Test> Public Sub ParseSimpleObject()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)("{""Navn"":""Petter""}  ")
        Assert.AreEqual("Petter", p.Navn)
        'Assert.AreEqual(43, p.Alder)
    End Sub

    <Test> Public Sub ParseTextWithEscapeObject()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)("{""Navn"":""Petter\nGjermund\\ \""Han er skikkelig tøff\"" ""}")
        Assert.AreEqual("Petter" & vbCrLf & "Gjermund\ ""Han er skikkelig tøff"" "  , p.Navn)
        'Assert.AreEqual(43, p.Alder)
    End Sub

    <Test> Public Sub ParseComplexObject()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)("{""Navn"":""Petter"",""TestInfo"": {""Name"":""Nils""  }   }")
        Assert.AreEqual("Petter", p.Navn)
        Assert.AreEqual("Nils", p.TestInfo.Name)
        'Assert.AreEqual(43, p.Alder)
    End Sub

    <Test> Public Sub ParseSimpleObjectWithInteger()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)("{""Navn"":""Petter"",""Alder"":  42   }")
        Assert.AreEqual("Petter", p.Navn)
        Assert.AreEqual(42, p.Alder)
    End Sub

    <Test> Public Sub ConsumeJsonWithComments()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)(My.Resources.ConsumeJsonWithComments)
        Assert.AreEqual("Petter", p.Navn)
        Assert.AreEqual(42, p.Alder)
    End Sub

    <Test> Public Sub ParseSimpleObjectWithDouble()
        Dim p = Utils.Json.Reader.StringToObject(Of Person)("{""Navn"":""Petter"",""Speed"":  4.2,""Alder"":42  }")
        Assert.AreEqual("Petter", p.Navn)
        Assert.AreEqual(4.2, p.Speed)
    End Sub


    <Test> Public Sub ParseEmptyList()
        Dim p = Reader.StringToObject(Of List(Of Person))("[]")
        Assert.IsInstanceOf(Of List(Of Person))(p)
        Assert.AreEqual(0, p.Count)
    End Sub

    <Test> Public Sub ParseListOfOneSimpleObject()
        Dim p = Reader.StringToObject(Of List(Of Person))("[{""Navn"":""Petter"",""Speed"":  4.2,""Alder"":42  }]")
        Assert.AreEqual("Petter", p(0).Navn)
        Assert.AreEqual(4.2, p(0).Speed)
    End Sub

    <Test> Public Sub ParseListOfManySimpleObject()
        Dim p = Reader.StringToObject(Of List(Of Person))("[{""Navn"":""Petter"",""Speed"":  4.2,""Alder"":42  },{""Navn"":""Gjermund"",""Speed"":  4.0,""Alder"":40  }]")
        Assert.AreEqual("Gjermund", p(1).Navn)
        Assert.AreEqual(4.0, p(1).Speed)
    End Sub

    <Test> Public Sub ParseListOfManyValueTypes()
        Dim p = Reader.StringToObject(Of List(Of Integer))("[1,2,3]")
        Assert.AreEqual(1, p(0))
        Assert.AreEqual(2, p(1))
    End Sub

    <Test> Public Sub ParseListOfStrings()
        Dim p = Reader.StringToObject(Of List(Of String))("[""1"",""2"",""3""]")
        Assert.AreEqual("1", p(0))
        Assert.AreEqual("2", p(1))
    End Sub

    <Test> Public Sub ParseListAsPartOfObject()
        Dim p = Reader.StringToObject(Of Test)("{""Name"":""Petter"",""Scores"" : [""1"",""2"",""3""]}")
        Assert.AreEqual("Petter", p.Name)
        Assert.AreEqual("1", p.Scores(0))
    End Sub

    <Test> Public Sub ParseValueTypesIntoArray()
        Dim p = Reader.StringToObject(Of TestWithIntArray)("{""Name"":""Petter"",""Scores"" : [1,2,3]}")
        Assert.AreEqual(1, p.Scores(0))
    End Sub

    <Test> Public Sub ParseValueTypesDoubleIntoArray()
        Dim p = Reader.StringToObject(Of TestWithDArray)("{""Name"":""Petter"",""Scores"" : [1.2,2.34,3.12]}")
        Assert.AreEqual(1.2, p.Scores(0))
    End Sub

    <Test> Public Sub Readguid()
        Dim p = Reader.StringToObject(Of Holder(Of Guid))("{""Value"":""FE41254C-FFFC-4121-8345-7353C5D128DC""}")
        Assert.AreEqual(New Guid("FE41254C-FFFC-4121-8345-7353C5D128DC"), p.Value)


    End Sub

    <Test> Public sub EmptyObjectIsRead()
        
        Assert.DoesNotThrow(Sub() Newtonsoft.Json.JsonConvert.DeserializeObject(of Test)("{}"))
        Assert.DoesNotThrow(Sub() LazyFramework.Utils.Json.Reader.StringToObject(of Test)("{}"))

    End sub

    <test> public sub LongValueIsParsed
        
        Dim v As ClassWithLong
        Assert.DoesNotThrow(Sub() v = Reader.StringToObject(Of ClassWithLong)("{""Value"":1446212820320}"))
        Assert.AreEqual(1446212820320,v.Value)


    End Sub

    <test> public sub LongValueIsParsedToProperty
        
        Dim v As ClassWithLongProperty
        Assert.DoesNotThrow(Sub() v = Reader.StringToObject(Of ClassWithLongProperty)("{""Value"":1446212820320}"))
        Assert.AreEqual(1446212820320,v.Value)


    End Sub

    Public Class ClassWithLongProperty
        Public Property Value As Long
    End Class


    <test> Public sub GenericObjectAsDictionary
        
        Dim toTest = "{""Integer"" : 1,""Double"":2.1,""String"":""A string""}"
        Dim res =    Newtonsoft.Json.JsonConvert.DeserializeObject(of Dictionary(Of String,Object))(toTest)


        Assert.AreEqual(1,res("Integer"))
        Assert.AreEqual(2.1,res("Double"))
        Assert.AreEqual("A string",res("String"))
        Assert.IsInstanceOf(Of Int64)(res("Integer"))
        Assert.IsInstanceOf(Of Double)(res("Double"))
    End sub
End Class