Imports LazyFramework.CQRS.ExecutionProfile

Namespace Validation
 Public Interface IValidateAction
        Sub InternalValidate(ByVal executionProfile As IExecutionProfile, ByVal action As IAmAnAction)

    End Interface
End NameSpace
