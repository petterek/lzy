Imports LazyFramework.Data

Public Class ServerConnectionInfo
    Inherits LazyFramework.Data.ServerConnectionInfo

    Public Sub New()
    End Sub

    Public Sub New(connString As String)
        MyBase.New(connString)
        If connString Is Nothing Then
            Throw New System.ArgumentNullException(NameOf(connString))
        End If

    End Sub
    Public Sub New(userName As String, password As String, server As String, initalCatalog As String)
        MyBase.New(String.Format("server={0};Database={1};User ID={2};Password={3};pooling=true;", server, initalCatalog, userName, password))
    End Sub

    Public Overrides Function GetProvider() As IDataAccessProvider
        Return New DataProvider
    End Function


    Public Class CreateParamsFromDto

        Public Shared TypeMap As New Dictionary(Of Type, DbType) From {
         {GetType(Integer), DbType.Int32},
         {GetType(Date), DbType.DateTime2},
         {GetType(ULong), DbType.UInt64},
         {GetType(String), DbType.String},
         {GetType(Decimal), DbType.Decimal},
         {GetType(Double), DbType.Double},
         {GetType(Guid), DbType.Guid}
        }


        Public Shared Function Create(dto As Object) As LazyFramework.Data.ParameterInfoCollection
            Dim ret As New ParameterInfoCollection

            For Each p In dto.GetType().GetFields()
                If TypeMap.ContainsKey(p.FieldType) Then
                    ret.Add(p.Name, TypeMap(p.FieldType), p.GetValue(dto))
                End If
            Next

            Return ret
        End Function
    End Class

End Class