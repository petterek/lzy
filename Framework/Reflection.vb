Imports System.IO
Imports System.Linq.Expressions
Imports System.Reflection

Public Class Reflection
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="t"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FindAllClassesOfTypeInApplication(ByVal t As Type, Optional ByVal skipSystem As Boolean = True, Optional ByVal forceLoad As Boolean = True) As List(Of Type)

        Return AllTypes.FindAll(Function(type) t.IsAssignableFrom(type))

    End Function
    Public Shared Function FindTypeFromGuid(guid As Guid) As Type
        Dim ret As Type = Nothing
        If AllTypes.Count > 0 Then
            _guidmap.TryGetValue(guid, ret)
        End If



        Return ret
    End Function

    Public Shared Function CreateSetter(type As Type, name As String) As Action(Of Object, Object)
        Dim mInfo As MemberInfo
        mInfo = SearchForSetterInfo(type, name)
        If mInfo Is Nothing Then
            mInfo = SearchForFieldInfo(type, name)
        End If

        If mInfo IsNot Nothing Then
            Return CreateSetter(mInfo)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function CreateSetter(memberInfo As MemberInfo) As Action(Of Object, Object)
        Dim targetType = memberInfo.DeclaringType
        Dim exTarget = Expression.Parameter(GetType(Object), "t")
        Dim exValue = Expression.Parameter(GetType(Object), "p")
        Dim res As Action(Of Object, Object)

        Dim exBody As Expression
        If TypeOf (memberInfo) Is FieldInfo Then
            exBody = Expression.Assign(Expression.Field(Expression.Convert(exTarget, targetType), DirectCast(memberInfo, FieldInfo)), Expression.Convert(exValue, DirectCast(memberInfo, FieldInfo).FieldType))
        ElseIf TypeOf (memberInfo) Is PropertyInfo Then
            exBody = Expression.Assign(Expression.Property(Expression.Convert(exTarget, targetType), CType(memberInfo, PropertyInfo)), Expression.Convert(exValue, CType(memberInfo, PropertyInfo).PropertyType))
        Else
            Throw New NotSupportedException
        End If

        res = Expression.Lambda(Of Action(Of Object, Object))(exBody, exTarget, exValue).Compile()

        Return res
    End Function


    Private Shared _allTypes As List(Of Type)
    Private Shared _guidmap As New Dictionary(Of Guid, Type)

    Private Shared ReadOnly _PadLock As New Object
    Private Shared ReadOnly Property AllTypes() As List(Of Type)
        Get
            Dim a As Assembly
            Dim f As FileInfo
            Dim assembly As Assembly
            Dim type As Type
            Dim tle As Exception
            Dim allFiles As New List(Of String)

            If _allTypes Is Nothing Then
                SyncLock _PadLock
                    If _allTypes Is Nothing Then
                        Dim allTypesTemp = New List(Of Type)
                        Dim loaded As New List(Of Assembly)
                        Dim toIgnore As New System.Text.RegularExpressions.Regex(TypeValidation.IgnoreAssemblies, Text.RegularExpressions.RegexOptions.Compiled Or Text.RegularExpressions.RegexOptions.IgnoreCase)

                        For Each a In AppDomain.CurrentDomain.GetAssemblies()
                            allFiles.Add(a.FullName)
                            If Not a.GlobalAssemblyCache AndAlso Not a.IsDynamic Then
                                loaded.Add(a)
                            End If
                        Next


                        Try
                            Dim fileInfos As FileInfo() = New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll", SearchOption.AllDirectories)

                            For Each f In fileInfos

                                If Not String.IsNullOrWhiteSpace(TypeValidation.IgnoreAssemblies) AndAlso toIgnore.IsMatch(f.FullName) Then Continue For

                                Dim dllIsLoaded As Boolean = False

                                For Each a In loaded
                                    If a.ManifestModule.ScopeName.ToLower = f.Name.ToLower Then
                                        dllIsLoaded = True
                                        Exit For
                                    End If
                                Next

                                If Not dllIsLoaded Then
                                    Try
                                        loaded.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(f.FullName)))
                                    Catch ex As Exception
                                        'Gidder ikke gjøre noe... ;)
                                        Continue For
                                    End Try
                                End If
                            Next
                        Catch ex As Exception
                            Throw New ApplicationException("DLL Loading", ex)
                        End Try


                        For Each assembly In loaded
                            Try
                                Dim getTypes As Type()
                                If Not String.IsNullOrWhiteSpace(TypeValidation.IgnoreAssemblies) AndAlso toIgnore.IsMatch(assembly.FullName) Then Continue For
                                getTypes = assembly.GetTypes
                                For Each type In getTypes
                                    Try
                                        If type.IsClass AndAlso Not type.IsAbstract Then
                                            allTypesTemp.Add(type)
                                            _guidmap.Add(type.GUID, type)
                                        End If
                                    Catch ex As Exception
                                        Throw New ApplicationException("Add type" & type.FullName, ex)
                                    End Try
                                Next
                            Catch ex As ReflectionTypeLoadException
                                Trace.Write(ex.GetType.Name)
                                Dim s As String
                                s = assembly.Location
                                For Each tle In ex.LoaderExceptions
                                    s += tle.Message & vbCrLf
                                Next
                                Throw New ApplicationException(s)
                            Catch ex As Exception
                                Throw
                            End Try
                        Next





                        _allTypes = allTypesTemp

                    End If

                End SyncLock
            End If
            Return _allTypes
        End Get
    End Property




    ''' <summary>
    ''' Finds the private field named _Name or Public field Name  if existes in the object 
    ''' </summary>
    ''' <param name="currType"></param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Shared Function SearchForFieldInfo(currType As Type, name As String) As FieldInfo
        Dim fieldInfo As FieldInfo = Nothing

        While fieldInfo Is Nothing AndAlso currType IsNot Nothing
            fieldInfo = currType.GetField("_" & name, BindingFlags.IgnoreCase Or BindingFlags.NonPublic Or BindingFlags.Instance)
            If fieldInfo Is Nothing Then
                fieldInfo = currType.GetField(name, BindingFlags.IgnoreCase Or BindingFlags.Instance Or BindingFlags.Public)
            End If
            currType = currType.BaseType
        End While

        Return fieldInfo
    End Function

    Public Shared Function SearchForSetterInfo(currType As Type, name As String) As PropertyInfo
        Dim propertyInfo As PropertyInfo = Nothing
        While propertyInfo Is Nothing AndAlso currType IsNot Nothing
            propertyInfo = currType.GetProperty(name, BindingFlags.IgnoreCase Or BindingFlags.NonPublic Or BindingFlags.Instance)
            If propertyInfo IsNot Nothing Then
                If propertyInfo.CanWrite Then
                    Return propertyInfo
                End If
            End If
            currType = currType.BaseType
        End While

        Return Nothing
    End Function


End Class