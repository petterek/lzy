Namespace Security
    Public Interface IProvideSecurityContext
        Property ContextId As Integer
        Function Context() As Object
    End Interface
End NameSpace