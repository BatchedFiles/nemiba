Imports WebImageBoardControls
Imports ThreadUtils
Imports System.Threading
Imports NemibaWebAPI
Imports ImageboardUtils
Imports I2P

Partial Class DefaultPage
	Inherits System.Web.UI.Page

	Public DivLogo As String = "Добро пожаловать. Снова."
	Public PostPassword As String
	Public BoardName As String
	Public MoveToThread As Boolean
	Public MaxFiles1 As Integer
	Public MaxMessageLength As Integer

	''' <summary>
	''' Максимальное количество тредов на страницу
	''' </summary>
	''' <remarks></remarks>
	Private MaxThreadsPerPage As Integer

	''' <summary>
	''' Максимальное количество отображаемых сообщений в треде на странице
	''' </summary>
	''' <remarks></remarks>
	Private MaxMessagesPerPagePerThread As Integer

	Private m_Settings As CookieSettings

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		' UNDONE Вынести в отдельную функцию всё, что не PostBack
		Dim strSiteName As WebConfigSettings = GetSiteName()
		' Получаем имя раздела
		BoardName = GetBoardName(strSiteName)
		'ucAnswer.BoardName = BoardName

		' Настройки пользователя
		MoveToThread = m_Settings.MoveToThread
		PostPassword = m_Settings.Password

		If User.Identity.IsAuthenticated Then
			' Проверка на необходимость удаления нитей
			Dim strDeleteMessages As String = Request.Form.Item("chkDeleteMessage")
			If Not String.IsNullOrEmpty(strDeleteMessages) Then
				ImageboardUtils.RemoveMessages(BoardName, strDeleteMessages, String.Empty, Request.Form.Item("chkFileOnly") = "on", True)
			End If
		End If
		' Если не форма отправки, то загружаем тред
		If Not Page.IsPostBack Then
			LoadThread(User.Identity.Name)
		End If
	End Sub

	Private Function GetBoardName(ByRef strSiteName As WebConfigSettings) As String
		Const Arrow As String = " » "
		If Request.QueryString.Count > 0 Then
			Dim strRequestQueryString As String = Request.QueryString.Item("board")
			If String.IsNullOrEmpty(strRequestQueryString) Then
				GetBoardName = "default"
			Else
				Select Case strRequestQueryString
					Case "faq"
						If String.IsNullOrEmpty(strSiteName.SiteName) Then
							DivLogo = strSiteName.FaqLinkText
						Else
							DivLogo = strSiteName.SiteName & Arrow & strSiteName.FaqLinkText
						End If
						GetBoardName = "faq"
					Case "rules"
						If String.IsNullOrEmpty(strSiteName.SiteName) Then
							DivLogo = strSiteName.RulesLinkText
						Else
							DivLogo = strSiteName.SiteName & Arrow & strSiteName.RulesLinkText
						End If
						GetBoardName = "rules"
					Case "news"
						If String.IsNullOrEmpty(strSiteName.SiteName) Then
							DivLogo = strSiteName.NewsLinkText
						Else
							DivLogo = strSiteName.SiteName & Arrow & strSiteName.NewsLinkEnabled
						End If
						GetBoardName = "news"
					Case "terms"
						If String.IsNullOrEmpty(strSiteName.SiteName) Then
							DivLogo = strSiteName.TermsLinkText
						Else
							DivLogo = strSiteName.SiteName & Arrow & strSiteName.TermsLinkText
						End If
						GetBoardName = "terms"
					Case Else
						If String.IsNullOrEmpty(strSiteName.SiteName) Then
							DivLogo = strSiteName.DefaultLinkText
						Else
							DivLogo = strSiteName.SiteName & Arrow & strSiteName.DefaultLinkText
						End If
						GetBoardName = "default"
				End Select
			End If
		Else
			If String.IsNullOrEmpty(strSiteName.SiteName) Then
				DivLogo = strSiteName.DefaultLinkText
			Else
				DivLogo = strSiteName.SiteName & Arrow & strSiteName.DefaultLinkText
			End If
			GetBoardName = "default"
		End If
		Me.Title = DivLogo
	End Function

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		m_Settings = GetCookieSettings(Request.Cookies)
		ApplyTheme(Me, m_Settings.Theme)
	End Sub

	Private Sub LoadThread(ByVal strUserName As String)
		' Выборка из базы
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		Dim colBoardSettings As BoardInfo = objThread.GetBoardInfo()

		' Проверяем права пользователя
		' Если подходят, то показываем форму ответа
		' и поле для удаления тредов
		Dim ThreadManage As Boolean
		Dim BanUsers As Boolean
		Dim NewsManage As Boolean
		If User.Identity.IsAuthenticated Then
			Dim objUserInfo As MembershipUser = Membership.GetUser
			NewsManage = ImageboardUtils.ValidateRoles(objUserInfo.UserName, MethodsManage.Редактор, BoardName)
			If NewsManage Then
				' Количество прикрепляемых файлов
				MaxFiles1 = Convert.ToInt32(colBoardSettings.MaxFilesPerMessage)
				MaxMessageLength = Convert.ToInt32(colBoardSettings.MaxMessageLength)
			End If
			ThreadManage = ValidateRoles(objUserInfo.UserName, MethodsManage.Выверяющ, BoardName)
			BanUsers = ValidateRoles(objUserInfo.UserName, MethodsManage.Досмот, BoardName)
		End If

		objThread.BoardName = BoardName
		' Обновляем навигацию по страницам
		Dim intPageNumber As Long
		Long.TryParse(Request.QueryString.Item("page"), intPageNumber)
		With divNavigate
			.BoardName = BoardName
			.CurrentPage = intPageNumber
			.PagesCount = objThread.GetPagesCount(False)
		End With

		With divThreads
			.BoardThread = objThread
			.ThreadNumber = 0
			.BoardName = BoardName
			.PageNumber = intPageNumber
			.RealUserName = strUserName
			.StartPosition = 0
			.NSFWRating = m_Settings.Nsfw
			.ResourceDirectory = ResFolder
			.UserCookie = Session.SessionID
			.ThreadManage = ThreadManage
			.BanUsers = BanUsers
			.GetArchiveThread = False
			.ColHiddenThreads = GetHiddenThreadNumbers(BoardName, Request.Cookies.Get(HidddenThreadsCookie))
		End With
	End Sub

	Protected Sub ucAnswer_CaptchaNotValid(ByVal Sender As Object, ByVal ErrorMessage As String)
		Dim strErrorQueryString As String = "code=" & ErrorType.CaptchaNotValid & "&" & "status=" & ErrorMessage
		Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
	End Sub

	Protected Sub ucAnswer_ThreadCreateNeeded(ByVal Sender As Object, ByVal mInfo As WebImageBoardControls.Answer.MessageInfo)
		Dim objThreadInfo As BoardThreadInfo
		' Проверить права пользователя
		Dim objUserInfo As MembershipUser = Membership.GetUser

		If ValidateRoles(objUserInfo.UserName, MethodsManage.Редактор, BoardName) Then
			Dim objThread As ImageBoardThreadBase = GetBoardThread()
			objThread.BoardName = BoardName
			objThread.EnableSendNewMessageToEmail = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSendNewMessagesToEmail"))
			objThread.SendNewMessageEmailAddress = ConfigurationManager.AppSettings("SendNewMessagesEmailAddress")
			' Создание треда в базе данных
			objThreadInfo = objThread.CreateThread(String.Empty, mInfo.Rating, String.Empty, String.Empty, mInfo.Subject, mInfo.MessageText, String.Empty, False, GetIpAddress(Request), mInfo.PostedFiles, mInfo.VideoLinks, mInfo.MagicCaptcha, objUserInfo.UserName, True)
		Else
			' Не положено
			objThreadInfo = New BoardThreadInfo
			objThreadInfo.ErrorInfo = New ErrorInfo
			objThreadInfo.ErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objThreadInfo.ErrorInfo.ErrorString = "Не положено"
		End If
		Select Case objThreadInfo.ErrorInfo.ErrorStatus
			Case ErrorType.None
				' Ошибок нет
				' Очищаем треды
				divThreads.Controls.Clear()
				' Загружаем треды
				'LoadThread(GetBoardName(String.Empty), objWebAPI, objUserInfo.UserName)
			Case Else
				' Печатаем сообщение об ошибке
				Dim strErrorQueryString As String = "code=" & objThreadInfo.ErrorInfo.ErrorStatus & "&" & "status=" & objThreadInfo.ErrorInfo.ErrorString
				Server.Transfer("~/errors.aspx?" & strErrorQueryString, False)
		End Select
	End Sub

	Protected Sub ucAnswer_RegisterUserNeeded(ByVal Sender As Object, ByVal mInfo As WebImageBoardControls.Answer.MessageInfo)
		Dim objWebAPI As NemibaWebAPI = New NemibaWebAPI()

		' Пытаемся зарегистрировать пользователя
		'Dim strCookie As String = GenerateRandomString(True)
		Dim objUserInfo As MembershipUser = Membership.CreateUser(mInfo.UserName, mInfo.Password, mInfo.Email)
		'If objUserInfo.UserName.Length > 0 Then
		' Ошибок нет
		' Отправляем пользователю печенье с именем
		'Dim cookie As HttpCookie = New HttpCookie(InternetsCreatorCookie, strCookie)
		'cookie.Expires = Date.Now.AddMonths(1)
		'Response.Cookies.Add(cookie)
		'Else
		'Dim objErrorInfo As ErrorInfo = New ErrorInfo
		'With objErrorInfo
		'.ErrorStatus = ErrorType.AccessViolation
		'.ErrorString = "Нельзя зарегистрироваться здесь."
		'End With
		'DisplayErrorString(Me, objErrorInfo)

		'DisplayErrorString(Me, objErrorInfo)
		'End If
		Response.Redirect("~/manage.aspx", True)
	End Sub

	Protected Sub LoginView_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles kgkgkg.PreRender
		st(kgkgkg)
	End Sub

	Private Sub st(ByVal ct As Control)
		For Each ctt As Control In ct.Controls
			st(ctt)
			If TypeOf ctt Is Answer Then
				Dim ucAnswer As Answer = CType(ctt, Answer)
				With ucAnswer
					.Password = PostPassword
					.BoardName = BoardName
					.MoveToThread = MoveToThread
					.MaxFiles = MaxFiles1
					.MaxMessageLength = MaxMessageLength
				End With
			End If
		Next
	End Sub
End Class
