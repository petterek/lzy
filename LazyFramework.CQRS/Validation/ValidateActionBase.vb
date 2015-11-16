Namespace Validation
    Public MustInherit Class ValidateActionBase(Of TAction As IAmAnAction)
        Implements IValidateAction

        Friend Sub InternalValidate(profile As ExecutionProfile.IExecutionProfile, action As IAmAnAction) Implements IValidateAction.InternalValidate
            '    ValidatAction(CType(action, TAction))
            Dim val As New ValidationException

            Dim [getType] As Type = Me.GetType

            For Each p In [getType].GetMethods(System.Reflection.BindingFlags.DeclaredOnly Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
                Try
                    Dim invokeParams = p.GetParameters()
                    If invokeParams.Count = 1 Then
                        If p.GetParameters(0).ParameterType.IsAssignableFrom(action.GetType) Then
                            p.Invoke(Me, {profile, action})
                        Else
                            Throw New UnableToMapAsValidatorFunction(p.Name)
                        End If
                    ElseIf invokeParams.Count = 2 Then
                        If p.GetParameters(0).ParameterType.IsAssignableFrom(GetType(ExecutionProfile.IExecutionProfile)) AndAlso p.GetParameters(1).ParameterType.IsAssignableFrom(action.GetType) Then
                            p.Invoke(Me, {profile, action})
                        Else
                            Throw New UnableToMapAsValidatorFunction(p.Name)
                        End If
                    End If

                Catch ex As Exception
                    val.ExceptionList.Add(p.Name, ex.InnerException)
                End Try

            Next

            If val.ExceptionList.Count > 0 Then
                Throw val
            End If

        End Sub

        '        Public MustOverride Sub ValidatAction(action As TAction)

    End Class

    Friend Class UnableToMapAsValidatorFunction
        Inherits Exception
        Public Sub New(name As String)
            MyBase.New(name)
        End Sub
    End Class
End Namespace
