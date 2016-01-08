Namespace Utils.Json
    Public Class JSonConfig

        public Const DateWriteFormatString As String = "yyyy-MM-ddTHH\:mm\:ss.FFFKz"
        public Const DateParseFormatString As String = "yyyy-MM-ddTHH\:mm\:ss.FFFK"

        Public Sub ObjectToString(value As Object)
            Writer.ObjectToString(Me, value)
        End Sub


        Function FormatDate(format As ToString(Of Date)) As JSonConfig
            Return Me
        End Function
    End Class
End NameSpace