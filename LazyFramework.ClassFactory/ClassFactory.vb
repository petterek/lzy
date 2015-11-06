
Imports LazyFramework.Utils

''' <summary>
''' This is an IoC factory. Possible extensions to be made is to configure things from a config file. 
''' 
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
                    ProcessStore(type) = New TypeInfo(Of T) With {.DefaultType = dType, .CurrentType = dType}
                End If
            End SyncLock
        End If
        'Sjekker først om det er satt en type for denne sessionen. Dette tar høyde både for web og winthreads
        If Session IsNot Nothing AndAlso Session.ContainsKey(Of T)() Then
            ti = Session.GetInstance(type)
            'Vi returnerer instance fra sessionstore
            If ti.PersistInstance Then
                If ti.CurrentInstance Is Nothing Then
                    ti.CurrentInstance = CType(ti, TypeInfo(Of T)).CreateInstance
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

        Return CType(ti, TypeInfo(Of T)).CreateInstance
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Shared Function GetTypeInstance(Of T)() As T
        Dim ti As ITypeInfo = Nothing


        If Session IsNot Nothing AndAlso Session.ContainsKey(Of T)() Then
            ti = Session.GetInstance(GetType(T))
        ElseIf ProcessStore.ContainsKey(GetType(T)) Then
            ti = ProcessStore((GetType(T)))
        End If

        If ti Is Nothing Then
            Throw New NotConfiguredException(GetType(T).ToString)
        End If

        Dim tiSpesific = CType(ti, TypeInfo(Of T))

        If ti.CurrentType IsNot Nothing Then
            If ti.PersistInstance Then
                If ti.CurrentInstance Is Nothing Then
                    ti.CurrentInstance = tiSpesific.CreateInstance()
                End If
                Return CType(ti.CurrentInstance, T)
            Else
                Return tiSpesific.CreateInstance()
            End If
        Else
            'See if there is any implementations of this interface in the ApplicationPool
            'If only 1 exist then return this one, if more existes then throw exception.

            Throw New NotConfiguredException(GetType(T).ToString)
        End If
    End Function

    Private Shared Sub RegisterInterfaceMapping(Of T)(list As Type)
        ProcessStore(GetType(T)) = New ClassFactory.TypeInfo(Of T) With {.CurrentType = list}
        For Each inter In GetType(T).GetInterfaces
            ProcessStore(inter) = New ClassFactory.TypeInfo(Of T) With {.CurrentType = list}
        Next
    End Sub

    Public Shared Function TryInstantiateType(Of T)(ByRef ret As T) As Boolean
        Dim type As Type = Nothing
        If TryFindType(Of T)(type) Then
            ret = CType(Activator.CreateInstance(type), T)
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function FindType(Of T)() As Type

        If Not LazyFramework.ClassFactory.ContainsKey(Of T) Then
            Dim list = LazyFramework.Reflection.FindAllClassesOfTypeInApplication(GetType(T))
            If list.Count = 0 Then
                Throw New TypeNotFoundException(Of T)
            End If
            If list.Count > 1 Then
                Throw New ToManyInstancesConfiguredForInterface(Of T)
            End If
            RegisterInterfaceMapping(Of T)(list(0))
        End If

        Return ProcessStore((GetType(T))).CurrentType

    End Function

    Public Shared Function TryFindType(Of T)(ByRef res As Type) As Boolean
        Dim ret As Type
        Try
            ret = FindType(Of T)()
        Catch ex As Exception
            res = Nothing
            Return False
        End Try
        res = ret
        Return True
    End Function


    Public Shared Function GetTypeInstance(Of T)(key As String) As T
        Dim list As Dictionary(Of String, TypeInfo(Of T))
        Dim ti As ITypeInfo = Nothing

        If Session IsNot Nothing AndAlso Session.ContainsKey(Of Dictionary(Of String, TypeInfo(Of T)))() Then
            ti = Session.GetInstance(GetType(Dictionary(Of String, TypeInfo(Of T))))
        ElseIf ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo(Of T)))) Then
            ti = ProcessStore((GetType(Dictionary(Of String, TypeInfo(Of T)))))
        End If

        If ti Is Nothing Then
            Throw New NotConfiguredException(GetType(T).ToString)
        End If

        list = CType(ti.CurrentInstance, Dictionary(Of String, TypeInfo(Of T)))
        If Not list.ContainsKey(key) Then
            Throw New NotConfiguredException(key)
        End If
        If list(key).PersistInstance Then
            Return CType(list(key).CurrentInstance, T)
        Else
            Return list(key).CreateInstance
        End If

    End Function

    Public Shared Function GetDefaultInstaceForType(Of TT)() As TT
        Dim tp = GetType(TT)
        If ProcessStore.ContainsKey(tp) Then
            Dim ti As ITypeInfo = ProcessStore(tp)
            If ti.DefaultType IsNot Nothing Then
                Return CType(ti, TypeInfo(Of TT)).CreateDefaultInstance 'CType(Activator.CreateInstance(ti.DefaultType), TT)
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

        RegisterInterfaceMapping(Of T)(GetType(TConfigedType))

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
        ProcessStore(GetType(T)) = New TypeInfo(Of T) With {.CurrentType = instance.GetType, .CurrentInstance = instance, .PersistInstance = True}

        For Each inter In type.GetInterfaces
            If ProcessStore.ContainsKey(inter) Then
                Throw New AllreadyMappedException(GetType(T).ToString)
            End If

            Dim genType As System.Type = GetType(TypeInfo(Of T)).GetGenericTypeDefinition().MakeGenericType(inter)

            Dim insert = Activator.CreateInstance(genType)
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
        If Not ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo(Of T)))) Then
            ProcessStore(GetType(Dictionary(Of String, TypeInfo(Of T)))) = New TypeInfo(Of T) With {.CurrentType = GetType(Dictionary(Of String, TypeInfo(Of T))), .CurrentInstance = New Dictionary(Of String, TypeInfo(Of T)), .PersistInstance = True}
        End If

        Dim list As Dictionary(Of String, TypeInfo(Of T)) = CType(ProcessStore(GetType(Dictionary(Of String, TypeInfo(Of T)))).CurrentInstance, Dictionary(Of String, TypeInfo(Of T)))
        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo(Of T) With {.CurrentInstance = instance, .PersistInstance = True, .CurrentType = instance.GetType})
        Else
            list(key).CurrentInstance = instance
        End If
    End Sub

    Public Shared Sub SetTypeInstance(Of TType)(key As String, instanceType As Type)

        If Not ProcessStore.ContainsKey(GetType(Dictionary(Of String, TypeInfo(Of TType)))) Then
            ProcessStore(GetType(Dictionary(Of String, TypeInfo(Of TType)))) = New TypeInfo(Of TType) With {.CurrentType = GetType(Dictionary(Of String, TypeInfo(Of TType))), .CurrentInstance = New Dictionary(Of String, TypeInfo(Of TType)), .PersistInstance = True}
        End If
        Dim list As Dictionary(Of String, TypeInfo(Of TType)) = CType(ProcessStore(GetType(Dictionary(Of String, TypeInfo(Of TType)))).CurrentInstance, Dictionary(Of String, TypeInfo(Of TType)))

        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo(Of TType) With {.PersistInstance = False, .CurrentType = instanceType})
        Else
            list(key).CurrentType = instanceType
        End If

    End Sub


    Public Shared Function ContainsKey(Of TKey)() As Boolean

        If Session IsNot Nothing AndAlso Session.ContainsKey(Of TKey)() Then Return True

        Return ProcessStore.ContainsKey(GetType(TKey))
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

        Session.SetInstance(GetType(T), New TypeInfo(Of T) With {.CurrentType = instance.GetType, .PersistInstance = True, .CurrentInstance = instance})
    End Sub

    Public Shared Sub SetTypeInstanceForSession(Of T)(ByVal key As String, ByVal instance As T)
        If Not Session.ContainsKey(Of Dictionary(Of String, TypeInfo(Of T)))() Then
            Session.SetInstance(GetType(Dictionary(Of String, TypeInfo(Of T))), New TypeInfo(Of T) With {.CurrentType = GetType(Dictionary(Of String, TypeInfo(Of T))), .CurrentInstance = New Dictionary(Of String, TypeInfo(Of T)), .PersistInstance = True})
        End If

        Dim list As Dictionary(Of String, TypeInfo(Of T)) = CType(Session.GetInstance(GetType(Dictionary(Of String, TypeInfo(Of T)))).CurrentInstance, Dictionary(Of String, TypeInfo(Of T)))

        If Not list.ContainsKey(key) Then
            list.Add(key, New TypeInfo(Of T) With {.CurrentInstance = instance, .PersistInstance = True, .CurrentType = instance.GetType})
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

        Session.SetInstance(GetType(T), New TypeInfo(Of T) With {.CurrentType = GetType(TConfigedType), .PersistInstance = persist})
    End Sub

    Public Shared Function RemoveTypeInstanceForSession(Of TT)() As Boolean
        If Session Is Nothing Then Return True

        If Session.ContainsKey(Of TT)() Then
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