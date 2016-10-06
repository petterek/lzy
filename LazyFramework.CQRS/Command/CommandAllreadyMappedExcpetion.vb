Imports System.Runtime.Serialization

Namespace Command
    <Serializable>
    Friend Class CommandAllreadyMappedExcpetion
        Inherits Exception

        Private type As Type



        Public Sub New(type As Type)
            MyBase.New(type.FullName)
            Me.type = type
        End Sub


    End Class
End Namespace
