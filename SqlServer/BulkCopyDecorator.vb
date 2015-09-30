Imports System.Reflection


Friend Class BulkCopyDecorator(Of T)
    Implements IDataReader

    Private ReadOnly _enumerable As IEnumerable(Of T)
    Private ReadOnly _enumerator As IEnumerator(Of T)
    Private _isClosed As Boolean
    Private _propertyInfos As PropertyInfo()
    Private ReadOnly _typeInfo As Type

    Public Sub New(ByVal enumerable As IEnumerable(Of T))
        _enumerable = enumerable
        _enumerator = _enumerable.GetEnumerator

        _typeInfo = GetType(T)
        _propertyInfos = _typeInfo.GetProperties()
    End Sub

    Public Sub Close() Implements IDataReader.Close
        _enumerator.Dispose()
        _isClosed = True
    End Sub

    Public ReadOnly Property Depth As Integer Implements IDataReader.Depth
        Get
            Throw New NotImplementedException("Depth")
        End Get
    End Property

    Public Function GetSchemaTable() As DataTable Implements IDataReader.GetSchemaTable
        Throw New NotImplementedException("GetSchemaTable")
    End Function

    Public ReadOnly Property IsClosed As Boolean Implements IDataReader.IsClosed
        Get
            Return _isClosed
        End Get
    End Property

    Public Function NextResult() As Boolean Implements IDataReader.NextResult
        _isClosed = Not _enumerator.MoveNext()
        Return Not _isClosed
    End Function

    Public Function Read() As Boolean Implements IDataReader.Read
        _isClosed = Not _enumerator.MoveNext()
        Return Not _isClosed
    End Function

    Public ReadOnly Property RecordsAffected As Integer Implements IDataReader.RecordsAffected
        Get
            Return _enumerable.Count
        End Get
    End Property

    Public ReadOnly Property FieldCount As Integer Implements IDataRecord.FieldCount
        Get
            Return _propertyInfos.Length
        End Get
    End Property

    Public Function GetBoolean(i As Integer) As Boolean Implements IDataRecord.GetBoolean
        Throw New NotImplementedException("GetBoolean")
    End Function

    Public Function GetByte(i As Integer) As Byte Implements IDataRecord.GetByte
        Throw New NotImplementedException("GetByte")
    End Function

    Public Function GetBytes(i As Integer, fieldOffset As Long, buffer() As Byte, bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetBytes
        Throw New NotImplementedException("GetBytes")
    End Function

    Public Function GetChar(i As Integer) As Char Implements IDataRecord.GetChar
        Throw New NotImplementedException("GetChar")
    End Function

    Public Function GetChars(i As Integer, fieldoffset As Long, buffer() As Char, bufferoffset As Integer, length As Integer) As Long Implements IDataRecord.GetChars
        Throw New NotImplementedException("GetChars")
    End Function

    Public Function GetData(i As Integer) As IDataReader Implements IDataRecord.GetData
        Throw New NotImplementedException("GetData")
    End Function

    Public Function GetDataTypeName(i As Integer) As String Implements IDataRecord.GetDataTypeName
        Return _propertyInfos(i).PropertyType.FullName
    End Function

    Public Function GetDateTime(i As Integer) As Date Implements IDataRecord.GetDateTime
        Throw New NotImplementedException("GetDateTime")
    End Function

    Public Function GetDecimal(i As Integer) As Decimal Implements IDataRecord.GetDecimal
        Throw New NotImplementedException("GetDecimal")
    End Function

    Public Function GetDouble(i As Integer) As Double Implements IDataRecord.GetDouble
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), Double)
    End Function

    Public Function GetFieldType(i As Integer) As Type Implements IDataRecord.GetFieldType
        Return _propertyInfos(i).PropertyType
    End Function

    Public Function GetFloat(i As Integer) As Single Implements IDataRecord.GetFloat
        Throw New NotImplementedException("GetFloat")
    End Function

    Public Function GetGuid(i As Integer) As Guid Implements IDataRecord.GetGuid
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), Guid)
    End Function

    Public Function GetInt16(i As Integer) As Short Implements IDataRecord.GetInt16
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), Int16)
    End Function

    Public Function GetInt32(i As Integer) As Integer Implements IDataRecord.GetInt32
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), Int32)
    End Function

    Public Function GetInt64(i As Integer) As Long Implements IDataRecord.GetInt64
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), Int64)
    End Function

    Public Function GetName(i As Integer) As String Implements IDataRecord.GetName
        Return _propertyInfos(i).Name
    End Function

    Public Function GetOrdinal(name As String) As Integer Implements IDataRecord.GetOrdinal
        Dim x As Integer = 0
        For Each e In _propertyInfos
            If e.Name = name Then
                Return x
            End If
            x += 1
        Next

        Return -1
    End Function

    Public Function GetString(i As Integer) As String Implements IDataRecord.GetString
        Return DirectCast(_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing), String)
    End Function

    Public Function GetValue(i As Integer) As Object Implements IDataRecord.GetValue
        Return _propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing)
    End Function

    Public Function GetValues(values() As Object) As Integer Implements IDataRecord.GetValues
        Throw New NotImplementedException("GetValues")
    End Function

    Public Function IsDBNull(i As Integer) As Boolean Implements IDataRecord.IsDBNull
        Return TypeOf (_propertyInfos(i).GetMethod().Invoke(_enumerator.Current, Nothing)) Is DBNull
    End Function

    Default Public Overloads ReadOnly Property Item(i As Integer) As Object Implements IDataRecord.Item
        Get
            Throw New NotImplementedException("Item")
        End Get
    End Property

    Default Public Overloads ReadOnly Property Item(name As String) As Object Implements IDataRecord.Item
        Get
            Throw New NotImplementedException("Item")
        End Get
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean


    ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
