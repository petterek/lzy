Namespace Dto
    Public MustInherit Class DtoBase
        Implements ISupportActionNameList

        Private ReadOnly _Actionlist As New List(Of String)
        Public ReadOnly Property Actions As List(Of String) Implements ISupportActionNameList.Actions
            Get
                Return _Actionlist
            End Get
        End Property
    End Class
End Namespace
