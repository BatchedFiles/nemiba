Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports ThreadUtils
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Возвращает сообщение с разметкой в Html
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(EnableSession:=True, Description:="Возвращает сообщение с разметкой в Html"), SoapHeader("sHeader")> _
	Public Function GetPostHtml(ByVal BoardName As String, ByVal MessageNumber As Long) As String
		Dim rp As RolePrincipal = HttpContext.Current.User
		Return ThreadUtils.GetPostHtml(BoardName, MessageNumber, rp.Identity.Name)
	End Function

	''' <summary>
	''' Возвращает количество страниц на разделе
	''' </summary>
	''' <param name="BoardName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает количество страниц на разделе"), SoapHeader("sHeader")> _
	Public Function GetPagesCount(ByVal BoardName As String, ByVal ArchiveThread As Boolean) As Long
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		objThread.LockThreadBase()
		GetPagesCount = objThread.GetPagesCount(ArchiveThread)
		objThread.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает список тредов на странице
	''' </summary>
	''' <param name="BoardName">Раздел</param>
	''' <param name="PageNumber">Номер страницы</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает список тредов"), SoapHeader("sHeader")> _
	Public Function GetThreadList(ByVal BoardName As String, _
	ByVal PageNumber As Long, ByVal ArchiveThread As Boolean) As List(Of ThreadInfo)
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		objThread.LockThreadBase()
		GetThreadList = objThread.GetThreadList(PageNumber, ArchiveThread)
		objThread.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает номер следующего сообщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает номер следующего сообщения на разделе"), SoapHeader("sHeader")> _
	Public Function GetNextMessageNumber(ByVal BoardName As String) As Long
		If ValidateUserRoles(MethodsManage.Зой, String.Empty) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			Return objBoard.NextMessageNumber
		Else
			Return (New Random).Next
		End If
	End Function

	''' <summary>
	''' Устанавливает номер следующего сообщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <remarks></remarks>
	<WebMethod(Description:="Устанавливает номер следующего сообщения на разделе"), SoapHeader("sHeader")> _
	Public Function SetNextMessageNumber(ByVal BoardName As String, ByVal NextNumber As Long) As ErrorInfo
		SetNextMessageNumber = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Зой, String.Empty) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objBoard.NextMessageNumber = NextNumber
		Else
			SetNextMessageNumber.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	''' <summary>
	''' Возвращает список всех сообщений в треде
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает список всех сообщений в треде"), SoapHeader("sHeader")> _
	Public Function GetMessageList(ByVal BoardName As String, _
		ByVal ThreadNumber As Long) As List(Of Long)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		objBoard.LockThreadBase()
		GetMessageList = objBoard.GetMessageList(ThreadNumber)
		objBoard.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает список всех сообщений в треде начиная с некоторого номера сообщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <param name="StartMessageNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает список всех сообщений в треде начиная с некоторого номера сообщения"), SoapHeader("sHeader")> _
	 Public Function GetMessageStartAtList(ByVal BoardName As String, _
	   ByVal ThreadNumber As Long, _
	   ByVal StartMessageNumber As Long) As List(Of Long)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		objBoard.LockThreadBase()
		GetMessageStartAtList = objBoard.GetMessageListStartAt(ThreadNumber, StartMessageNumber)
		objBoard.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает сообщение со списком ответов и файлов
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает сообщение со списком ответов и файлов"), SoapHeader("sHeader")> _
	Public Function GetMessage(ByVal BoardName As String, _
	   ByVal MessageNumber As Long) As MessageInfo
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		objBoard.LockThreadBase()
		GetMessage = objBoard.GetMessage(MessageNumber)
		GetMessage.Cookie = String.Empty
		objBoard.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает список ответов по номеру собщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает список ответов по номеру собщения"), SoapHeader("sHeader")> _
	  Public Function GetAnswerList(ByVal BoardName As String, _
	  ByVal MessageNumber As Long) As List(Of AnswerInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		objBoard.LockThreadBase()
		GetAnswerList = objBoard.GetAnswerList(MessageNumber)
		objBoard.UnLockThreadBase()
	End Function

	''' <summary>
	''' Возвращает список файлов по номеру сообщения
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Возвращает список файлов по номеру сообщения"), SoapHeader("sHeader")> _
	Public Function GetFileList(ByVal BoardName As String, _
		ByVal MessageNumber As Long) As List(Of ImageboardFileInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		objBoard.LockThreadBase()
		GetFileList = objBoard.GetFileList(MessageNumber)
		objBoard.UnLockThreadBase()
	End Function

	''' <summary>
	''' Отправка жалобы на сообщение
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageNumber"></param>
	''' <param name="СomplaintText"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Отправка жалобы на сообщение"), SoapHeader("sHeader")> _
	Public Function CreateComplaint(ByVal BoardName As String, _
	   ByVal MessageNumber As Long, _
	   ByVal СomplaintText As String) As ErrorInfo
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		Return objBoard.CreateComplaint(MessageNumber, СomplaintText)
	End Function

	''' <summary>
	''' Получение списка жалоб раздела
	''' </summary>
	''' <param name="BoardName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получение списка жалоб раздела"), SoapHeader("sHeader")> _
	Public Function GetComplaintList(ByVal BoardName As String) As List(Of ComplaintInfo)
		If ValidateUserRoles(MethodsManage.Кляузник, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			Return objBoard.GetComplaintList
		Else
			Return New List(Of ComplaintInfo)
		End If
	End Function

	''' <summary>
	''' Удаление жалобы
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="СomplaintId"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаление жалобы"), SoapHeader("sHeader")> _
	  Public Function RemoveComplaint(ByVal BoardName As String, _
	 ByVal СomplaintId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Кляузник, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveComplaint(СomplaintId)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Cоздание треда
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(EnableSession:=True, Description:="Создание треда"), SoapHeader("sHeader")> _
	Public Function CreateThread(ByVal BoardName As String, _
	  ByVal Rating As NSFWRating, _
	  ByVal UserName As String, _
	  ByVal Email As String, _
	  ByVal Subject As String, _
	  ByVal MessageText As String, _
	  ByVal Password As String, _
	  ByVal PostedFiles As List(Of Byte()), _
	  ByVal PostedMimeType As List(Of String), _
	  ByVal VideoLinks As List(Of String), _
	  ByVal TopThread As Boolean, _
	  ByVal DenyForAnswersGroups As List(Of DenyRolesForThreadAnswersInfo)) As BoardThreadInfo

		Dim objFile As ImageboardPostedFile
		Dim colFiles As List(Of ImageboardPostedFile) = New List(Of ImageboardPostedFile)
		For i As Integer = 0 To PostedFiles.Count - 1
			objFile = New ImageboardPostedFile
			With objFile
				.ContentLength = PostedFiles.Item(i).Length
				.ContentType = PostedMimeType.Item(i)
				.FileName = String.Empty
				Dim ms As MemoryStream = New MemoryStream(PostedFiles.Item(i))
				ms.Seek(0, SeekOrigin.Begin)
				.InputStream = ms
			End With
			colFiles.Add(objFile)
		Next
		Dim RealUserName As String
		If sHeader Is Nothing Then
			RealUserName = Membership.GetUser.UserName
		Else
			If Membership.ValidateUser(sHeader.UserName, sHeader.Password) Then
				RealUserName = sHeader.UserName
			Else
				RealUserName = String.Empty
			End If
		End If
		If Not ValidateRoles(RealUserName, MethodsManage.Выверяющ, BoardName) Then
			TopThread = False
		End If
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		CreateThread = objThread.CreateThread(Session.SessionID, Rating, UserName, Email, Subject, MessageText, Password, TopThread, GetIpAddress(HttpContext.Current.Request), colFiles, VideoLinks, Nothing, RealUserName, ValidateRoles(RealUserName, MethodsManage.Редактор, BoardName))
		' Удаление потока
		For Each objFile In colFiles
			objFile.InputStream.Close()
		Next
	End Function

	''' <summary>
	''' Ответ в тред
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(EnableSession:=True, Description:="Ответ в тред"), SoapHeader("sHeader")> _
	Public Function AnswerToThread(ByVal BoardName As String, _
	ByVal ThreadNumber As Long, ByVal Rating As NsfwRating, _
	  ByVal UserName As String, _
	  ByVal Email As String, _
	  ByVal Subject As String, _
	  ByVal MessageText As String, _
	  ByVal Password As String, _
	  ByVal PostedFiles As List(Of Byte()), _
	  ByVal PostedMimeType As List(Of String), _
	  ByVal VideoLinks As List(Of String)) As BoardThreadInfo

		Dim objFile As ImageboardPostedFile
		Dim colFiles As List(Of ImageboardPostedFile) = New List(Of ImageboardPostedFile)
		For i As Integer = 0 To PostedFiles.Count - 1
			objFile = New ImageboardPostedFile
			With objFile
				.ContentLength = PostedFiles.Item(i).Length
				.ContentType = PostedMimeType.Item(i)
				.FileName = String.Empty
				Dim ms As MemoryStream = New MemoryStream(PostedFiles.Item(i))
				ms.Seek(0, SeekOrigin.Begin)
				.InputStream = ms
			End With
			colFiles.Add(objFile)
		Next
		Dim RealUserName As String
		If sHeader Is Nothing Then
			RealUserName = Membership.GetUser.UserName
		Else
			If Membership.ValidateUser(sHeader.UserName, sHeader.Password) Then
				RealUserName = sHeader.UserName
			Else
				RealUserName = String.Empty
			End If
		End If
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		AnswerToThread = objThread.AddMessageToThread(ThreadNumber, Session.SessionID, Rating, UserName, Email, Subject, MessageText, Password, GetIpAddress(HttpContext.Current.Request), colFiles, VideoLinks, Nothing, RealUserName, ValidateRoles(RealUserName, MethodsManage.Редактор, BoardName))
		' Удаление потока
		For Each objFile In colFiles
			objFile.InputStream.Close()
		Next
	End Function

	''' <summary>
	''' Получение списка групп, запрещённых для ответа в тред
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получение списка групп, запрещённых для ответа в тред"), SoapHeader("sHeader")> _
	Public Function GetRolesFromAnswersThreadDenyList(ByVal BoardName As String, _
	 ByVal ThreadNumber As Long) As List(Of DenyRolesForThreadAnswersInfo)
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			Return objBoard.GetRolesFromAnswersThreadDenyList(ThreadNumber)
		Else
			Return New List(Of DenyRolesForThreadAnswersInfo)
		End If
	End Function

	''' <summary>
	''' Добавление группы в запрещённые для ответа в тред
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <param name="Rolename"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Добавление группы в запрещённые для ответа в тред"), SoapHeader("sHeader")> _
	Public Function AddRoleFromAnswersThreadDeny(ByVal BoardName As String, _
	  ByVal ThreadNumber As Long, _
	  ByVal RoleName As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.AddRoleFromAnswersThreadDeny(ThreadNumber, RoleName)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Удаление группы из списка запрещённых для ответа в тред
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <param name="RoleName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаление группы из списка запрещённых для ответа в тред"), SoapHeader("sHeader")> _
	Public Function RemoveRoleFromAnswersThreadDeny(ByVal BoardName As String, _
	   ByVal ThreadNumber As Long, _
	   ByVal RoleName As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objThread As ImageBoardThreadBase = GetBoardThread()
			objThread.BoardName = BoardName
			objErrorInfo = objThread.RemoveRoleFromAnswersThreadDeny(ThreadNumber, RoleName)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Удаляет сообщение
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="MessageList"></param>
	''' <param name="FilesOnly"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаление сообщений"), SoapHeader("sHeader")> _
	Public Function RemoveMessages(ByVal BoardName As String, _
	  ByVal MessageList As List(Of Long), ByVal Password As String, _
	  ByVal FilesOnly As Boolean) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		Dim IsAdmin As Boolean
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			IsAdmin = True
		End If
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		For Each MessageNumber As Long In MessageList
			objThread.RemoveMessage(MessageNumber, Password, FilesOnly, IsAdmin)
		Next
		Return objErrorInfo
	End Function

	''' <summary>
	''' Редактирование закреплённости треда
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <param name="TopFlag"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Редактирование закреплённости треда"), SoapHeader("sHeader")> _
	Public Function EditThreadTopFlag(ByVal BoardName As String, _
	  ByVal ThreadNumber As Long, _
	 ByVal TopFlag As Boolean) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.EditThreadAlwaysOnTopFlag(ThreadNumber, TopFlag)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Перенос треда на другой раздел
	''' </summary>
	''' <param name="OldBoardName"></param>
	''' <param name="NewBoardName"></param>
	''' <param name="ThreadNumber"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Перенос треда на другой раздел"), SoapHeader("sHeader")> _
	  Public Function MoveThread(ByVal OldBoardName As String, _
	 ByVal NewBoardName As String, _
	 ByVal ThreadNumber As Long) As BoardThreadInfo
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = OldBoardName
		Return objBoard.MoveThread(ThreadNumber, NewBoardName)
	End Function
End Class
