Public Class PluginValidator
    Inherits TypeValidation.CheckTypeProviderBaseClass


    Public Overrides Function GetTypes() As System.Collections.Generic.List(Of String)
        Dim ret As New List(Of String)

        For Each s As ModificationObjectPluginElement In LazyFrameworkConfiguration.Current.DataModificationPlugins
            ret.Add(s.Type)
        Next
        Return ret
    End Function

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Datamodificationplugins"
        End Get
    End Property
End Class
