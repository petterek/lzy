Namespace Command
    Public Interface IAmACommand
        Inherits IActionBase

        Function Result() As Object
        Sub SetResult(o As Object)
    End Interface
End Namespace
