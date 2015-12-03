Imports NUnit.Framework

<TestFixture> public Class Reflection

    <Test>Public Sub FindMatchingClassFromNameProperties      
        Assert.AreEqual(1, LazyFramework.Reflection.AllTypes.InNamespace("Test").NameStartsWith("CanThis").NameEndsWith("OrNot").IsAssignableFrom(Of IMarker)().Count)
    End Sub


    Public Interface IMarker
    End Interface

    Public Class CanThisBeFoundOrNot
        Implements IMarker
    End Class


End Class