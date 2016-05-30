Namespace Sorting
    Public Interface ISortResult
        Function CompareObjects(x As Object, y As Object) As Integer
    End Interface

    Public MustInherit Class SortResultBase(Of TAction, TDto)
        Inherits Comparer(Of TDto)
        Implements ISortResult

        Public Overridable Function CompareObjects(x As Object, y As Object) As Integer Implements ISortResult.CompareObjects
            Return Compare(CType(x, TDto), CType(y, TDto))
        End Function
    End Class

End NameSpace