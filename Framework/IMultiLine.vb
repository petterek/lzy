''' <summary>
''' Tells the dataaccesslayer that this is a collection supporting multiple objects
''' </summary>
''' <remarks></remarks>
Public Interface IMultiLine
    ''' <summary>
    ''' Here the base collection handles the creation of new objects in a list
    ''' </summary>
    ''' <remarks></remarks>
    Function NextRow() As Boolean
    Sub SetMode(ByVal mode As IMultiLineMode)
    ReadOnly Property Count() As Integer


    Enum IMultiLineMode
        Write = 0
        Read = 1
    End Enum




End Interface

