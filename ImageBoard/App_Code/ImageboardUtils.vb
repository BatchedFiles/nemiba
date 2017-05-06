Imports System.Threading
Imports System.Web.Configuration
Imports System.Security.Cryptography
Imports I2P
Imports NemibaWebAPI

Public Class ImageboardUtils


	Public Const ResFolder As String = "~/res"

	Private Const ConfigPath As String = "/ImageBoard"
	
	Public Const HidddenThreadsCookie As String = "a"
	Private Const ThemeCookie As String = "b"
	Private Const HideLeftFrameCookie As String = "c"
	Private Const MoveToThreadCookie As String = "d"
	Private Const PostFormPositionCookie As String = "e"
	Private Const ShowRulesCookie As String = "f"
	Private Const HideUserNameCookie As String = "g"
	Private Const NsfwCookie As String = "h"
	Private Const PasswordCookie As String = "i"

	Public Structure CookieSettings
		''' <summary>
		''' Номер темы
		''' </summary>
		''' <remarks></remarks>
		Public Theme As Integer
		''' <summary>
		''' Скрытие левого фрэйма
		''' </summary>
		''' <remarks></remarks>
		Public HideLeftFrame As Integer
		''' <summary>
		''' Перемещение к треду после ответа
		''' </summary>
		''' <remarks></remarks>
		Public MoveToThread As Integer
		''' <summary>
		''' Положение формы ответа
		''' </summary>
		''' <remarks></remarks>
		Public PostFormPosition As Integer
		''' <summary>
		''' Отображение правил
		''' </summary>
		''' <remarks></remarks>
		Public ShowRules As Integer
		''' <summary>
		''' Отображение имени пользователя и адреса почты
		''' </summary>
		''' <remarks></remarks>
		Public HideUserName As Integer
		''' <summary>
		''' Рейтинг цензуры
		''' </summary>
		''' <remarks></remarks>
		Public Nsfw As Integer
		''' <summary>
		''' Пароль для удаления постов
		''' </summary>
		''' <remarks></remarks>
		Public Password As String
	End Structure

	Public Structure QueryBoardInfo
		Public BoardName As String
		Public PageNumber As Long
		Public ThreadNumber As Long
	End Structure

	Public Structure WebConfigSettings
		Public SiteName As String
		Public SiteSlogan As String
		Public SecretKey As String
		Public LoginLinkEnabled As Boolean
		Public DefaultLinkEnabled As Boolean
		Public FaqLinkEnabled As Boolean
		Public NewsLinkEnabled As Boolean
		Public RulesLinkEnabled As Boolean
		Public TermsLinkEnabled As Boolean
		Public CaptchaChars As String
		Public SendNewPostToEmailEnabled As Boolean
		Public SendNewPostToEmailAddress As String
		Public CreateNewThreadText As String
		Public AnswerToThreadText As String
		Public UserNameMustExists As Boolean
		Public DefaultSubject As String
		Public FooterTopText As String
		Public FooterBottomText As String
		Public DefaultLinkText As String
		Public RulesLinkText As String
		Public FaqLinkText As String
		Public NewsLinkText As String
		Public TermsLinkText As String
	End Structure

	''' <summary>
	''' Возвращает объект настроек из печенья
	''' </summary>
	''' <param name="Cookies"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetCookieSettings(ByVal Cookies As HttpCookieCollection) As CookieSettings
		'Private Const ThemeCookie As String = "t"
		'Private Const HideLeftFrameCookie As String = "l"
		'Private Const MoveToThreadCookie As String = "m"
		'Private Const PostFormPositionCookie As String = "f"
		'Private Const ShowRulesCookie As String = "r"
		'Private Const HideUserNameCookie As String = "u"
		'Private Const NsfwCookie As String = "n"
		'Private Const PasswordCookie As String = "p"
		Dim objSettings As CookieSettings
		With objSettings
			.Password = String.Empty
		End With

		' Тема
		Dim cookie As HttpCookie = Cookies.Get(ThemeCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.Theme = intValue
			End If
		End If

		' Левый фрейм
		cookie = Cookies.Get(HideLeftFrameCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.HideLeftFrame = intValue
			End If
		End If

		' Перейти к треду
		cookie = Cookies.Get(MoveToThreadCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.MoveToThread = intValue
			End If
		End If

		' Положение формы отправки сообщения
		cookie = Cookies.Get(PostFormPositionCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.PostFormPosition = intValue
			End If
		End If

		' Показывать правила
		cookie = Cookies.Get(ShowRulesCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.ShowRules = intValue
			End If
		End If

		' Скрывать имя пользователя
		cookie = Cookies.Get(HideUserNameCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.HideUserName = intValue
			End If
		End If
		'Private Const PasswordCookie As String = "p"

		' NSFW
		cookie = Cookies.Get(NsfwCookie)
		If cookie IsNot Nothing Then
			Dim strValue As String = cookie.Value
			Dim intValue As Integer
			If Integer.TryParse(strValue, intValue) Then
				objSettings.Nsfw = intValue
			End If
		End If

		' Пароль
		cookie = Cookies.Get(PasswordCookie)
		If cookie IsNot Nothing Then
			objSettings.Password = cookie.Value
		End If

		Return objSettings
	End Function

	Public Shared Function SetCookieSetthing(ByVal objSettings As CookieSettings) As List(Of HttpCookie)
		Dim colCookies As New List(Of HttpCookie)
		'Private Const ThemeCookie As String = "t"
		'Private Const HideLeftFrameCookie As String = "l"
		'Private Const MoveToThreadCookie As String = "m"
		'Private Const PostFormPositionCookie As String = "f"
		'Private Const ShowRulesCookie As String = "r"
		'Private Const HideUserNameCookie As String = "u"
		'Private Const NsfwCookie As String = "n"
		'Private Const PasswordCookie As String = "p"

		Dim objCookie As New HttpCookie(ThemeCookie, objSettings.Theme.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(HideLeftFrameCookie, objSettings.HideLeftFrame.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(MoveToThreadCookie, objSettings.MoveToThread.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(PostFormPositionCookie, objSettings.PostFormPosition.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(ShowRulesCookie, objSettings.ShowRules.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(HideUserNameCookie, objSettings.HideUserName.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(NsfwCookie, objSettings.Nsfw.ToString)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		objCookie = New HttpCookie(PasswordCookie, objSettings.Password)
		objCookie.Expires = Date.Now.AddMonths(1)
		colCookies.Add(objCookie)

		' Вернуть печенье
		Return colCookies
	End Function

	Public Shared Function GetQueryBoardInfo() As QueryBoardInfo
		Dim q As QueryBoardInfo
		Dim intThreadNumber As Long
		Dim intPageNumber As Long
		With q
			.BoardName = HttpContext.Current.Request.QueryString.Item("board")

			If HttpContext.Current.Request.QueryString.Item("thread") IsNot Nothing Then
				Long.TryParse(HttpContext.Current.Request.QueryString.Item("thread"), intThreadNumber)
			End If
			.ThreadNumber = intThreadNumber

			If HttpContext.Current.Request.QueryString.Item("page") IsNot Nothing Then
				Long.TryParse(HttpContext.Current.Request.QueryString.Item("page"), intPageNumber)
			End If
			.PageNumber = Math.Abs(intPageNumber)
		End With
		Return q
	End Function

	''' <summary>
	''' Возвращает строку с IP-адресом отправителя или его полный адрес назначения в base64
	''' </summary>
	''' <returns>Строка с IP-адресом</returns>
	''' <remarks></remarks>
	Public Shared Function GetIpAddress(ByVal objRequest As HttpRequest) As String
		Dim strAddress As String = objRequest.UserHostAddress
		If strAddress = "127.0.0.1" OrElse strAddress = "0000:0000:0000:0000:0000:0000:0000:0001" Then
			Dim Destb64 As String = objRequest.Headers.Item("X-I2P-DestHash")
			If Not String.IsNullOrEmpty(Destb64) Then
				Return Destb64
			End If
		End If
		Return strAddress
	End Function

	''' <summary>
	''' Применяет тему для страницы
	''' </summary>
	''' <param name="objPage">Страница для применения темы</param>
	''' <param name="intTheme">Номер темы</param>
	''' <remarks></remarks>
	Public Shared Sub ApplyTheme(ByVal objPage As Page, ByVal intTheme As Integer)
		Select Case intTheme
			Case 1
				objPage.Theme = "Burichan"
			Case 2
				objPage.Theme = "Futaba"
			Case 3
				objPage.Theme = "Gurochan"
			Case 4
				objPage.Theme = "Muon"
			Case 5
				objPage.Theme = "Neutron"
			Case 6
				objPage.Theme = "Photon"
			Case 7
				objPage.Theme = "Electron"
			Case 8
				objPage.Theme = "Terminal"
			Case Else
				objPage.Theme = "Photon"
		End Select
	End Sub

	''' <summary>
	''' Возвращает имя сайта из конфигурации
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetSiteName() As WebConfigSettings
		' Get the configuration object to access the related Web.config file.
		Dim config As Configuration = WebConfigurationManager.OpenWebConfiguration(ConfigPath)

		' Get the object related to the <identity> section.
		Dim section As AppSettingsSection = config.AppSettings
		Dim wc As WebConfigSettings
		With wc
			.SiteName = section.Settings.Item("SiteName").Value
			.SiteSlogan = section.Settings.Item("SiteSlogan").Value
			.SecretKey = section.Settings.Item("SecretKey").Value
			.LoginLinkEnabled = Boolean.Parse(section.Settings.Item("EnableLoginLink").Value)
			.DefaultLinkEnabled = Boolean.Parse(section.Settings.Item("EnableDefaultLink").Value)
			.FaqLinkEnabled = Boolean.Parse(section.Settings.Item("EnableFaqLink").Value)
			.NewsLinkEnabled = Boolean.Parse(section.Settings.Item("EnableNewsLink").Value)
			.RulesLinkEnabled = Boolean.Parse(section.Settings.Item("EnableRulesLink").Value)
			.TermsLinkEnabled = Boolean.Parse(section.Settings.Item("EnableTermsLink").Value)
			.CaptchaChars = section.Settings.Item("CartchaChars").Value
			.SendNewPostToEmailEnabled = Boolean.Parse(section.Settings.Item("EnableSendNewMessagesToEmail").Value)
			.SendNewPostToEmailAddress = section.Settings.Item("SendNewMessagesEmailAddress").Value
			.CreateNewThreadText = section.Settings.Item("CreateNewThreadText").Value
			.AnswerToThreadText = section.Settings.Item("AnswerToThreadText").Value
			.UserNameMustExists = Boolean.Parse(section.Settings.Item("UserNameMustExists").Value)
			.DefaultSubject = section.Settings.Item("DefaultSubject").Value
			.FooterTopText = section.Settings.Item("FooterTopText").Value
			.FooterBottomText = section.Settings.Item("FooterBottomText").Value
			.DefaultLinkText = section.Settings.Item("DefaultLinkText").Value
			.RulesLinkText = section.Settings.Item("RulesLinkText").Value
			.FaqLinkText = section.Settings.Item("FaqLinkText").Value
			.NewsLinkText = section.Settings.Item("NewsLinkText").Value
			.TermsLinkText = section.Settings.Item("TermsLinkText").Value
		End With
		Return wc
	End Function

	''' <summary>
	''' Генерирует случайную строку размером в 20 символов
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GenerateRandomString() As String
		Const TextChars As String = "zxcvbnmasdfghjklqwertyuiopZXCVBNMASDFGHJKLQWERTYUIOP0123456789"
		Dim objRandom As New Random
		Dim q = Enumerable.Range(0, 19).Select(Function(n) TextChars.Chars(objRandom.Next(TextChars.Length)))
		Return New String(q.ToArray)
	End Function

	''' <summary>
	''' Удаляет сообщение
	''' </summary>
	''' <param name="BoardName"></param>
	''' <param name="DeleteMessages"></param>
	''' <param name="Password"></param>
	''' <param name="FilesOnly"></param>
	''' <param name="blnAdmin"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function RemoveMessages(ByVal BoardName As String, _
	 ByVal DeleteMessages As String, _
	 ByVal Password As String, _
	 ByVal FilesOnly As Boolean, _
	 ByVal blnAdmin As Boolean) As ErrorInfo
		Dim objThread As ImageBoardThreadBase = GetBoardThread()
		objThread.BoardName = BoardName
		' удаляем отмеченные треды на основе прав пользователя
		For Each strMessageNumber As String In Regex.Split(DeleteMessages, ",")
			' Переводим номер сообщения в число
			Dim intMessageNumber As Long
			Long.TryParse(strMessageNumber, intMessageNumber)
			If intMessageNumber > 0 Then
				objThread.RemoveMessage(intMessageNumber, Password, FilesOnly, blnAdmin)
			End If
		Next
		Return New ErrorInfo
	End Function

	'''' <summary>
	'''' Создаёт папки для раздела
	'''' </summary>
	'''' <param name="BoardName"></param>
	'''' <returns></returns>
	'''' <remarks></remarks>
	'Public Shared Function CreateBoardBase(ByVal dbBoards As SQLiteConnection, _
	'ByVal BoardName As String) As Integer
	'	' Папка с файлами
	'	Dim strImageFolder As String = Path.Combine(HttpContext.Current.Server.MapPath(ResFolder), BoardName)
	'	' Если не существует, то создаём
	'	If Not Directory.Exists(strImageFolder) Then
	'		Directory.CreateDirectory(strImageFolder)
	'	End If
	'End Function

	''' <summary>
	''' Возвращает объект треда
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function GetBoardThread() As ImageBoardThreadBase
		' Прочитать файл конфигурации и вернуть соответствующий объект
		Dim config As ImageboardDatabseProviderConfigSectionHandler = System.Configuration.ConfigurationManager.GetSection("imageBoardDatabaseProviders/imageBoardDatabaseProvider")
		Dim objThread As ImageBoardThreadBase = Activator.CreateInstance(Type.GetType(config.type))
		objThread.ApplicationName = config.ApplicationName
		objThread.ResourceDirectory = HttpContext.Current.Server.MapPath(config.ResFolder)
		objThread.ConnectionString = WebConfigurationManager.ConnectionStrings(config.ConnectionString).ConnectionString
		Return objThread
	End Function

	'''' <summary>
	'''' Повышает уровень блокировки чтения до блокировки записи
	'''' </summary>
	'''' <returns></returns>
	'''' <remarks></remarks>
	'Public Shared Function UpgradeToWriterLock() As LockCookie
	'	Dim objBaseLock As ReaderWriterLock = CType(HttpContext.Current.Application.Item("BoardBaseLock"), ReaderWriterLock)
	'	Return objBaseLock.UpgradeToWriterLock(Timeout.Infinite)
	'End Function

	'''' <summary>
	'''' Вобзвращает состояние потока к тому, которое было до вызова повышения уровня блокировки
	'''' </summary>
	'''' <param name="Value"></param>
	'''' <remarks></remarks>
	'Public Shared Sub DowngradeFromWriterLock(ByVal Value As LockCookie)
	'	Dim objBaseLock As ReaderWriterLock = CType(HttpContext.Current.Application.Item("BoardBaseLock"), ReaderWriterLock)
	'	objBaseLock.DowngradeFromWriterLock(Value)
	'End Sub

	''' <summary>
	''' Проверяет права пользователя
	''' </summary>
	''' <param name="FlagName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function ValidateRoles(ByVal UserName As String, ByVal FlagName As MethodsManage, ByVal BoardName As String) As Boolean
		If Roles.IsUserInRole(UserName, "Зой") Then
			Return True
		Else
			Return Roles.IsUserInRole(UserName, FlagName.ToString)
		End If
		'Private Function ValidateBoard(ByVal dbUsers As SQLite.SQLiteConnection, _
		'ByVal BoardName As String, _
		'ByVal FlagName As String) As Boolean
		'	Dim objCommand As SQLite.SQLiteCommand = New SQLite.SQLiteCommand(String.Format("SELECT COUNT(*) FROM ""AdminBoards"" WHERE ""BoardName""=""{0}"" AND ""FlagId""=""{1}""", BoardName, FlagName), dbUsers)
		'	Dim objReader As SQLite.SQLiteDataReader = objCommand.ExecuteReader
		'	objReader.Read()
		'	ValidateBoard = Convert.ToBoolean(objReader.Item(0))
		'	objReader.Close()
		'	objCommand.Dispose()
		'End Function
		'Const Comand As String = "SELECT ""FlagName"" FROM ""AdminFlags"" WHERE ""RoleName""=(SELECT ""RoleName"" FROM ""Roles"" WHERE ""Enabled""=1 AND ""RoleName""=(SELECT ""RoleName"" FROM ""UsersInRoles"" WHERE ""UserName""=(SELECT ""UserName"" FROM ""Users"" WHERE ""Enabled""=1 AND ""UserName""=""{0}"")))"
		'Dim objCommand As SQLite.SQLiteCommand
		'Dim colFlags As List(Of MethodsManage) = New List(Of MethodsManage)
		'Dim dbUsers As SQLite.SQLiteConnection = OpenBoardBase(True)
		'' Получаем все флаги групп пользователя
		'objCommand = New SQLite.SQLiteCommand(String.Format(Comand, GetSafeString(UserName)), dbUsers)
		'Dim objReader As SQLite.SQLiteDataReader = objCommand.ExecuteReader
		'Do While objReader.Read()
		'	colFlags.Add(objReader.Item(0))
		'Loop
		'objReader.Close()
		'objCommand.Dispose()
		'' Зой имеет все права, поэтому можно сразу возвращать True
		'' если в списке есть флаг зоя
		'If colFlags.Contains(MethodsManage.Зой) Then
		'	ValidateRoles = True
		'Else
		'	Select Case FlagName
		'		'Case MethodsManage.Зой
		'		'	ValidateRoles = colFlags.Contains(MethodsManage.Зой)
		'		Case MethodsManage.Бот
		'			ValidateRoles = colFlags.Contains(MethodsManage.Бот)
		'		Case MethodsManage.Ролевик
		'			ValidateRoles = colFlags.Contains(MethodsManage.Ролевик)

		'		Case MethodsManage.ДваждыАдминистратор
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыАдминистратор)
		'		Case MethodsManage.Администратор
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыАдминистратор)
		'			If Not ValidateRoles Then
		'				If colFlags.Contains(MethodsManage.Администратор) Then
		'					If BoardName.Length > 0 Then
		'						ValidateRoles = ValidateBoard(dbUsers, BoardName, MethodsManage.Администратор)
		'					End If
		'				End If
		'			End If

		'		Case MethodsManage.ДваждыВыверяющ
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыВыверяющ)
		'		Case MethodsManage.Выверяющ
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыВыверяющ)
		'			If Not ValidateRoles Then
		'				If colFlags.Contains(MethodsManage.Выверяющ) Then
		'					If BoardName.Length > 0 Then
		'						ValidateRoles = ValidateBoard(dbUsers, BoardName, MethodsManage.Выверяющ)
		'					End If
		'				End If
		'			End If

		'		Case MethodsManage.ДваждыДосмот
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыДосмот)
		'		Case MethodsManage.Досмот
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыДосмот)
		'			If Not ValidateRoles Then
		'				If colFlags.Contains(MethodsManage.Досмот) Then
		'					If BoardName.Length > 0 Then
		'						ValidateRoles = ValidateBoard(dbUsers, BoardName, MethodsManage.Досмот)
		'					End If
		'				End If
		'			End If

		'		Case MethodsManage.ДваждыРедактор
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыРедактор)
		'		Case MethodsManage.Редактор
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыРедактор)
		'			If Not ValidateRoles Then
		'				If colFlags.Contains(MethodsManage.Редактор) Then
		'					If BoardName.Length > 0 Then
		'						ValidateRoles = ValidateBoard(dbUsers, BoardName, MethodsManage.Редактор)
		'					End If
		'				End If
		'			End If

		'		Case MethodsManage.Спамер
		'			ValidateRoles = colFlags.Contains(MethodsManage.Спамер)

		'		Case MethodsManage.ДваждыКляузник
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыКляузник)
		'		Case MethodsManage.Кляузник
		'			ValidateRoles = colFlags.Contains(MethodsManage.ДваждыКляузник)
		'			If Not ValidateRoles Then
		'				If colFlags.Contains(MethodsManage.Кляузник) Then
		'					If BoardName.Length > 0 Then
		'						ValidateRoles = ValidateBoard(dbUsers, BoardName, MethodsManage.Кляузник)
		'					End If
		'				End If
		'			End If
		'	End Select
		'End If
		'CloseBoardBase(dbUsers, True)

	End Function

End Class
