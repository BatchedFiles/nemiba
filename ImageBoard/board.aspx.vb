Imports ThreadUtils
Imports ImageboardUtils
Imports System.Threading
Imports System.Drawing
Imports WebImageBoardControls
Imports NemibaWebAPI
Imports I2P

Partial Class BoardPage
	Inherits System.Web.UI.Page

	''' <summary>
	''' Текст в заголовке раздела, например, Гиперчан » Тестирование
	''' </summary>
	''' <remarks></remarks>
	Public DivLogo As String

	''' <summary>
	''' Пароль
	''' </summary>
	''' <remarks></remarks>
	Public PostPassword As String

	''' <summary>
	''' Возвожность управления тредом
	''' </summary>
	''' <remarks></remarks>
	Private ThreadManage As Boolean

	''' <summary>
	''' Возможность управления баном
	''' </summary>
	''' <remarks></remarks>
	Private BanManage As Boolean

	''' <summary>
	''' Возможность создавать треды на системных разделах
	''' </summary>
	''' <remarks></remarks>
	Private NewsManage As Boolean

	''' <summary>
	''' Режим каталога
	''' </summary>
	''' <remarks></remarks>
	Private CatalogMode As Boolean

	Private m_Settings As CookieSettings

	Protected Sub ucAnswer_AnswerToThread(ByVal Sender As Object, ByVal mInfo As Answer.MessageInfo) Handles ucAnswer.AnswerToThread
		Dim NewsManage As Boolean
		' Кодируем строку, чтобы не было кавычек
		Dim objThreadInfo As BoardThreadInfo
		Dim strAddress As String = GetIpAddress(Request)

		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = mInfo.BoardName
		objThread.EnableSendNewMessageToEmail = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSendNewMessagesToEmail"))
		objThread.SendNewMessageEmailAddress = ConfigurationManager.AppSettings("SendNewMessagesEmailAddress")
		objThreadInfo = objThread.AddMessageToThread(mInfo.ThreadNumber, Session.SessionID, mInfo.Rating, mInfo.UserName, mInfo.Email, mInfo.Subject, mInfo.MessageText, mInfo.Password, strAddress, mInfo.PostedFiles, mInfo.VideoLinks, mInfo.MagicCaptcha, User.Identity.Name, NewsManage)
		' Выборка из базы
		Dim colBoardSettings As BoardInfo = objThread.GetBoardInfo()

		Select Case objThreadInfo.ErrorInfo.ErrorStatus
			Case ErrorType.None
				' Ошибок нет
				Response.StatusCode = 201
				Response.Status = "201 Created"
				If mInfo.MoveToThread OrElse m_Settings.MoveToThread > 1 Then
					' Перезагружаем тред
					Response.RedirectLocation = ResolveUrl(String.Format("~/{0}/src/{1}.aspx", mInfo.BoardName, mInfo.ThreadNumber))
					LoadThreadOrPage(mInfo.BoardName, User.Identity.Name, colBoardSettings.IsReadOnly, ThreadManage, mInfo.ThreadNumber, 0)
				Else
					' Обновляет треды на странице
					Response.RedirectLocation = ResolveUrl(String.Format("~/{0}/0.aspx", mInfo.BoardName))
					LoadThreadOrPage(mInfo.BoardName, User.Identity.Name, colBoardSettings.IsReadOnly, ThreadManage, 0, 0)
				End If
			Case Else
				' Печатаем сообщение об ошибке
				Dim strErrorQueryString As String = "code=" & objThreadInfo.ErrorInfo.ErrorStatus & "&" & "status=" & objThreadInfo.ErrorInfo.ErrorString
				Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
		End Select
	End Sub

	Protected Sub ucAnswer_CaptchaNotValid(ByVal Sender As Object, ByVal ErrorMessage As String) Handles ucAnswer.CaptchaNotValid
		Dim strErrorQueryString As String = "code=" & ErrorType.CaptchaNotValid & "&" & "status=" & ErrorMessage
		Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
	End Sub

	''' <summary>
	''' Создание треда в базе данных
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="UserName"></param>
	''' <param name="Email"></param>
	''' <param name="Subject"></param>
	''' <param name="MessageText"></param>
	''' <param name="PostedFiles"></param>
	''' <param name="VideoLinks"></param>
	''' <param name="MagicCaptcha"></param>
	''' <param name="WithoutFiles"></param>
	''' <param name="MoveToThread"></param>
	''' <param name="TopThread"></param>
	''' <param name="Password"></param>
	''' <param name="Rating"></param>
	''' <remarks></remarks>
	Private Sub CreateThread(ByVal BoardName As String, _
	 ByVal UserName As String, _
	 ByVal Email As String, _
	 ByVal Subject As String, _
	 ByVal MessageText As String, _
	 ByVal PostedFiles As List(Of HttpPostedFile), _
	 ByVal VideoLinks As List(Of String), _
	 ByVal MagicCaptcha As Bitmap, _
	 ByVal WithoutFiles As Boolean, _
	 ByVal MoveToThread As Boolean, _
	 ByVal TopThread As Boolean, _
	 ByVal Password As String, _
	 ByVal Rating As Long)
		Dim objThreadInfo As BoardThreadInfo
		Dim strBoardName As String = HttpUtility.HtmlEncode(BoardName)
		Dim strAddress As String = GetIpAddress(Request)

		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = strBoardName
		objThread.EnableSendNewMessageToEmail = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSendNewMessagesToEmail"))
		objThread.SendNewMessageEmailAddress = ConfigurationManager.AppSettings("SendNewMessagesEmailAddress")
		objThreadInfo = objThread.CreateThread(Session.SessionID, Rating, UserName, Email, Subject, MessageText, Password, TopThread, strAddress, PostedFiles, VideoLinks, MagicCaptcha, User.Identity.Name, NewsManage)
		' Выборка из базы
		Dim colBoardSettings As BoardInfo = objThread.GetBoardInfo()

		Select Case objThreadInfo.ErrorInfo.ErrorStatus
			Case ErrorType.None
				'Если был создан ресурс, то серверу следует вернуть ответ 201 (Created)
				'с указанием URI нового ресурса в заголовке Location.
				' Ошибок нет
				Response.StatusCode = 201
				If MoveToThread OrElse m_Settings.MoveToThread > 1 Then
					' Перезагружаем тред
					Response.AppendHeader("Location", ResolveUrl(String.Format("~/{0}/src/{1}.aspx", strBoardName, objThreadInfo.ThreadNumber)))
					LoadThreadOrPage(strBoardName, User.Identity.Name, colBoardSettings.IsReadOnly, ThreadManage, objThreadInfo.ThreadNumber, 0)
				Else
					' Обновляет треды на странице
					Response.AppendHeader("Location", ResolveUrl(String.Format("~/{0}/0.aspx", strBoardName)))
					LoadThreadOrPage(strBoardName, User.Identity.Name, colBoardSettings.IsReadOnly, ThreadManage, 0, 0)
				End If
			Case Else
				' Печатаем сообщение об ошибке
				Dim strErrorQueryString As String = "code=" & objThreadInfo.ErrorInfo.ErrorStatus & "&" & "status=" & objThreadInfo.ErrorInfo.ErrorString
				Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
		End Select
	End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		' получаем имя борды и избавляемся от кавычек
		Dim BoardName As String = Request.QueryString.Item("board")
		If String.IsNullOrEmpty(BoardName) Then
			' Пишем ошибку
			Dim strErrorQueryString As String = "code=" & ErrorType.BoardNotFound & "&" & "status=" & BoardName
			Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
		Else
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			' Выборка из базы
			Dim colBoardSettings As BoardInfo = objBoard.GetBoardInfo()
			If String.IsNullOrEmpty(colBoardSettings.BoardName) Then
				' Доска не существует
				Dim strErrorQueryString As String = "code=" & ErrorType.BoardNotFound & "&" & "status=" & BoardName
				Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
			Else
				Dim objUserInfo As MembershipUser = Membership.GetUser
				Dim strSiteName As WebConfigSettings = GetSiteName()
				Me.Title = BoardName & " » " & colBoardSettings.Description
				DivLogo = If(String.IsNullOrEmpty(strSiteName.SiteName), colBoardSettings.Description, strSiteName.SiteName & " » " & colBoardSettings.Description)
				' Проверить права пользователя	
				If User.Identity.IsAuthenticated Then
					NewsManage = ImageboardUtils.ValidateRoles(objUserInfo.UserName, MethodsManage.Редактор, BoardName)
					ThreadManage = ValidateRoles(objUserInfo.UserName, MethodsManage.Выверяющ, BoardName)
					BanManage = ValidateRoles(objUserInfo.UserName, MethodsManage.Досмот, BoardName)
				End If
				' Если только чтение, то не показываем форму ответа
				If ((objUserInfo Is Nothing OrElse String.IsNullOrEmpty(objUserInfo.UserName)) AndAlso colBoardSettings.AnonymousAccess > AnonymousAccess.OnlyCreateThread) OrElse colBoardSettings.IsReadOnly OrElse (ImageBoardThreadBase.IsSystemBoard(BoardName) AndAlso Not NewsManage) Then
					ucAnswer.Visible = False
					tdDeletePost.Visible = False
				Else
					Boolean.TryParse(Request.QueryString.Item("catalog"), CatalogMode)

					ucAnswer.CaptchaMode = colBoardSettings.CaptchaEnable ' Капча
					ucAnswer.MaxFiles = Convert.ToInt32(colBoardSettings.MaxFilesPerMessage) ' Количество прикрепляемых файлов

					ucAnswer.Password = m_Settings.Password
					PostPassword = m_Settings.Password

					' Загрузка правил
					Dim blnShowRules As Boolean
					If m_Settings.ShowRules > 0 Then
						Select Case m_Settings.ShowRules
							Case 1
								blnShowRules = True
							Case 2
								blnShowRules = False
							Case Else
								blnShowRules = colBoardSettings.ShowFaq
						End Select
					Else
						blnShowRules = colBoardSettings.ShowFaq
					End If
					If blnShowRules Then
						' Общие
						For Each objRules As RulesInfo In objBoard.GetRulesList()
							ucAnswer.AddRulesRow(objRules.RulesText)
						Next
						If colBoardSettings.BumpLimit > 0 Then
							ucAnswer.AddRulesRow("Нить перестанет подниматься после " & colBoardSettings.BumpLimit.ToString & " сообщений.")
						End If
						If Not colBoardSettings.AnonymousAccess = AnonymousAccess.OnlyCreateThread Then
							ucAnswer.AddRulesRow("Нити с числом ответов более " & colBoardSettings.MaxMessagesCanDeleteThread.ToString & " не могут быть удалены.")
						End If
						If Not colBoardSettings.NewThreadWithoutFilesCreate Then
							ucAnswer.AddRulesRow("Для создания нитей нужно прикрепить как минимум один файл.")
						End If
						ucAnswer.AddRulesRow("Поддерживаемые типы файлов: ")
						' Показать разрешённые типы файлов на разделе
						For Each objMime As MimeInfo In objBoard.GetMimeList()
							ucAnswer.AddRulesRow(String.Format("{0} ({1} байт)", objMime.Extension, objMime.MaxFileLength))
						Next
						ucAnswer.AddRulesRow("Оригинальное название файлов может быть видно.")
						ucAnswer.AddRulesRow("Для изображений больше " & colBoardSettings.ThumbnailWidth.ToString & "x" & colBoardSettings.ThumbnailHeight.ToString & " пикселей будет создана миникартинка.")
						ucAnswer.AddRulesRow("Перед отправкой сообщения прочитай ", "FAQ", "~/?board=faq", ".")
					End If

					' Флажок Вернуться к треду
					ucAnswer.MoveToThread = (m_Settings.MoveToThread > 1)
					' Скрыть имя пользователя
					If m_Settings.HideUserName = 2 Then
						With ucAnswer
							.UserNameVisible = False
							.EMailVisible = False
						End With
					End If
					ucAnswer.MaxMessageLength = Convert.ToInt32(colBoardSettings.MaxMessageLength)
					ucAnswer.BoardName = BoardName
					ucAnswer.UserNameVisible = colBoardSettings.UserNameEnable ' Имя пользователя
				End If
					'************************************************************
					If IsPostBack Then
						' Проверка на необходимость удаления нитей
						Dim strDeleteMessages As String = Request.Form.Item("chkDeleteMessage")
						If Not String.IsNullOrEmpty(strDeleteMessages) Then
							ImageboardUtils.RemoveMessages(BoardName, strDeleteMessages, Request.Form.Item("txtPostPassword"), Request.Form.Item("chkFileOnly") = "on", ThreadManage)
						End If
					Else
						'If Not Convert.ToBoolean(colBoardSettings.Item("PreModerationEnabled")) Then
						'	ucAnswer.HideRatingList()
						'End If
						' Загружаем треды
						Dim astrThreadNumber() As String
						Dim intThreadNumber As Long
						Dim intStartPosition As Long
						If Request.QueryString.Item("thread") IsNot Nothing Then
							astrThreadNumber = Request.QueryString.Item("thread").Split("+")
							If astrThreadNumber.Length > 0 Then
								Long.TryParse(astrThreadNumber(0), intThreadNumber)
								If astrThreadNumber.Length > 1 Then
									Long.TryParse(astrThreadNumber(1), intStartPosition)
								End If
							End If
						End If
						LoadThreadOrPage(BoardName, User.Identity.Name, colBoardSettings.IsReadOnly, ThreadManage, intThreadNumber, intStartPosition)
					End If
			End If
		End If
	End Sub

	''' <summary>
	''' Загружает страницу или тред
	''' </summary>
	''' <param name="strUserName"></param>
	''' <param name="blnReadOnly"></param>
	''' <param name="ThreadManage"></param>
	''' <remarks></remarks>
	Private Sub LoadThreadOrPage(ByVal BoardName As String, _
	  ByVal strUserName As String, _
	  ByVal blnReadOnly As Boolean, _
	  ByVal ThreadManage As Boolean, _
	  ByVal intThreadNumber As Long, _
	  ByVal intStartPosition As Long)
		Dim intPageNumber As Long
		Long.TryParse(Request.QueryString.Item("page"), intPageNumber)
		intPageNumber = Math.Abs(intPageNumber)
		If CatalogMode Then
			ucAnswer.ThreadMode = Answer.ThreadState.Catalog
		Else
			If intThreadNumber > 0 Then
				' показываем тред
				ucAnswer.ThreadMode = Answer.ThreadState.AnswerToThread
				ucAnswer.ThreadNumber = intThreadNumber
				' Изменяем положение формы ответа
				If m_Settings.PostFormPosition = 2 Then
					Me.Controls.Remove(ucAnswer)
					plhBottom.Controls.Add(ucAnswer)
				End If
				'' Проверяем, была ли загрузка треда успешной
				'' Не показываем архивные треды
				'Select Case LoadThread(dbThreads, objUserInfo.UserName, BoardName, intThreadNumber, intStartPosition, divThreads, intCurrentNSFWRating, colBoardSettings.PreModerationEnabled, blnReadOnly, ThreadManage, BanUsers, colBoardSettings.UserNameEnable, colBoardSettings.DefaultUserName, colBoardSettings.TimeEnable, Session.SessionID, False)
				'	Case ErrorType.None
				'	Case ErrorType.ThreadNotFound
				'		Dim objErrorInfo As ErrorInfo = New ErrorInfo
				'		With objErrorInfo
				'			.ErrorStatus = ErrorType.ThreadNotFound
				'			.ErrorString = intThreadNumber.ToString
				'		End With
				'		Session.Add("ErrorInfo", objErrorInfo)
				'		Server.Transfer("~/errors.aspx", False)
				'		'DisplayErrorString(Me, objErrorInfo)
				'End Select
			Else
				' Чтобы установить галочку «Прикрепить тред»
				ucAnswer.ThreadManage = ThreadManage
				'' загружаем страницу
				'' Получаем блокировку и загружаем страницу
				'Select Case LoadPage(dbThreads, objUserInfo.UserName, BoardName, divThreads, colBoardSettings.PreModerationEnabled, intPageNumber, colBoardSettings.MaxThreadsPerPage, colBoardSettings.MaxMessagesPerPagePerThread, GetHiddenThreadNumbers(BoardName, Request.Cookies.Get(HidddenThreadsCookie)), blnReadOnly, ThreadManage, BanUsers, colBoardSettings.UserNameEnable, colBoardSettings.DefaultUserName, colBoardSettings.TimeEnable, Session.SessionID, intCurrentNSFWRating)
				'	Case ErrorType.None
				'	Case Else
				'		divThreads.Controls.Add(New LiteralControl("<p>В салонѣ тихо, пыльно и пусто. Только въ одном изъ угловъ вышиваетъ гладью пожилая княжна.</p>"))
				'End Select
			End If
		End If
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		With divThreads
			.BoardThread = objThread
			.ThreadNumber = intThreadNumber
			.BoardName = BoardName
			.PageNumber = intPageNumber
			.RealUserName = strUserName
			.StartPosition = intStartPosition
			.NSFWRating = m_Settings.Nsfw
			.ResourceDirectory = ResFolder
			.UserCookie = Session.SessionID
			.ThreadManage = ThreadManage
			.BanUsers = BanManage
			.GetArchiveThread = False
			.ColHiddenThreads = GetHiddenThreadNumbers(BoardName, Request.Cookies.Get(HidddenThreadsCookie))
			.TextAreaClientId = ucAnswer.MessageTextClientId
			.ThreadNumberTextClientId = ucAnswer.ThreadNumberTextClientId
			.AnswerToThreadLabelClientId = ucAnswer.AnswerToThreadLabelClientId
			.IsCatalog = CatalogMode
		End With

		' Обновляем навигацию по страницам
		With divNavigate
			.BoardName = BoardName
			.CurrentPage = intPageNumber
			.PagesCount = objThread.GetPagesCount(False)
		End With
	End Sub

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		m_Settings = GetCookieSettings(Request.Cookies)
		ApplyTheme(Me, m_Settings.Theme)
	End Sub

	Protected Sub ucAnswer_RegisterUserNeeded(ByVal Sender As Object, ByVal mInfo As Answer.MessageInfo) Handles ucAnswer.RegisterUserNeeded
		' Пытаемся зарегистрировать пользователя
		Dim objUserInfo As MembershipUser = Membership.CreateUser(mInfo.UserName, mInfo.Password, mInfo.Email)
		' Нельзя создавать треды регистрации пользователей на системных разделах
		If Not ImageBoardThreadBase.IsSystemBoard(mInfo.BoardName) Then
			CreateThread(mInfo.BoardName, mInfo.UserName, mInfo.Email, mInfo.Subject, mInfo.MessageText, mInfo.PostedFiles, mInfo.VideoLinks, mInfo.MagicCaptcha, mInfo.WithoutFiles, mInfo.MoveToThread, False, mInfo.Password, mInfo.Rating)
		End If
		Response.Redirect("~/manage.aspx", True)
	End Sub

	Protected Sub ucAnswer_ThreadCreateNeeded(ByVal Sender As Object, ByVal mInfo As Answer.MessageInfo) Handles ucAnswer.ThreadCreateNeeded
		CreateThread(mInfo.BoardName, mInfo.UserName, mInfo.Email, mInfo.Subject, mInfo.MessageText, mInfo.PostedFiles, mInfo.VideoLinks, mInfo.MagicCaptcha, mInfo.WithoutFiles, mInfo.MoveToThread, mInfo.TopThread, mInfo.Password, mInfo.Rating)
	End Sub

End Class
