Imports System.Web
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Net.Mail
Imports System.Net.Configuration

Public Class SQLiteBoardDatabaseProvider
	Inherits ImageBoardThreadBase

	Private m_BoardName As String
	Private m_ApplicationName As String
	Private m_UserName As String
	Private m_Connection As SQLiteConnection
	Private m_ResFolder As String

	Public Overrides Property ResourceDirectory() As String
		Get
			Return m_ResFolder
		End Get
		Set(ByVal value As String)
			m_ResFolder = value
		End Set
	End Property

	Public Overrides Property EnableSendNewMessageToEmail() As Boolean

	Public Overrides Property SendNewMessageEmailAddress() As String

	Public Overrides Property ConnectionString() As String

	Public Overrides Property ApplicationName() As String
		Get
			Return m_ApplicationName
		End Get
		Set(ByVal value As String)
			m_ApplicationName = value
		End Set
	End Property

	Public Overrides Property BoardName() As String
		Get
			Return m_BoardName
		End Get
		Set(ByVal value As String)
			m_BoardName = value
		End Set
	End Property

	Public Overrides Property NextMessageNumber() As Long
		Get
			Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
			NextMessageNumber = GetNextMessageNumber(dbBoards)
			CloseBoardBase(True)
		End Get
		Set(ByVal value As Long)
			Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
			SetNextMessageNumber(dbBoards, value)
			CloseBoardBase(False)
		End Set
	End Property

	Private Function GetNextMessageNumber(ByVal objConnection As SQLiteConnection) As Long
		Dim objCommand As New SQLiteCommand("SELECT NextNumber FROM NextMessageNumber WHERE BoardName=$BoardName", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		objReader.Read()
		If objReader.HasRows Then
			GetNextMessageNumber = objReader.GetInt64(0)
		Else
			GetNextMessageNumber = 0
		End If
		objReader.Close()
		objCommand.Dispose()
	End Function

	Private Sub SetNextMessageNumber(ByVal objConnection As SQLiteConnection, ByVal Value As Long)
		Dim objCommand As New SQLiteCommand("UPDATE NextMessageNumber SET NextNumber=$NextNumber WHERE BoardName=$BoardName", objConnection)
		objCommand.Parameters.AddWithValue("$NextNumber", Value)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.ExecuteNonQuery()
		objCommand.Dispose()
	End Sub

	Public Overloads Overrides Function RemoveMessage(ByVal MessageNumber As Long, _
	ByVal Password As String, _
	ByVal FilesOnly As Boolean, _
	ByVal Admin As Boolean) As ErrorInfo
		RemoveMessage = New ErrorInfo
		Dim objConnection As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		'Dim objLock As LockCookie = UpgradeToWriterLock()
		' Получаем номер треда
		Dim intThreadNumber As Long = GetThreadNumber(objConnection, MessageNumber, False, False)

		Dim objCommand As SQLiteCommand
		If intThreadNumber = MessageNumber Then
			' Удаляем тред
			If Admin Then
				If FilesOnly Then
					' ставим флаг, что файлы удалены
					objCommand = New SQLiteCommand("UPDATE Files SET FileDeleted=1 WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber", objConnection)
				Else
					objCommand = New SQLiteCommand("DELETE FROM Threads WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber", objConnection)
				End If
				objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
				objCommand.Parameters.AddWithValue("$ThreadNumber", intThreadNumber)
			Else
				' Проверить совпадение 
				If FilesOnly Then
					' ставим флаг, что файлы удалены
					objCommand = New SQLiteCommand("UPDATE Files SET FileDeleted=1 WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber AND (SELECT Password FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$ThreadNumber)=$Password AND (SELECT COUNT(*) FROM Messages WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber) < (SELECT MaxMessagesCanDeleteThread FROM Boards WHERE BoardName=$BoardName)", objConnection)
				Else
					objCommand = New SQLiteCommand("DELETE FROM Threads WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber AND (SELECT Password FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$ThreadNumber)=$Password AND (SELECT COUNT(*) FROM Messages WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber) < (SELECT MaxMessagesCanDeleteThread FROM Boards WHERE BoardName=$BoardName)", objConnection)
				End If
				objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
				objCommand.Parameters.AddWithValue("$ThreadNumber", intThreadNumber)
				objCommand.Parameters.AddWithValue("$Password", Password)
			End If
		Else
			' удаляем сообщение
			If FilesOnly Then
				' Ставим флаг, что файлы удалены
				If Admin Then
					objCommand = New SQLiteCommand("UPDATE Files SET FileDeleted=1 WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber", objConnection)
					objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
					objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
				Else
					objCommand = New SQLiteCommand("UPDATE Files SET FileDeleted=1 WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber AND (SELECT COUNT(*) FROM Messages WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber AND Password=$Password) > 0", objConnection)
					objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
					objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
					objCommand.Parameters.AddWithValue("$Password", Password)
				End If
			Else
				' Ставим флаг, что сообщение удалено
				If Admin Then
					objCommand = New SQLiteCommand("UPDATE Messages SET MessageDeleted=1 WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber", objConnection)
					objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
					objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
				Else
					objCommand = New SQLiteCommand("UPDATE Messages SET MessageDeleted=1 WHERE BoardName=$BoardName AND MessageNumber=$MessageNumber AND Password=$Password", objConnection)
					objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
					objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
					objCommand.Parameters.AddWithValue("$Password", Password)
				End If
			End If
		End If
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		' Удалить файлы с диска
		RemoveFilesFromDisk(objConnection)
		'DowngradeFromWriterLock(objLock)
		CloseBoardBase(False)
		If intRows = 0 Then
			RemoveMessage.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function RemoveArchiveThread(ByVal ThreadNumber As Long) As ErrorInfo
		Return New ErrorInfo
	End Function

	''' <summary>
	''' Удаление помеченных файлов с диска
	''' </summary>
	''' <remarks></remarks>
	Private Sub RemoveFilesFromDisk(ByVal objConnection As SQLiteConnection)
		Dim strFileFolder As String = Path.Combine(m_ResFolder, m_BoardName)
		' Файлы
		Dim objCommand As New SQLiteCommand("SELECT * FROM Files WHERE BoardName=$BoardName AND FileDeleted=1", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		'objCommand.Parameters.AddWithValue("$ThreadNumber", ThreadNumber)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		' Получение всего списка файлов в треде
		Do While objReader.Read()
			Dim objFile As ImageboardFileInfo = ReadFile(objReader)
			' Удаление картинки
			Select Case objFile.FileType
				Case FileType.None
					' Удаление файла с диска
					Try
						File.Delete(Path.Combine(strFileFolder, objFile.UrlFilePath))
					Catch ex As Exception
					End Try
				Case FileType.Image
					If Not String.IsNullOrEmpty(objFile.ThumbUrlFilePath) Then
						' Удаление картинки предпросмотра
						Try
							File.Delete(Path.Combine(strFileFolder, objFile.ThumbUrlFilePath))
						Catch ex As Exception
						End Try
					End If
					' Удаление файла с диска
					Try
						File.Delete(Path.Combine(strFileFolder, objFile.UrlFilePath))
					Catch ex As Exception
					End Try
				Case FileType.MagicCaptcha
					' Удаление файла с диска
					Try
						File.Delete(Path.Combine(strFileFolder, objFile.UrlFilePath))
					Catch ex As Exception
					End Try
				Case Else
					' Не нужно удалять
			End Select
		Loop
		objReader.Close()
		objCommand.Dispose()
		objCommand = New SQLiteCommand("DELETE FROM Files WHERE FileDeleted=1", objConnection)
		objCommand.ExecuteNonQuery()
		objCommand.Dispose()
	End Sub

	''' <summary>
	''' Возвращает ложную дату
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function GetFakeDate() As Date
		Return Now.AddSeconds(New Random().Next(-15, 15)).ToUniversalTime()
	End Function

	''' <summary>
	''' Проверяет возможность создавать треды на разделе
	''' </summary>
	''' <param name="objBoardInfo"></param>
	''' <param name="NewsManage"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function ValidateCreateThread(ByVal objConnection As SQLiteConnection, _
	  ByVal objBoardInfo As BoardInfo, _
	  ByVal UserName As String, _
	  ByVal NewsManage As Boolean) As BoardThreadInfo

		Dim objThreadInfo As New BoardThreadInfo
		objThreadInfo.ErrorInfo = New ErrorInfo
		If String.IsNullOrEmpty(objBoardInfo.BoardName) Then
			' Раздел не существует
			With objThreadInfo.ErrorInfo
				.ErrorStatus = ErrorType.BoardNotFound
				.ErrorString = "Ничего не найдено."
			End With
			Return objThreadInfo
		End If
		' Проверка на системные разделы
		If IsSystemBoard(objBoardInfo.BoardName) AndAlso Not NewsManage Then
			' Доска только для чтения, не прокатит
			With objThreadInfo.ErrorInfo
				.ErrorStatus = ErrorType.AccessViolation
				.ErrorString = "Раздел предназначен только для чтения."
			End With
			Return objThreadInfo
		End If
		' Проверка на разделы только для чтения
		If objBoardInfo.IsReadOnly AndAlso Not NewsManage Then
			' Доска только для чтения, не прокатит
			With objThreadInfo.ErrorInfo
				.ErrorStatus = ErrorType.ReadOnlyBoard
				.ErrorString = "Раздел предназначен только для чтения."
			End With
			Return objThreadInfo
		End If
		'' Проверим возможность создавать треды на разделе
		'Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT COUNT(*) FROM DenyForBoardAnswers WHERE BoardName=$BoardName AND NOT RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)", objConnection)
		'objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		'objCommand.Parameters.AddWithValue("$UserName", UserName)
		'Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		'objReader.Read()
		'' Читаем
		'' Доска только для чтения
		'With objThreadInfo.ErrorInfo
		'	.ErrorStatus = ErrorType.AccessViolation
		'	.ErrorString = "Тебе нельзя распространять свои нити на этом разделе."
		'End With
		'objReader.Close()
		'objCommand.Dispose()
		Return objThreadInfo
	End Function

	''' <summary>
	''' Проверка возможности отвечать в тред
	''' </summary>
	''' <param name="objBoardInfo"></param>
	''' <param name="strMessage"></param>
	''' <param name="PostedFilesCount"></param>
	''' <param name="IsAnswerToThread">Флаг, показывающий ответ в тред</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function ValidateAnswerToThread(ByVal objConnection As SQLiteConnection, ByVal objBoardInfo As BoardInfo, ByVal strMessage As String, ByVal PostedFilesCount As Integer, ByVal IsAnswerToThread As Boolean) As BoardThreadInfo
		Dim objThreadInfo As New BoardThreadInfo
		objThreadInfo.ErrorInfo = New ErrorInfo
		' Проверка на анонимный доступ
		If String.IsNullOrEmpty(m_UserName) Then
			Select Case objBoardInfo.AnonymousAccess
				Case AnonymousAccess.OnlyCreateThread
					If IsAnswerToThread Then
						objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.ReadOnlyBoard
						objThreadInfo.ErrorInfo.ErrorString = "Отвечать в треды на этом разделе нельзя"
						Return objThreadInfo
					End If
				Case AnonymousAccess.OnlyForRead
					objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.ReadOnlyBoard
					objThreadInfo.ErrorInfo.ErrorString = "Раздел предназначен только для чтения"
					Return objThreadInfo
				Case AnonymousAccess.Hidden
					objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.BoardNotFound
					objThreadInfo.ErrorInfo.ErrorString = "Раздел не найден"
					Return objThreadInfo
			End Select
		End If
		' Проверка на размер сообщения
		If strMessage.Length > Convert.ToInt32(objBoardInfo.MaxMessageLength) Then
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.MessageTooLong
			objThreadInfo.ErrorInfo.ErrorString = strMessage.Length.ToString
			Return objThreadInfo
		End If
		' Проверка наличия файлов
		If objBoardInfo.FilesDeny And PostedFilesCount > 0 Then
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.OnlyText
			objThreadInfo.ErrorInfo.ErrorString = "Это же текстач, файлы не нужны."
			Return objThreadInfo
		End If
		' Проверить сообщение на запрещённые слова и фразы
		Dim res As List(Of BadWordInfo) = New List(Of BadWordInfo)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM BadWords WHERE BoardName=$BoardName", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			res.Add(ReadBadWord(objReader))
		Loop
		objReader.Close()
		objCommand.Dispose()
		For Each objBadWord As BadWordInfo In res
			' Здесь проверка за слова-паразиты
			If Regex.IsMatch(strMessage, objBadWord.Template) Then
				Select Case objBadWord.ReplacementFlag
					Case ReplacementFlag.None
						' Ничего не делаем
					Case ReplacementFlag.Replace
						' Заменяем
						strMessage = Regex.Replace(strMessage, objBadWord.Template, objBadWord.ReplacementString)
					Case ReplacementFlag.Error
						' Выходим
						objReader.Close()
						objCommand.Dispose()
						objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.DenyStringFound
						objThreadInfo.ErrorInfo.ErrorString = "Запрещено отправлять такие фразы."
						Return objThreadInfo
					Case ReplacementFlag.Remove
						' Убираем
						strMessage = Regex.Replace(strMessage, objBadWord.Template, String.Empty)
				End Select
			End If
		Next
		Return objThreadInfo
	End Function

	''' <summary>
	''' Добавление сообщения в базу данных
	''' </summary>
	''' <param name="datNow"></param>
	''' <param name="intThreadNumber"></param>
	''' <param name="NewsManage"></param>
	''' <param name="Cookie"></param>
	''' <param name="NSFWRating"></param>
	''' <param name="strUserName"></param>
	''' <param name="strEmail"></param>
	''' <param name="strSubject"></param>
	''' <param name="strMessage"></param>
	''' <param name="strPassword"></param>
	''' <param name="strClientIP"></param>
	''' <param name="RealUserName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function AddMessageIntoDataBase(ByVal objConnection As SQLiteConnection, _
	 ByVal datNow As Date, _
	 ByVal intThreadNumber As Long, _
	 ByVal NewsManage As Boolean, _
	 ByVal Cookie As String, _
	  ByVal NSFWRating As Long, _
	  ByVal strUserName As String, _
	  ByVal strEmail As String, _
	  ByVal strSubject As String, _
	  ByVal strMessage As String, _
	  ByVal strPassword As String, _
	  ByVal strClientIP As String, _
	  ByVal RealUserName As String) As Long
		Dim objCommand As SQLiteCommand
		' Получаем номер свободного сообщения
		AddMessageIntoDataBase = GetNextMessageNumber(objConnection)

		' Вставка сообщения
		Dim intVerified As Integer
		If NewsManage Then
			intVerified = 1
		End If
		If intThreadNumber = 0 Then
			intThreadNumber = AddMessageIntoDataBase
		End If
		objCommand = New SQLiteCommand("INSERT INTO Messages (BoardName, MessageNumber, ThreadNumber, Verified, Cookie, NSFWRating, UserName, RealUserName, Email, Subject, MessageText, Password, DateTime, Ip) VALUES ($BoardName, $MessageNumber, $ThreadNumber, $Verified, $Cookie, $NSFWRating, $UserName, $RealUserName, $Email, $Subject, $MessageText, $Password, $DateTime, $Ip)", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", AddMessageIntoDataBase)
		objCommand.Parameters.AddWithValue("$ThreadNumber", intThreadNumber)
		objCommand.Parameters.AddWithValue("$Verified", intVerified)
		objCommand.Parameters.AddWithValue("$Cookie", Cookie)
		objCommand.Parameters.AddWithValue("$NSFWRating", Convert.ToInt64(NSFWRating))
		objCommand.Parameters.AddWithValue("$UserName", GetTrip(strUserName))
		objCommand.Parameters.AddWithValue("$RealUserName", RealUserName)
		objCommand.Parameters.AddWithValue("$Email", strEmail)
		objCommand.Parameters.AddWithValue("$Subject", strSubject)
		objCommand.Parameters.AddWithValue("$MessageText", strMessage)
		objCommand.Parameters.AddWithValue("$Password", strPassword)
		objCommand.Parameters.AddWithValue("$DateTime", datNow.Ticks)
		objCommand.Parameters.AddWithValue("$Ip", strClientIP)
		objCommand.ExecuteNonQuery()
		objCommand.Dispose()
	End Function

	''' <summary>
	''' Добавление ответов на сообщение в базу данных
	''' </summary>
	''' <param name="objAnswers"></param>
	''' <param name="MessageNumber"></param>
	''' <remarks></remarks>
	Private Sub AddAnswerIntoDataBase(ByVal objConnection As SQLiteConnection, _
	  ByVal objAnswers As LinkAnswersData, _
	 ByVal MessageNumber As Long)
		For Each tmpAnswer As LinkAnswersData.LinkMessage In objAnswers.LinkMessages
			' Не добавляем ответы в системные разделы
			If Not IsSystemBoard(tmpAnswer.BoardName) Then
				Dim External As Integer
				If tmpAnswer.BoardName <> m_BoardName Then
					External = 1
				End If
				Dim strCommand As String = "INSERT INTO Answers VALUES($AnswerBoardName, $AnswerMessageNumber, $BoardName, $MessageNumber)"
				Dim objCommand1 As SQLiteCommand = New SQLiteCommand(strCommand, objConnection)
				objCommand1.Parameters.AddWithValue("$AnswerBoardName", tmpAnswer.BoardName)
				objCommand1.Parameters.AddWithValue("$AnswerMessageNumber", tmpAnswer.MessageNumber)
				objCommand1.Parameters.AddWithValue("$BoardName", m_BoardName)
				objCommand1.Parameters.AddWithValue("$MessageNumber", MessageNumber)
				objCommand1.ExecuteNonQuery()
				objCommand1.Dispose()
			End If
		Next
	End Sub

	Public Overloads Overrides Function CreateThread(ByVal Cookie As String, _
	  ByVal Rating As NsfwRating, _
	  ByVal strUserName As String, _
	  ByVal strEmail As String, _
	  ByVal strSubject As String, _
	  ByVal strMessage As String, _
	  ByVal strPassword As String, _
	  ByVal TopThread As Boolean, _
	  ByVal strClientIP As String, _
	  ByVal PostedFiles As List(Of ImageboardPostedFile), _
	  ByVal VideoLinks As List(Of String), _
	  ByVal MagicCaptcha As Bitmap, _
	  ByVal RealUserName As String, _
	  ByVal NewsManage As Boolean) As BoardThreadInfo
		Dim objThreadInfo As BoardThreadInfo = New BoardThreadInfo
		objThreadInfo.ErrorInfo = New ErrorInfo
		Dim objFileData As FileData
		' Текущая дата
		Dim datNow As Date

		Dim objConnection As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Получаем настройки раздела
		Dim colBoardSettings As BoardInfo = GetBoardInfo()
		' Проверка возможности создавать тред
		objThreadInfo = ValidateCreateThread(objConnection, colBoardSettings, RealUserName, NewsManage)
		If objThreadInfo.ErrorInfo.ErrorStatus <> ErrorType.None Then
			Return objThreadInfo
		End If
		' Проверка возможности отвечать в тред
		objThreadInfo = ValidateAnswerToThread(objConnection, colBoardSettings, strMessage, PostedFiles.Count, False)
		If objThreadInfo.ErrorInfo.ErrorStatus <> ErrorType.None Then
			CloseBoardBase(True)
			Return objThreadInfo
		End If
		' Проверка на наличие файла в ОП-посте
		If MagicCaptcha Is Nothing Then	' Если капча-ведунья не существует
			If PostedFiles.Count = 0 And VideoLinks.Count = 0 And Not Convert.ToBoolean(colBoardSettings.NewThreadWithoutFilesCreate) Then
				objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.FileRequired
				objThreadInfo.ErrorInfo.ErrorString = "В этом разделе нити должны стартовать с файлами."
				CloseBoardBase(True)
				Return objThreadInfo
			End If
		End If
		' Проверка типов файлов
		objFileData = ValidateFiles(objConnection, strMessage.Length, RealUserName, colBoardSettings.MaxFilesPerMessage, colBoardSettings.ThumbnailWidth, colBoardSettings.ThumbnailHeight, RealUserName, PostedFiles, VideoLinks, MagicCaptcha)
		If objFileData.ErrorStatus <> ErrorType.None Then
			objThreadInfo.ErrorInfo.ErrorStatus = objFileData.ErrorStatus
			objThreadInfo.ErrorInfo.ErrorString = objFileData.ErrorString
			objFileData.Dispose()
			CloseBoardBase(True)
			Return objThreadInfo
		End If
		' Если файл не указан, то текст сообщения не может быть пустым
		If objFileData.FilesData.Count = 0 AndAlso strMessage.Length = 0 Then
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.MessageIsEmpty
			objThreadInfo.ErrorInfo.ErrorString = "Если файл не указан, то текст сообщения не может быть пустым."
			CloseBoardBase(True)
			Return objThreadInfo
		End If

		strUserName = GetSafeString(strUserName)
		strEmail = GetSafeString(strEmail)
		strSubject = GetSafeString(strSubject)
		strMessage = GetSafeString(strMessage)

		' Ссылки
		Dim objAnswers As LinkAnswersData = WakabaMark.ReplaceLink(m_BoardName, strMessage)
		' вакабамарк
		strMessage = WakabaMark.WakabaMark(objAnswers.MessageText)

		' Текущая дата
		datNow = GetFakeDate()

		Dim objLock As Threading.LockCookie = UpgradeToWriterLock()
		'****************************************************
		' Добавление сообщения в базу данных
		objThreadInfo.ThreadNumber = AddMessageIntoDataBase(objConnection, datNow, 0, NewsManage, Cookie, Rating, strUserName, strEmail, strSubject, strMessage, strPassword, strClientIP, RealUserName)

		' Создать тред
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO Threads (BoardName, ThreadNumber, LastMessageNumber, IsTop) VALUES ($BoardName, $ThreadNumber, $ThreadNumber, $IsTop)", objConnection)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$ThreadNumber", objThreadInfo.ThreadNumber)
		If TopThread Then
			objCommand.Parameters.AddWithValue("$IsTop", 1)
		Else
			objCommand.Parameters.AddWithValue("$IsTop", 0)
		End If
		objCommand.ExecuteNonQuery()
		objCommand.Dispose()

		'' Проверим текущее количество тредов
		'' Если количество тредов больше или равно максимально возможному
		'' Удаляем последний тред
		'objCommand = New SQLite.SQLiteCommand("SELECT COUNT(*) FROM ""Threads""", dbThreads)
		'objReader = objCommand.ExecuteReader
		'objReader.Read()
		'Dim intThreadsCount As Long = Convert.ToInt64(objReader.Item(0))
		'objReader.Close()
		'objCommand.Dispose()
		''*************************************************************
		'If intThreadsCount >= (Convert.ToInt64(colBoardSettings.Item("MaxPagesCount")) + 1) * Convert.ToInt64(colBoardSettings.Item("MaxThreadsPerPage")) Then
		'	' Получить номер треда для удаления
		'	objCommand = New SQLite.SQLiteCommand("SELECT ""ThreadNumber"" FROM ""Threads"" ORDER BY ""LastMessageNumber"" ASC LIMIT 1", dbThreads)
		'	objReader = objCommand.ExecuteReader
		'	objReader.Read()
		'	Dim intRemovedThreadNumber As Long = Convert.ToInt64(objReader.Item(0))
		'	objReader.Close()
		'	objCommand.Dispose()
		'	' Удаляем файлы с диска
		'	RemoveFilesFromDisk(intRemovedThreadNumber.ToString)
		'	' Удаляем тред из базы 
		'	objCommand = New SQLite.SQLiteCommand("DETELE FROM ""Threads"" WHERE ""ThreadNumber""=" & intRemovedThreadNumber.ToString, dbThreads)
		'	objCommand.ExecuteReader()
		'	objCommand.Dispose()
		'End If
		'**************************************************************
		' Добавляем файлы в таблицу
		AddFilesInToDataBase(objConnection, objThreadInfo.ThreadNumber, objThreadInfo.ThreadNumber, objFileData)

		'****************************************************
		' Добавляем ответы
		AddAnswerIntoDataBase(objConnection, objAnswers, objThreadInfo.ThreadNumber)
		DowngradeFromWriterLock(objLock)
		CloseBoardBase(True)

		' Отправка почты
		If EnableSendNewMessageToEmail Then
			SendNewMessage(True, objThreadInfo.ThreadNumber, 0, Rating, strUserName, strEmail, strSubject, strMessage, strClientIP, objFileData, VideoLinks)
		End If
		objFileData.Dispose()
		' Возвращение данных
		Return objThreadInfo

	End Function

	Public Overloads Overrides Function CreateThread(ByVal Cookie As String, _
	  ByVal Rating As NsfwRating, _
	  ByVal strUserName As String, _
	  ByVal strEmail As String, _
	  ByVal strSubject As String, _
	  ByVal strMessage As String, _
	  ByVal strPassword As String, _
	  ByVal TopThread As Boolean, _
	  ByVal strClientIP As String, _
	  ByVal PostedFiles As List(Of HttpPostedFile), _
	  ByVal VideoLinks As List(Of String), _
	  ByVal MagicCaptcha As Bitmap, _
	  ByVal RealUserName As String, _
	  ByVal NewsManage As Boolean) As BoardThreadInfo
		' Преобразование файлов 
		Dim colFiles As List(Of ImageboardPostedFile) = New List(Of ImageboardPostedFile)
		For Each oFile As HttpPostedFile In PostedFiles
			Dim oNemibaFile As ImageboardPostedFile = New ImageboardPostedFile
			With oNemibaFile
				.ContentLength = oFile.ContentLength
				.ContentType = oFile.ContentType
				.FileName = oFile.FileName
				.InputStream = oFile.InputStream
			End With
			colFiles.Add(oNemibaFile)
		Next
		Return CreateThread(Cookie, Rating, strUserName, strEmail, strSubject, strMessage, strPassword, TopThread, strClientIP, colFiles, VideoLinks, MagicCaptcha, RealUserName, NewsManage)
	End Function

	Public Overloads Overrides Function AddMessageToThread(ByVal ThreadNumber As Long, _
	 ByVal Cookie As String, _
	 ByVal Rating As NsfwRating, _
	 ByVal strUserName As String, _
	 ByVal strEmail As String, _
	 ByVal strSubject As String, _
	 ByVal strMessage As String, _
	 ByVal strPassword As String, _
	 ByVal strClientIP As String, _
	 ByVal PostedFiles As List(Of ImageboardPostedFile), _
	 ByVal VideoLinks As List(Of String), _
	 ByVal MagicCaptcha As Bitmap, _
	 ByVal RealUserName As String, _
	 ByVal NewsManage As Boolean) As BoardThreadInfo

		Dim objThreadInfo As BoardThreadInfo = New BoardThreadInfo
		objThreadInfo.ErrorInfo = New ErrorInfo
		Dim objFileData As FileData
		' Текущая дата
		Dim datNow As Date

		Dim objConnection As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Получаем настройки раздела
		Dim colBoardSettings As BoardInfo = GetBoardInfo()
		' Проверка возможности создавать тред
		objThreadInfo = ValidateCreateThread(objConnection, colBoardSettings, RealUserName, NewsManage)
		If objThreadInfo.ErrorInfo.ErrorStatus <> ErrorType.None Then
			CloseBoardBase(True)
			Return objThreadInfo
		End If
		' Проверка возможности отвечать в тред
		objThreadInfo = ValidateAnswerToThread(objConnection, colBoardSettings, strMessage, PostedFiles.Count, True)
		If objThreadInfo.ErrorInfo.ErrorStatus <> ErrorType.None Then
			CloseBoardBase(True)
			Return objThreadInfo
		End If
		' Проверка типов файлов
		objFileData = ValidateFiles(objConnection, strMessage.Length, RealUserName, colBoardSettings.MaxFilesPerMessage, colBoardSettings.ThumbnailWidth, colBoardSettings.ThumbnailHeight, RealUserName, PostedFiles, VideoLinks, MagicCaptcha)
		If objFileData.ErrorStatus <> ErrorType.None Then
			objThreadInfo.ErrorInfo.ErrorStatus = objFileData.ErrorStatus
			objThreadInfo.ErrorInfo.ErrorString = objFileData.ErrorString
			objFileData.Dispose()
			CloseBoardBase(True)
			Return objThreadInfo
		End If
		' Если файл не указан, то текст сообщения не может быть пустым
		If objFileData.FilesData.Count = 0 AndAlso strMessage.Length = 0 Then
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.MessageIsEmpty
			objThreadInfo.ErrorInfo.ErrorString = "Если файл не указан, то текст сообщения не может быть пустым."
			CloseBoardBase(True)
			Return objThreadInfo
		End If

		strUserName = GetSafeString(strUserName)
		strEmail = GetSafeString(strEmail)
		strSubject = GetSafeString(strSubject)
		strMessage = GetSafeString(strMessage)

		' Ссылки
		Dim objAnswers As LinkAnswersData = WakabaMark.ReplaceLink(m_BoardName, strMessage)
		' вакабамарк
		strMessage = WakabaMark.WakabaMark(objAnswers.MessageText)

		' Текущая дата
		datNow = GetFakeDate()
		' Получаем номер треда
		Dim intMessageNumber As Long
		objThreadInfo.ThreadNumber = GetThreadNumber(objConnection, ThreadNumber, True, False)
		If objThreadInfo.ThreadNumber > 0 Then
			Dim objLock As Threading.LockCookie = UpgradeToWriterLock()
			' Добавление сообщения в базу данных
			intMessageNumber = AddMessageIntoDataBase(objConnection, datNow, objThreadInfo.ThreadNumber, NewsManage, Cookie, Rating, strUserName, strEmail, strSubject, strMessage, strPassword, strClientIP, RealUserName)
			' Добавляем файлы в таблицу
			AddFilesInToDataBase(objConnection, objThreadInfo.ThreadNumber, intMessageNumber, objFileData)
			' Добавляем ответы
			AddAnswerIntoDataBase(objConnection, objAnswers, intMessageNumber)
			DowngradeFromWriterLock(objLock)
		Else
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.ThreadNotFound
			objThreadInfo.ErrorInfo.ErrorString = ThreadNumber.ToString
		End If

		CloseBoardBase(True)

		If EnableSendNewMessageToEmail Then
			SendNewMessage(False, objThreadInfo.ThreadNumber, intMessageNumber, Rating, strUserName, strEmail, strSubject, strMessage, strClientIP, objFileData, VideoLinks)
		End If
		objFileData.Dispose()
		' Возвращение данных
		Return objThreadInfo

	End Function

	Private Sub SendNewMessage(ByVal NewThread As Boolean, _
	 ByVal ThreadNumber As Long, _
	 ByVal MessageNumber As Long, _
  ByVal NSFWRating As Long, _
  ByVal strUserName As String, _
  ByVal strEmail As String, _
  ByVal strSubject As String, _
  ByVal strMessage As String, _
  ByVal strClientIP As String, _
  ByVal objFileData As FileData, _
  ByVal VideoLinks As List(Of String))

		Dim objMessage As New MailMessage()
		' Адреса «кому»
		For Each s In SendNewMessageEmailAddress.Split(";"c)
			objMessage.To.Add(s)
		Next
		Dim strBody As New StringBuilder

		' Текст сообщения
		With strBody
			.AppendLine(String.Format("Имя пользователя: {0}", strUserName))
			.AppendLine(String.Format("Настоящее имя: {0}", m_UserName))
			.AppendLine(String.Format("Элетропочта: {0}", strEmail))
			.AppendLine(String.Format("Адрес: {0}", strClientIP))
			.AppendLine(String.Format("Тема сообщения: {0}", strSubject))
			.AppendLine(String.Format("Рейтинг цензуры: {0}", NSFWRating))
			For Each strVideo As String In VideoLinks
				.AppendLine(String.Format("Ссылка на видео: {0}", strVideo))
			Next
			.AppendLine()
			.AppendLine("Комментарий")
			.AppendLine(strMessage)
		End With

		' Вложения
		For Each objFile As FileData.FileData In objFileData.FilesData
			If objFile.FileType <> FileType.VideoLink AndAlso objFile.FileType <> FileType.Svg Then
				objFile.fs.InputStream.Seek(0, SeekOrigin.Begin)
				Dim objAttachment As Attachment = New Attachment(objFile.fs.InputStream, objFile.fs.FileName, objFile.fs.ContentType)
				objMessage.Attachments.Add(objAttachment)
			End If
		Next

		objMessage.Body = strBody.ToString
		objMessage.BodyEncoding = System.Text.Encoding.UTF8
		objMessage.Subject = If(NewThread, String.Format("На разделе {0} появился новый тред {1}", m_BoardName, ThreadNumber), String.Format("В треде {0} на разделе {1} появилось сообщение {2}", ThreadNumber, m_BoardName, MessageNumber))
		objMessage.SubjectEncoding = System.Text.Encoding.UTF8

		' Отправка почты
		Using objSmtp As New SmtpClient
			'objSmtp.EnableSsl = True
			objSmtp.Send(objMessage)
		End Using
	End Sub

	Public Overloads Overrides Function AddMessageToThread(ByVal ThreadNumber As Long, _
	 ByVal Cookie As String, _
	 ByVal Rating As NsfwRating, _
	 ByVal strUserName As String, _
	 ByVal strEmail As String, _
	 ByVal strSubject As String, _
	 ByVal strMessage As String, _
	 ByVal strPassword As String, _
	 ByVal strClientIP As String, _
	 ByVal PostedFiles As List(Of HttpPostedFile), _
	 ByVal VideoLinks As List(Of String), _
	 ByVal MagicCaptcha As Bitmap, _
	 ByVal RealUserName As String, _
	 ByVal NewsManage As Boolean) As BoardThreadInfo
		' Преобразование файлов 
		Dim colFiles As List(Of ImageboardPostedFile) = New List(Of ImageboardPostedFile)
		For Each oFile As HttpPostedFile In PostedFiles
			Dim oNemibaFile As ImageboardPostedFile = New ImageboardPostedFile
			With oNemibaFile
				.ContentLength = oFile.ContentLength
				.ContentType = oFile.ContentType
				.FileName = oFile.FileName
				.InputStream = oFile.InputStream
			End With
			colFiles.Add(oNemibaFile)
		Next
		Return AddMessageToThread(ThreadNumber, Cookie, Rating, strUserName, strEmail, strSubject, strMessage, strPassword, strClientIP, colFiles, VideoLinks, MagicCaptcha, RealUserName, NewsManage)
	End Function

	''' <summary>
	''' Добавление файлов в базу данных
	''' </summary>
	''' <remarks></remarks>
	Private Sub AddFilesInToDataBase(ByVal objConnection As SQLiteConnection, _
	ByVal intThreadNumber As Long, _
	ByVal intMessageNumber As Long, _
	ByVal objFileData As FileData)
		Const InsertToFileBase As String = "INSERT INTO Files (BoardName, MessageNumber, ThreadNumber, UrlFilePath, FileLength, ImageWidth, ImageHeight, ThumbUrlFilePath, ThumbWidth, ThumbHeight, FileType, ShortFileName, MediaType, ShortMediaType) VALUES ($BoardName, $MessageNumber, $ThreadNumber, $UrlFilePath, $FileLength, $ImageWidth, $ImageHeight, $ThumbUrlFilePath, $ThumbWidth, $ThumbHeight, $FileType, $ShortFileName, $MediaType, $ShortMediaType)"
		For Each objFile As FileData.FileData In objFileData.FilesData
			Dim objCommand As New SQLiteCommand(InsertToFileBase, objConnection)
			objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
			objCommand.Parameters.AddWithValue("$MessageNumber", intMessageNumber)
			objCommand.Parameters.AddWithValue("$ThreadNumber", intThreadNumber)
			objCommand.Parameters.AddWithValue("$UrlFilePath", objFile.UrlFilePath)
			objCommand.Parameters.AddWithValue("$MediaType", objFile.MediaType)
			objCommand.Parameters.AddWithValue("$ShortMediaType", objFile.ShortMediaType)
			Select Case objFile.FileType
				Case FileType.MagicCaptcha
					objCommand.Parameters.AddWithValue("$FileLength", objFile.MagicCaptchaLength)
					objCommand.Parameters.AddWithValue("$ImageWidth", objFile.FileWidth)
					objCommand.Parameters.AddWithValue("$ImageHeight", objFile.FileHeight)
					objCommand.Parameters.AddWithValue("$ThumbUrlFilePath", objFile.ThumbUrlFilePath)
					objCommand.Parameters.AddWithValue("$ThumbWidth", objFile.ThumbWidth)
					objCommand.Parameters.AddWithValue("$ThumbHeight", objFile.ThumbHeight)
					objCommand.Parameters.AddWithValue("$FileType", Convert.ToInt64(FileType.MagicCaptcha))
					objCommand.Parameters.AddWithValue("$ShortFileName", objFile.ShortFileName)
				Case FileType.VideoLink
					objCommand.Parameters.AddWithValue("$FileLength", 0)
					objCommand.Parameters.AddWithValue("$ImageWidth", 0)
					objCommand.Parameters.AddWithValue("$ImageHeight", 0)
					objCommand.Parameters.AddWithValue("$ThumbUrlFilePath", String.Empty)
					objCommand.Parameters.AddWithValue("$ThumbWidth", 0)
					objCommand.Parameters.AddWithValue("$ThumbHeight", 0)
					objCommand.Parameters.AddWithValue("$FileType", Convert.ToInt64(FileType.VideoLink))
					objCommand.Parameters.AddWithValue("$ShortFileName", String.Empty)
				Case Else
					objCommand.Parameters.AddWithValue("$FileLength", objFile.fs.ContentLength)
					objCommand.Parameters.AddWithValue("$ImageWidth", objFile.FileWidth)
					objCommand.Parameters.AddWithValue("$ImageHeight", objFile.FileHeight)
					objCommand.Parameters.AddWithValue("$ThumbUrlFilePath", objFile.ThumbUrlFilePath)
					objCommand.Parameters.AddWithValue("$ThumbWidth", objFile.ThumbWidth)
					objCommand.Parameters.AddWithValue("$ThumbHeight", objFile.ThumbHeight)
					objCommand.Parameters.AddWithValue("$FileType", Convert.ToInt64(objFile.FileType))
					objCommand.Parameters.AddWithValue("$ShortFileName", objFile.ShortFileName)
			End Select
			objCommand.ExecuteNonQuery()
			objCommand.Dispose()
		Next
		' Сохраняем файлы на диск
		For Each objFile As FileData.FileData In objFileData.FilesData
			Dim strFilePath As String = Path.Combine(Path.Combine(m_ResFolder, m_BoardName), objFile.UrlFilePath)
			Select Case objFile.FileType
				Case FileType.Image
					' Сохраняем картинку предпросмора
					If objFile.ThumbBitmap IsNot Nothing Then
						objFile.ThumbBitmap.Save(Path.Combine(Path.Combine(m_ResFolder, m_BoardName), objFile.ThumbUrlFilePath), Imaging.ImageFormat.Jpeg)
					End If
					' Сохраняем оригинал изображения
					objFile.fs.SaveAs(strFilePath)
				Case FileType.MagicCaptcha
					' Сохраняем капчу
					objFile.ThumbBitmap.Save(strFilePath, Imaging.ImageFormat.Jpeg)
				Case FileType.VideoLink
					' Нет файлов для сохранения
					' Тут просто ссылка на видео
				Case Else
					' Сохраняем файл
					objFile.fs.SaveAs(strFilePath)
			End Select
		Next
	End Sub

	''' <summary>
	''' Проверяет загружаемые файлы на допустимость
	''' </summary>
	''' <param name="objConnection">Открытое подключение к базе данных</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function ValidateFiles(ByVal objConnection As SQLiteConnection, _
  ByVal MessageLength As Long, ByVal UserName As String, _
  ByVal MaxFilesPerMessage As Long, _
  ByVal DefaultThumbnailWidth As Long, _
  ByVal DefaultThumbnailHeight As Long, _
  ByVal RealUserName As String, _
  ByVal PostedFiles As List(Of ImageboardPostedFile), _
  ByVal VideoLinks As List(Of String), _
  ByVal MagicCaptcha As Bitmap) As FileData
		Dim objFileData As New FileData
		If MagicCaptcha Is Nothing Then
			' Обрабатываем ссылки на файлы
			For i As Integer = 0 To CInt(Math.Min(MaxFilesPerMessage - 1, PostedFiles.Count - 1))
				' Получаем миме-строку с этим файлом
				Dim objCommand As New SQLiteCommand("SELECT * FROM MimeTypes WHERE BoardName=$BoardName AND Mime=$Mime AND MaxFileLength>$MaxFileLength", objConnection)
				objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
				objCommand.Parameters.AddWithValue("$Mime", PostedFiles.Item(i).ContentType)
				objCommand.Parameters.AddWithValue("$MaxFileLength", PostedFiles.Item(i).ContentLength)
				Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
				If objReader.HasRows Then
					objReader.Read()
					Dim objMime As MimeInfo = ReadMime(objReader)
					objReader.Close()
					objCommand.Dispose()
					objCommand = New SQLiteCommand("SELECT COUNT(*) FROM DenyForFiles WHERE DenyId=$DenyId AND RoleName IN (SELECT RoleName FROM UsersInRoles WHERE UserName=$UserName)", objConnection)
					objCommand.Parameters.AddWithValue("$DenyId", objMime.MimeTypeId)
					objCommand.Parameters.AddWithValue("$UserName", UserName)
					objReader = objCommand.ExecuteReader()
					objReader.Read()

					If Convert.ToInt64(objReader.Item(0)) = 0 Then
						' Не найдено запрещённых идентификаторов Миме
						' Закрываем читателей базы данных
						objReader.Close()
						objCommand.Dispose()
						' Вычисление хэша файла
						Dim key() As Byte = {3, 4, 8, 0, 1, 8, 6, 2, 0, 5, 5, 5, 1, 5, 8, 0, 8, 4, 1, 6}
						Dim objMD5 As New HMACMD5(key)
						Dim HashValue() As Byte = objMD5.ComputeHash(PostedFiles.Item(i).InputStream)
						' Преобразование массива байт в строку в 16‐ричном формате
						Dim strFileName As String = String.Join(String.Empty, HashValue.Select(Function(x) x.ToString("x2")))
						Erase HashValue
						objMD5.Clear()

						Dim objFile As FileData.FileData = New FileData.FileData
						' Путь к файлу
						objFile.UrlFilePath = strFileName & objMime.Extension
						objFile.MediaType = PostedFiles.Item(i).ContentType
						' Файл уже существует
						If File.Exists(Path.Combine(Path.Combine(m_ResFolder, m_BoardName), objFile.UrlFilePath)) Then
							' Файл запрещён к загрузке
							objFileData.ErrorStatus = ErrorType.DublicateFile
							objFileData.ErrorString = "Этот файл уже существует на этом разделе"
							Return objFileData
						Else
							With objFile
								.fs = PostedFiles.Item(i)
								.ThumbWidth = 41
								.ThumbHeight = 43
								' Имя файла без путей и расширений
								.ShortFileName = GetSafeString(String.Join(String.Empty, PostedFiles.Item(i).FileName.Take(128)))
							End With
							' Получаем и выбираем тип файла
							objFile.FileType = objMime.FileType
							Select Case objFile.FileType
								Case FileType.Image
									Try
										' Получаем изображение для предпросмотра
										Dim b As New Bitmap(PostedFiles.Item(i).InputStream)
										objFile.FileWidth = b.Width
										objFile.FileHeight = b.Height

										If b.Width > DefaultThumbnailWidth OrElse b.Height > DefaultThumbnailHeight Then
											' Вычисляем коэффициент масштабирования
											Dim dblK As Double = If(b.Width > b.Height, b.Width / DefaultThumbnailWidth, b.Height / DefaultThumbnailHeight)
											With objFile
												.ThumbWidth = Math.Max(Convert.ToInt32(b.Width / dblK), 1)
												.ThumbHeight = Math.Max(Convert.ToInt32(b.Height / dblK), 1)
												.ThumbUrlFilePath = strFileName & "thumb.jpg"
												' Масштабируем
												.ThumbBitmap = CType(b.GetThumbnailImage(objFile.ThumbWidth, objFile.ThumbHeight, AddressOf GetThumbnailImageAbort, IntPtr.Zero), Bitmap)
											End With
										Else
											With objFile
												.ThumbWidth = b.Width
												.ThumbHeight = b.Height
												.ThumbUrlFilePath = String.Empty '.UrlFilePath
											End With
										End If
										objFile.ShortMediaType = "image/jpeg"
										' Вроде всё хорошо
										b.Dispose()
									Catch ex As Exception
										' Файл запрещён к загрузке
										objFileData.ErrorStatus = ErrorType.FakeFileType
										objFileData.ErrorString = "Файл не является изображением"
										Return objFileData
									End Try
								Case FileType.None
									' Путь к картинке
									objFile.ThumbUrlFilePath = "img" & Path.AltDirectorySeparatorChar & "generic.png"
									objFile.ShortMediaType = "image/png"
							End Select
							objFileData.FilesData.Add(objFile)
						End If
					Else
						' Этой группе нельзя загружать такой файл
						objReader.Close()
						objCommand.Dispose()
						objFileData.ErrorStatus = ErrorType.AccessViolation
						objFileData.ErrorString = "Тебе не положено загружать такие файлы"
						Return objFileData
					End If
				Else
					' Строка миме не найдена
					' Файл запрещён к загрузке
					objReader.Close()
					objCommand.Dispose()
					objFileData.ErrorStatus = ErrorType.DenyMime
					objFileData.ErrorString = "Файл этого типа не разрешён к загрузке"
					Return objFileData
				End If
			Next i
			' Обрабатываем ссылки на видео
			For i As Integer = 0 To CInt(Math.Min(MaxFilesPerMessage - 1, VideoLinks.Count - 1))
				' необходимо преобразовать из
				' http://www.youtube.com/watch?v=TfVdOvyMd9M&NR=1&feature=endscreen
				' Вот в это
				'"http://www.youtube.com/v/1OHKCzTXXV8"
				'Dim r As Regex = New Regex("")
				'pattern = "\[b\](?<messagetext>.*)\[/b\]"
				Dim reg As New Regex("v=(?<video>[a-zA-Z0-9]+)")

				Dim m As Match = reg.Matches(VideoLinks(i)).Item(0)
				If m IsNot Nothing Then
					Dim strVideo As String = m.Groups("video").ToString

					Dim objFile As New FileData.FileData
					objFile.FileType = FileType.VideoLink
					objFile.UrlFilePath = "http://www.youtube.com/v/" & strVideo
					objFileData.FilesData.Add(objFile)
				End If
				'Dim intMessageNumber As Long
				'For Each m As Match In reg.Matches(strMessage)
				'	intMessageNumber = ParseString(m.Groups("messagenumber").ToString)
				'	If intMessageNumber > 0 Then
				'		objAnswers.AddAnswer(m_strBoardName, intMessageNumber)
				'		strMessage = reg.Replace(strMessage, "<a href=""board.aspx?reply=${messagenumber}&board=" & m_strBoardName & "&thread=" & intMessageNumber & """>&gt;&gt;${messagenumber}</a>", 1)
				'	End If
				'Next
			Next i
		Else
			' Капча‐ведунья
			Dim objFile As New FileData.FileData
			' Вычисление хэша файла
			Dim ms As New MemoryStream
			Dim key() As Byte = {3, 4, 8, 0, 1, 8, 6, 2, 0, 5, 5, 5, 1, 5, 8, 0, 8, 4, 1, 6}
			Dim objMD5 As New HMACMD5(key)
			MagicCaptcha.Save(ms, Imaging.ImageFormat.Jpeg)
			ms.Seek(0, SeekOrigin.Begin)
			Dim HashValue() As Byte = objMD5.ComputeHash(ms.ToArray)
			Dim strFileName As String = String.Join(String.Empty, HashValue.Select(Function(x) x.ToString("x2")))
			Erase HashValue
			objMD5.Clear()
			objFile.MagicCaptchaLength = ms.Length
			ms.Close()
			' Путь к файлу
			objFile.UrlFilePath = strFileName & ".jpg"
			objFile.MediaType = "image/jpeg"
			objFile.ShortMediaType = "image/jpeg"
			' Файл уже существует
			If File.Exists(Path.Combine(Path.Combine(m_ResFolder, m_BoardName), objFile.UrlFilePath)) Then
				' Файл запрещён к загрузке
				objFileData.ErrorStatus = ErrorType.DublicateFile
				objFileData.ErrorString = "Этот файл уже существует на этом разделе."
				Return objFileData
			Else
				With objFile
					.ThumbBitmap = MagicCaptcha
					' Имя файла без путей и расширений
					.ShortFileName = "Оригинал"	'objFile.UrlFilePath '(New Random).Next.ToString 'sbShort.ToString & oMimeRow.extension
					' Тип файла — капча-ведунья
					.FileType = FileType.MagicCaptcha
					.FileWidth = MagicCaptcha.Width
					.FileHeight = MagicCaptcha.Height
				End With
				With objFile
					.ThumbWidth = MagicCaptcha.Width
					.ThumbHeight = MagicCaptcha.Height
					.ThumbUrlFilePath = String.Empty '.UrlFilePath
				End With
				objFileData.FilesData.Add(objFile)
			End If
		End If
		' Проверим количество файлов
		If objFileData.FilesData.Count > MaxFilesPerMessage Then
			' Слишком много файлов
			objFileData.ErrorStatus = ErrorType.TooManyFiles
			objFileData.ErrorString = "Слишком много файлов на сообщение"
		Else
			If objFileData.FilesData.Count = 0 AndAlso MessageLength = 0 Then
				' Слишком много файлов
				objFileData.ErrorStatus = ErrorType.MessageIsEmpty
				objFileData.ErrorString = "Сообщение не может быть пустым"
			End If
		End If
		Return objFileData
	End Function

	' Эта функция нужна для правильно работы метода получения эскиза
	Private Shared Function GetThumbnailImageAbort() As Boolean
		' Не удалять
		Return False
	End Function

	Public Overrides Function EditThreadTopFlag(ByVal ThreadNumber As Long, ByVal TopFlag As Boolean) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function MoveThread(ByVal ThreadNumber As Long, ByVal NewBoardName As String) As BoardThreadInfo
		Return New BoardThreadInfo
	End Function

	''' <summary>
	''' Генерация трипкода
	''' </summary>
	''' <param name="UserName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function GetTrip(ByVal UserName As String) As String
		If String.IsNullOrEmpty(UserName) Then
			Return UserName
		Else
			' Ищем символ #
			If UserName.Contains("#") Then
				Dim intStart As Integer = UserName.IndexOf("#")
				Dim strLeft As String = UserName.Substring(0, intStart)
				Dim Password As String = UserName.Substring(intStart + 1)
				If Password.Length > 0 AndAlso strLeft.Length > 0 Then
					Return strLeft & "!" & GetHash(Password)
				Else
					Return UserName
				End If
			Else
				Return UserName
			End If
		End If
	End Function

	Public Overrides Function GetBoardInfo() As BoardInfo
		Dim objBoard As BoardInfo
		Dim AnonAccess As AnonymousAccess = If(String.IsNullOrEmpty(m_UserName), AnonymousAccess.OnlyForRead, AnonymousAccess.Hidden)
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Выборка всех пунктов меню из базы
		Dim objCommand As New SQLiteCommand("SELECT * FROM Boards WHERE BoardName=$BoardName AND AnonymousAnswer<=$AnonAccess", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$AnonAccess", AnonAccess)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		If objReader.HasRows Then
			objReader.Read()
			objBoard = ReadBoard(objReader)
		Else
			objBoard = Nothing
		End If
		' Закрываем базу
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
		' Нужно создать массив чтобы его потом вернуть
		Return objBoard
	End Function

	Public Overrides Property UserName() As String
		Get
			Return m_UserName
		End Get
		Set(ByVal value As String)
			m_UserName = value
		End Set
	End Property

	Public Overrides Function GetBoardsInfoList() As System.Collections.Generic.List(Of BoardInfo)
		Dim AnonAccess As AnonymousAccess = If(String.IsNullOrEmpty(m_UserName), AnonymousAccess.OnlyForRead, AnonymousAccess.Hidden)
		GetBoardsInfoList = New List(Of BoardInfo)
		' Открываем базу данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Выборка всех пунктов меню из базы
		Dim objCommand As New SQLiteCommand("SELECT * FROM Boards WHERE AnonymousAnswer<=$AnonAccess", dbBoards)
		objCommand.Parameters.AddWithValue("$AnonAccess", AnonAccess)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			GetBoardsInfoList.Add(ReadBoard(objReader))
		Loop
		' Закрываем базу
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

	Public Overrides Function CreateBoard(ByVal Description As String, _
	  ByVal MaxMessagesPerPagePerThread As Long, _
	  ByVal DefaultUserName As String, _
	  ByVal MaxThreadsPerPage As Long, _
	  ByVal MaxPagesCount As Long, _
	  ByVal BumpLimit As Long, _
	  ByVal MaxFilesPerMessage As Long, _
	  ByVal NewThreadWithoutFilesCreate As Boolean, _
	  ByVal CaptchaEnable As String, _
	  ByVal TimeEnable As Boolean, _
	  ByVal FilesDeny As Boolean, _
	  ByVal MaxMessageLength As Long, _
	  ByVal IsReadOnly As Boolean, _
	  ByVal Access As AnonymousAccess, _
	  ByVal MaxMessagesCanDeleteThread As Long, _
	  ByVal IsHidden As Boolean, _
	  ByVal MenuGroupId As Long, _
	  ByVal UserNameEnable As Boolean, _
	  ByVal ThumbnailWidth As Long, _
	  ByVal ThumbnailHeight As Long, _
	  ByVal PreModerationEnabled As Boolean, _
	  ByVal ShowFaq As Boolean) As ErrorInfo
		'' Записываем изменения
		'Dim strData As String = String.Format("INSERT OR IGNORE INTO ""Boards"" VALUES (""{0}"", ""{1}"", {2}, ""{3}"", {4}, {5}, {6}, {7}, {8}, ""{9}"", {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27})", _
		'   GetSafeString(BoardName), _
		'   GetSafeString(Description), _
		'   MaxMessagesPerPagePerThread.ToString, _
		'   GetSafeString(DefaultUserName), _
		'   MaxThreadsPerPage.ToString, _
		'   MaxPagesCount.ToString, _
		'   BumpLimit.ToString, _
		'   MaxFilesPerMessage.ToString, _
		'   Convert.ToInt64(NewThreadWithoutFilesCreate).ToString, _
		'   GetSafeString(CaptchaEnable), _
		'   Convert.ToInt64(TimeEnable).ToString, _
		'   Convert.ToInt64(FilesDeny).ToString, _
		'   MaxMessageLength.ToString, _
		'   Convert.ToInt64(IsReadOnly).ToString, _
		'   MaxMessagesCanDeleteThread.ToString, _
		'   Convert.ToInt64(IsHidden).ToString, _
		'   MenuGroupId.ToString, _
		'   Convert.ToInt64(UserNameEnable).ToString, _
		'   ThumbnailWidth.ToString, _
		'   ThumbnailHeight.ToString, _
		'   Convert.ToInt64(PreModerationEnabled).ToString, _
		'   Convert.ToInt64(ShowAllFaq).ToString, _
		'   Convert.ToInt64(ShowBumpLimit).ToString, _
		'   Convert.ToInt64(ShowMaxMessagesCanDeleteThread).ToString, _
		'   Convert.ToInt64(ShowAllowFileTypes).ToString, _
		'   Convert.ToInt64(ShowFileRenameMessage).ToString, _
		'   Convert.ToInt64(ShowThumbnailSize), _
		'   Convert.ToInt64(ShowFaqLinkMessage).ToString)

		'Dim objCommand As SQLite.SQLiteCommand = New SQLite.SQLiteCommand(strData, dbBoards)
		'objCommand.ExecuteNonQuery()
		'objCommand.Dispose()
		Return New ErrorInfo
	End Function

	Public Overrides Function CreateBoard(ByVal Description As String, _
	   ByVal MenuGroupId As Long) As ErrorInfo
		' Открываем базу данных на запись
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As New SQLiteCommand("INSERT INTO Boards(BoardName, Description, MenuGroupId) VALUES($BoardName, $Description, $MenuGroupId)", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$Description", Description)
		objCommand.Parameters.AddWithValue("$MenuGroupId", MenuGroupId)
		objCommand.ExecuteNonQuery()
		' Закрываем базу
		objCommand.Dispose()
		CloseBoardBase(False)
		' Создать каталог
		Directory.CreateDirectory(Path.Combine(m_ResFolder, m_BoardName))
		Return New ErrorInfo
	End Function

	Public Overrides Function SetBoardDescription(ByVal Description As String) As I2P.ImageBoard.ErrorInfo
		' Открываем базу данных на запись
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As New SQLiteCommand("UPDATE Boards SET Description=$Description WHERE BoardName=$BoardName", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$Description", Description)
		objCommand.ExecuteNonQuery()
		' Закрываем базу
		objCommand.Dispose()
		CloseBoardBase(False)
		Return New ErrorInfo
	End Function

	Public Overrides Function RemoveBoard() As ErrorInfo
		Dim rb As New ErrorInfo
		If IsSystemBoard(BoardName) Then
			rb.ErrorStatus = ErrorType.IsSystemBoard
			rb.ErrorString = "Нельзя удалять системные разделы"
		Else
			' Открываем базу данных
			Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
			Dim objCommand As New SQLiteCommand("DELETE FROM Boards WHERE BoardName=$BoardName", dbBoards)
			objCommand.Parameters.AddWithValue("$BoardName", BoardName)
			objCommand.ExecuteNonQuery()
			' Закрываем базу
			objCommand.Dispose()
			CloseBoardBase(False)
			' Удалить каталог
			Directory.Delete(Path.Combine(m_ResFolder, m_BoardName), True)
		End If
		Return rb
	End Function

	Public Overrides Function EditBoard(Optional ByVal Description As String = Nothing, _
	 Optional ByVal MaxMessagesPerPagePerThread As Long = 8, _
	 Optional ByVal DefaultUserName As String = "Anonymous", _
	 Optional ByVal MaxThreadsPerPage As Long = 10, _
	 Optional ByVal MaxPagesCount As Long = 20, _
	 Optional ByVal BumpLimit As Long = 500, _
	 Optional ByVal MaxFilesPerMessage As Long = 1, _
	 Optional ByVal NewThreadWithoutFilesCreate As Boolean = False, _
	 Optional ByVal CaptchaEnable As String = "Enable:1|BackgroundNoise:0|FontWarping:0|LineNoise:0|Colored:0", _
	 Optional ByVal TimeEnable As Boolean = True, _
	 Optional ByVal FilesDeny As Boolean = False, _
	 Optional ByVal MaxMessageLength As Long = 8192, _
	 Optional ByVal IsReadOnly As Boolean = False, _
	 Optional ByVal Access As AnonymousAccess = AnonymousAccess.All, _
	 Optional ByVal MaxMessagesCanDeleteThread As Long = 100, _
	 Optional ByVal IsHidden As Boolean = False, _
	 Optional ByVal MenuGroupId As Long = 1, _
	 Optional ByVal UserNameEnable As Boolean = True, _
	 Optional ByVal ThumbnailWidth As Long = 150, _
	 Optional ByVal ThumbnailHeight As Long = 150, _
	 Optional ByVal PreModerationEnabled As Boolean = False, _
	 Optional ByVal ShowFaq As Boolean = True) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function CheckMessage(ByVal MessageNumber As Long) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function RunSql(ByVal Sql As String) As ErrorInfo
		Dim objErrorInfo As New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As New SQLiteCommand(Sql, dbBoards)
		objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		Return objerrorinfo
	End Function
End Class
