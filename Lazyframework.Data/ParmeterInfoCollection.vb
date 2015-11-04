
<Serializable> Public NotInheritable Class ParmeterInfoCollection
    Inherits Dictionary(Of String, ParameterInfo)

    Public Overloads Function Add(ByVal name As String, ByVal dbType As DbType) As ParameterInfo
        Return Add(name, dbType, 0, False, New ValueNotSet)
    End Function

    Public Overloads Function Add(ByVal name As String, ByVal dbType As DbType, ByVal value As Object) As ParameterInfo
        Return Add(name, dbType, 0, False, value)
    End Function

    Public Overloads Function Add(ByVal name As String, ByVal dbType As DbType, nullable As Boolean, ByVal value As Object) As ParameterInfo
        Return Add(name, dbType, 0, nullable, value)
    End Function

    Public Overloads Function Add(ByVal name As String, ByVal dbType As DbType, ByVal size As Integer, ByVal allowNulls As Boolean) As ParameterInfo
        Return Add(name, dbType, size, allowNulls, New ValueNotSet)
    End Function
    
    Public Overloads Function Add(ByVal name As String, ByVal dbType As DbType, ByVal size As Integer, ByVal allowNulls As Boolean, ByVal value As Object) As ParameterInfo
        Dim p As New ParameterInfo
        p.Name = name
        p.DbType = dbType
        p.AllowNull = allowNulls
        If size <> 0 Then
            p.Size = size
        End If

        If value IsNot Nothing Then
            If value.GetType IsNot GetType(ValueNotSet) Then
                p.Value = value
            End If
        Else
            If p.AllowNull Then p.Value = DBNull.Value
        End If


        Add(p.Name, p)
        Return p
    End Function


    Private Class ValueNotSet
    End Class

    Public Sub New()
    End Sub

    Protected Sub New(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
    
    Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
        MyBase.GetObjectData(info, context)
    End Sub

End Class
