Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile

Namespace ActionLink
    Public MustInherit Class ActionLinkBase
        Implements IActionBase

        Private _profile as IExecutionProfile

        Public Overridable Function ActionName() As String Implements IActionBase.ActionName
            Return Me.GetType.FullName
        End Function

        Public Overridable ReadOnly Property Tag As String
            Get
                Return Nothing
            End Get
        End Property


        Public MustOverride Function IsAvailable() As Boolean Implements IActionBase.IsAvailable
        Public MustOverride Function IsAvailable(profile As IExecutionProfile) As Boolean Implements IActionBase.IsAvailable
        Public MustOverride Function IsAvailable(profile As IExecutionProfile, o As Object) As Boolean Implements IActionBase.IsAvailable

        Public Function Contexts() As IEnumerable(Of ActionContext.ActionContext)
            Return ActionContext.Handling.GetContextsForAction(Me)
        End Function

        Public Sub SetProfile(profile As IExecutionProfile) Implements IActionBase.SetProfile
            _profile = profile
        End Sub

        Public Function ExecutionProfile() As IExecutionProfile Implements IActionBase.ExecutionProfile
            Return _profile
        End Function
    End Class

    Public MustInherit Class ActionLinkBase(Of TContext)
        Inherits ActionLinkBase

        Public Overrides Function IsAvailable() As Boolean
            Return True
        End Function

        Public Overrides Function IsAvailable(profile As IExecutionProfile) As Boolean
            Return IsActionAvailable(profile)
        End Function

        Public Overrides Function IsAvailable(profile As IExecutionProfile, o As Object) As Boolean
            Return IsActionAvailable(profile, CType(o, TContext))
        End Function


        Public Overridable Function IsActionAvailable(profile As IExecutionProfile) As Boolean
            Return True
        End Function

        Public Overridable Function IsActionAvailable(profile As IExecutionProfile, entity As TContext) As Boolean
            Return True
        End Function


    End Class

End Namespace
