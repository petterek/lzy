''' <summary>
''' An enum of possible returnvalues from then dataaccesslayer
''' </summary>
''' <remarks></remarks>
<Flags(), CLSCompliant(True)> _
Public Enum FillResultEnum As Integer
    ''' <summary>
    ''' Set when query returns no rows
    ''' </summary>
    ''' <remarks></remarks>
    NoData = 0
    ''' <summary>
    ''' Set when only 1 line is returned from then query
    ''' </summary>
    ''' <remarks></remarks>
    DataFound = 1
    ''' <summary>
    ''' Set when more than 1 line is returned from query
    ''' </summary>
    ''' <remarks></remarks>
    MultipleLinesFound = 2
    ''' <summary>
    ''' Set when an dataupdate query returned no errors
    ''' </summary>
    ''' <remarks></remarks>
    UpdateOK = 4
End Enum


<Flags()> _
Public Enum DataAccessTypes As Integer
    SQLServer = 1
    Oracle = 2
    MySql = 4
    Excel = 8
    CSV = 16
End Enum

