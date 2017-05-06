Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Sub LockThreadBase()
		m_Connection = OpenBoardBase(ConnectionString, True)
	End Sub

	Public Overrides Sub UnLockThreadBase()
		CloseBoardBase(True)
	End Sub

	Public Overrides Function GetAnswerList(ByVal MessageNumber As Long) As List(Of AnswerInfo)
		GetAnswerList = New List(Of AnswerInfo)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM Answers WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber", m_Connection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			GetAnswerList.Add(ReadAnswer(objReader))
		Loop
		objReader.Close()
		objCommand.Dispose()
	End Function

	Public Overrides Function GetFileList(ByVal MessageNumber As Long) As List(Of ImageboardFileInfo)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM Files WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber", m_Connection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		GetFileList = New List(Of ImageboardFileInfo)
		Do While objReader.Read()
			GetFileList.Add(ReadFile(objReader))
		Loop
		objReader.Close()
		objCommand.Dispose()
	End Function

	Public Overrides Function GetMessageList(ByVal ThreadNumber As Long) As System.Collections.Generic.List(Of Long)
		GetMessageList = New List(Of Long)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT MessageNumber FROM Messages WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber", m_Connection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$ThreadNumber", ThreadNumber)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read()
			GetMessageList.Add(objReader.GetInt64(0))
		Loop
		objReader.Close()
		objCommand.Dispose()
	End Function

	Public Overrides Function GetMessagesCount(ByVal ThreadNumber As Long) As Long
		' Получить поличество сообщений
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM Messages WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber", m_Connection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$ThreadNumber", ThreadNumber)
		'objCommand.CommandText = String.Format("SELECT COUNT(*) FROM ""Messages"" WHERE ""ThreadNumber""={0} AND ""BoardName""=""{1}""", strOPThreadNumber, m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
		objReader.Read()
		GetMessagesCount = objReader.GetInt64(0)
		objReader.Close()
		objCommand.Dispose()
	End Function

	Public Overrides Function GetMessageListStartAt(ByVal ThreadNumber As Long, ByVal StartMessageNumber As Long) As System.Collections.Generic.List(Of Long)
		'' загружаем последние MaxMessagesPerPagePerThread сообщений 
		'Dim objCommand As DbCommand = objConnection.CreateCommand
		'objCommand.CommandText = String.Format("SELECT * FROM ""Messages"" WHERE ""ThreadNumber""={0} AND ""BoardName""=""{1}"" ORDER BY ""MessageNumber"" ASC LIMIT -1 OFFSET 1", strOPThreadNumber, m_BoardName)
		'Dim objReader As IDataReader = objCommand.ExecuteReader
		Return New List(Of Long)
	End Function

	Public Overrides Function GetMessageListInPage(ByVal ThreadNumber As Long) As List(Of Long)
		GetMessageListInPage = New List(Of Long)
		' '' загружаем последние MaxMessagesPerPagePerThread сообщений 
		'objCommand.CommandText = String.Format("SELECT * FROM ""Messages"" WHERE ""ThreadNumber""={0} AND ""BoardName""=""{1}"" ORDER BY ""MessageNumber"" ASC LIMIT {2} OFFSET {3}", strOPThreadNumber, m_BoardName, m_MaxMessagesPerPagePerThread, intMessagesCount - m_MaxMessagesPerPagePerThread + 1)
		'				Else
		'objCommand.CommandText = String.Format("SELECT * FROM ""Messages"" WHERE ""ThreadNumber""={0} AND ""BoardName""=""{1}"" ORDER BY ""MessageNumber"" ASC LIMIT {2} OFFSET 1", strOPThreadNumber, m_BoardName, intMessagesCount)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT MessageNumber FROM Messages WHERE ThreadNumber=$ThreadNumber AND BoardName=$BoardName ORDER BY MessageNumber ASC LIMIT (SELECT MaxMessagesPerPagePerThread FROM Boards WHERE BoardName=$BoardName) OFFSET ((SELECT COUNT(*) FROM Messages WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber) - (SELECT MaxMessagesPerPagePerThread FROM Boards WHERE BoardName=$BoardName) + 1)", m_Connection)
		objCommand.Parameters.AddWithValue("$ThreadNumber", ThreadNumber)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objreader As SQLiteDataReader = objCommand.ExecuteReader
		Do While objreader.Read
			GetMessageListInPage.Add(objreader.GetInt64(0))
		Loop
	End Function

	Public Overrides Function GetPagesCount(ByVal ArchiveThread As Boolean) As Long
		Dim objConnection As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		Dim MaxThreadsPerPage As Long = GetMaxThreadsPerPage(objConnection)
		' Получаем номера тредов, запрещённых на чтение
		Dim objCommand As SQLiteCommand = New SQLiteCommand( _
		  "SELECT COUNT(*) FROM Threads WHERE BoardName=$BoardName AND Archive=$Archive AND ThreadNumber NOT IN" & _
		 "(SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND DenyForReading=1 AND RoleName IN" & _
		  "(SELECT RoleName FROM Roles WHERE RoleName IN(SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)))", objConnection)
		objCommand.Parameters.AddWithValue("$Archive", Convert.ToInt64(ArchiveThread))
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$UserName", m_UserName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
		objReader.Read()
		Dim intThreadsCount As Long = objReader.GetInt64(0)
		If intThreadsCount Mod MaxThreadsPerPage = 0 Then
			GetPagesCount = intThreadsCount \ MaxThreadsPerPage - 1
		Else
			GetPagesCount = intThreadsCount \ MaxThreadsPerPage
		End If
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

	Public Overrides Function GetThreadList(ByVal PageNumber As Long, ByVal ArchiveThread As Boolean) As List(Of ThreadInfo)
		GetThreadList = New List(Of ThreadInfo)
		Dim Access As AnonymousAccess
		If String.IsNullOrEmpty(m_UserName) Then
			Access = AnonymousAccess.OnlyForRead
		Else
			Access = AnonymousAccess.Hidden
		End If
		Dim objCommand As SQLiteCommand
		If PageNumber < 0 Then
			' Получаем все треды
			objCommand = New SQLiteCommand("SELECT * FROM Threads WHERE BoardName=$BoardName AND Archive=$Archive AND AnonymousAnswer <= $AnonymousAnswer AND NOT ThreadNumber IN (SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND DenyForReading=1 AND RoleName IN(SELECT RoleName FROM Roles WHERE RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName))) ORDER BY IsTop DESC, LastMessageNumber DESC", m_Connection)
			objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
			objCommand.Parameters.AddWithValue("$Archive", Convert.ToInt64(ArchiveThread))
			objCommand.Parameters.AddWithValue("$AnonymousAnswer", Convert.ToInt64(Access))
			objCommand.Parameters.AddWithValue("$UserName", m_UserName)
			'		objCommand.CommandText = String.Format())) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"";END TRANSACTION;", m_BoardName, m_RealUserName)
		Else
			Dim MaxThreadsPerPage As Long = GetMaxThreadsPerPage(m_Connection)
			' Получаем треды на конкретной странице
			objCommand = New SQLiteCommand("SELECT * FROM Threads WHERE BoardName=$BoardName AND Archive=$Archive AND AnonymousAnswer <= $AnonymousAnswer AND NOT ThreadNumber IN (SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND DenyForReading=1 AND RoleName IN(SELECT RoleName FROM Roles WHERE RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName))) ORDER BY IsTop DESC, LastMessageNumber DESC LIMIT $MaxThreadsPerPage OFFSET $PageNumber * $MaxThreadsPerPage", m_Connection)
			objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
			objCommand.Parameters.AddWithValue("$Archive", Convert.ToInt64(ArchiveThread))
			objCommand.Parameters.AddWithValue("$AnonymousAnswer", Convert.ToInt64(Access))
			objCommand.Parameters.AddWithValue("$UserName", m_UserName)
			objCommand.Parameters.AddWithValue("$PageNumber", PageNumber)
			objCommand.Parameters.AddWithValue("$MaxThreadsPerPage", MaxThreadsPerPage)
			', m_BoardName, m_RealUserName, m_MaxThreadsPerPage, m_PageNumber * m_MaxThreadsPerPage)
		End If
		'Else
		'	If PageNumber < 0 Then
		'		objCommand.CommandText = String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=0 AND ""AnonymousAnswer"" < 2 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"" DESC;END TRANSACTION;", m_BoardName, m_RealUserName)
		'	Else
		'		objCommand.CommandText = String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=0 AND ""AnonymousAnswer"" < 2 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"" DESC LIMIT {2} OFFSET {3};END TRANSACTION;", m_BoardName, m_RealUserName, m_MaxThreadsPerPage, m_PageNumber * m_MaxThreadsPerPage)
		'	End If
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
		Do While objReader.Read
			GetThreadList.Add(ReadThread(objReader))
		Loop
		objReader.Close()
		objCommand.Dispose()
	End Function

	Private Function GetMaxThreadsPerPage(ByVal objConnection As SQLiteConnection) As Long
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT MaxThreadsPerPage FROM Boards WHERE BoardName=$BoardName", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		If objReader.HasRows Then
			objReader.Read()
			GetMaxThreadsPerPage = objReader.GetInt64(0)
		Else
			GetMaxThreadsPerPage = 0
		End If
		objReader.Close()
		objCommand.Dispose()
	End Function

	Private Function GetMaxMessagesPerPagePerThread(ByVal objConnection As SQLiteConnection) As Long
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT MaxMessagesPerPagePerThread FROM Boards WHERE BoardName=$BoardName", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		If objReader.HasRows Then
			objReader.Read()
			GetMaxMessagesPerPagePerThread = objReader.GetInt64(0)
		Else
			GetMaxMessagesPerPagePerThread = 0
		End If
		objReader.Close()
		objCommand.Dispose()
	End Function

	Public Overrides Function GetThreadNumber(ByVal MessageNumber As Long, ByVal DenyForWriting As Boolean, ByVal Archive As Boolean) As Long
		Return GetThreadNumber(m_Connection, MessageNumber, DenyForWriting, Archive)
	End Function

	Private Overloads Function GetThreadNumber(ByVal objConnection As SQLiteConnection, ByVal MessageNumber As Long, ByVal DenyForWriting As Boolean, ByVal Archive As Boolean) As Long
		Dim objCommand As SQLiteCommand
		If Archive Then
			objCommand = New SQLiteCommand("SELECT ThreadNumber FROM Threads WHERE BoardName=$BoardName AND ThreadNumber=(SELECT ThreadNumber FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber) AND Archive=1 AND NOT ThreadNumber IN (SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND DenyForReading=1 AND RoleName IN (SELECT RoleName FROM UsersInRoles WHERE RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)))", objConnection)
		Else
			If DenyForWriting Then
				objCommand = New SQLiteCommand("SELECT ThreadNumber FROM Threads WHERE BoardName=$BoardName AND ThreadNumber=(SELECT ThreadNumber FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber) AND Archive=0 AND NOT ThreadNumber IN (SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND RoleName IN (SELECT RoleName FROM UsersInRoles WHERE RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)))", objConnection)
			Else
				objCommand = New SQLiteCommand("SELECT ThreadNumber FROM Threads WHERE BoardName=$BoardName AND ThreadNumber=(SELECT ThreadNumber FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber) AND Archive=0 AND NOT ThreadNumber IN (SELECT ThreadNumber FROM DenyForThreadAnswers WHERE BoardName=$BoardName AND DenyForReading=1 AND RoleName IN (SELECT RoleName FROM UsersInRoles WHERE RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)))", objConnection)
			End If
		End If
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
		objCommand.Parameters.AddWithValue("$UserName", m_UserName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
		If objReader.HasRows Then
			objReader.Read()
			GetThreadNumber = objReader.GetInt64(0)
		Else
			GetThreadNumber = 0
		End If
		objReader.Close()
		objCommand.Dispose()
	End Function

	''' <summary>
	''' Возвращает сообщение
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Overrides Function GetMessage(ByVal MessageNumber As Long) As MessageInfo
		' UNDONE Проверить права пользователя: анонимный доступ и доступ по ролям
		' Получить сообщение пользователя
		Dim objCommand As SQLiteCommand = New SQLiteCommand("BEGIN TRANSACTION;SELECT * FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber; SELECT * FROM Files WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber; SELECT * FROM Answers WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber; END TRANSACTION", m_Connection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
		objReader.Read()
		GetMessage = ReadMessage(objReader)
		objReader.NextResult()
		Dim colFiles As List(Of ImageboardFileInfo) = New List(Of ImageboardFileInfo)
		' Получить файлы, ассоциированные с сообщением
		Do While objReader.Read
			colFiles.Add(ReadFile(objReader))
		Loop
		GetMessage.Files = colFiles
		objReader.NextResult()
		Dim colAnswers As List(Of AnswerInfo) = New List(Of AnswerInfo)
		' Получить ответы, ассоциированные с сообщением
		Do While objReader.Read
			colAnswers.Add(ReadAnswer(objReader))
		Loop
		GetMessage.Answers = colAnswers
		objReader.Close()
		objCommand.Dispose()
	End Function

	Private Function ReadMessage(ByVal objReader As SQLiteDataReader) As MessageInfo
		ReadMessage = New MessageInfo
		With ReadMessage
			''0 "BoardName" TEXT NOT NULL,
			''1 "MessageNumber" INTEGER PRIMARY KEY NOT NULL UNIQUE,
			''2 "ThreadNumber" INTEGER NOT NULL,
			''3 "Verified" BOOL NOT NULL DEFAULT 0,
			''4 "Cookie" TEXT NOT NULL DEFAULT "Undefined",
			''5 "NSFWRating" INTEGER NOT NULL DEFAULT 0,
			''6 "UserName" TEXT,
			''7 "RealUserName" TEXT,
			''8 "Email" TEXT,
			''9 "Subject" TEXT,
			''10 "MessageText" TEXT,
			''11 "Password" TEXT,
			''12 "DateTime" INTEGER NOT NULL,
			''13 "Ip" TEXT,
			''14 "MessageDeleted" BOOL NOT NULL DEFAULT 0

			.MessageNumber = objReader.GetInt64(1)
			.Checked = objReader.GetBoolean(3)
			If Not objReader.IsDBNull(4) Then
				.Cookie = objReader.GetString(4)
			End If
			.Rating = CType(objReader.GetInt64(5), NsfwRating)
			If Not objReader.IsDBNull(6) Then
				.UserName = objReader.GetString(6)
			End If
			If Not objReader.IsDBNull(7) Then
				.RealUserName = objReader.GetString(7)
			End If
			If Not objReader.IsDBNull(8) Then
				.EMail = objReader.GetString(8)
			End If
			If Not objReader.IsDBNull(9) Then
				.Subject = objReader.GetString(9)
			End If
			If Not objReader.IsDBNull(10) Then
				.MessageText = objReader.GetString(10)
			End If
			If Not objReader.IsDBNull(12) Then
				.DateTime = Date.FromBinary(objReader.GetInt64(12))
			End If
			.MessageDeleted = objReader.GetBoolean(14)
		End With
	End Function

	Private Function ReadThread(ByVal objReader As SQLiteDataReader) As ThreadInfo
		ReadThread = New ThreadInfo
		With ReadThread
			.ThreadNumber = objReader.GetInt64(1)
			'.LastMessageNumber = objReader.GetInt64(2)
			.IsTop = objReader.GetBoolean(3)
			.IsArchive = objReader.GetBoolean(4)
			.AnonymousAccess = CType(objReader.GetInt64(5), AnonymousAccess)
		End With
	End Function

	Private Function ReadFile(ByVal objReader As SQLiteDataReader) As ImageboardFileInfo
		ReadFile = New ImageboardFileInfo
		With ReadFile
			'0 .BoardName = objReader.Item(0)
			'1 "MessageNumber" INTEGER NOT NULL,
			'2 "ThreadNumber" INTEGER NOT NULL,
			'3 "UrlFilePath" TEXT NOT NULL,
			.UrlFilePath = objReader.GetString(3)
			'"FileLength" INTEGER NOT NULL,
			.FileLength = objReader.GetInt64(4)
			'"ImageWidth" INTEGER NOT NULL,
			.ImageWidth = objReader.GetInt64(5)
			'"ImageHeight" INTEGER NOT NULL,
			.ImageHeight = objReader.GetInt64(6)
			'"ThumbUrlFilePath" TEXT,
			.ThumbUrlFilePath = objReader.GetString(7)
			'"ThumbWidth" INTEGER NOT NULL DEFAULT 0,
			.ThumbWidth = objReader.GetInt64(8)
			'"ThumbHeight" INTEGER NOT NULL DEFAULT 0,
			.ThumbHeight = objReader.GetInt64(9)
			'"FileDeleted" BOOL NOT NULL DEFAULT 0,
			.FileDeleted = objReader.GetBoolean(10)
			'"FileType" INTEGER NOT NULL,
			.FileType = CType(objReader.GetInt32(11), FileType)
			'"ShortFileName" TEXT NOT NULL
			.ShortFileName = objReader.GetString(12)
			.MediaType = objReader.GetString(13)
			.ShortMediaType = objReader.GetString(14)
		End With
	End Function

	Private Function ReadAnswer(ByVal objReader As SQLiteDataReader) As AnswerInfo
		ReadAnswer = New AnswerInfo
		With ReadAnswer
			If objReader.GetString(0) <> m_BoardName Then
				.External = True
			End If
			.MessageNumber = objReader.GetInt64(1)
			.AnswerBoardName = objReader.GetString(2)
			.AnswerMessageNumber = objReader.GetInt64(3)
		End With
	End Function

	Private Function ReadBadWord(ByVal objReader As SQLiteDataReader) As BadWordInfo
		ReadBadWord = New BadWordInfo
		With ReadBadWord
			.BadWordId = objReader.GetInt64(0)
			.Template = objReader.GetString(2)
			.ReplacementString = objReader.GetString(3)
			.ReplacementFlag = CType(objReader.GetInt64(4), ReplacementFlag)
		End With
	End Function

	Private Function ReadBoard(ByVal objReader As SQLiteDataReader) As BoardInfo
		ReadBoard = New BoardInfo
		With ReadBoard
			.BoardName = objReader.GetString(0)
			.Description = objReader.GetString(1)
			.MaxMessagesPerPagePerThread = objReader.GetInt64(2)
			.DefaultUserName = objReader.GetString(3)
			.MaxThreadsPerPage = objReader.GetInt64(4)
			.MaxPagesCount = objReader.GetInt64(5)
			.BumpLimit = objReader.GetInt64(6)
			.MaxFilesPerMessage = objReader.GetInt64(7)
			.NewThreadWithoutFilesCreate = objReader.GetBoolean(8)
			.CaptchaEnable = objReader.GetString(9)
			.TimeEnable = objReader.GetBoolean(10)
			.FilesDeny = objReader.GetBoolean(11)
			.MaxMessageLength = objReader.GetInt64(12)
			.IsReadOnly = objReader.GetBoolean(13)
			.AnonymousAccess = CType(objReader.GetInt64(14), AnonymousAccess)
			.MaxMessagesCanDeleteThread = objReader.GetInt64(15)
			.IsHidden = objReader.GetBoolean(16)
			.MenuGroupId = objReader.GetInt64(17)
			.UserNameEnable = objReader.GetBoolean(18)
			.ThumbnailWidth = objReader.GetInt64(19)
			.ThumbnailHeight = objReader.GetInt64(20)
			.PreModerationEnabled = objReader.GetBoolean(21)
			.ShowFaq = objReader.GetBoolean(22)
		End With
	End Function

End Class
