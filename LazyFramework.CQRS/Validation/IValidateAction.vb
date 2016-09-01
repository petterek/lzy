Imports LazyFramework.CQRS.ExecutionProfile

Namespace Validation
    Public Interface IValidateAction
        Sub InternalValidate(ByVal executionProfile As Object, ByVal action As IAmAnAction)
    End Interface
End Namespace
