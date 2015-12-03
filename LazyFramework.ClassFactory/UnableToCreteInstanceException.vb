Public Class UnableToCreteInstanceException
    Inherits Exception

    Public Sub New(type As Type)
        MyBase.New(type.FullName)
    End Sub
End Class