Imports NUnit.Framework
Imports System.Security.Principal
Imports LazyFramework.CQRS
Imports LazyFramework.CQRS.ExecutionProfile

<TestFixture> Public Class ValidatorTest

    <Test> Public Sub ValidatorsIsCalled()
        Assert.Throws(Of CQRS.Validation.ValidationException)(Sub() CQRS.Validation.Handling.ValidateAction(New ToValidate))

        Try
            CQRS.Validation.Handling.ValidateAction(New ToValidate)
        Catch ex As CQRS.Validation.ValidationException
            Assert.AreEqual(2, ex.ExceptionList.Count)
        End Try

    End Sub

End Class

Public Class ToValidate
    Inherits CQRS.ActionBase
    
    Property Id As Integer

    Public Overrides Function IsAvailable(profile As IExecutionProfile, o As Object) As Boolean
        Return True
    End Function

    Public Overrides Function IsAvailable(profile As IExecutionProfile) As Boolean
        Return True
    End Function

    Public Overloads Overrides Function IsAvailable() As Boolean
        Return True
    End Function
End Class


Public Class ToValidateValidator
    Inherits CQRS.Validation.ValidateActionBase(Of ToValidate)

    Public Sub CheckId(action As ToValidate)
        If action.Id = 0 Then
            Throw New MissingFieldException
        End If
    End Sub

    Public Sub IdIsLessThan1000(action As ToValidate)
        If action.Id < 1000 Then
            Throw New ArgumentOutOfRangeException
        End If
    End Sub

    Private Sub DoNotCall()
        Throw New NotImplementedException
    End Sub

End Class
