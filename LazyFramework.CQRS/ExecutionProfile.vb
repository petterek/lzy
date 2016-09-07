
Public MustInherit Class ExecutionProfile
    Public Property Action As IActionBase
    Public Property ClientContext As Object
    Friend Property ActionHandler As Func(Of Object, Object)
    Friend Property Transformer As Transform.ITransformerFactory
    Public Property ActionIsAvailable As Availability.ICommandAvailability
    Public Property ActionSecurity As Security.IActionSecurity
    Public Property ValidateAction As Validation.IValidateAction
    Public Property RequestStart As Long
    Public Property RequestEnd As Long

    Public Sub Start()
        RequestStart = Now().Ticks
    End Sub

    Public Sub Stopp()
        RequestEnd = Now.Ticks
    End Sub

End Class

''' <summary>
''' 
''' </summary>
''' <typeparam name="TQuery">Type of Query</typeparam>
''' <typeparam name="TBo">Type of business object related to this query</typeparam>
''' <typeparam name="TDto">Type of Dto object related to this query.</typeparam>
Public Class QueryExecutionProfile(Of TQuery, TBo, TDto)
    Inherits ExecutionProfile
    Public Sub New()
    End Sub

    Public Sub New(handler As Func(Of TQuery, Object))
        ActionHandler = New Func(Of Object, Object)(Function(action) handler(CType(action, TQuery)))
    End Sub


    Private _localTransformer As Transform.TransformerFactoryBase(Of TBo, TDto)
    Public Property ResultTransformer As Transform.TransformerFactoryBase(Of TBo, TDto)
        Get
            Return _localTransformer
        End Get
        Set(value As Transform.TransformerFactoryBase(Of TBo, TDto))

            If value Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(value))
            End If

            Transformer = value
            _localTransformer = value
        End Set
    End Property

End Class


Public Class CommandExecutionBase
    Inherits ExecutionProfile
    Public Property Entity As Object

End Class

Public Class CommandExecutionProfile(Of TCommand As Command.IAmACommand, TBusinessObject, TDto)
    Inherits CommandExecutionBase

    Public Property BusinessObject As TBusinessObject
        Get
            Return CType(Me.Entity, TBusinessObject)
        End Get
        Set(value As TBusinessObject)
            Me.Entity = value
        End Set
    End Property

    Public Sub New(handler As Func(Of TCommand, Object))
        ActionHandler = New Func(Of Object, Object)(Function(action) handler(CType(action, TCommand)))
    End Sub


    Private _localTransformer As Transform.TransformerFactoryBase(Of TBusinessObject, TDto)
    Public Property ResultTransformer As Transform.TransformerFactoryBase(Of TBusinessObject, TDto)
        Get
            Return _localTransformer
        End Get
        Set(value As Transform.TransformerFactoryBase(Of TBusinessObject, TDto))

            If value Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(value))
            End If
            Transformer = value
            _localTransformer = value
        End Set
    End Property


End Class

