Imports System.Web.Security
Imports System.Configuration.Provider
Imports System.Collections.Specialized
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Diagnostics
Imports System.Web
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Configuration
Imports System.Threading

Public Class SQLiteMembershipProvider
	Inherits MembershipProvider

	Private newPasswordLength As Integer = 8
	Private machineKey As MachineKeySection
	Private m_ConnectionString As String

	Public Overrides Sub Initialize(ByVal name As String, ByVal config As NameValueCollection)

		If config Is Nothing Then _
		  Throw New ArgumentNullException("config")

		If name Is Nothing OrElse name.Length = 0 Then _
		  name = "SqliteMembershipProvider"

		If String.IsNullOrEmpty(config("description")) Then
			config.Remove("description")
			config.Add("description", "Simple Sqlite Membership provider")
		End If

		' Initialize the abstract base class.
		MyBase.Initialize(name, config)


		pApplicationName = GetConfigValue(config("applicationName"), System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath)
		pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config("maxInvalidPasswordAttempts"), "5"))
		pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config("passwordAttemptWindow"), "10"))
		pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config("minRequiredAlphaNumericCharacters"), "1"))
		pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config("minRequiredPasswordLength"), "7"))
		pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config("passwordStrengthRegularExpression"), ""))
		pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config("enablePasswordReset"), "True"))
		pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config("enablePasswordRetrieval"), "True"))
		pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config("requiresQuestionAndAnswer"), "False"))
		pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config("requiresUniqueEmail"), "True"))

		Dim temp_format As String = config("passwordFormat")
		If String.IsNullOrEmpty(temp_format) Then
			temp_format = "Clear"
		End If

		Select Case temp_format
			Case "Hashed"
				pPasswordFormat = MembershipPasswordFormat.Hashed
			Case "Encrypted"
				pPasswordFormat = MembershipPasswordFormat.Encrypted
			Case "Clear"
				pPasswordFormat = MembershipPasswordFormat.Clear
			Case Else
				Throw New ProviderException("Password format not supported.")
		End Select

		' Get encryption and decryption key information from the configuration.
		Dim cfg As System.Configuration.Configuration = _
		  WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath)
		machineKey = CType(cfg.GetSection("system.web/machineKey"), MachineKeySection)

		If machineKey.ValidationKey.Contains("AutoGenerate") Then
			If PasswordFormat <> MembershipPasswordFormat.Clear Then
				Throw New ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.")
			End If
		End If

		' Строка подключения
		Dim objConnectionStringSettings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(config("connectionStringName"))
		If objConnectionStringSettings Is Nothing OrElse String.IsNullOrEmpty(objConnectionStringSettings.ConnectionString) Then
			Throw New ProviderException("Connection string is empty for SQLiteMembershipProvider. Check the web configuration file (web.config).")
		End If

		m_ConnectionString = objConnectionStringSettings.ConnectionString

	End Sub

	' A helper function to retrieve config values from the configuration file.
	Private Function GetConfigValue(ByVal configValue As String, ByVal defaultValue As String) As String
		If String.IsNullOrEmpty(configValue) Then
			Return defaultValue
		Else
			Return configValue
		End If
	End Function

