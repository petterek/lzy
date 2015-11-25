Namespace Sorting
    Public Class Handler

        Private Shared _AllSorters As Dictionary(Of Type, ISortResult)
        Private Shared _padlock As New Object

        Private Shared ReadOnly Property AllSorters As Dictionary(Of Type, ISortResult)
            Get
                If _AllSorters Is Nothing Then
                    SyncLock _padlock
                        If _AllSorters Is Nothing Then
                            Dim temp As New Dictionary(Of Type, ISortResult)
                            For Each t In LazyFramework.Reflection.FindAllClassesOfTypeInApplication(GetType(ISortResult))
                                Dim actionType = t.BaseType.GetGenericArguments(0)
                                Dim dtoType = t.BaseType.GetGenericArguments(1)
                                temp(actionType) = CType(Activator.CreateInstance(t), ISortResult)
                            Next
                            _AllSorters = temp
                        End If
                    End SyncLock

                End If
                Return _AllSorters
            End Get

        End Property

        Public Shared Sub SortResult(action As IAmAnAction, list As Object)
            If list Is Nothing Then Return
            If Not TypeOf (list) Is IList Then Return
            If Not AllSorters.ContainsKey(action.GetType) Then Return

            Dim sorter As ISortResult = AllSorters(action.GetType)
            DirectCast(list, List(Of Object)).Sort(addressof sorter.CompareObjects)
            
        End Sub

    End Class
End Namespace