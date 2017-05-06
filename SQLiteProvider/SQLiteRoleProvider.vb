Imports System.Web.Security
Imports System.Configuration.Provider
Imports System.Collections.Specialized
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Diagnostics
Imports System.Web
Imports System.Globalization
Imports Microsoft.VisualBasic

Public Class SqliteRoleProvider
	Inherits RoleProvider

	Private pApplicationName As String
	Private m_ConnectionString As String

	Public Overrides Sub Initialize(ByVal name As String, ByVal config As NameValueCollection)

		' Инициализация значений из файла конфигурации

		If config Is Nothing Then
			Throw New ArgumentNullException("config")
		End If
		If name Is Nothing OrElse name.Length = 0 Then
			name = "SqliteRoleProvider"
		End If
		If String.IsNullOrEmpty(config("description")) Then
			config.Remove("description")
			config.Add("description", "Simple Sqlite Role provider")
		End If

		' Initialize the abstract base class.
		MyBase.Initialize(name, config)

		If config("applicationName") Is Nothing OrElse config("applicationName").Trim() = "" Then
			pApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
		Else
			pApplicationName = config("applicationName")
		End If

		' Строка подключения
		Dim objConnectionStringSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(config("connectionStringName"))
		If objConnectionStringSettings Is Nothing OrElse String.IsNullOrEmpty(objConnectionStringSettings.ConnectionString) Then
			Throw New ProviderException("Connection string is empty for SQLiteMembershipProvider. Check the web configuration file (web.config).")
		End If
		m_ConnectionString = objConnectionStringSettings.ConnectionString

	End Sub

	Public Overrides Property ApplicationName() As String
		Get
			Return pApplicationName
		End Get
		Set(ByVal value As String)
			pApplicationName = value
		End Set
	End Property

	Public Overrides Sub AddUsersToRoles(ByVal usernames As String(), _
  ByVal rolenames As String())

		For Each rolename As String In rolenames
			If Not RoleExists(rolename) Then
				Throw New ProviderException("Role name not found.")
			End If
		Next

		For Each username As String In usernames
			If username.Contains(",") Then
				Throw New ArgumentException("User names cannot contain commas.")
			End If

			For Each rolename As String In rolenames
				If IsUserInRole(username, rolename) Then
					Throw New ProviderException("User is already in role.")
				End If
			Next
		Next

		Dim conn As SQLiteConnection = OpenBoardBase(m_connectionstring, False)

		For Each username As String In usernames
			For Each rolename As String In rolenames
				Dim cmd As SQLiteCommand = New SQLiteCommand("INSERT INTO UsersInRoles Values($UserName, $RoleName, $ApplicationName)", conn)
				cmd.Parameters.AddWithValue("$UserName", username)
				cmd.Parameters.AddWithValue("$RoleName", rolename)
				cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)
				cmd.ExecuteNonQuery()
				cmd.Dispose()
			Next
		Next

		CloseBoardBase(False)
	End Sub

	Public Overrides Sub CreateRole(ByVal rolename As String)

		If rolename.Contains(",") Then
			Throw New ArgumentException("Role names cannot contain commas.")
		End If

		If RoleExists(rolename) Then
			Throw New ProviderException("Role name already exists.")
		End If

		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim cmd As SQLiteCommand = New SQLiteCommand("INSERT INTO Roles (RoleName, ApplicationName) Values($Rolename, $ApplicationName)", conn)
		cmd.Parameters.AddWithValue("$Rolename", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)
		cmd.ExecuteNonQuery()
		cmd.Dispose()
		CloseBoardBase(False)
	End Sub

	Public Overrides Function DeleteRole(ByVal rolename As String, ByVal throwOnPopulatedRole As Boolean) As Boolean

		If Not RoleExists(rolename) Then
			Throw New ProviderException("Role does not exist.")
		End If

		If throwOnPopulatedRole AndAlso GetUsersInRole(rolename).Length > 0 Then
			Throw New ProviderException("Cannot delete a populated role.")
		End If
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim cmd As SQLiteCommand = New SQLiteCommand("DELETE FROM Roles WHERE RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$RoleName", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		cmd.ExecuteNonQuery()
		cmd.Dispose()
		CloseBoardBase(False)

		Return True
	End Function

	Public Overrides Function GetAllRoles() As String()
		Dim colRoles As List(Of String) = New List(Of String)
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT RoleName FROM Roles WHERE ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)
		Dim reader As SQLiteDataReader = Nothing

		reader = cmd.ExecuteReader()
		Do While reader.Read()
			colRoles.Add(reader.GetString(0))
		Loop
		If Not reader Is Nothing Then
			reader.Close()
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return colRoles.ToArray
	End Function

	Public Overrides Function GetRolesForUser(ByVal username As String) As String()
		Dim colRoles As List(Of String) = New List(Of String)
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$UserName", username)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim reader As SQLiteDataReader = Nothing

		reader = cmd.ExecuteReader()

		Do While reader.Read()
			colRoles.Add(reader.GetString(0))
		Loop
		If Not reader Is Nothing Then
			reader.Close()
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return colRoles.ToArray

	End Function

	Public Overrides Function GetUsersInRole(ByVal rolename As String) As String()
		Dim colRoles As List(Of String) = New List(Of String)
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT UserName FROM UsersInRoles WHERE RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$RoleName", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim reader As SQLiteDataReader = Nothing

		reader = cmd.ExecuteReader()

		Do While reader.Read()
			colRoles.Add(reader.GetString(0))
		Loop
		If Not reader Is Nothing Then
			reader.Close()
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return colRoles.ToArray

	End Function

	Public Overrides Function IsUserInRole(ByVal username As String, ByVal rolename As String) As Boolean

		Dim userIsInRole As Boolean = False
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM UsersInRoles  WHERE UserName=$UserName AND RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$UserName", username)
		cmd.Parameters.AddWithValue("$RoleName", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim numRecs As Integer = CType(cmd.ExecuteScalar(), Integer)

		If numRecs > 0 Then
			userIsInRole = True
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return userIsInRole
	End Function

	Public Overrides Sub RemoveUsersFromRoles(ByVal usernames As String(), ByVal rolenames As String())

		For Each rolename As String In rolenames
			If Not RoleExists(rolename) Then
				Throw New ProviderException("Role name not found.")
			End If
		Next

		For Each username As String In usernames
			For Each rolename As String In rolenames
				If Not IsUserInRole(username, rolename) Then
					Throw New ProviderException("User is not in role.")
				End If
			Next
		Next
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)

		For Each username As String In usernames
			For Each rolename As String In rolenames
				Dim cmd As SQLiteCommand = New SQLiteCommand("DELETE FROM UsersInRoles WHERE UserName=$UserName AND RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
				cmd.Parameters.AddWithValue("$UserName", username)
				cmd.Parameters.AddWithValue("$RoleName", rolename)
				cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)
				cmd.ExecuteNonQuery()
				cmd.Dispose()
			Next
		Next

		CloseBoardBase(False)
	End Sub

	Public Overrides Function RoleExists(ByVal rolename As String) As Boolean
		Dim exists As Boolean = False
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM Roles WHERE RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$RoleName", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim numRecs As Integer = CType(cmd.ExecuteScalar(), Integer)

		If numRecs > 0 Then
			exists = True
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return exists
	End Function

	Public Overrides Function FindUsersInRole(ByVal rolename As String, _
 ByVal usernameToMatch As String) As String()
		Dim colUsers As List(Of String) = New List(Of String)
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim cmd As SQLiteCommand = New SQLiteCommand("SELECT Username FROM UsersInRoles WHERE UserName LIKE $UserName AND RoleName=$RoleName AND ApplicationName=$ApplicationName", conn)
		cmd.Parameters.AddWithValue("$UserName", usernameToMatch)
		cmd.Parameters.AddWithValue("$RoleName", rolename)
		cmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim reader As SQLiteDataReader = Nothing

		reader = cmd.ExecuteReader()

		Do While reader.Read()
			colUsers.Add(reader.GetString(0))
		Loop
		If Not reader Is Nothing Then
			reader.Close()
		End If
		cmd.Dispose()
		CloseBoardBase(True)

		Return colUsers.ToArray
	End Function

End Class
