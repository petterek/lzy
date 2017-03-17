Public Interface IWriterInstance
    Sub Create(command As CommandInfo)
    Function Create(Of T)(command As CommandInfo) As T
    Sub Update(command As CommandInfo)
    Sub Delete(command As CommandInfo)
End Interface
