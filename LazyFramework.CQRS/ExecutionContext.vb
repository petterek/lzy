Imports LazyFramework.CQRS



    Public NotInheritable Class ExecutionContext


        Private Shared AllContext As Dictionary(Of Type, Type)

        Private Shared PadLock As New Object

        Friend Shared Function GetContextForAction(action As IAmAnAction) As IContext
            If AllContext Is Nothing Then
                SyncLock PadLock
                    If AllContext Is Nothing Then
                        Dim temp As New Dictionary(Of Type, Type)
                        For Each context In Reflection.FindAllClassesOfTypeInApplication(GetType(IContext))
                            Dim key As Type
                            Dim toSearch As Type
                            toSearch = context.BaseType
                            While toSearch IsNot Nothing
                                If toSearch.IsGenericType Then
                                    key = toSearch.GenericTypeArguments(0)
                                    temp(key) = context
                                End If
                                toSearch = toSearch.BaseType
                            End While

                        Next
                        AllContext = temp
                    End If
                End SyncLock
            End If

            If AllContext.ContainsKey(action.GetType) Then
                Return CType(Activator.CreateInstance(AllContext(action.GetType)), IContext)
            Else
                Return Nothing
            End If

        End Function



        Friend Interface IContext
            Sub StartSession(action As IAmAnAction)
            Sub EndSession()

        End Interface
    ''' <summary>
    ''' Sets up a classfactory session to wrap the Action in. 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>

    Public MustInherit Class Context(Of T As IAmAnAction)
        Implements IContext

        Private session As LazyFramework.ClassFactory.SessionInstance

        Public Overridable Sub SetupCache(action As T)

        End Sub

        Friend Sub StartSession(action As IAmAnAction) Implements IContext.StartSession
            session = New LazyFramework.ClassFactory.SessionInstance
            SetupCache(CType(action, T))
        End Sub

        Friend Sub EndSession() Implements IContext.EndSession
            session.Complete()
            session.Dispose()
            session = Nothing
        End Sub

    End Class
End Class
