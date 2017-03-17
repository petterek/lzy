Public Interface IDataStore
    Sub Exec(Of T As New)(command As CommandInfo, data As List(Of T))
    Sub Exec(Of T As New)(command As CommandInfo, data As FillStatus(Of T))
    Sub Exec(command As CommandInfo, data As Object)
    Sub Exec(command As CommandInfo)
    Sub Exec(Of T As Structure)(command As CommandInfo, data As ICollection(Of T), colName As String)
    Function ExecScalar(Of T)(command As CommandInfo) As T
    Sub GetStream(Of T As {New, WillDisposeThoseForU})(command As CommandInfo, data As T)
End Interface
