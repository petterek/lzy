
Imports System.Runtime.CompilerServices
Imports LazyFramework.Utils

''' <summary>
''' This is an IoC class factory. 
''' </summary>
''' <remarks></remarks>
Public Class ClassFactory
    Public Shared Property LogToDebug As Boolean = False
    Private Shared ReadOnly ProcessStore As New Dictionary(Of Type, ITypeInfo)
    Private Shared ReadOnly SyncLockObject As New Object

    Const SlotName As String = "sessionStoreForFactory"

    Friend Shared Property Session As SessionInstance
        Get
            If Not ResponseThread.ThreadHasKey(SlotName) Then Return Nothing
            Return ResponseThread.GetThreadValue(Of SessionInstance)(SlotName)
        End Get
        Set(value As SessionInstance)
            ResponseThread.SetThreadValue(SlotName, value)
        End Set
    End Property


    Public Shared Sub Clear()
        ProcessStore.Clear()
        If LogToDebug Then
            Debug.Print("Classfactory.CLEAR")
        End If
    End Sub

    ''' <summary>
    ''' Main method if the lzyFactory
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TDefaultType"></typeparam>
    ''' <returns>A newly created instance of the passed in interface.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetTypeInstance(Of T, TDefaultType As {T, New})() As T
        Dim type As Type = GetType(T)
        Dim dType = GetType(TDefaultType)
        Dim ti As ITypeInfo

        'Da sjekker vi om den finnes noe setting for denne i den vanlige listen,
        'Vi må alltid sette denne i store for å kunne lagre default typen
        If Not ProcessStore.ContainsKey(type) Then
            SyncLock SyncLockObject
                If Not ProcessStore.ContainsKey(type) Then
                    If Not type.IsInterface Then
                        Throw New NotSupportedException("Type parameter T must be an Interface")
                    End If
                    ProcessStore(type) = New TypeInfo With {.DefaultType = dType, .CurrentType = dType}
                End If
            End SyncLock
        End If
        'Sjekker først om det er satt en type for denne sessionen. Dette tar høyde både for web og winthreads
        If Session IsNot Nothing AndAlso Session.ContainsKey(type) Then
            ti = Session.GetInstance(type)
            'Vi returnerer instance fra sessionstore
            If ti.PersistInstance Then
                If ti.CurrentInstance Is Nothing Then
                    ti.CurrentInstance = CType(ti, TypeInfo).CreateInstance
                End If
                If LogToDebug Then
                    Debug.Print("You asked for:" & GetType(T).ToString & " You got:" & ti.CurrentInstance.GetType.ToString)
                End If
                Return CType(ti.CurrentInstance, T)
            End If
        Else
            ti = ProcessStore(type)
        End If

        'Denne er ikke spurt på før så da setter vi default typen for denne..
        If ti.DefaultType Is Nothing Then
            ti.DefaultType = dType
        End If

        If LogToDebug Then
            Debug.Print("You asked for:" & GetType(T).ToString & " You got:" & ti.CurrentType.ToString)
        End If

        If ti.CurrentInstance IsNot Nothing Then Return CType(ti.CurrentInstance, T)

        Return CType(ti.CreateInstance, T)
    End Function

    
    Private Shared Function GetTypeInstance(type As Type) As Object
        Dim ti As ITypeInfo = Nothing

        If Session IsNot Nothing AndAlso Session.ContainsKey(type) Then
            ti = Session.GetInstance(type)
        ElseIf ProcessStore.ContainsKey(type) Then
            ti = ProcessStore(type)
        End If

        If ti Is Nothing Then
            Throw New NotConfiguredException(type.ToString)
        End If

        If ti.CurrentType IsNot Nothing Then
            If ti.PersistInstance Then
                If ti.CurrentInstance Is Nothing Then
                    ti.CurrentInstance = ti.CreateInstance()
                End If
                Return ti.CurrentInstance
            Else
                Return ti.CreateInstance
            End If
        Else
            'See if there is any implementations of this interface in the ApplicationPool
            'If only 1 exist then return this one, if more existes then throw exception.

            Throw New NotConfiguredException(type.ToString)
        End If
    End Function

    Public Shared Function GetTypeInstance(Of T)() As T
        Return CType(GetTypeInstance(GetType(T)), T)
    End Function

    Private Shared Sub RegisterInterfaceMapping(T As Type, list As Type)
        ProcessStore(T) = New ClassFactory.TypeInfo With {.CurrentType = list}
        For Each inter In T.GetInterfaces
            ProcessStore(inter) = New ClassFactory.TypeInfo With {.CurrentType = list}
        Next
    End Sub

    Public Shared Function TryInstantiateType(Of T)(ByRef ret As T) As Boolean
        Dim type As Type = Nothing
        If TryFindType(GetType(T),type) Then
            ret = CType(Construct(type), T)
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function TryInstantiateType(t As Type, ByRef ret As Object) As Boolean
        Dim foundType As Type = Nothing
        If TryFindType(t,foundtype) Then
            ret = Construct(foundType)
            Return True
        Else
            Return False
        End If
    End Function




    Public Shared Function FindType(t As Type) As Type

        If Not LazyFramework.ClassFactory.ContainsKey(t) Then
            Dim list = LazyFramework.Reflection.FindAllClassesOfTypeInApplication(t)
            If list.Count = 0 Then
                Throw New TypeNotFoundException(t)
            End If
            If list.Count > 1 Then
                Throw New ToManyInstancesConfiguredForInterface(t)
            End If
            RegisterInterfaceMapping(t, list(0))
        End If

        Return ProcessStore(t).CurrentType

    End Function

    Public Shared Function TryFindType( T As Type,ByRef res As Type) As Boolean
        Dim ret As Type
        Try
            ret = FindType(T)
        Catch ex As Exception
            res = Nothing
            Return False
        End Try
        res = ret
        Return True
    End Function

    Public Shared Function Construct(Of T)() As T

        Return CType(Construct(GetType(T)), T)

    End Function

    Public Shared Function Construct(type As Type) As Object
        If type.IsInterface Then
            Dim created As Object = Nothing
            If TryInstantiateType(type, created) Then
                Return created
            Else
                Throw new UnableToCreteInstanceException(type)
            End If
        End If

        Dim constructors = type.GetConstructors()
        If constructors.Count > 1 Then
            Throw New AmbigiousConstructorException(type)
        End If

        Dim c = constructors(0)
        Dim params As New List(Of Object)
        For Each p In c.GetParameters
            params.Add(Construct(p.ParameterType))
        Next
        Return c.Invoke(params.ToArray)
    End Function


    Public Shared Function GetTypeInstance(Of T)(key As String) As T
        Dim list As Dictionary(Of String, TypeInfo)
        Dim ti As ITypeInfo = Nothing

        If Session IsNot Nothing AndAlso Session.ContainsKey(GetType(Dictionary(Of String, TypeInfo))) Then
            ti = Session.GetInstance(GetType(Dictionary(Of String, TypeInfo)))
        ElseIf ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo))) Then
            ti = ProcessStore((GetType(Dictionary(Of String, TypeInfo))))
        End If

        If ti Is Nothing Then
            Throw New NotConfiguredException(GetType(T).ToString)
        End If

        list = CType(ti.CurrentInstance, Dictionary(Of String, TypeInfo))
        If Not list.ContainsKey(key) Then
            Throw New NotConfiguredException(key)
        End If
        If list(key).PersistInstance Then
            Return CType(list(key).CurrentInstance, T)
        Else
            Return CType(list(key).CreateInstance, T)
        End If

    End Function

    Public Shared Function GetDefaultInstaceForType(Of TT)() As TT
        Dim tp = GetType(TT)
        If ProcessStore.ContainsKey(tp) Then
            Dim ti As ITypeInfo = ProcessStore(tp)
            If ti.DefaultType IsNot Nothing Then
                Return CType(ti.CreateDefaultInstance, TT) 'CType(Activator.CreateInstance(ti.DefaultType), TT)
            End If
        End If
        Throw New NotSupportedException("Default type is not register for type")
    End Function

    ''' <summary>
    ''' Use this method to override the default type to use in GetTypeInstance. 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TConfigedType"></typeparam>
    ''' <remarks></remarks>
    Public Shared Sub SetTypeInstance(Of T, TConfigedType As {New, T})()

        Dim type As Type = GetType(T)
        If Not type.IsInterface Then
            Throw New NotSupportedException("Type parameter T must be an Interface")
        End If

        RegisterInterfaceMapping(type, GetType(TConfigedType))

        If LogToDebug Then
            Debug.WriteLine("Type set: " & GetType(T).ToString)
        End If
    End Sub


    Public Shared Sub SetTypeInstance(Of T)(instance As T)
        Dim type As Type = GetType(T)
        If Not type.IsInterface Then
            Throw New NotSupportedException("Type parameter T must be an Interface")
        End If

        If ProcessStore.ContainsKey(GetType(T)) Then
            Throw New AllreadyMappedException(GetType(T).ToString)
        End If
        ProcessStore(GetType(T)) = New TypeInfo With {.CurrentType = instance.GetType, .CurrentInstance = instance, .PersistInstance = True}

        For Each inter In type.GetInterfaces
            If ProcessStore.ContainsKey(inter) Then
                Throw New AllreadyMappedException(GetType(T).ToString)
            End If

            Dim insert = New TypeInfo
            CType(insert, ITypeInfo).CurrentInstance = instance
            CType(insert, ITypeInfo).CurrentType = instance.GetType
            CType(insert, ITypeInfo).PersistInstance = True

            ProcessStore(inter) = CType(insert, ITypeInfo)
        Next


        If LogToDebug Then
            Debug.WriteLine("Type set: " & GetType(T).ToString)
        End If
    End Sub

    Public Class AllreadyMappedException
        Inherits Exception

        Public Sub New(s As String)
            MyBase.New(s)
        End Sub
    End Class

    Public Shared Sub SetTypeInstance(Of T)(ByVal key As String, ByVal instance As T)
        If Not ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo))) Then
            ProcessStore(GetType(Dictionary(Of String, TypeInfo))) = New TypeInfo With {.CurrentType = GetType(Dictionary(Of String, TypeInfo)), .CurrentInstance = New Dictionary(Of String, TypeInfo), .PersistInstance = True}
        End If

        Dim list As Dictionary(Of String, TypeInfo) = CType(ProcessStore(GetType(Dictionary(Of String, TypeInfo))).CurrentInstance, Dictionary(Of String, TypeInfo))
        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo With {.CurrentInstance = instance, .PersistInstance = True, .CurrentType = instance.GetType})
        Else
            list(key).CurrentInstance = instance
        End If
    End Sub

    Public Shared Sub SetTypeInstance(Of TType)(key As String, instanceType As Type)

        If Not ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo))) Then
            ProcessStore(GetType(Dictionary(Of String, TypeInfo))) = New TypeInfo With {.CurrentType = GetType(Dictionary(Of String, TypeInfo)), .CurrentInstance = New Dictionary(Of String, TypeInfo), .PersistInstance = True}
        End If
        Dim list As Dictionary(Of String, TypeInfo) = CType(ProcessStore(GetType(Dictionary(Of String, TypeInfo))).CurrentInstance, Dictionary(Of String, TypeInfo))

        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo With {.PersistInstance = False, .CurrentType = instanceType})
        Else
            list(key).CurrentType = instanceType
        End If

    End Sub


    Public Shared Function ContainsKey(t As Type) As Boolean

        If Session IsNot Nothing AndAlso Session.ContainsKey(t) Then Return True

        Return ProcessStore.ContainsKey(t)
    End Function


    Public Shared Function ContainsKey(Of TKey)(name As String) As Boolean

        If Session IsNot Nothing AndAlso Session.ContainsKey(Of TKey)(name) Then Return True

        If ProcessStore.ContainsKey(GetType(Dictionary(Of String, TKey))) Then
            Return CType(ProcessStore(GetType(Dictionary(Of String, TKey))).CurrentInstance, Dictionary(Of String, TKey)).ContainsKey(name)
        End If

        Return False
    End Function

    Public Shared Function RemoveTypeInstance(Of TT)() As Boolean

        If ProcessStore.ContainsKey(GetType(TT)) Then
            If LogToDebug Then
                Debug.WriteLine("Type removed: " & GetType(TT).ToString)
            End If
            Return ProcessStore.Remove(GetType(TT))
        End If
        Return False
    End Function

    ''' <summary>
    ''' Store a reference to an typeinstance in a 'session' slot. 
    ''' Will only affect the current call in a websession, or the current thread in a win session 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TConfigedType"></typeparam>
    ''' <remarks></remarks>
    Public Shared Sub SetTypeInstanceForSession(Of T, TConfigedType As {New, T})()
        SetTypeInstanceForSession(Of T, TConfigedType)(False)
    End Sub

    Public Shared Sub SetTypeInstanceForSession(Of T)(instance As T)

        If Session Is Nothing Then
            Throw New SessionNotCreatedException
        End If

        Session.SetInstance(GetType(T), New TypeInfo With {.CurrentType = instance.GetType, .PersistInstance = True, .CurrentInstance = instance})
    End Sub

    Public Shared Sub SetTypeInstanceForSession(Of T)(ByVal key As String, ByVal instance As T)
        If Not Session.ContainsKey(GetType(Dictionary(Of String, TypeInfo))) Then
            Session.SetInstance(GetType(Dictionary(Of String, TypeInfo)), New TypeInfo With {.CurrentType = GetType(Dictionary(Of String, TypeInfo)), .CurrentInstance = New Dictionary(Of String, TypeInfo), .PersistInstance = True})
        End If

        Dim list As Dictionary(Of String, TypeInfo) = CType(Session.GetInstance(GetType(Dictionary(Of String, TypeInfo))).CurrentInstance, Dictionary(Of String, TypeInfo))

        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo With {.CurrentInstance = instance, .PersistInstance = True, .CurrentType = instance.GetType})
        Else
            list(key).CurrentInstance = instance
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TConfigedType"></typeparam>
    ''' <param name="persist">Set to true to cache the instance of the created class for the session. </param>
    ''' <remarks></remarks>
    Public Shared Sub SetTypeInstanceForSession(Of T, TConfigedType As {New, T})(persist As Boolean)
        Dim type As Type = GetType(T)

        If Not type.IsInterface Then
            Throw New NotSupportedException("Type parameter T must be an Interface")
        End If

        If Not type.IsAssignableFrom(GetType(TConfigedType)) Then
            Throw New NotSupportedException("Type parameter T must be assignable from parameter TConfigedType")
        End If

        If Not GetType(ISessionAware).IsAssignableFrom(GetType(TConfigedType)) Then
            Throw New NotSupportedException("Type parameter T must implement ISessionAware")
        End If

        If Session Is Nothing Then
            Throw New SessionNotCreatedException
        End If

        Session.SetInstance(GetType(T), New TypeInfo With {.CurrentType = GetType(TConfigedType), .PersistInstance = persist})
    End Sub

    Public Shared Function RemoveTypeInstanceForSession(Of TT)() As Boolean
        If Session Is Nothing Then Return True

        If Session.ContainsKey(GetType(TT)) Then
            Return Session.Remove(GetType(TT))
        End If

        Return True
    End Function

    Public Shared Sub SessionComplete()
        If Session Is Nothing Then
            Throw New SessionNotCreatedException
        End If
        Session.Complete()
    End Sub

    Public Shared Sub SessionStart()
        If Session Is Nothing Then
            Throw New SessionNotCreatedException
        End If

        Session.Start()
    End Sub



End Class

Public Module Extensions
    <extension>Public sub AsSingleton(types As List(Of Type))
        types.ForEach(Sub(e)

                      End Sub)
    End sub
End Module
