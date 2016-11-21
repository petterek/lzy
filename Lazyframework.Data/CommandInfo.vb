Imports System.Data



Public Class CommandInfo
    Private _commandText As String

    Public Overridable Property CommandText() As String
        Get
            Return _commandText
        End Get
        Set(ByVal value As String)
            _commandText = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Overridable ReadOnly Property CommandType() As CommandType
        Get
            Return CommandType.Text
        End Get
    End Property

    Private _typeOfCommand As CommandTypeEnum

    Public Overridable Property TypeOfCommand() As CommandTypeEnum
        Get
            Return _typeOfCommand
        End Get
        Set(ByVal value As CommandTypeEnum)
            _typeOfCommand = value
        End Set
    End Property

    Private _parameters As ParmeterInfoCollection

    Public ReadOnly Property Parameters() As ParmeterInfoCollection
        Get
            If _parameters Is Nothing Then
                _parameters = New ParmeterInfoCollection
            End If
            Return _parameters
        End Get
    End Property
    

    'Private _commandDuration As Long
    'Public Property CommandDuration() As Long
    '    Get
    '        Return _commandDuration
    '    End Get
    '    Set(ByVal value As Long)
    '        _commandDuration = value
    '    End Set
    'End Property

    'Private _count As Long = 1
    'Public Property Count() As Long
    '    Get
    '        Return _count
    '    End Get
    '    Set(ByVal value As Long)
    '        _count = value
    '    End Set
    'End Property

'    Public Property CommandQuery() As Linq.Expressions.Expression


    ''' <summary>
    ''' Implemented as a helper to ease copying the command to the Querywindow.
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim res As New System.Text.StringBuilder

        For Each p In Me.Parameters
            res.AppendLine(String.Format("Declare @{0}  {1} = {2}", p.Value.Name, p.Value.DbType.ToString, p.Value.Value))
        Next

        res.AppendLine(CommandText)
        Return res.ToString

    End Function

End Class