#Region "Свойства"

	Private pApplicationName As String
	Private pEnablePasswordReset As Boolean
	Private pEnablePasswordRetrieval As Boolean
	Private pRequiresQuestionAndAnswer As Boolean
	Private pRequiresUniqueEmail As Boolean
	Private pMaxInvalidPasswordAttempts As Integer
	Private pPasswordAttemptWindow As Integer
	Private pPasswordFormat As MembershipPasswordFormat
	Private pMinRequiredNonAlphanumericCharacters As Integer
	Private pMinRequiredPasswordLength As Integer
	Private pPasswordStrengthRegularExpression As String

	Public Overrides Property ApplicationName() As String
		Get
			Return pApplicationName
		End Get
		Set(ByVal value As String)
			pApplicationName = value
		End Set
	End Property

	Public Overrides ReadOnly Property EnablePasswordReset() As Boolean
		Get
			Return pEnablePasswordReset
		End Get
	End Property

	Public Overrides ReadOnly Property EnablePasswordRetrieval() As Boolean
		Get
			Return pEnablePasswordRetrieval
		End Get
	End Property

	Public Overrides ReadOnly Property RequiresQuestionAndAnswer() As Boolean
		Get
			Return pRequiresQuestionAndAnswer
		End Get
	End Property

	Public Overrides ReadOnly Property RequiresUniqueEmail() As Boolean
		Get
			Return pRequiresUniqueEmail
		End Get
	End Property

	Public Overrides ReadOnly Property MaxInvalidPasswordAttempts() As Integer
		Get
			Return pMaxInvalidPasswordAttempts
		End Get
	End Property

	Public Overrides ReadOnly Property PasswordAttemptWindow() As Integer
		Get
			Return pPasswordAttemptWindow
		End Get
	End Property

	Public Overrides ReadOnly Property PasswordFormat() As MembershipPasswordFormat
		Get
			Return pPasswordFormat
		End Get
	End Property

	Public Overrides ReadOnly Property MinRequiredNonAlphanumericCharacters() As Integer
		Get
			Return pMinRequiredNonAlphanumericCharacters
		End Get
	End Property

	Public Overrides ReadOnly Property MinRequiredPasswordLength() As Integer
		Get
			Return pMinRequiredPasswordLength
		End Get
	End Property

	Public Overrides ReadOnly Property PasswordStrengthRegularExpression() As String
		Get
			Return pPasswordStrengthRegularExpression
		End Get
	End Property

