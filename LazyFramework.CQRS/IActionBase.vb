Imports System.Security.Principal
Imports LazyFramework.CQRS.ExecutionProfile


''' <summary>
    ''' Marker interface for all actions. 
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IActionBase
        
        Sub SetProfile(profile As iExecutionProfile)
        Function ExecutionProfile As IExecutionProfile
        Function ActionName() As String
        Function IsAvailable() As Boolean
        Function IsAvailable(profile As IExecutionProfile) As Boolean
        Function IsAvailable(profile As IExecutionProfile, o As Object) As Boolean


    End Interface

    

