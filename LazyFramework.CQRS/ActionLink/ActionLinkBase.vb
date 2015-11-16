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
        
        Public Function Contexts() As IEnumerable(Of ActionContext.ActionContext)
            Return ActionContext.Handling.GetContextsForAction(Me)
        End Function

        'Public Sub SetProfile(profile As IExecutionProfile) Implements IActionBase.SetProfile
        '    _profile = profile
        'End Sub

        'Public Function ExecutionProfile() As IExecutionProfile Implements IActionBase.ExecutionProfile
        '    Return _profile
        'End Function
    End Class

    Public MustInherit Class ActionLinkBase(Of TContext)
        Inherits ActionLinkBase
        

    End Class

End Namespace
