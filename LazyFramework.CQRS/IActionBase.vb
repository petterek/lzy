Imports System.Security.Principal


''' <summary>
''' Marker interface for all actions. 
''' 
''' </summary>
''' <remarks></remarks>
Public Interface IActionBase

    Function ActionName() As String

    Function IsAvailable() As Boolean
    Function IsAvailable(user As IPrincipal) As Boolean
    Function IsAvailable(user As IPrincipal, o As Object) As Boolean

End Interface



