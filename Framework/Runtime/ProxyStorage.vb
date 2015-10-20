Namespace Runtime
    Public Class ProxyStorage(Of TKey, TValue)
        Implements IDictionary(Of TKey, TValue)

        Private ReadOnly _myValues As New Dictionary(Of TKey, TValue)
        Private ReadOnly _Origin As IDictionary(Of TKey, TValue)

        Public Sub New(origin As IDictionary(Of TKey, TValue))
            _Origin = origin
        End Sub

        Public Function ContainsKey(key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).ContainsKey
            If _myValues.ContainsKey(key) Then
                Return True
            Else
                Return _Origin.ContainsKey(key)
            End If
        End Function

        Default Public Property Item(key As TKey) As TValue Implements IDictionary(Of TKey, TValue).Item
            Get
                If _myValues.ContainsKey(key) Then
                    Return _myValues.Item(key)
                Else
                    Return _Origin(key)
                End If
            End Get
            Set(value As TValue)
                _myValues.Item(key) = value
            End Set
        End Property


        Private Sub Add(item As KeyValuePair(Of TKey, TValue)) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Add
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Clear
            _myValues.Clear()
        End Sub

        Private Function Contains(item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Contains
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of TKey, TValue), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).CopyTo
            Throw New NotImplementedException
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Count
            Get
                Return _myValues.Count + _Origin.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Private Function Remove(item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Remove

        End Function

        Public Sub Add(key As TKey, value As TValue) Implements IDictionary(Of TKey, TValue).Add
            _myValues.Add(key, value)
        End Sub
        
        Public ReadOnly Property Keys As ICollection(Of TKey) Implements IDictionary(Of TKey, TValue).Keys
            Get
                Return _myValues.Keys
            End Get
        End Property

        Public Function Remove(key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).Remove
            Return _myValues.Remove(key)
        End Function

        Public Function TryGetValue(key As TKey, ByRef value As TValue) As Boolean Implements IDictionary(Of TKey, TValue).TryGetValue
            If _myValues.ContainsKey(key) Then
                value = _myValues.Item(key)
                Return True
            ElseIf _Origin.ContainsKey(key) Then
                value = _Origin(key)
                Return True
            End If
            Return False
        End Function

        Public ReadOnly Property Values As ICollection(Of TValue) Implements IDictionary(Of TKey, TValue).Values
            Get
                Dim ret As New List(Of TValue)
                ret.AddRange(_myValues.Values)
                ret.AddRange(_Origin.Values)
                Return ret
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, TValue)) Implements IEnumerable(Of KeyValuePair(Of TKey, TValue)).GetEnumerator
            Return Nothing
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Nothing
        End Function
    End Class
End NameSpace