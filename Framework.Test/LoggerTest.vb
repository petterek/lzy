Imports LazyFramework.Logging
Imports NUnit.Framework

<TestFixture> Public Class LoggerTest
    <Test> Public Sub AddWriterToList()

        LazyFramework.Logging.Log.AddWriter(Of SomeInfoClass)(New LogWriter)

    End Sub

    <Test> Public Sub WriteToLoggers()
        Dim logWriter1 As LogWriter = New LogWriter
        Dim objectLogger As New ObjectLogger

        Log.AddWriter(Of SomeInfoClass)(logWriter1)
        Log.AddWriter(Of object)(objectLogger)

        LazyFramework.Logging.Log.Write(Of SomeInfoClass)( New SomeInfoClass,LogLevelEnum.Verbose)

        Assert.IsInstanceOf(Of SomeInfoClass)(logWriter1.JustForTest)
        Assert.IsNotNull(objectLogger.anyObject)

    End Sub

End Class

Public Class SomeInfoClass
    Public A As String
End Class



Public Class ObjectLogger
    Implements ILogWriter

    Public anyObject As Object

    Public Sub Write(info As LogInfo) Implements ILogWriter.Write
        anyObject = info.LogData
    End Sub

    Public Function Level() As LogLevelEnum Implements ILogWriter.Level
        Return LogLevelEnum.Verbose
    End Function
End Class

Public Class LogWriter
    Implements LazyFramework.Logging.ILogWriter

    Public JustForTest As SomeInfoClass

    Public Sub Write(info As LogInfo) Implements ILogWriter.Write
        JustForTest = DirectCast(info.LogData, SomeInfoClass)
    End Sub

    Public Function Level() As LogLevelEnum Implements ILogWriter.Level
        Return LogLevelEnum.Verbose
    End Function
End Class