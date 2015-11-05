Imports System.Security.Principal
Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile
Imports LazyFramework.CQRS.Security


Public Class ActionInfo

    Private Shared _actionsForType As Dictionary(Of Type, List(Of Type))
    Private Shared ReadOnly Pl As New Object

    Private Shared ReadOnly Property AllActions As Dictionary(Of Type, List(Of Type))
        Get
            If _actionsForType Is Nothing Then
                SyncLock Pl
                    If _actionsForType Is Nothing Then
                        Dim temp As New Dictionary(Of Type, List(Of Type))
                        For Each el In Reflection.FindAllClassesOfTypeInApplication(GetType(IActionBase))
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


    Public Shared Function GetAvailableActionsForType(profile As IExecutionProfile, entityType As Type) As List(Of IActionBase)
        Dim ret As New List(Of IActionBase)
        If AllActions.ContainsKey(entityType) Then
            For Each t In _actionsForType(entityType)
                Dim createInstance As IActionBase = CType(Activator.CreateInstance(t), IActionBase)
                If createInstance.IsAvailable(profile) Then
                    ret.Add(createInstance)
                End If
            Next
        End If
        Return ret
    End Function

    Public Shared Function GetAvailableActionsForEntity(profile As IExecutionProfile, entity As Object) As List(Of IActionBase)
        Dim ret As New List(Of IActionBase)

        If TypeOf entity Is ActionContext.ActionContext Then
            For Each action In DirectCast(entity, ActionContext.ActionContext).Actions
                If CheckAvailability(entity, action, profile) Then
                    ret.Add(action)
                End If
            Next
        Else
            If AllActions.ContainsKey(entity.GetType) Then
                For Each t In _actionsForType(entity.GetType)
                    If GetType(Query.QueryBase).IsAssignableFrom(t) Then Continue For 'We do not want quries in action list. 

                    Dim createInstance As IActionBase = CType(Activator.CreateInstance(t), IActionBase)

                    If CheckAvailability(entity, createInstance, profile) Then
                        ret.Add(createInstance)
                    End If
                Next
            End If
        End If


        Return ret
    End Function

    Private Shared Function CheckAvailability(ByVal entity As Object, ByVal createInstance As IActionBase, profile As IExecutionProfile) As Boolean

        If createInstance.IsAvailable(profile, entity) Then
            If TypeOf (createInstance) Is CommandBase Then
                CType(createInstance, CommandBase).SetInnerEntity(entity)
            End If
            If Not ActionSecurity.Current.UserCanRunThisAction(profile, createInstance, If(TypeOf (entity) Is IProvideSecurityContext, DirectCast(entity, IProvideSecurityContext).Context, entity)) Then
                Return False
            End If
            Return True
        End If
        Return False
    End Function
End Class
