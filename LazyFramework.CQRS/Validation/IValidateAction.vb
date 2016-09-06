Imports LazyFramework.CQRS.QueryExecutionProfile

Namespace Validation
    Public Interface IValidateAction
        Sub InternalValidate(ByVal action As IAmAnAction)
    End Interface
End Namespace
