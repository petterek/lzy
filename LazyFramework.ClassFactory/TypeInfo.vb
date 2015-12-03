Imports System.Reflection.Emit

Partial Class ClassFactory

    Public Interface ITypeInfo
        Property CurrentType As Type
        Property DefaultType As Type
        Property PersistInstance As Boolean
        Property CurrentInstance As Object

        Function CreateInstance As Object
        Function CreateDefaultInstance As Object
    End Interface



    Public Class TypeInfo
        Implements ITypeInfo

        Private Delegate Function CreateNew() As Object

        Public Property CurrentType As Type Implements ITypeInfo.CurrentType
        Public Property DefaultType As Type Implements ITypeInfo.DefaultType
        Public Property PersistInstance As Boolean = False Implements ITypeInfo.PersistInstance

        Public Property CurrentInstance As Object Implements ITypeInfo.CurrentInstance

        Private _Constructor As CreateNew
        Private _DefaultConstructor As CreateNew

        Public Function CreateDefaultInstance() As Object Implements ITypeInfo.CreateDefaultInstance

            Return ClassFactory.Construct(DefaultType)
            
            'If _DefaultConstructor Is Nothing Then
            '    Dim ctor = DefaultType.GetConstructors()(0)
            '    Dim method As New DynamicMethod(String.Empty, DefaultType, {}, True)
            '    Dim gen = method.GetILGenerator()
            '    gen.Emit(OpCodes.Newobj, ctor)
            '    gen.Emit(OpCodes.Ret)
            '    _DefaultConstructor = CType(method.CreateDelegate(GetType(CreateNew)), CreateNew)
            'End If

            'Return _DefaultConstructor()
        End Function

        Public Function CreateInstance() As Object Implements ITypeInfo.CreateInstance

            Dim ctor = CurrentType.GetConstructors()(0) 'Her skal vi gjøre noe mer lureri..

            Return ClassFactory.Construct(CurrentType)
            

            'If _Constructor Is Nothing Then
            '    Dim ctor = CurrentType.GetConstructors()(0) 'Her skal vi gjøre noe mer lureri..
            '    Dim method As New DynamicMethod(String.Empty, CurrentType, {}, True)
            '    Dim gen = method.GetILGenerator()
            '    gen.Emit(OpCodes.Newobj, ctor)
            '    gen.Emit(OpCodes.Ret)
            '    _Constructor = CType(method.CreateDelegate(GetType(CreateNew)), CreateNew)
            'End If

            'Return _Constructor()

        End Function
    End Class
End Class


Partial Class ClassFactory
    Public Class TypeNotFoundException
        Inherits Exception
        Public Type As Type
        Public Sub New(t As Type)
            MyBase.New(t.FullName & vbCrLf & "Configure this in your application")
        End Sub

    End Class

    Public Class ToManyInstancesConfiguredForInterface
        Inherits TypeNotFoundException
        Public Sub New(t As Type)
            MyBase.New(t)
        End Sub

    End Class

End Class