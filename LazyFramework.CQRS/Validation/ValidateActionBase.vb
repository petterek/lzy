
Namespace Validation
    Public MustInherit Class ValidateActionBase(Of TAction As IActionBase)
        Implements IValidateAction

        Public Overridable ReadOnly Property DetailedExceptionInfo As Boolean = False

        Friend Sub InternalValidate(ByVal action As IActionBase) Implements IValidateAction.InternalValidate
            '    ValidatAction(CType(action, TAction))
            Dim val As New ValidationException
            Dim exList As New Dictionary(Of String, Exception)


            Dim [getType] As Type = Me.GetType

            For Each p In [getType].GetMethods(System.Reflection.BindingFlags.DeclaredOnly Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance)
                Try
                    If p.GetParameters.Count = 1 AndAlso p.GetParameters(0).ParameterType.IsAssignableFrom(action.GetType) Then
                        p.Invoke(Me, {action})
                    End If
                Catch ex As Exception
                    exList.Add(p.Name, ex.InnerException)
                End Try
            Next

            If exList.Any Then
                If Not DetailedExceptionInfo Then
                    val.ExceptionList = exList
                    Throw val
                Else
                    Dim ex As New DetailedValidationException(exList)
                    Throw ex
                End If

            End If

        End Sub

    End Class
End Namespace
