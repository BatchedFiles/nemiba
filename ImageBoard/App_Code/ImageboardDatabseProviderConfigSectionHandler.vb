Imports Microsoft.VisualBasic

Public Class ImageboardDatabseProviderConfigSectionHandler
	Inherits ConfigurationSection

	<ConfigurationProperty("type")> _
 Public Property type() As String
		Get
			Return Me("type").ToString
		End Get
		Set(ByVal value As String)
			Me("type") = value
		End Set
	End Property

	<ConfigurationProperty("resFolder")> _
	  Public Property ResFolder() As String
		Get
			Return Me("resFolder").ToString
		End Get
		Set(ByVal value As String)
			Me("resFolder") = value
		End Set
	End Property

	<ConfigurationProperty("connectionString")> _
	 Public Property ConnectionString() As String
		Get
			Return Me("connectionString").ToString
		End Get
		Set(ByVal value As String)
			Me("connectionString") = value
		End Set
	End Property

	<ConfigurationProperty("applicationName")> _
  Public Property ApplicationName() As String
		Get
			Return Me("applicationName").ToString
		End Get
		Set(ByVal value As String)
			Me("applicationName") = value
		End Set
	End Property
End Class
