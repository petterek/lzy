

Namespace CQRS.Transform
    Public Interface ISortingFunction
        Function SortingFunc(action As IAmAnAction) As Comparison(Of Object)
    End Interface

    Public Interface ITransformerFactory
        Inherits ISortingFunction
        Property RunAsParallel As Boolean
        Function GetTransformer(action As IAmAnAction, ent As Object) As ITransformEntityToDto
    End Interface
End Namespace
