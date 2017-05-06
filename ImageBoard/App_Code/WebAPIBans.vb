Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI

	<WebMethod(Description:="Пломбирует адрес"), SoapHeader("sHeader")> _
	Public Function CreateBan(ByVal IpAddress As String, _
		ByVal Reason As String, _
		ByVal ExpiresDate As Date, _
		ByVal BoardName As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
			'CreateBanInternal(IpAddress, Reason, ExpiresDate, BoardName)
		Else
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	'Private Sub CreateBanInternal(ByVal IpAddress As String, _
	' ByVal Reason As String, _
	' ByVal ExpiresDate As Date, _
	' ByVal BoardName As String)
	'	Dim dbBoards As SQLite.SQLiteConnection = OpenBoardBase(False)
	'	Dim objCommand As SQLite.SQLiteCommand = New SQLite.SQLiteCommand(String.Format("INSERT INTO ""Bans"" (""IpAddress"", ""Reason"", ""ExpiresDate"", ""BoardName"") VALUES (""{0}"", ""{1}"", {2}, ""{3}"")", GetSafeString(IpAddress), GetSafeString(Reason), ExpiresDate.Ticks.ToString, GetSafeString(BoardName)), dbBoards)
	'	objCommand.ExecuteNonQuery()
	'	objCommand.Dispose()
	'	CloseBoardBase(dbBoards, False)
	'End Sub

	'<WebMethod(Description:="Пломбирует адрес по номеру сообщения"), SoapHeader("sHeader")> _
	'Public Function CreateBanFromMessageNumber(ByVal MessageNumber As Long, _
	'  ByVal Reason As String, _
	'  ByVal ExpiresDate As Date, _
	'  ByVal BoardName As String) As ErrorInfo
	'	Dim objErrorInfo As ErrorInfo = New ErrorInfo
	'	If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
	'		Dim strIp As String = String.Empty
	'		' Открываем базу данных
	'		Dim dbBoards As SQLiteConnection = OpenBoardBase(True)
	'		' Выборка всех пунктов меню из базы
	'		Dim objCommand As SQLiteCommand = New SQLiteCommand(String.Format("SELECT COUNT(*) FROM ""Boards"" WHERE ""BoardName""=""{0}""", BoardName), dbBoards)
	'		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
	'		If objReader.HasRows Then
	'			' Получить айпишник сообщения
	'			Dim strMessagesFile As String = Path.Combine(HttpContext.Current.Server.MapPath(AppDataFolder), BoardName & ".sqlite")
	'			Dim dbThreads As SQLiteConnection = New SQLiteConnection("Data Source = """ & strMessagesFile & """;Version=3;")
	'			' Блокировка
	'			Dim objThreadsLock As ReaderWriterLock = CType(Application.Item("PagesLock"), Dictionary(Of String, ReaderWriterLock)).Item(BoardName)
	'			objThreadsLock.AcquireReaderLock(Timeout.Infinite)
	'			dbThreads.Open()
	'			Dim objIpCommand As SQLiteCommand = New SQLiteCommand(String.Format("SELECT ""Ip"" FROM ""Messages"" WHERE ""MessageNumber""=""{0}""", BoardName), dbBoards)
	'			Dim objIpReader As SQLiteDataReader = objCommand.ExecuteReader()
	'			If objIpReader.HasRows Then
	'				objIpReader.Read()
	'				' Добавить айпишник в запломбированные
	'				strIp = objReader.Item(0)
	'			Else
	'				objErrorInfo.ErrorStatus = ErrorType.ThreadNotFound
	'			End If
	'			objIpReader.Close()
	'			objIpCommand.Dispose()
	'			dbThreads.Close()
	'			' Снимаем блокировку
	'			objThreadsLock.ReleaseReaderLock()
	'		Else
	'			objErrorInfo.ErrorStatus = ErrorType.BoardNotFound
	'		End If
	'		' Закрываем базу
	'		objReader.Close()
	'		objCommand.Dispose()
	'		CloseBoardBase(dbBoards, True)
	'		' Если айпишник получен, то добавляем бан
	'		If Not String.IsNullOrEmpty(strIp) Then
	'			'CreateBanInternal(strIp, Reason, ExpiresDate, BoardName)
	'		End If
	'	Else
	'		objErrorInfo.ErrorStatus = ErrorType.AccessViolation
	'		objErrorInfo.ErrorString = "Не положено"
	'	End If
	'	Return objErrorInfo
	'End Function

	'<WebMethod(Description:="Удаляет адрес из блокировок"), SoapHeader("sHeader")> _
	'Public Function RemoveBan(ByVal BoardName As String, ByVal BanId As Long) As ErrorInfo
	'	Dim objErrorInfo As ErrorInfo = New ErrorInfo
	'	If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
	'		Dim dbBoards As SQLiteConnection = OpenBoardBase(False)
	'		Dim objCommand As SQLiteCommand = New SQLiteCommand(String.Format("DELETE FROM ""Bans"" WHERE ""BanId""={0}", BanId.ToString), dbBoards)
	'		objCommand.ExecuteNonQuery()
	'		objCommand.Dispose()
	'		CloseBoardBase(dbBoards, False)
	'	Else
	'		objErrorInfo.ErrorStatus = ErrorType.AccessViolation
	'		objErrorInfo.ErrorString = "Не положено"
	'	End If
	'	Return objErrorInfo
	'End Function

	'<WebMethod(Description:="Получает список запломбированных адресов"), SoapHeader("sHeader")> _
	'Public Function GetBanList(ByVal BoardName As String) As List(Of BanInfo)
	'	Dim colBans As List(Of BanInfo) = New List(Of BanInfo)
	'	If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
	'		' Открываем базу данных
	'		Dim dbBoards As SQLiteConnection = OpenBoardBase(True)
	'		Dim objCommand As SQLiteCommand = New SQLiteCommand(String.Format("SELECT * FROM ""Bans"" WHERE ""BoardName""=""{0}""", BoardName), dbBoards)
	'		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
	'		Do While objReader.Read
	'			Dim objBanInfo As BanInfo = New BanInfo
	'			With objBanInfo
	'				.BanId = objReader.Item(0)
	'				' адрес не должен показываться
	'				'.IpAddress = objReader.Item(1)
	'				.Reason = objReader.Item(2)
	'				.ExpiresDate = Date.FromBinary(objReader.Item(3))
	'				.BoardName = objReader.Item(4)
	'			End With
	'			colBans.Add(objBanInfo)
	'		Loop
	'		objReader.Close()
	'		' Закрываем базу
	'		objCommand.Dispose()
	'		CloseBoardBase(dbBoards, True)
	'	End If
	'	Return colBans
	'End Function

End Class