#End Region

	Public Overrides Function ChangePassword(ByVal username As String, _
	  ByVal oldPwd As String, _
	  ByVal newPwd As String) As Boolean
		If Not ValidateUser(username, oldPwd) Then
			Return False
		End If

		Dim args As ValidatePasswordEventArgs = New ValidatePasswordEventArgs(username, newPwd, True)

		OnValidatingPassword(args)

		If args.Cancel Then
			If Not args.FailureInformation Is Nothing Then
				Throw args.FailureInformation
			Else
				Throw New ProviderException("Change password canceled due to New password validation failure.")
			End If
		End If

		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE Users SET Password=$Password, LastPasswordChangedDate=$LastPasswordChangedDate WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$Password", EncodePassword(newPwd))
		objCommand.Parameters.AddWithValue("$LastPasswordChangedDate", DateTime.Now.Ticks)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim rowsAffected As Integer = 0

		rowsAffected = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)

		If rowsAffected > 0 Then
			Return True
		End If
		Return False
	End Function

	Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal username As String, _
	  ByVal password As String, _
	  ByVal newPwdQuestion As String, _
	  ByVal newPwdAnswer As String) As Boolean
		If Not ValidateUser(username, password) Then
			Return False
		End If
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE Users SET PasswordQuestion=$PasswordQuestion, PasswordAnswer=$PasswordAnswer WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$PasswordQuestion", newPwdQuestion)
		objCommand.Parameters.AddWithValue("$PasswordAnswer", EncodePassword(newPwdAnswer))
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)
		Dim rowsAffected As Integer

		rowsAffected = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)

		If rowsAffected > 0 Then
			Return True
		End If
		Return False
	End Function

	Public Overrides Function CreateUser(ByVal UserName As String, _
	ByVal password As String, _
	ByVal email As String, _
	ByVal passwordQuestion As String, _
	ByVal passwordAnswer As String, _
	ByVal isApproved As Boolean, _
	ByVal providerUserKey As Object, _
	ByRef status As MembershipCreateStatus) As MembershipUser

		Dim Args As ValidatePasswordEventArgs = New ValidatePasswordEventArgs(UserName, password, True)

		OnValidatingPassword(Args)

		If Args.Cancel Then
			status = MembershipCreateStatus.InvalidPassword
			Return Nothing
		End If


		If RequiresUniqueEmail AndAlso GetUserNameByEmail(email) <> "" Then
			status = MembershipCreateStatus.DuplicateEmail
			Return Nothing
		End If

		Dim u As MembershipUser = GetUser(UserName, False)

		If u Is Nothing Then
			Dim createDate As DateTime = DateTime.Now

			If providerUserKey Is Nothing Then
				providerUserKey = Guid.NewGuid()
			Else
				If Not TypeOf providerUserKey Is Guid Then
					status = MembershipCreateStatus.InvalidProviderUserKey
					Return Nothing
				End If
			End If

			Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
			Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO Users " & _
			 " (PKID, UserName, Password, Email, PasswordQuestion, " & _
			 " PasswordAnswer, IsApproved," & _
			 " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," & _
			 " ApplicationName, Enabled, LastLockedOutDate," & _
			 " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " & _
			 " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" & _
			 " Values($PKID, $UserName, $Password, $Email, $PasswordQuestion," & _
			 " $PasswordAnswer, $IsApproved, " & _
			 " $Comment, $CreationDate, $CreationDate, $CreationDate, " & _
			 " $ApplicationName, 1, $CreationDate, 0, $CreationDate, 0, $CreationDate)", conn)
			objCommand.Parameters.AddWithValue("$PKID", providerUserKey.ToString)
			objCommand.Parameters.AddWithValue("$UserName", UserName)
			objCommand.Parameters.AddWithValue("$Password", EncodePassword(password))
			objCommand.Parameters.AddWithValue("$Email", email)
			objCommand.Parameters.AddWithValue("$PasswordQuestion", passwordQuestion)
			objCommand.Parameters.AddWithValue("$PasswordAnswer", EncodePassword(passwordAnswer))
			objCommand.Parameters.AddWithValue("$IsApproved", isApproved)
			objCommand.Parameters.AddWithValue("$Comment", String.Empty)
			objCommand.Parameters.AddWithValue("$CreationDate", createDate.Ticks)
			objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

			Dim recAdded As Integer = objCommand.ExecuteNonQuery()

			If recAdded > 0 Then
				status = MembershipCreateStatus.Success
			Else
				status = MembershipCreateStatus.UserRejected
			End If
			objCommand.Dispose()
			CloseBoardBase(False)

			Return GetUser(UserName, False)
		Else
			status = MembershipCreateStatus.DuplicateUserName
		End If

		Return Nothing
	End Function

	Public Overrides Function DeleteUser(ByVal username As String, _
	 ByVal deleteAllRelatedData As Boolean) As Boolean
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM Users WHERE UserName=$UserName AND Applicationname=$Applicationname", conn)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$Applicationname", pApplicationName)

		Dim rowsAffected As Integer = objCommand.ExecuteNonQuery()

		If deleteAllRelatedData Then
			' Process commands to delete all data for the user in the database.
		End If
		objCommand.Dispose()
		CloseBoardBase(False)

		If rowsAffected > 0 Then
			Return True
		End If
		Return False
	End Function

	Public Overrides Function GetAllUsers(ByVal pageIndex As Integer, _
	ByVal pageSize As Integer, _
	  ByRef totalRecords As Integer) _
	  As MembershipUserCollection
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM Users WHERE ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)
		Dim users As MembershipUserCollection = New MembershipUserCollection()

		Dim reader As SQLiteDataReader = Nothing

		totalRecords = CInt(objCommand.ExecuteScalar())

		If totalRecords > 0 Then

			objCommand = New SQLiteCommand("SELECT PKID, UserName, Email, PasswordQuestion," & _
			" Comment, IsApproved, Enabled, CreationDate, LastLoginDate," & _
			" LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
			" FROM Users WHERE ApplicationName=$ApplicationName ", conn)
			objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

			reader = objCommand.ExecuteReader()

			Dim counter As Integer = 0
			Dim startIndex As Integer = pageSize * pageIndex
			Dim endIndex As Integer = startIndex + pageSize - 1

			Do While reader.Read()
				If counter >= startIndex Then
					Dim u As MembershipUser = GetUserFromReader(reader)
					users.Add(u)
				End If

				If counter >= endIndex Then objCommand.Cancel()

				counter += 1
			Loop
		End If

		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		Return users
	End Function

	Public Overrides Function GetNumberOfUsersOnline() As Integer
		Dim onlineSpan As TimeSpan = New TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0)
		Dim compareTime As DateTime = DateTime.Now.Subtract(onlineSpan)

		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM Users WHERE LastActivityDate > $LastActivityDate AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$LastActivityDate", compareTime.Ticks)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim numOnline As Integer = 0

		numOnline = CInt(objCommand.ExecuteScalar())
		objCommand.Dispose()
		CloseBoardBase(True)

		Return numOnline
	End Function

	Public Overrides Function GetPassword(ByVal username As String, ByVal answer As String) As String

		If Not EnablePasswordRetrieval Then
			Throw New ProviderException("Password Retrieval Not Enabled.")
		End If

		If PasswordFormat = MembershipPasswordFormat.Hashed Then
			Throw New ProviderException("Cannot retrieve Hashed passwords.")
		End If
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT Password, PasswordAnswer, Enabled FROM Users WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim password As String = String.Empty
		Dim passwordAnswer As String = String.Empty
		Dim reader As SQLiteDataReader = Nothing

		reader = objCommand.ExecuteReader(CommandBehavior.SingleRow)

		If reader.HasRows Then
			reader.Read()

			If reader.GetBoolean(2) Then
				Throw New MembershipPasswordException("The supplied user is locked out.")
			End If

			password = reader.GetString(0)
			passwordAnswer = reader.GetString(1)
		Else
			Throw New MembershipPasswordException("The supplied user name is not found.")
		End If
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		If RequiresQuestionAndAnswer AndAlso Not CheckPassword(answer, passwordAnswer) Then
			UpdateFailureCount(username, "passwordAnswer")

			Throw New MembershipPasswordException("Incorrect password answer.")
		End If


		If PasswordFormat = MembershipPasswordFormat.Encrypted Then
			password = UnEncodePassword(password)
		End If

		Return password
	End Function

	Public Overrides Function GetUser(ByVal username As String, ByVal userIsOnline As Boolean) As MembershipUser
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT PKID, UserName, Email, PasswordQuestion," & _
		" Comment, IsApproved, Enabled, CreationDate, LastLoginDate," & _
		" LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" & _
		" FROM Users  WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim u As MembershipUser = Nothing
		Dim reader As SQLiteDataReader = Nothing

		reader = objCommand.ExecuteReader()

		If reader.HasRows Then
			reader.Read()
			u = GetUserFromReader(reader)

			If userIsOnline Then
				Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE Users SET LastActivityDate=$LastActivityDate " & _
				 "WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
				updateCmd.Parameters.AddWithValue("$LastActivityDate", DateTime.Now.Ticks)
				updateCmd.Parameters.AddWithValue("$UserName", username)
				updateCmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

				Dim objLock As LockCookie = UpgradeToWriterLock()
				updateCmd.ExecuteNonQuery()
				updateCmd.Dispose()
				DowngradeFromWriterLock(objLock)
			End If
		End If
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		Return u
	End Function

	Public Overrides Function GetUser(ByVal providerUserKey As Object, ByVal userIsOnline As Boolean) As MembershipUser
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," & _
		   " Comment, IsApproved, Enabled, CreationDate, LastLoginDate," & _
		   " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" & _
		   " FROM Users  WHERE PKID=$PKID", conn)
		objCommand.Parameters.AddWithValue("$PKID", providerUserKey)

		Dim u As MembershipUser = Nothing
		Dim reader As SQLiteDataReader = Nothing

		reader = objCommand.ExecuteReader()

		If reader.HasRows Then
			reader.Read()
			u = GetUserFromReader(reader)

			If userIsOnline Then
				Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE Users SET LastActivityDate=$LastActivityDate WHERE PKID=$PKID", conn)
				updateCmd.Parameters.AddWithValue("$LastActivityDate", DateTime.Now.Ticks)
				updateCmd.Parameters.AddWithValue("$PKID", providerUserKey)

				Dim objLock As LockCookie = UpgradeToWriterLock()
				updateCmd.ExecuteNonQuery()
				updateCmd.Dispose()
				DowngradeFromWriterLock(objLock)
			End If
		End If
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		Return u
	End Function

	Private Function GetUserFromReader(ByVal reader As SQLiteDataReader) As MembershipUser
		'"SELECT ""PKID"", ""UserName"", ""Email"", ""PasswordQuestion""," & _
		'	   " ""Comment"", ""IsApproved"", ""Enabled"", ""CreationDate"", ""LastLoginDate""," & _
		'	   " ""LastActivityDate"", ""LastPasswordChangedDate"", ""LastLockedOutDate"" " & _
		'	   " FROM ""Users""  " & _
		'	   " WHERE ""ApplicationName"" = ""{0}"" "
		Dim providerUserKey As Object = reader.GetValue(0)
		Dim username As String = reader.GetString(1)
		Dim email As String = reader.GetString(2)

		Dim passwordQuestion As String = ""
		If Not reader.GetValue(3) Is DBNull.Value Then
			passwordQuestion = reader.GetString(3)
		End If
		Dim comment As String = ""
		If Not reader.GetValue(4) Is DBNull.Value Then
			comment = reader.GetString(4)
		End If
		Dim isApproved As Boolean = reader.GetBoolean(5)
		Dim Enabled As Boolean = reader.GetBoolean(6)
		Dim creationDate As DateTime = Date.FromBinary(reader.GetInt64(7))

		Dim lastLoginDate As DateTime = New DateTime()
		If Not reader.GetValue(8) Is DBNull.Value Then
			lastLoginDate = Date.FromBinary(reader.GetInt64(8))
		End If
		Dim lastActivityDate As DateTime = Date.FromBinary(reader.GetInt64(9))
		Dim lastPasswordChangedDate As DateTime = Date.FromBinary(reader.GetInt64(10))

		Dim lastLockedOutDate As DateTime = New DateTime()
		If Not reader.GetValue(11) Is DBNull.Value Then _
		  lastLockedOutDate = Date.FromBinary(reader.GetInt64(11))

		Dim u As MembershipUser = New MembershipUser(Me.Name, username, providerUserKey, email, passwordQuestion, comment, isApproved, Enabled, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate)

		Return u
	End Function

	Public Overrides Function UnlockUser(ByVal username As String) As Boolean
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE Users SET Enabled=1, LastLockedOutDate=$LastLockedOutDate WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$LastLockedOutDate", DateTime.Now.Ticks)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim rowsAffected As Integer

		rowsAffected = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)

		If rowsAffected > 0 Then
			Return True
		End If
		Return False
	End Function

	Public Overrides Function GetUserNameByEmail(ByVal email As String) As String
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT UserName FROM Users WHERE Email=$Email AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$Email", email)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		objReader.Read()
		If objReader.HasRows Then
			GetUserNameByEmail = objReader.GetString(0)
		Else
			GetUserNameByEmail = String.Empty
		End If
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)

	End Function

	Public Overrides Function ResetPassword(ByVal username As String, _
	 ByVal answer As String) As String

		If Not EnablePasswordReset Then
			Throw New NotSupportedException("Password Reset is not enabled.")
		End If

		If answer Is Nothing AndAlso RequiresQuestionAndAnswer Then
			UpdateFailureCount(username, "passwordAnswer")

			Throw New ProviderException("Password answer required for password Reset.")
		End If

		Dim newPassword As String = _
		  System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters)


		Dim Args As ValidatePasswordEventArgs = _
		  New ValidatePasswordEventArgs(username, newPassword, True)

		OnValidatingPassword(Args)

		If Args.Cancel Then
			If Not Args.FailureInformation Is Nothing Then
				Throw Args.FailureInformation
			Else
				Throw New MembershipPasswordException("Reset password canceled due to password validation failure.")
			End If
		End If

		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT PasswordAnswer, Enabled FROM Users WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim passwordAnswer As String = String.Empty
		Dim reader As SQLiteDataReader = Nothing

		reader = objCommand.ExecuteReader(CommandBehavior.SingleRow)

		If reader.HasRows Then
			reader.Read()

			If reader.GetBoolean(1) Then _
			  Throw New MembershipPasswordException("The supplied user is locked out.")

			passwordAnswer = reader.GetString(0)
		Else
			Throw New MembershipPasswordException("The supplied user name is not found.")
		End If

		If RequiresQuestionAndAnswer AndAlso Not CheckPassword(answer, passwordAnswer) Then
			UpdateFailureCount(username, "passwordAnswer")

			Throw New MembershipPasswordException("Incorrect password answer.")
		End If

		Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE Users SET Password=$Password, LastPasswordChangedDate=$LastPasswordChangedDate WHERE UserName=$UserName AND ApplicationName=$ApplicationName AND Enabled=1", conn)
		updateCmd.Parameters.AddWithValue("$Password", EncodePassword(newPassword))
		updateCmd.Parameters.AddWithValue("$LastPasswordChangedDate", DateTime.Now.Ticks)
		updateCmd.Parameters.AddWithValue("$UserName", username)
		updateCmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim objLock As LockCookie = UpgradeToWriterLock()
		Dim rowsAffected As Integer = updateCmd.ExecuteNonQuery()
		updateCmd.Dispose()
		DowngradeFromWriterLock(objLock)
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		If rowsAffected > 0 Then
			Return newPassword
		Else
			Throw New MembershipPasswordException("User not found, or user is locked out. Password not Reset.")
		End If
	End Function

	Public Overrides Sub UpdateUser(ByVal user As MembershipUser)
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE Users SET Email=$Email, Comment=$Comment, IsApproved=$IsApproved WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$Email", user.Email)
		objCommand.Parameters.AddWithValue("$Comment", user.Comment)
		objCommand.Parameters.AddWithValue("$IsApproved", user.IsApproved)
		objCommand.Parameters.AddWithValue("$UserName", user.UserName)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
	End Sub

	Public Overrides Function ValidateUser(ByVal username As String, _
	ByVal password As String) As Boolean
		Dim isValid As Boolean = False
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT Password, IsApproved FROM Users WHERE UserName=$UserName AND ApplicationName=$ApplicationName AND Enabled=1", conn)
		objCommand.Parameters.AddWithValue("$UserName", username)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim reader As SQLiteDataReader = Nothing
		Dim isApproved As Boolean = False
		Dim pwd As String = String.Empty

		reader = objCommand.ExecuteReader()

		If reader.HasRows Then
			reader.Read()
			pwd = reader.GetString(0)
			isApproved = reader.GetBoolean(1)
		End If

		If CheckPassword(password, pwd) Then
			If isApproved Then
				isValid = True

				Dim updateCmd As SQLiteCommand = New SQLiteCommand("UPDATE Users SET LastLoginDate=$LastLoginDate WHERE UserName=$UserName AND ApplicationName=$ApplicationName", conn)
				updateCmd.Parameters.AddWithValue("$LastLoginDate", DateTime.Now.Ticks)
				updateCmd.Parameters.AddWithValue("$UserName", username)
				updateCmd.Parameters.AddWithValue("$ApplicationName", pApplicationName)

				Dim objLock As LockCookie = UpgradeToWriterLock()
				updateCmd.ExecuteNonQuery()
				updateCmd.Dispose()
				DowngradeFromWriterLock(objLock)
			End If
		Else
			UpdateFailureCount(username, "password")
		End If
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		Return isValid
	End Function

	Private Sub UpdateFailureCount(ByVal username As String, ByVal failureType As String)

		'Dim conn As OdbcConnection = New OdbcConnection(connectionString)
		'Dim cmd As OdbcCommand = New OdbcCommand("SELECT FailedPasswordAttemptCount, " & _
		' "  FailedPasswordAttemptWindowStart, " & _
		' "  FailedPasswordAnswerAttemptCount, " & _
		' "  FailedPasswordAnswerAttemptWindowStart " & _
		' "  FROM Users  " & _
		' "  WHERE Username = ? AND ApplicationName = ?", conn)

		'cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username
		'cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName

		'Dim reader As OdbcDataReader = Nothing
		'Dim windowStart As DateTime = New DateTime()
		'Dim failureCount As Integer = 0

		'	conn.Open()

		'	reader = cmd.ExecuteReader(CommandBehavior.SingleRow)

		'	If reader.HasRows Then
		'		reader.Read()

		'		If failureType = "password" Then
		'			failureCount = reader.GetInt32(0)
		'			windowStart = reader.GetDateTime(1)
		'		End If

		'		If failureType = "passwordAnswer" Then
		'			failureCount = reader.GetInt32(2)
		'			windowStart = reader.GetDateTime(3)
		'		End If
		'	End If

		'	reader.Close()

		'	Dim windowEnd As DateTime = windowStart.AddMinutes(PasswordAttemptWindow)

		'	If failureCount = 0 OrElse DateTime.Now > windowEnd Then
		'		' First password failure or outside of PasswordAttemptWindow. 
		'		' Start a New password failure count from 1 and a New window starting now.

		'		If failureType = "password" Then _
		'		  cmd.CommandText = "UPDATE Users  " & _
		'		  "  SET FailedPasswordAttemptCount = ?, " & _
		'		  "      FailedPasswordAttemptWindowStart = ? " & _
		'		  "  WHERE Username = ? AND ApplicationName = ?"

		'		If failureType = "passwordAnswer" Then _
		'		  cmd.CommandText = "UPDATE Users  " & _
		'		  "  SET FailedPasswordAnswerAttemptCount = ?, " & _
		'		  "      FailedPasswordAnswerAttemptWindowStart = ? " & _
		'		  "  WHERE Username = ? AND ApplicationName = ?"

		'		cmd.Parameters.Clear()

		'		cmd.Parameters.Add("@Count", OdbcType.Int).Value = 1
		'		cmd.Parameters.Add("@WindowStart", OdbcType.DateTime).Value = DateTime.Now
		'		cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username
		'		cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName

		'		If cmd.ExecuteNonQuery() < 0 Then _
		'		  Throw New ProviderException("Unable to update failure count and window start.")
		'	Else
		'		failureCount += 1

		'		If failureCount >= MaxInvalidPasswordAttempts Then
		'			' Password attempts have exceeded the failure threshold. Lock out
		'			' the user.

		'			cmd.CommandText = "UPDATE Users  " & _
		'			   "  SET Enabled = ?, LastLockedOutDate = ? " & _
		'			   "  WHERE Username = ? AND ApplicationName = ?"

		'			cmd.Parameters.Clear()

		'			cmd.Parameters.Add("@Enabled", OdbcType.Bit).Value = True
		'			cmd.Parameters.Add("@LastLockedOutDate", OdbcType.DateTime).Value = DateTime.Now
		'			cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username
		'			cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName

		'			If cmd.ExecuteNonQuery() < 0 Then _
		'			  Throw New ProviderException("Unable to lock out user.")
		'		Else
		'			' Password attempts have not exceeded the failure threshold. Update
		'			' the failure counts. Leave the window the same.

		'			If failureType = "password" Then _
		'			  cmd.CommandText = "UPDATE Users  " & _
		'			  "  SET FailedPasswordAttemptCount = ?" & _
		'			  "  WHERE Username = ? AND ApplicationName = ?"

		'			If failureType = "passwordAnswer" Then _
		'			  cmd.CommandText = "UPDATE Users  " & _
		'			  "  SET FailedPasswordAnswerAttemptCount = ?" & _
		'			  "  WHERE Username = ? AND ApplicationName = ?"

		'			cmd.Parameters.Clear()

		'			cmd.Parameters.Add("@Count", OdbcType.Int).Value = failureCount
		'			cmd.Parameters.Add("@Username", OdbcType.VarChar, 255).Value = username
		'			cmd.Parameters.Add("@ApplicationName", OdbcType.VarChar, 255).Value = pApplicationName

		'			If cmd.ExecuteNonQuery() < 0 Then _
		'			  Throw New ProviderException("Unable to update failure count.")
		'		End If
		'	End If
		'	If Not reader Is Nothing Then reader.Close()
		'	conn.Close()
	End Sub

	Private Function CheckPassword(ByVal password As String, ByVal dbpassword As String) As Boolean
		Dim pass1 As String = password
		Dim pass2 As String = dbpassword

		Select Case PasswordFormat
			Case MembershipPasswordFormat.Encrypted
				pass2 = UnEncodePassword(dbpassword)
			Case MembershipPasswordFormat.Hashed
				pass1 = EncodePassword(password)
			Case Else
		End Select

		If pass1 = pass2 Then
			Return True
		End If
		Return False
	End Function

	Private Function EncodePassword(ByVal password As String) As String
		Dim encodedPassword As String = password

		Select Case PasswordFormat
			Case MembershipPasswordFormat.Clear

			Case MembershipPasswordFormat.Encrypted
				encodedPassword = _
				  Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)))
			Case MembershipPasswordFormat.Hashed
				Dim hash As HMACSHA1 = New HMACSHA1()
				hash.Key = HexToByte(machineKey.ValidationKey)
				encodedPassword = _
				  Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)))
			Case Else
				Throw New ProviderException("Unsupported password format.")
		End Select

		Return encodedPassword
	End Function

	Private Function UnEncodePassword(ByVal encodedPassword As String) As String
		Dim password As String = encodedPassword

		Select Case PasswordFormat
			Case MembershipPasswordFormat.Clear

			Case MembershipPasswordFormat.Encrypted
				password = _
				  Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)))
			Case MembershipPasswordFormat.Hashed
				Throw New ProviderException("Cannot unencode a hashed password.")
			Case Else
				Throw New ProviderException("Unsupported password format.")
		End Select

		Return password
	End Function

	Private Function HexToByte(ByVal hexString As String) As Byte()
		Dim ReturnBytes((hexString.Length \ 2) - 1) As Byte
		For i As Integer = 0 To ReturnBytes.Length - 1
			ReturnBytes(i) = Convert.ToByte(hexString.Substring(i * 2, 2), 16)
		Next
		Return ReturnBytes
	End Function

	Public Overrides Function FindUsersByName(ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As MembershipUserCollection
		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT Count(*) FROM Users WHERE UserName LIKE $UserName AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$UserName", usernameToMatch)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		Dim users As MembershipUserCollection = New MembershipUserCollection()

		Dim reader As SQLiteDataReader = Nothing

		totalRecords = CInt(objCommand.ExecuteScalar())

		If totalRecords > 0 Then
			objCommand = New SQLiteCommand("SELECT PKID, UserName, Email, PasswordQuestion," & _
			  " Comment, IsApproved, Enabled, CreationDate, LastLoginDate," & _
			  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
			  " FROM Users  " & _
			  " WHERE UserName LIKE $UserName AND ApplicationName=$ApplicationName", conn)
			objCommand.Parameters.AddWithValue("$UserName", usernameToMatch)
			objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

			reader = objCommand.ExecuteReader()

			Dim counter As Integer = 0
			Dim startIndex As Integer = pageSize * pageIndex
			Dim endIndex As Integer = startIndex + pageSize - 1

			Do While reader.Read()
				If counter >= startIndex Then
					Dim u As MembershipUser = GetUserFromReader(reader)
					users.Add(u)
				End If

				If counter >= endIndex Then objCommand.Cancel()

				counter += 1
			Loop
		End If
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()
		CloseBoardBase(True)

		Return users
	End Function

	Public Overrides Function FindUsersByEmail(ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As MembershipUserCollection
		Dim users As MembershipUserCollection = New MembershipUserCollection()
		Dim reader As SQLiteDataReader = Nothing
		totalRecords = 0

		Dim conn As SQLiteConnection = OpenBoardBase(m_ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," & _
		" Comment, IsApproved, Enabled, CreationDate, LastLoginDate," & _
		" LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " & _
		" FROM Users WHERE Email LIKE $Email AND ApplicationName=$ApplicationName", conn)
		objCommand.Parameters.AddWithValue("$Email", emailToMatch)
		objCommand.Parameters.AddWithValue("$ApplicationName", pApplicationName)

		reader = objCommand.ExecuteReader()

		Dim counter As Integer
		Dim startIndex As Integer = pageSize * pageIndex
		Dim endIndex As Integer = startIndex + pageSize - 1

		Do While reader.Read()
			If counter >= startIndex Then
				Dim u As MembershipUser = GetUserFromReader(reader)
				users.Add(u)
			End If

			If counter >= endIndex Then
				objCommand.Cancel()
			End If
			counter += 1
			totalRecords += 1
		Loop
		If Not reader Is Nothing Then
			reader.Close()
		End If
		objCommand.Dispose()

		CloseBoardBase(True)

		Return users
	End Function

End Class
