Imports Microsoft.VisualBasic
Imports ImageboardUtils
Imports WebImageBoardControls
Imports I2P

Public Class ThreadUtils

	'''' <summary>
	'''' Возвращает список тредов, доступных для чтения
	'''' </summary>
	'''' <param name="PageNumber">Номер страницы. Если меньше нуля, то возвращает все треды в разделе</param>
	'''' <returns></returns>
	'''' <remarks></remarks>
	'''' <param name="dbThreads">Открытое подключение к базе данных</param>
	'''' <param name="RealUserName">Имя пользователя, должно быть настоящим</param>
	'''' <param name="ArchiveThread">Если установлен в <c>True</c>, то возвращает список только архивных тредов</param>
	'''' <param name="BoardName">Доска для списка тредов</param>
	'Public Shared Function GetThreadList(ByVal dbThreads As SQLite.SQLiteConnection, _
	' ByVal BoardName As String, _
	' ByVal RealUserName As String, _
	' ByVal PageNumber As Long, _
	' ByVal ArchiveThread As Boolean) As Generic.List(Of ThreadInfo)
	'	GetThreadList = New Generic.List(Of ThreadInfo)
	'	Dim objCommand As SQLite.SQLiteCommand
	'	If ArchiveThread Then
	'		If PageNumber < 0 Then
	'			' Получаем все треды
	'			objCommand = New SQLite.SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=1 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"";END TRANSACTION;", BoardName, RealUserName), dbThreads)
	'		Else
	'			objCommand = New SQLite.SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=1 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"" DESC LIMIT (SELECT MaxThreadsPerPage FROM Boards WHERE ""BoardName""=""{0}"") OFFSET {2} * (SELECT MaxThreadsPerPage FROM Boards WHERE ""BoardName""=""{0}"");END TRANSACTION;", BoardName, RealUserName, PageNumber), dbThreads)
	'		End If
	'	Else
	'		If PageNumber < 0 Then
	'			objCommand = New SQLite.SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=0 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"" DESC;END TRANSACTION;", BoardName, RealUserName), dbThreads)
	'		Else
	'			objCommand = New SQLite.SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT * FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""Archive""=0 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN(SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{1}""))) ORDER BY ""IsTop"" DESC, ""LastMessageNumber"" DESC LIMIT (SELECT MaxThreadsPerPage FROM Boards WHERE ""BoardName""=""{0}"") OFFSET {2} * (SELECT MaxThreadsPerPage FROM Boards WHERE ""BoardName""=""{0}"");END TRANSACTION;", BoardName, RealUserName, PageNumber), dbThreads)
	'		End If
	'	End If
	'	Dim objReader As SQLite.SQLiteDataReader = objCommand.ExecuteReader
	'	Do While objReader.Read
	'		Dim objThread As ThreadInfo = New ThreadInfo
	'		With objThread
	'			.ThreadNumber = objReader.GetInt64(0)
	'			.LastMessageNumber = objReader.GetInt64(1)
	'			.IsTop = objReader.GetBoolean(2)
	'			.IsArchive = objReader.GetBoolean(3)
	'			.AnonymousAccess = objReader.GetInt64(4)
	'		End With
	'		GetThreadList.Add(objThread)
	'	Loop
	'	objReader.Close()
	'	objCommand.Dispose()
	'End Function

	'''' <summary>
	'''' Возвращает номер треда
	'''' </summary>
	'''' <param name="dbThreads">Открытое подключение к базе данных</param>
	'''' <param name="MessageNumber">Номер сообщения</param>
	'''' <param name="RealUserName">Настоящее имя пользователя</param>
	'''' <param name="Archive">Если установлен в True, то получаем номер треда из архива</param>
	'''' <param name="DenyForWriting">Запрет на ответ в тред</param>
	'''' <param name="BoardName">Доска</param>
	'''' <returns></returns>
	'''' <remarks></remarks>
	'Public Shared Function GetThreadNumber(ByVal dbThreads As SQLiteConnection, _
	'  ByVal BoardName As String, _
	'  ByVal MessageNumber As Long, _
	'  ByVal RealUserName As String, _
	'  ByVal DenyForWriting As Boolean, _
	'  ByVal Archive As Boolean) As Long
	'	' Получаем номер треда без архивных тредов
	'	' и не запрещённых для чтения текущим пользователем
	'	Dim objCommand As SQLiteCommand
	'	If Archive Then
	'		objCommand = New SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT ""ThreadNumber"" FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""ThreadNumber""=(SELECT ""ThreadNumber"" FROM ""Messages"" WHERE ""BoardName""=""{0}"" AND ""MessageNumber""={1}) AND ""Archive""=1 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN (SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{2}"")));END TRANSACTION;", BoardName, MessageNumber, RealUserName), dbThreads)
	'	Else
	'		If DenyForWriting Then
	'			objCommand = New SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT ""ThreadNumber"" FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""ThreadNumber""=(SELECT ""ThreadNumber"" FROM ""Messages"" WHERE ""BoardName""=""{0}"" AND ""MessageNumber""={1}) AND ""Archive""=0 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""RoleName"" IN (SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{2}"")));END TRANSACTION;", BoardName, MessageNumber, RealUserName), dbThreads)
	'		Else
	'			objCommand = New SQLiteCommand(String.Format("BEGIN TRANSACTION;SELECT ""ThreadNumber"" FROM ""Threads"" WHERE ""BoardName""=""{0}"" AND ""ThreadNumber""=(SELECT ""ThreadNumber"" FROM ""Messages"" WHERE ""BoardName""=""{0}"" AND ""MessageNumber""={1}) AND ""Archive""=0 AND NOT ""ThreadNumber"" IN (SELECT ""ThreadNumber"" FROM ""DenyForThreadAnswers"" WHERE ""BoardName""=""{0}"" AND ""DenyForReading""=1 AND ""RoleName"" IN (SELECT ""RoleName"" FROM ""Roles"" WHERE ""RoleName"" IN (SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=""{2}"")));END TRANSACTION;", BoardName, MessageNumber, RealUserName), dbThreads)
	'		End If
	'	End If
	'	Dim objReader As SQLiteDataReader = objCommand.ExecuteReader
	'	If objReader.HasRows Then
	'		objReader.Read()
	'		GetThreadNumber = Convert.ToInt64(objReader.Item(0))
	'	End If
	'	objReader.Close()
	'	objCommand.Dispose()
	'End Function

	''' <summary>
	''' Устанавливаем печенье на скрытие треда
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <remarks></remarks>
	Public Shared Sub HideThread(ByVal cookie As HttpCookie, _
	  ByVal BoardName As String, _
	  ByVal ThreadNumber As Long)
		If cookie Is Nothing Then
			cookie = New HttpCookie(HidddenThreadsCookie)
		End If
		Dim sb As StringBuilder = New StringBuilder()
		Dim objThreads As List(Of String) = New List(Of String)
		If Not String.IsNullOrEmpty(cookie.Value) Then
			objThreads.AddRange(cookie.Value.Split(" "))
		End If
		Dim strBoard As String = BoardName & "-" & ThreadNumber.ToString
		If objThreads.Contains(strBoard) Then
			objThreads.Remove(strBoard)
		Else
			objThreads.Insert(0, strBoard)
		End If
		For Each strTemp As String In objThreads
			sb.Append(strTemp & " ")
		Next
		If sb.Length > 0 Then
			sb.Length -= 1
		End If
		cookie.Value = sb.ToString
		cookie.Expires = Date.Now.AddDays(30)
		HttpContext.Current.Response.Cookies.Add(cookie)
	End Sub

	''' <summary>
	''' Получаем номера срытых пользователем тредов
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetHiddenThreadNumbers(ByVal BoardName As String, _
	 ByVal cookie As HttpCookie) As List(Of Long)
		Dim objHiddenThread As List(Of Long) = New List(Of Long)

		' Теперь просто смотрим на печенье
		If cookie IsNot Nothing Then
			Dim objThreads() As String = cookie.Value.Split(" ")
			For Each strTemp As String In objThreads
				Dim strMas() As String = strTemp.Split("-")
				If strMas.Length = 2 Then
					If strMas(0) = BoardName Then
						Dim intThreadNumber As Long
						Long.TryParse(strMas(1), intThreadNumber)
						objHiddenThread.Add(intThreadNumber)
					End If
				End If
			Next
		End If
		Return objHiddenThread
	End Function

	''' <summary>
	''' Возвращает Html-текст сообщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <param name="RealUserName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetPostHtml(ByVal BoardName As String, _
		 ByVal MessageNumber As String, _
		 ByVal RealUserName As String) As String
		Dim objPost As BoardPage = New BoardPage
		Dim objMemoryStream As MemoryStream = New MemoryStream
		Dim objStreamWriter As StreamWriter = New StreamWriter(objMemoryStream)
		Dim objWriter As HtmlTextWriter = New HtmlTextWriter(objStreamWriter)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		With objPost
			.BoardThread = objBoard
			.BoardName = BoardName
			.ThreadNumber = MessageNumber
			.PostNumber = MessageNumber
			'.PageNumber = intPageNumber
			.RealUserName = RealUserName
			'.StartPosition = intStartPosition
			'.NSFWRating = 0 'intCurrentNSFWRating
			.ThreadManage = False ' ThreadManage
			.BanUsers = False 'BanUsers
			.UserCookie = HttpContext.Current.Session.SessionID
			'.GetArchiveThread = False
			.ResourceDirectory = ResFolder
			'.MaxMessagesPerPagePerThread = colBoardSettings.MaxMessagesPerPagePerThread
			'.MaxThreadsPerPage = colBoardSettings.MaxThreadsPerPage
			'.ColHiddenThreads = GetHiddenThreadNumbers(BoardName, HttpContext.Current.Request.Cookies.Get(HidddenThreadsCookie))
		End With

		objPost.RenderControl(objWriter)
		objWriter.Flush()
		objMemoryStream.Seek(0, SeekOrigin.Begin)
		Dim objReader As StreamReader = New StreamReader(objMemoryStream)
		GetPostHtml = objReader.ReadToEnd


		objReader.Close()
		objStreamWriter.Close()
		objWriter.Close()
		objMemoryStream.Close()
	End Function

End Class
