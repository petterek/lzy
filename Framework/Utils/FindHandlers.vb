Imports System.Reflection

Namespace Utils
    Public Class FindHandlers
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Shared Function FindAllHandlerDelegates(toScan As IEnumerable(Of Type), paramType As Type, allowMulti As Boolean) As ActionHandlerMapper
        '    Dim ret As New ActionHandlerMapper
            
        '    For Each t As Type In toScan
        '        Dim methodInfos As MethodInfo() = t.GetMethods(BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static)
        '        For Each func In methodInfos 'Finner alle funksjoner som er ligger på denne
        '            If func.GetParameters.Count = 1 Then
        '                Dim param = func.GetParameters(0)
        '                Dim parameterType As Type = param.ParameterType
        '                If parameterType.IsByRef Then
        '                    parameterType = parameterType.GetElementType
        '                End If

        '                If paramType.IsAssignableFrom(parameterType) Then
        '                    If Not ret.ContainsKey(parameterType) Then
        '                        ret.Add(parameterType, New List(Of MethodInfo))
        '                    End If

        '                    If ret(parameterType).Count = 0 Then
        '                        ret(parameterType).Add(func)
        '                    Else
        '                        If allowMulti Then
        '                            ret(parameterType).Add(func)
        '                        Else
        '                            'Throw New AllreadyMappedException(parameterType.ToString)
        '                        End If
        '                    End If
        '                End If
        '            End If
        '        Next
        '    Next
        '    Return ret
        'End Function
        
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
