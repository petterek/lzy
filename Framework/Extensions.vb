Imports System.Reflection
Imports System.Runtime.CompilerServices
Public Module Extensions
    <Extension> Public Iterator Function NameEndsWith(toSearch As IEnumerable(Of Type), search As String) As IEnumerable(Of Type)
        For Each e In toSearch
            If e.Name.EndsWith(search) Then Yield e
        Next
    End Function

    <Extension> Public Iterator Function NameStartsWith(toSearch As IEnumerable(Of Type), search As String) As IEnumerable(Of Type)
        For Each e In toSearch
            If e.Name.StartsWith(search) Then Yield e
        Next
    End Function

    <Extension> Public Iterator Function InNamespace(toSearch As IEnumerable(Of Type), search As String) As IEnumerable(Of Type)
        For Each e In toSearch
            If e.FullName.Contains(search & ".") Then Yield e
        Next
    End Function

    <Extension> Public Iterator Function IsAssignableFrom(Of T)(toSearch As IEnumerable(Of Type)) As IEnumerable(Of Type)
        Dim type = GetType(T)
        For Each e In toSearch
            If type.IsAssignableFrom(e) Then Yield e
        Next
    End Function

    <Extension> Public Iterator Function AllMethods(toSearch As IEnumerable(Of Type)) As IEnumerable(Of MethodInfo)
        For Each e In toSearch
            For Each m In e.GetMethods
                Yield m
            Next
        Next
    End Function

    <Extension> Public Iterator Function NameEndsWith(toSearch As IEnumerable(Of MethodInfo), search As String) As IEnumerable(Of MethodInfo)
        For Each m In toSearch
            If m.Name.EndsWith(search) Then Yield m
        Next

    End Function
    <Extension> Public Iterator Function NameStartsWith(toSearch As IEnumerable(Of MethodInfo), search As String) As IEnumerable(Of MethodInfo)
        For Each m In toSearch
            If m.Name.StartsWith(search) Then Yield m
        Next
    End Function
    <Extension> Public Iterator Function SignatureIs(toSearch As IEnumerable(Of MethodInfo), ParamArray types() As Type) As IEnumerable(Of MethodInfo)
        For Each m In toSearch

            Dim params = m.GetParameters
            Dim isValid As Boolean = True

            If params.Length <> types.Length Then
                Continue For
            End If

            For x = 0 To params.Length - 1
                Dim parameterType As Type

                parameterType = params(x).ParameterType

                If parameterType.IsByRef Then
                    parameterType = parameterType.GetElementType
                End If

                If Not types(x).IsAssignableFrom(parameterType) Then
                    isValid = False
                    Exit For
                End If
            Next
            If isValid Then Yield m
        Next
    End Function



    <Extension> Public Iterator Function IsFunction(toSearch As IEnumerable(Of MethodInfo)) As IEnumerable(Of MethodInfo)
        For Each m In toSearch
            If m.ReturnType IsNot GetType(System.Void) Then Yield m
        Next
    End Function

    <Extension> Public Iterator Function IsSub(toSearch As IEnumerable(Of MethodInfo)) As IEnumerable(Of MethodInfo)
        For Each m In toSearch
            If m.ReturnType Is GetType(System.Void) Then Yield m
        Next
    End Function

End Module