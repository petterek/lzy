Imports NUnit.Framework
Imports LazyFramework.Data

<TestFixture> Public Class ExtensionMethodsTest

    <Test> Public Sub ListIsMapedToDictionary()
        Dim list As New List(Of TestObj)

        Dim guid1 As Guid = Guid.Parse("63c2ea90-6046-4cc8-b779-c22c2097af81")

        list.Add(New TestObj() With {.SomeKey = Guid.Parse("3d93ef89-7095-44c2-ba5f-e48064564368"), .SomeValue = "Value1"})
        list.Add(New TestObj() With {.SomeKey = guid1, .SomeValue = "value2"})


        Dim map = list.ToKeyIndex(Of Guid)(Function(e) e.SomeKey)

        Assert.AreEqual("value2", map(guid1).SomeValue)


    End Sub


    Private Class TestObj
        Public SomeKey As Guid
        Public SomeValue As String
    End Class
End Class
