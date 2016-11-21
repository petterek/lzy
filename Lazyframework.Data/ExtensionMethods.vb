Imports System.Runtime.CompilerServices

Public Module ExtensionMethods

    <Extension()> Public Function ToKeyIndex(Of TKey, TValue)(toMap As IEnumerable(Of TValue), getKey As Func(Of TValue, TKey)) As Dictionary(Of TKey, TValue)

        Dim ret As New Dictionary(Of TKey, TValue)

        For Each v In toMap
            ret.Add(getKey(v), v)
        Next
        Return ret
    End Function
End Module
