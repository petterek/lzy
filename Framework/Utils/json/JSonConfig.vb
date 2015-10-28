Namespace Utils.Json
    Public Class JSonConfig

        Public Sub ObjectToString(value As Object)
            Writer.ObjectToString(Me, value)
        End Sub


        Function FormatDate(format As ToString(Of Date)) As JSonConfig
            Return Me
        End Function
    End Class
End NameSpace