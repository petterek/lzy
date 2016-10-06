Namespace ActionContext
    Public Class Handling

        Private Shared _AllMappings As New Dictionary(Of Type, List(Of ActionContext))
        Private Shared ReadOnly Property AllMappings As Dictionary(Of Type, List(Of ActionContext))
            Get
                Return _AllMappings
            End Get
        End Property

        Public Shared Sub AddContext(Of TAction As IActionBase)(context As ActionContext)
            If Not _AllMappings.ContainsKey(GetType(TAction)) Then
                _AllMappings(GetType(TAction)) = New List(Of ActionContext)
            End If
            _AllMappings(GetType(TAction)).Add(context)
            _AllContexts.Add(context.Name, context)
        End Sub

        Private Shared _AllContexts As New Dictionary(Of String, ActionContext)
        Public Shared ReadOnly Property AllContexts() As IDictionary(Of String, ActionContext)
            Get
                Return _AllContexts
            End Get
        End Property

        Public Shared Iterator Function GetContextsForAction(action As IActionBase) As IEnumerable(Of ActionContext)
            If AllMappings.ContainsKey(action.GetType) Then
                For Each a In AllMappings(action.GetType)
                    Yield a
                Next
            End If
        End Function
    End Class
End NameSpace