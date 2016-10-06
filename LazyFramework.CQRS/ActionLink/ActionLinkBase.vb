Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile

Namespace ActionLink
    Public MustInherit Class ActionLinkBase
        Implements IActionBase

        Private _profile As IExecutionProfile

        Public Overridable Function ActionName() As String Implements IActionBase.ActionName
            Return Me.GetType.FullName
        End Function

        Public Overridable ReadOnly Property Tag As String
            Get
                Return Nothing
            End Get
        End Property

        Public MustOverride Function IsAvailable() As Boolean Implements IActionBase.IsAvailable
        Public MustOverride Function IsAvailable(user As IPrincipal) As Boolean Implements IActionBase.IsAvailable
        Public MustOverride Function IsAvailable(user As IPrincipal, o As Object) As Boolean Implements IActionBase.IsAvailable


        Public Function Contexts() As IEnumerable(Of ActionContext.ActionContext)
            Return ActionContext.Handling.GetContextsForAction(Me)
        End Function

    End Class

    Public MustInherit Class ActionLinkBase(Of TContext)
        Inherits ActionLinkBase

        Public Overrides Function IsAvailable() As Boolean
            Return True
        End Function

        Public Overrides Function IsAvailable(user As IPrincipal) As Boolean
            Return IsActionAvailable(user)
        End Function

        Public Overrides Function IsAvailable(user As IPrincipal, o As Object) As Boolean
            Return IsActionAvailable(user, CType(o, TContext))
        End Function


        Public Overridable Function IsActionAvailable(user As IPrincipal) As Boolean
            Return True
        End Function

        Public Overridable Function IsActionAvailable(user As IPrincipal, entity As TContext) As Boolean
            Return True
        End Function
    End Class

End Namespace
