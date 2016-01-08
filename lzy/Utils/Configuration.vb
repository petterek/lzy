Imports System.Configuration

''' <summary>
''' The configuration class. 
''' You must put a configuration section of this type in your .config file.
''' </summary>
''' <remarks>
''' <example>
''' &lt;configSections&gt;
'''		&lt;section name="LazyFrameworkConfiguration" type="LazyFramework.LazyFrameworkConfiguration,LazyFramework.Base"/&gt;
'''	&lt;/configSections&gt;
'''	&lt;LazyFrameworkConfiguration datastoreType="SqlServer" &gt;
'''		&lt;servers&gt;
'''			&lt;add name="local" connectionString="Server=PETTER-BB\sqlexpress;Database=[DBName];User ID=sa;Password=;Trusted_Connection=true" /&gt;
'''		&lt;/servers&gt;
'''	&lt;/LazyFrameworkConfiguration&gt;
''' </example>
'''</remarks>
Public Class LazyFrameworkConfiguration
    Inherits ConfigurationSection
    Implements ILazyFrameworkConfiguration


    ''' <summary>
    ''' The type of datastore you want to access. 
    ''' Default is SQLServer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ConfigurationProperty("datastoreType", DefaultValue:="SQLServer")> _
    Public ReadOnly Property DatastoreType() As String Implements ILazyFrameworkConfiguration.DatastoreType
        Get
            Return Me("datastoreType").ToString
        End Get
    End Property

    <ConfigurationProperty("objectStoreDatabase")> _
    Public ReadOnly Property ObjectStoreDatabase() As String Implements ILazyFrameworkConfiguration.ObjectStoreDatabase
        Get
            Return CType(Me("objectStoreDatabase"), String)
        End Get
    End Property

    <ConfigurationProperty("raiseFillCompleteEvent", defaultvalue:=False)> _
    Public ReadOnly Property RaiseFillCompleteEvent() As Boolean Implements ILazyFrameworkConfiguration.RaiseFillCompleteEvent
        Get
            Return CType(Me("raiseFillCompleteEvent"), Boolean)
        End Get
    End Property


    ''' <summary>
    ''' Gets the servers.
    ''' Add a list of servers that your application will access. 
    ''' connectionString="Server=PETTER-BB\sqlexpress;Database=[DBName];User ID=sa;Password=;Trusted_Connection=true"
    ''' The [DBName] will be replaced by whatever preceeds the @ in your database name.
    ''' </summary>
    ''' <value>The servers.</value>
    <ConfigurationProperty("servers", IsDefaultCollection:=False), ConfigurationCollection(GetType(ServersCollection), AddItemName:="add", ClearItemsName:="clear", RemoveItemName:="remove")> _
    Public ReadOnly Property Servers() As ServersCollection Implements ILazyFrameworkConfiguration.Servers
        Get
            Dim ServerCollection As ServersCollection = CType(Me("servers"), ServersCollection)
            Return ServerCollection
        End Get
    End Property

    <ConfigurationProperty("DataModificationPlugins", IsDefaultCollection:=False), ConfigurationCollection(GetType(ModifieObjectPluginCollection), AddItemName:="add", ClearItemsName:="clear", RemoveItemName:="remove")> _
    Public ReadOnly Property DataModificationPlugins() As ModifieObjectPluginCollection Implements ILazyFrameworkConfiguration.DataModificationPlugins
        Get
            Dim PreSave As ModifieObjectPluginCollection = CType(Me("DataModificationPlugins"), ModifieObjectPluginCollection)
            Return PreSave
        End Get
    End Property

    <ConfigurationProperty("supressDatabindingErrors", defaultValue:=True)> _
    Public ReadOnly Property SupressDatabindingErrors() As Boolean Implements ILazyFrameworkConfiguration.SupressDatabindingErrors
        Get
            Return CType(Me("supressDatabindingErrors"), Boolean)
        End Get
    End Property

    <ConfigurationProperty("supressErrors", defaultValue:=False)> _
    Public ReadOnly Property SupressErrors() As Boolean Implements ILazyFrameworkConfiguration.SupressErrors
        Get
            Return CType(Me("supressErrors"), Boolean)
        End Get
    End Property

    <ConfigurationProperty("skipTypes", defaultValue:="")> _
    Public ReadOnly Property SkipTypes() As String Implements ILazyFrameworkConfiguration.SkipTypes
        Get
            Return TryCast(Me("skipTypes"), String)
        End Get
    End Property




    Private Shared _current As ILazyFrameworkConfiguration
    Private Shared ReadOnly Padlock As New Object
    ''' <summary>
    ''' Gets the current LazyFrameworkconfiguration object.
    ''' </summary>
    ''' <value>The current.</value>
    Public Shared ReadOnly Property Current() As ILazyFrameworkConfiguration
        Get
            If _current Is Nothing Then
                If ClassFactory.ContainsKey(Of ILazyFrameworkConfiguration)() Then
                    Return ClassFactory.GetTypeInstance(Of ILazyFrameworkConfiguration)()
                Else
                    _current = DirectCast(ConfigurationManager.GetSection("LazyFrameworkConfiguration"), LazyFrameworkConfiguration)
                End If
            End If
            Return _current
        End Get
    End Property


    Public Property EnableTiming As Boolean Implements ILazyFrameworkConfiguration.EnableTiming

    Public Property LogLevel As Integer Implements ILazyFrameworkConfiguration.LogLevel
