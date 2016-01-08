Imports System.Configuration

Public Interface ILazyFrameworkConfiguration
    ''' <summary>
    ''' The type of datastore you want to access. 
    ''' Default is SQLServer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ConfigurationProperty("datastoreType", DefaultValue:="SQLServer")>
    ReadOnly Property DatastoreType() As String

    <ConfigurationProperty("objectStoreDatabase")>
    ReadOnly Property ObjectStoreDatabase() As String

    <ConfigurationProperty("raiseFillCompleteEvent", defaultvalue:=False)>
    ReadOnly Property RaiseFillCompleteEvent() As Boolean

    ''' <summary>
    ''' Gets the servers.
    ''' Add a list of servers that your application will access. 
    ''' connectionString="Server=PETTER-BB\sqlexpress;Database=[DBName];User ID=sa;Password=;Trusted_Connection=true"
    ''' The [DBName] will be replaced by whatever preceeds the @ in your database name.
    ''' </summary>
    ''' <value>The servers.</value>
    <ConfigurationProperty("servers", IsDefaultCollection:=False), ConfigurationCollection(GetType(ServersCollection), AddItemName:="add", ClearItemsName:="clear", RemoveItemName:="remove")>
    ReadOnly Property Servers() As ServersCollection

    <ConfigurationProperty("DataModificationPlugins", IsDefaultCollection:=False), ConfigurationCollection(GetType(ModifieObjectPluginCollection), AddItemName:="add", ClearItemsName:="clear", RemoveItemName:="remove")>
    ReadOnly Property DataModificationPlugins() As ModifieObjectPluginCollection

    <ConfigurationProperty("supressDatabindingErrors", defaultValue:=True)>
    ReadOnly Property SupressDatabindingErrors() As Boolean

    <ConfigurationProperty("supressErrors", defaultValue:=False)>
    ReadOnly Property SupressErrors() As Boolean

    <ConfigurationProperty("skipTypes", defaultValue:="")>
    ReadOnly Property SkipTypes() As String

    Property LogLevel As Integer
    Property EnableTiming As Boolean

End Interface
