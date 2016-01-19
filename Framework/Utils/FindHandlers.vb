Imports System.Reflection

Namespace Utils
    Public Class FindHandlers
        
        Public Shared Function FindAllMultiHandlers(Of THolder, T)() As Dictionary(Of Type, MethodList)
            Dim ret As New Dictionary(Of Type, MethodList)
            Dim allClasses = Reflection.FindAllClassesOfTypeInApplication(GetType(THolder))

            For Each typeFound As Type In allClasses
                If typeFound.IsAbstract Then
                    Continue For
                End If
                ret(typeFound.BaseType.GenericTypeArguments(0)) = New MethodList(typeFound)
            Next
            Return ret
        End Function
        
        Public Class MethodList
            Public Methods As New List(Of MethodInfo)
            Friend Sub New(t As Type)
                For Each mi In t.GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance Or BindingFlags.DeclaredOnly)
                    Methods.Add(mi)
                Next
                Type = t
            End Sub

            Public Type As Type
            Public Function CreateInstance() As Object
                Return Activator.CreateInstance(Type)
            End Function
        End Class
    End Class
End Namespace
