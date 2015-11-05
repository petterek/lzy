Imports System.Security.Principal
Imports LazyFramework.CQRS
Imports LazyFramework.CQRS.ExecutionProfile

Public MustInherit Class ActionBase
    Implements IAmAnAction

    Private ReadOnly _GUID As Guid
    Private ReadOnly _TimeStamp As Long
    Private _EndTimeStamp As Long
    Protected _profile As IExecutionProfile

    Public Sub New()
        _GUID = System.Guid.NewGuid
        _TimeStamp = Now.Ticks
    End Sub

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Friend Sub ActionComplete() Implements IAmAnAction.ActionComplete
        _EndTimeStamp = Now.Ticks
    End Sub

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Sub SetProfile(u As IExecutionProfile) Implements IActionBase.SetProfile
        _profile = u
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
        Return Me.GetType.FullName.Replace("."c, "")
    End Function

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Function TimeStamp() As Long Implements IAmAnAction.TimeStamp
        Return _TimeStamp
    End Function
    

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustOverride Function IsAvailable(user As IExecutionProfile, o As Object) As Boolean Implements IAmAnAction.IsAvailable
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public MustOverride Function IsAvailable(user As IExecutionProfile) As Boolean Implements IActionBase.IsAvailable
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
    
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function ExecutionProfile() As IExecutionProfile Implements IAmAnAction.ExecutionProfile
        Return _profile
    End Function
End Class

