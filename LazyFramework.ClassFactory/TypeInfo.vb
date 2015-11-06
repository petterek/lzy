Imports System.Reflection.Emit

Partial Class ClassFactory

    Public Interface ITypeInfo
        Property CurrentType As Type
        Property DefaultType As Type
        Property PersistInstance As Boolean
        Property CurrentInstance As Object
    End Interface



    Public Class TypeInfo(Of T)
        Implements ITypeInfo

        Private Delegate Function CreateNew() As T

        Public Property CurrentType As Type Implements ITypeInfo.CurrentType
        Public Property DefaultType As Type Implements ITypeInfo.DefaultType
        Public Property PersistInstance As Boolean = False Implements ITypeInfo.PersistInstance

        Public Property CurrentInstance As Object Implements ITypeInfo.CurrentInstance

        Private _Constructor As CreateNew
        Private _DefaultConstructor As CreateNew

        Public Function CreateDefaultInstance() As T
            If _DefaultConstructor Is Nothing Then
                Dim ctor = DefaultType.GetConstructor({})
                Dim method As New DynamicMethod(String.Empty, DefaultType, {}, True)
                Dim gen = method.GetILGenerator()
                gen.Emit(OpCodes.Newobj, ctor)
                gen.Emit(OpCodes.Ret)
                _DefaultConstructor = CType(method.CreateDelegate(GetType(CreateNew)), CreateNew)
            End If

            Return _DefaultConstructor()
        End Function

        Public Function CreateInstance() As T

            If _Constructor Is Nothing Then
                Dim ctor = CurrentType.GetConstructor({})
                Dim method As New DynamicMethod(String.Empty, CurrentType, {}, True)
                Dim gen = method.GetILGenerator()
                gen.Emit(OpCodes.Newobj, ctor)
                gen.Emit(OpCodes.Ret)
                _Constructor = CType(method.CreateDelegate(GetType(CreateNew)), CreateNew)
            End If

            Return _Constructor()

        End Function
    End Class
End Class


Partial Class ClassFactory
    Public Class TypeNotFoundException(Of T)
        Inherits Exception
        Public Type As T
        Public Sub New
            MyBase.New(GetType(T).FullName & vbCrLf &  "Configure this in your application")
        End Sub

    End Class

    Public Class ToManyInstancesConfiguredForInterface (Of T)
        Inherits TypeNotFoundException(Of T)
        
    End Class

End Class