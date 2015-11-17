Imports System.Data.SqlClient
Imports System.Linq.Expressions
Imports System.Reflection


Public Class DataFiller

    Private ReadOnly _fields As New List(Of FieldInfoDecorator)

    Private Class FieldInfoDecorator
        Private ReadOnly _col As Integer
        Private ReadOnly _name As String
        Private ReadOnly _fieldInfo As FieldInfo
        Private ReadOnly _setter As Action(Of Object, Object)

        


        Public Sub New(ByVal reader As Object, ByVal col As Integer, ByVal fieldInfo As FieldInfo, ByVal name As String, ByVal mapByName As Boolean)
            _col = col
            _fieldInfo = fieldInfo
            _name = name
            If GetType(System.IO.Stream).IsAssignableFrom(_fieldInfo.FieldType) Then
                If TypeOf (reader) Is SqlDataReader Then
                    _getValue = AddressOf GetStream
                Else
                    Throw New NotSupportedException("Streams is only supported for SQLDataReader.")
                End If
            Else
                If mapByName Then
                    _getValue = AddressOf GetPrimitiveTypeFromName
                Else
                    _getValue = AddressOf GetPrimitiveType
                End If
            End If

            _setter = Reflection.CreateSetter(fieldInfo)
        End Sub

        Private ReadOnly _getValue As GetValueDelegate

        Private Function GetPrimitiveType(o As IDataReader) As Object
            Return o.GetValue(_col)
        End Function

        Private Function GetPrimitiveTypeFromName(o As IDataReader) As Object
            Return o.GetValue(o.GetOrdinal(_name))
        End Function


        Private Function GetStream(o As IDataReader) As System.IO.Stream
            Return CType(o, SqlDataReader).GetStream(_col)
        End Function

        Private Delegate Function GetValueDelegate(o As IDataReader) As Object

        Public Sub SetValueToObject(reader As IDataReader, o As Object)
            Dim tempValue = _getValue(reader)



            If TypeOf (tempValue) Is DBNull Then
                _setter(o, Nothing)
                '_fieldInfo.SetValue(o, Nothing)
            Else
                _setter(o, tempValue)
                '_fieldInfo.SetValue(o, tempValue)
            End If
        End Sub

    End Class


    Public Sub New(ByVal dataReader As IDataReader, ByVal t As Type, mapByName As Boolean)
        'Her kunne vi laget noe lureri for å gjøre dette med emitting av il, men det lar vi være enn så lenge. 

        Dim n As String
        For x = 0 To dataReader.FieldCount - 1
            n = dataReader.GetName(x)

            Dim fieldInfo As FieldInfo = Nothing
            Dim currType As Type
            currType = t

            fieldInfo = LazyFramework.Reflection.SearchForFieldInfo(currType,  n)
            

            If fieldInfo IsNot Nothing Then
                _fields.Add(New FieldInfoDecorator(dataReader, x, fieldInfo, n, mapByName))
            End If
        Next
    End Sub

    Public Sub FillObject(reader As IDataReader, data As Object)
        For Each fieldInfo In _fields
            fieldInfo.SetValueToObject(reader, data)
        Next
    End Sub
End Class