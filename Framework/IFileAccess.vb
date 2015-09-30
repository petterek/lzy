''' <summary>
''' For now this is a readonly interface
''' </summary>
''' <remarks></remarks>
Public Interface IFileAccess
    Function GetData(ByVal file As String, ByVal command As CommandInfo, ByVal o As IORDataObject, ByVal ParamArray parameters As Object()) As Boolean
End Interface


