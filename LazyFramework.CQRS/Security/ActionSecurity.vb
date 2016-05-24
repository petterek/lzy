Namespace Security
    Public Class ActionSecurity
        Private Shared _actionSecurity As IActionSecurity

        Public Shared Property Current As IActionSecurity
            Get
                Return _actionSecurity
            End Get
            Set(value As IActionSecurity)
                _actionSecurity = value
            End Set
        End Property
    End Class
End NameSpace