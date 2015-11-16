
Namespace Validation
    Public Class Handling

        Private Shared _allValidators As Dictionary(Of Type, List(Of IValidateAction))
        Private Shared ReadOnly PadLock As New Object

        Private Shared ReadOnly Property AllValidators As Dictionary(Of Type, List(Of IValidateAction))
            Get
                If _allValidators Is Nothing Then
                    SyncLock PadLock
                        If _allValidators Is Nothing Then
                            Dim temp As New Dictionary(Of Type, List(Of IValidateAction))
                            For Each t In Reflection.FindAllClassesOfTypeInApplication(GetType(IValidateAction))
                                If Not t.IsAbstract Then
                                    Dim tSpes = t.BaseType
                                    While Not tSpes.IsGenericType
                                        tSpes = tSpes.BaseType
                                        If tSpes Is Nothing Then 'Something wrong in this validator.. Can not have validators in non generic types..
                                            Continue For
                                        End If
                                    End While

                                    Dim key = tSpes.GetGenericArguments()(0)
                                    If Not temp.ContainsKey(key) Then
                                        temp.Add(key, New List(Of IValidateAction))
                                    End If
                                    temp(key).Add(CType(Activator.CreateInstance(t), IValidateAction))

                                End If
                            Next
                            _allValidators = temp
                        End If
                    End SyncLock
                End If
                Return _allValidators
            End Get
        End Property



        Public Shared Sub ValidateAction(profile As ExecutionProfile.IExecutionProfile, action As IAmAnAction)
            Dim t = action.GetType
            While t IsNot Nothing
                If AllValidators.ContainsKey(t) Then
                    For Each Validator In AllValidators(t)
                        Validator.InternalValidate(profile,action)
                    Next                    
                End If
                t = t.BaseType
            End While
        End Sub

    End Class
End Namespace
