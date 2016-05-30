Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Availability
    Public Class Handler

        Private Shared _availabilityList As New Dictionary(Of Type, ICommandAvilability)

        Private Shared padLock As New Object
        Public Shared ReadOnly Property AvailabilityList As Dictionary(Of Type, ICommandAvilability)
            Get
                Return _availabilityList
            End Get
        End Property

        Public Shared Function EntityIsAvailableForCommand(profile As ExecutionProfile.IExecutionProfile, command As IAmACommand, entity As Object) As Boolean

        End Function

        Public Shared Function CommandIsAvailable(profile As ExecutionProfile.IExecutionProfile, commandBase As IAmACommand) As Boolean
            If AvailabilityList.ContainsKey(commandBase.GetType) Then
                Return AvailabilityList(commandBase.GetType).IsAvailable(profile, commandBase)
            Else
                Return True
            End If
        End Function

        Public Shared Function ActionIsAvailable(executionProfile As IExecutionProfile, action As IActionBase) As Boolean

            Return True

        End Function
        Public Shared Function ActionIsAvailable(profile As IExecutionProfile, action As IActionBase, entity As Object) As Boolean

            If AvailabilityList.ContainsKey(action.GetType) Then
                'Return AvailabilityList(action.GetType).IsAvailable(profile, action)
            Else
                Return True
            End If

        End Function
    End Class



End Namespace