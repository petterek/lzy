Imports System.Reflection
Namespace Utils

    Public Class ActionHandlerMapper
        Inherits Dictionary(Of Type, List(Of MethodInfo))


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
End Namespace