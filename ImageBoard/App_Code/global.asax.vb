Imports Microsoft.VisualBasic
Imports System.Globalization
Imports ImageboardUtils
Imports System.IO

Public Class Global_asax
	Inherits System.Web.HttpApplication

	Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
		Dim objBoardThread As ImageBoardThreadBase = GetBoardThread()
		' Список меню
		Application("Menu") = objBoardThread.GetMenuList
		' Список разделов
		Application("Boards") = objBoardThread.GetBoardsInfoList()
	End Sub

	Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

	End Sub

	Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
		' Код, выполняемый при завершении работы приложения
	End Sub

	Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
		' Отказ перенаправить на страницу с ошибкой
		Server.Transfer("~/errors.aspx", False)
	End Sub

	Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
		' Код, выполняемый при запуске нового сеанса
	End Sub

	Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
		' Примечание. Событие Session_End вызывается только в том случае, если для режима sessionstate
		' задано значение InProc в файле Web.config. Если для режима сеанса задано значение StateServer 
		' или SQLServer, событие не порождается.
	End Sub

	Protected Sub Application_BeginRequest(ByVal sender As Object, ByVal e As System.EventArgs)
		' Язык имиджбрды — русский
		Response.AddHeader("Content-Language", "ru-RU,ru")

		Dim ContentNotModified As Boolean
		' Раздел, страница, тред
		' Страницы, для которых нужно включать кэширование
		If Request.FilePath.EndsWith("default.aspx") OrElse Request.Path.EndsWith("board.aspx") Then
			Dim strEtagHeader As String
			Dim dLastModified As Date

			Dim q As QueryBoardInfo = GetQueryBoardInfo()

			Dim oBoards As List(Of BoardInfo) = CType(Application("Boards"), List(Of BoardInfo))
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()

			If String.IsNullOrEmpty(q.BoardName) Then
				' Доска не определена, установить по умолчанию
				q.BoardName = "default"
				objBoard.BoardName = q.BoardName
			Else
				' Выборка из базы
				objBoard.BoardName = q.BoardName
				Dim colBoardSettings() As BoardInfo = (From b As BoardInfo In oBoards Where b.BoardName = q.BoardName).ToArray
				If colBoardSettings.Length = 0 Then
					' Доска не существует
					Dim strErrorQueryString As String = "code=" & ErrorType.BoardNotFound & "&" & "status=" & q.BoardName
					Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
					Exit Sub
				End If
				' Доска найдена
				' Проверить тред на существование?

				' UNDONE Добавить описание сайта
				' UNDONE Добавить ключевые слова Text too short - (spiders see about 17 words)
			End If
			objBoard.LockThreadBase() ' Блокировка базы на чтение

			If q.ThreadNumber = 0 Then
				Dim objThreads = objBoard.GetThreadList(q.PageNumber, False)
				If objThreads.Count = 0 Then
					' Поставить контрольную сумму
					strEtagHeader = q.BoardName & "_0"
					' Дата последнего изменения установлена как текущая
				Else
					Dim objMessages = objBoard.GetMessageList(objThreads.Item(0).ThreadNumber)
					If objMessages.Count = 0 Then
						strEtagHeader = q.BoardName & "_page_0"
						' Дата последнего изменения устанолена как текущая
					Else
						Dim objMessage = objBoard.GetMessage(objMessages.Item(objMessages.Count - 1))
						strEtagHeader = q.BoardName & "_" & "page" & q.PageNumber.ToString & "_" & objMessage.DateTime.ToBinary.ToString
						dLastModified = objMessage.DateTime
					End If
				End If
			Else
				Dim intThreadNumber = objBoard.GetThreadNumber(q.ThreadNumber, False, False)
				Dim objMessages = objBoard.GetMessageList(intThreadNumber)
				If objMessages.Count = 0 Then
					strEtagHeader = q.BoardName & "_thread_" & intThreadNumber.ToString
					' Дата последнего изменения устанолена как текущая
				Else
					Dim objMessage = objBoard.GetMessage(objMessages.Item(objMessages.Count - 1))
					strEtagHeader = q.BoardName & "_thread_" & intThreadNumber.ToString & "_" & objMessage.DateTime.ToBinary.ToString
					dLastModified = objMessage.DateTime
				End If
			End If

			objBoard.UnLockThreadBase()	' Разблокировка

			' Проверка установки ETag и даты изменения документа
			If Not String.IsNullOrEmpty(strEtagHeader) Then
				'  ETag
				Response.AddHeader("ETag", """" & strEtagHeader & """")	' с кавычками
				' Дата последнего изменения
				Response.AddHeader("Last-Modified", dLastModified.ToUniversalTime.ToString("R", DateTimeFormatInfo.InvariantInfo))
				' Хранить результат в общем кэше две минуты
				Response.AddHeader("Cache-Control", "public, max-age=120")

				' Проверить дату
				Dim strDate = Request.Headers.Item("If-Modified-Since")
				If Not String.IsNullOrEmpty(strDate) Then
					Dim ClientDate As Date
					Dim blnResult As Boolean = Date.TryParse(strDate.Split(";"c)(0).Replace("UTC", "GMT"), ClientDate)
					If blnResult AndAlso ClientDate.ToLocalTime >= New Date(dLastModified.Year, dLastModified.Month, dLastModified.Day, dLastModified.Hour, dLastModified.Minute, dLastModified.Second) Then
						ContentNotModified = True
					End If
				End If
				' Проверить ETag
				If Request.Headers.Item("If-None-Match") = """" & strEtagHeader & """" Then
					ContentNotModified = True
				End If
			End If
		End If

		If Request.FilePath.EndsWith("settings.aspx") OrElse Request.Path.EndsWith("api.aspx") OrElse Request.Path.EndsWith("manage.aspx") Then
			Dim f As FileInfo = New FileInfo(Server.MapPath(Request.FilePath))
			Dim dLastModified As Date = f.CreationTimeUtc
			Dim strEtagHeader As String = dLastModified.ToBinary.ToString
			'  ETag
			Response.AddHeader("ETag", """" & strEtagHeader & """")	' с кавычками
			' Дата последнего изменения
			Response.AddHeader("Last-Modified", dLastModified.ToString("R", DateTimeFormatInfo.InvariantInfo))
			' Дата протухания
			Response.AddHeader("Expires", Date.Now.ToUniversalTime.AddYears(1).ToString("R", DateTimeFormatInfo.InvariantInfo))

			' Проверить дату
			Dim strDate = Request.Headers.Item("If-Modified-Since")
			If Not String.IsNullOrEmpty(strDate) Then
				Dim dCurrent As Date
				Dim blnResult As Boolean = Date.TryParse(strDate.Split(";"c)(0), dCurrent)
				If blnResult AndAlso dCurrent.ToLocalTime.Subtract(dLastModified).TotalSeconds < 2 Then
					ContentNotModified = True
				Else
					' Проверить ETag
					If Request.Headers.Item("If-None-Match") = """" & strEtagHeader & """" Then
						ContentNotModified = True
					End If
				End If
			End If
		End If

		' Необходимо прекратить выполнение запроса
		If ContentNotModified Then
			' Документ не изменился
			Response.StatusCode = 304
			Response.End()
		End If

	End Sub

End Class
