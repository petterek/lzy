Namespace Sorting
    Public Class Handler

        Private Shared _AllSorters As New Dictionary(Of Type, ISortResult)
        Private Shared _padlock As New Object

        Private Shared ReadOnly Property AllSorters As Dictionary(Of Type, ISortResult)
            Get
                Return _AllSorters
            End Get

        End Property

        Public Shared Sub AddSorter(Of TAction As IActionBase)(sorter As ISortResult)
            If sorter Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(sorter))
            End If

            AllSorters.Add(GetType(TAction), sorter)
        End Sub

        Public Shared Sub SortResult(action As IActionBase, list As Object)
            If list Is Nothing Then Return
            If Not TypeOf (list) Is IList Then Return
            If Not AllSorters.ContainsKey(action.GetType) Then Return

            Dim sorter As ISortResult = AllSorters(action.GetType)
            DirectCast(list, List(Of Object)).Sort(AddressOf sorter.CompareObjects)

        End Sub

    End Class
End Namespace