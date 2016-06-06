Imports System.IO

Namespace Utils.Json
    Public Class Reader

        Public Shared Function StringToObject(Of T As New)(input As String) As T
            Dim mem As New MemoryStream
            Dim write As New StreamWriter(mem, Text.Encoding.UTF8)
            write.Write(input)
            write.Flush()
            write.BaseStream.Position = 0
            Return StringToObject(Of T)(New StreamReader(mem, System.Text.Encoding.UTF8))
        End Function

        Public Shared Function StringToObject(input As String, type As System.Type) As Object
            Dim mem As New MemoryStream
            Dim write As New StreamWriter(mem, Text.Encoding.UTF8)
            write.Write(input)
            write.Flush()
            write.BaseStream.Position = 0
            Return StringToObject(New ReadStream(New StreamReader(mem, System.Text.Encoding.UTF8)), type)
        End Function

        'Here we can add another function that accepts stream as parameter
        Public Shared Function StringToObject(Of T As New)(input As StreamReader) As T

            Return DirectCast(StringToObject(New ReadStream(input), GetType(T)), T)

        End Function

        Public Shared Function StringToObject(input As StreamReader, type As Type) As Object

            Return StringToObject(New ReadStream(input), type)

        End Function

        Friend Shared Function StringToObject(input As IReader, type As Type) As Object
            Dim builder As Builder

            If GetType(IList).IsAssignableFrom(type) Then
                builder = New ArrayBuilder()
            ElseIf GetType(IDictionary).IsAssignableFrom(type) Then
                builder = New DictionaryBuilder()
            ElseIf GetType(ICollection).IsAssignableFrom(type) Then
                builder = New ArrayBuilder()
            Else
                builder = New ObjectBuilder()
            End If

            Return builder.Parse(input, type)
        End Function



    End Class


    <Serializable> Friend Class PropertyNotFoundException
        Inherits Exception

        Public Sub New(ByVal result As String)
            MyBase.New(result)
        End Sub
    End Class

    <Serializable> Public Class UncompleteException
        Inherits Exception

    End Class

    <Serializable> Public Class MissingTokenException
        Inherits Exception

        Private ReadOnly _S As String

        Public Sub New(ByVal s As String, ByVal nextChar As IReader)
            MyBase.New("Missing: " & s & vbCrLf & "@" & nextChar.Position & nextChar.Read(25))
            _S = s
        End Sub

        Public ReadOnly Property Token As String
            Get
                Return _S
            End Get
        End Property
    End Class
End Namespace