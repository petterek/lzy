Imports LazyFramework.CQRS.Command
Imports LazyFramework.CQRS.ExecutionProfile

Namespace Availability
    Public Class Handler

        Private Shared _availabilityList As New Dictionary(Of Type, ICommandAvailability)

        Private Shared padLock As New Object
        Public Shared ReadOnly Property AvailabilityList As Dictionary(Of Type, ICommandAvailability)
            Get
                Return _availabilityList
            End Get
        End Property

        Public Shared Sub AddAvilabilityHandler(Of TCommand As IAmACommand)(handler As ICommandAvailability)
            _availabilityList.Add(GetType(TCommand), handler)
        End Sub

        Public Shared Function IsCommandAvailable(profile As ExecutionProfile.IExecutionProfile, commandBase As CommandBase) As Boolean
            If AvailabilityList.ContainsKey(commandBase.GetType) Then
                Return AvailabilityList(commandBase.GetType).IsAvailable(profile, commandBase, commandBase.GetInnerEntity())
            Else
                Return True
            End If
        End Function

        Public Shared Function IsActionAvailable(executionProfile As IExecutionProfile, action As IActionBase) As Boolean

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