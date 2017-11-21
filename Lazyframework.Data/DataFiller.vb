Imports System.Data.SqlClient
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.Serialization

Public Class DataFiller

    Private ReadOnly _fields As New List(Of FieldInfoDecorator)

    Private Class FieldInfoDecorator
        Private ReadOnly _col As Integer
        Private ReadOnly _name As String
        Private ReadOnly _memberInfo As MemberInfo
        Private ReadOnly _setter As Func(Of Object, Object, Object)



        Public Sub New(ByVal reader As Object, ByVal col As Integer, ByVal memberInfo As MemberInfo, ByVal name As String, ByVal mapByName As Boolean)
            _col = col
            _memberInfo = memberInfo
            _name = name

            Dim memberType As Type = Nothing

            Select Case memberInfo.MemberType
                Case MemberTypes.Property
                    memberType = CType(memberInfo, PropertyInfo).PropertyType
                Case MemberTypes.Field
                    memberType = CType(memberInfo, FieldInfo).FieldType
            End Select

            If memberType Is Nothing Then
                Throw New NotSupportedException("Member type is not supported. Only properties and fields are supported")
            End If

            If GetType(System.IO.Stream).IsAssignableFrom(memberType) Then
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

            _setter = Reflection.CreateSetter(memberInfo)
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
            Dim tempValue As Object
            Try
                tempValue = _getValue(reader)

                If TypeOf (tempValue) Is DBNull Then
                    _setter(o, Nothing)
                    '_fieldInfo.SetValue(o, Nothing)
                Else
                    _setter(o, tempValue)
                    '_fieldInfo.SetValue(o, tempValue)
                End If
            Catch ex As Exception
                Throw New UnableToSetValueException(ex, _name, _memberInfo, tempValue)
            End Try

        End Sub

    End Class


    Public Sub New(ByVal dataReader As IDataReader, ByVal t As Type, mapByName As Boolean)
        'Her kunne vi laget noe lureri for å gjøre dette med emitting av il, men det lar vi være enn så lenge. 
        Dim memberInfo As MemberInfo = Nothing
        Dim currType = t
        Dim n As String

        For x = 0 To dataReader.FieldCount - 1
            n = dataReader.GetName(x)
            memberInfo = LazyFramework.Reflection.SearchForFieldInfo(currType, n)
            If memberInfo IsNot Nothing Then
                _fields.Add(New FieldInfoDecorator(dataReader, x, memberInfo, n, mapByName))
            End If
        Next
    End Sub

    Public Sub FillObject(reader As IDataReader, data As Object)
        For Each fieldInfo In _fields
            fieldInfo.SetValueToObject(reader, data)
        Next
    End Sub
End Class

<Serializable>
Public Class UnableToSetValueException
    Inherits Exception

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Public Sub New(innerException As Exception, _name As String, _fieldInfo As MemberInfo, value As Object)
        MyBase.New(CreateMessage(_name, _fieldInfo, value), innerException)
        Me.Name = _name
        Me.FieldInfo = _fieldInfo

    End Sub

    Private Shared Function CreateMessage(_name As String, _fieldInfo As MemberInfo, value As Object) As String
        Dim retType As String = "UNKNOWN"
        Select Case _fieldInfo.MemberType
            Case MemberTypes.Property
                retType = CType(_fieldInfo, PropertyInfo).PropertyType.FullName()
            Case MemberTypes.Field
                retType = CType(_fieldInfo, FieldInfo).FieldType.FullName()
        End Select

        Return "Unable to map " + _name + " to " + retType + " with value '" + value.ToString() + "'"
    End Function

    Public ReadOnly Property Name As String
    Public ReadOnly Property FieldInfo As MemberInfo

    Protected Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Overrides Function ToString() As String
        Return "Unable to map the " + Name + " to the type " + FieldInfo.MemberType.ToString
    End Function
End Class
