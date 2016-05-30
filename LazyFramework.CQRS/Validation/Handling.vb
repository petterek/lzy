
Namespace Validation
    Public Class Handling

        Private Shared _allValidators As New Dictionary(Of Type, List(Of IValidateAction))
        Private Shared ReadOnly PadLock As New Object

        Private Shared ReadOnly Property AllValidators As Dictionary(Of Type, List(Of IValidateAction))
            Get

                Return _allValidators
            End Get
        End Property

        Public Shared Sub AddValidator(Of TAction As IAmAnAction)(validator As IValidateAction)
            If Not _allValidators.ContainsKey(GetType(TAction)) Then
                _allValidators(GetType(TAction)) = New List(Of IValidateAction)
            End If
            _allValidators(GetType(TAction)).Add(validator)
        End Sub

        Public Shared Sub ValidateAction(profile As ExecutionProfile.IExecutionProfile, action As IAmAnAction)
            Dim t = action.GetType
            While t IsNot Nothing
                If AllValidators.ContainsKey(t) Then
                    For Each Validator In AllValidators(t)
                        Validator.InternalValidate(profile, action)
                    Next
                End If
                t = t.BaseType
            End While
        End Sub

    End Class
End Namespace
