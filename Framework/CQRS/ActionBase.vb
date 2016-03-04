Imports System.Security.Principal

Namespace CQRS
    Public MustInherit Class ActionBase
        Implements IAmAnAction


        Private _User As IPrincipal
        Private ReadOnly _GUID As Guid
        Private ReadOnly _TimeStamp As Long
        Private _EndTimeStamp As Long

        Public Sub New()
            _GUID = System.Guid.NewGuid
            _TimeStamp = Now.Ticks

        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Friend Sub ActionComplete() Implements IAmAnAction.ActionComplete
            _EndTimeStamp = Now.Ticks
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Sub SetUser(u As IPrincipal)
            _User = u
        End Sub

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function EndTimeStamp() As Long Implements IAmAnAction.EndTimeStamp
            Return _EndTimeStamp
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function Guid() As Guid Implements IAmAnAction.Guid
            Return _GUID
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Overridable Function ActionName() As String Implements IAmAnAction.ActionName
            Return Me.GetType.FullName
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function TimeStamp() As Long Implements IAmAnAction.TimeStamp
            Return _TimeStamp
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
        Public Function User() As Security.Principal.IPrincipal Implements IAmAnAction.User
            If _User Is Nothing Then
                Return Runtime.Context.Current.CurrentUser
            End If
            Return _User
        End Function

        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustOverride Function IsAvailable(user As IPrincipal, o As Object) As Boolean Implements IAmAnAction.IsAvailable
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustOverride Function IsAvailable(user As IPrincipal) As Boolean Implements IActionBase.IsAvailable
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustOverride Function IsAvailable() As Boolean Implements IActionBase.IsAvailable

        Public Function Contexts() As IEnumerable(Of ActionContext.ActionContext)
            Return ActionContext.Handling.GetContextsForAction(Me)
        End Function


        'Public Overridable Sub OnActionBegin()
        'End Sub
        'Public Overridable Sub OnActionComplete()
        'End Sub

        Private _hsts As Long
        Public Sub HandlerStart() Implements IAmAnAction.HandlerStart
            _hsts = Now.Ticks
        End Sub

        Public Function HandlerStartTimeStamp() As Long Implements IAmAnAction.HandlerStartTimeStamp
            Return _hsts
        End Function
    End Class

    

    

End Namespace
