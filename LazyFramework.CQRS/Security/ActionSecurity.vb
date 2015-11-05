Namespace Security
    Public Class ActionSecurity
        Private Shared _actionSecurity As IActionSecurity

        Public Shared ReadOnly Property Current As IActionSecurity
            Get
                If _actionSecurity Is Nothing Then
                    'trenger ikke lock da denne er den samme hele tiden. 
                    'om det skulle bli dobbelt så har det ikke noe å si.. 
                    _actionSecurity = ClassFactory.GetTypeInstance(Of IActionSecurity)()
                End If
                Return _actionSecurity
            End Get
        End Property
    End Class
End NameSpace