End Class


Public Class ConfigCollection(Of t As {New, ConfigurationBaseElement})
    Inherits ConfigurationElementCollection

    Public Sub Add(ByVal ce As ServerConfigElement)
        BaseAdd(ce)
    End Sub

    Public Overrides ReadOnly Property CollectionType() As ConfigurationElementCollectionType
        Get
            Return ConfigurationElementCollectionType.AddRemoveClearMap
        End Get
    End Property


    Protected Overloads Overrides Function CreateNewElement() As ConfigurationElement
        Return New t
    End Function

    Protected Overrides Function GetElementKey(ByVal element As ConfigurationElement) As Object
        Return CType(element, t).Name
    End Function

    Default Public Shadows ReadOnly Property Item(ByVal Name As String) As t
        Get
            Return CType(BaseGet(Name), t)
        End Get
    End Property

    Public Function IndexOf(ByVal url As ServerConfigElement) As Integer
        Return BaseIndexOf(url)
    End Function
End Class

Public MustInherit Class ConfigurationBaseElement
    Inherits ConfigurationElement

    <ConfigurationProperty("name", IsRequired:=True, IsKey:=True)> _
    Public Property Name() As String
        Get
            Return CStr(Me("name"))
        End Get
        Set(ByVal value As String)
            Me("name") = value
        End Set
    End Property
End Class


Public Class ServersCollection
    Inherits ConfigCollection(Of ServerConfigElement)
End Class

Public Class ModifieObjectPluginCollection
    Inherits ConfigCollection(Of ModificationObjectPluginElement)
End Class


<Flags()> _
Public Enum PluginExecutionPointEnum As Integer
    Pre = 1
    Post = 2
    Both = 3
End Enum

''' <summary>
''' 
''' </summary>

Public Class ServerConfigElement
    Inherits ConfigurationBaseElement

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Gets or sets the connection string.
    ''' </summary>
    ''' <value>The connection string.</value>
    <ConfigurationProperty("connectionString", IsRequired:=True)> _
    Public Property ConnectionString() As String
        Get
            Return CStr(Me("connectionString"))
        End Get
        Set(ByVal value As String)
            Me("connectionString") = value
        End Set
    End Property
End Class

Public Class ModificationObjectPluginElement
    Inherits ConfigurationBaseElement

    <ConfigurationProperty("executionPoint", DefaultValue:="Post")> _
    Public ReadOnly Property ExecutionPoint() As PluginExecutionPointEnum
        Get
            Return CType(Me("executionPoint"), PluginExecutionPointEnum)
        End Get
    End Property

    <ConfigurationProperty("type", IsRequired:=True)> _
    Public ReadOnly Property Type() As String
        Get
            Return CStr(Me("type"))
        End Get
    End Property

    <ConfigurationProperty("params", IsRequired:=False, DefaultValue:="")> _
    Public ReadOnly Property params() As String
        Get
            Return CStr(Me("params"))
        End Get
    End Property
End Class


