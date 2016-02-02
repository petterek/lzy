Imports System.Security.Principal
Imports LazyFramework.CQRS.Command

Namespace CQRS
    Public Class ActionInfo

        Private Shared _actionsForType As Dictionary(Of Type, List(Of Type))
        Private Shared ReadOnly Pl As New Object

        Private Shared ReadOnly Property AllActions As Dictionary(Of Type, List(Of Type))
            Get
                If _actionsForType Is Nothing Then
                    SyncLock Pl
                        If _actionsForType Is Nothing Then
                            Dim temp As New Dictionary(Of Type, List(Of Type))
                            For Each el In TypeValidation.FindAllClassesOfTypeInApplication(GetType(IActionBase))
                                If el.IsAbstract Then Continue For

                                Dim basetype = el.BaseType
                                'Have to find the common basetype
                                While basetype IsNot Nothing AndAlso Not basetype.IsGenericType
                                    basetype = basetype.BaseType
                                End While

                                If basetype IsNot Nothing Then
                                    Dim getGenericArguments As Type = basetype.GetGenericArguments(0)
                                    If Not temp.ContainsKey(getGenericArguments) Then
                                        temp.Add(getGenericArguments, New List(Of Type))
                                    End If
                                    temp(getGenericArguments).Add(el)
                                End If
                            Next
                            _actionsForType = temp
                        End If
                    End SyncLock
                End If
                Return _actionsForType
            End Get
        End Property


        Public Shared Function AllActionsForType(ctxType As System.Type) As IEnumerable(Of IActionBase)
            Dim ret As New List(Of IActionBase)
            If AllActions.ContainsKey(ctxType) Then
                For Each t In _actionsForType(ctxType)
                    Dim createInstance As IActionBase = CType(Activator.CreateInstance(t), IActionBase)
                    ret.Add(createInstance)
                Next
            End If
            Return ret
        End Function


        Public Shared Function GetAvailableActionsForType(user As IPrincipal, entityType As Type) As List(Of IActionBase)
            Dim ret As New List(Of IActionBase)
            If AllActions.ContainsKey(entityType) Then
                For Each t In _actionsForType(entityType)
                    Dim createInstance As IActionBase = CType(Activator.CreateInstance(t), IActionBase)
                    If createInstance.IsAvailable(user) Then
                        ret.Add(createInstance)
                    End If
                Next
            End If
            Return ret
        End Function

        Public Shared Function GetAvailableActionsForEntity(user As IPrincipal, entity As Object, ByVal fromAction As IActionBase) As List(Of IActionBase)
            Dim ret As New List(Of IActionBase)

            If TypeOf entity Is ActionContext.ActionContext Then
                For Each action In DirectCast(entity, ActionContext.ActionContext).Actions
                    If CheckAvailability(entity, action, user) Then
                        ret.Add(action)
                    End If
                Next
            Else
                If ActionConstraintsForAction.ContainsKey(fromAction.GetType) Then
                    For Each a In ActionConstraintsForAction(fromAction.GetType).Actions
                        If CheckAvailability(entity, a, user) Then
                            ret.Add(a)
                        End If
                    Next
                Else
                    If AllActions.ContainsKey(entity.GetType) Then
                        For Each t In AllActions(entity.GetType)

                            If GetType(CQRS.Query.QueryBase).IsAssignableFrom(t) Then Continue For 'We do not want quries in action list. 

                            Dim createInstance As IActionBase = CType(Activator.CreateInstance(t), IActionBase)

                            If CheckAvailability(entity, createInstance, user) Then
                                ret.Add(createInstance)
                            End If
                        Next
                    End If

                End If
            End If

            Return ret
        End Function

        Private Shared _ActionConstraintsForAction As Dictionary(Of Type, IActionListForAction)
        Private Shared PadLock As New Object
        Private Shared ReadOnly Property ActionConstraintsForAction As Dictionary(Of Type, IActionListForAction)
            Get
                If _ActionConstraintsForAction Is Nothing Then
                    SyncLock PadLock
                        If _ActionConstraintsForAction Is Nothing Then
                            Dim temp As New Dictionary(Of Type, IActionListForAction)

                            For Each el In TypeValidation.FindAllClassesOfTypeInApplication(GetType(IActionListForAction))
                                If el.IsAbstract Then Continue For

                                Dim getGenericArguments As Type = el.BaseType.GetGenericArguments(0)
                                If Not temp.ContainsKey(getGenericArguments) Then
                                    temp.Add(getGenericArguments, CType(Activator.CreateInstance(el), IActionListForAction))
                                End If
                            Next

                            _ActionConstraintsForAction = temp
                        End If
                    End SyncLock
                End If
                Return _ActionConstraintsForAction
            End Get
        End Property

        Private Shared Function CheckAvailability(ByVal entity As Object, ByVal createInstance As IActionBase, ByVal user As IPrincipal) As Boolean

            If createInstance.IsAvailable(user, entity) Then
                If TypeOf (createInstance) Is CommandBase Then
                    CType(createInstance, CommandBase).SetInnerEntity(entity)
                End If
                If Not ActionSecurity.Current.UserCanRunThisAction(user, createInstance, If(TypeOf (entity) Is IProvideSecurityContext, DirectCast(entity, IProvideSecurityContext).Context, entity)) Then
                    Return False
                End If
                Return True
            End If
            Return False
        End Function
    End Class

    Public Interface IActionListForAction
        Function Actions() As IEnumerable(Of IActionBase)
    End Interface
    Public MustInherit Class ActionListForAction(Of T As IActionBase)
        implements IActionListForAction

        Public MustOverride Function Actions() As IEnumerable(Of IActionBase) Implements IActionListForAction.Actions
    End Class
End Namespace
