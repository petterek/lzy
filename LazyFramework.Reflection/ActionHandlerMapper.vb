Imports System.Reflection

Public Class ActionHandlerMapper
    Inherits Dictionary(Of Type, List(Of MethodInfo))

    Public Sub New(list As List(Of MethodInfo), allowMulti As Boolean)

        For Each m In list
            Dim parameterType As Type = m.GetParameters(0).ParameterType
            If parameterType.IsByRef Then
                parameterType = parameterType.GetElementType
            End If

            If Not ContainsKey(parameterType) Then
                Add(parameterType, New List(Of MethodInfo))
            End If

            If Me(parameterType).Count = 0 Then
                Me(parameterType).Add(m)
            Else
                If allowMulti Then
                    Me(parameterType).Add(m)
                Else
                    'Throw New AllreadyMappedException(parameterType.ToString)
                End If
            End If

        Next

    End Sub

    Public Sub AddActionHandler(of TAction)(action As Action(Of TAction ))
        
    End Sub

    Public Function TryFindHandler(t As Type, ByRef handler As MethodInfo) As Boolean
        Dim list = FindKeyForListAndCache(t)
        If list Is Nothing Then
            Return False
        End If
        handler = list(0)
        Return True
    End Function

    Public Function TryFindHandler(t As Type, ByRef handler As List(Of MethodInfo)) As Boolean
        Dim list = FindKeyForListAndCache(t)
        If list Is Nothing Then
            Return False
        End If
        handler = list
        Return True
    End Function

    Private Function FindKeyForListAndCache(t As Type) As List(Of MethodInfo)
        If ContainsKey(t) Then
            Return Item(t)
        End If

        Dim toSearchFor As Type
        toSearchFor = t

        'Looping basetypes...
        While toSearchFor IsNot Nothing
            If ContainsKey(toSearchFor) Then
                If toSearchFor <> t Then
                    'Does not exist in the list.. we add this to the index..
                    SyncLock Me
                        Item(t) = Item(toSearchFor)
                    End SyncLock
                End If

                Return Item(toSearchFor)
            End If
            toSearchFor = toSearchFor.BaseType
        End While

        'Looping interfaces... 
        For Each iter In t.GetInterfaces
            If ContainsKey(iter) Then
                If toSearchFor <> t Then
                    'Does not exist in the list.. we add this to the index..
                    SyncLock Me
                        Item(t) = Item(iter)
                    End SyncLock
                End If
                Return Item(iter)
            End If
        Next

        Return Nothing
    End Function

End Class