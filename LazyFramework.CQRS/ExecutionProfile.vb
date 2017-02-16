
Public MustInherit Class ExecutionProfile
    Public Property Action As IActionBase
    Public Property ClientContext As Object
    Friend Property ActionHandler As Func(Of Object, Object)
    Friend Property Transformer As Func(Of Object, Object)
    Public Property ActionSecurity As Security.IActionSecurity
    Public Property ValidateAction As Validation.IValidateAction
    Public Property RequestStart As Long
    Public Property RequestEnd As Long
    Public Property Sorter As Comparison(Of Object)
    Public Property RunAsParallel As Boolean = True

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


    Private _localTransformer As Func(Of TBo, TDto)
    Public Property ResultTransformer As Func(Of TBo, TDto)
        Get
            Return _localTransformer
        End Get
        Set(value As Func(Of TBo, TDto))

            If value Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(value))
            End If

            Transformer = New Func(Of Object, Object)(Function(f) value(CType(f, TBo)))
            _localTransformer = value
        End Set
    End Property

    Public WriteOnly Property ResultSorter As Func(Of TDto, TDto, Integer)
        Set(value As Func(Of TDto, TDto, Integer))
            Sorter = New Comparison(Of Object)(Function(o1, o2) value(CType(o1, TDto), CType(o2, TDto)))
        End Set
    End Property

End Class


Public Class CommandExecutionBase
    Inherits ExecutionProfile
    Friend Property Entity As Object

End Class


Public Class CommandExecutionProfile(Of TCommand As Command.IAmACommand, TBo)
    Inherits CommandExecutionBase

    Public Sub New(handler As Action(Of TCommand))
        ActionHandler = New Func(Of Object, Object)(Function()
                                                        handler(CType(Action, TCommand))
                                                        Return Nothing
                                                    End Function)
    End Sub


    Public Sub New(handler As Func(Of TCommand, TBo))
        ActionHandler = New Func(Of Object, Object)(Function()
                                                        Return handler(CType(Action, TCommand))
                                                    End Function)
    End Sub


    Public Property BusinessObject As TBo
        Get
            Return CType(Me.Entity, TBo)
        End Get
        Set(value As TBo)
            Me.Entity = value
        End Set
    End Property
End Class

Public Class CommandExecutionProfile(Of TCommand As Command.IAmACommand, TBo, TDto)
    Inherits CommandExecutionBase


    Public Sub New(handler As Func(Of TCommand, Object))
        ActionHandler = New Func(Of Object, Object)(Function(action) handler(CType(action, TCommand)))
    End Sub


    Private _localTransformer As Func(Of TBo, TDto)
    Public Property ResultTransformer As Func(Of TBo, TDto)
        Get
            Return _localTransformer
        End Get
        Set(value As Func(Of TBo, TDto))

            If value Is Nothing Then
                Throw New System.ArgumentNullException(NameOf(value))
            End If

            Transformer = New Func(Of Object, Object)(Function(f) value(CType(f, TBo)))
            _localTransformer = value
        End Set
    End Property

End Class

