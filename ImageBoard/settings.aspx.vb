Imports ImageboardUtils

Partial Class settings
	Inherits System.Web.UI.Page

	Private m_Settings As CookieSettings

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		' Настройки пользователя
		m_Settings = GetCookieSettings(Request.Cookies)
		If IsPostBack Then
			Dim intThemeNumber As Long
			Long.TryParse(Request.Form.Item("lstTheme"), intThemeNumber)
			m_Settings.Theme = intThemeNumber
		End If
		ApplyTheme(Me, m_Settings.Theme)
	End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim strSiteName As WebConfigSettings = GetSiteName()
		If Not String.IsNullOrEmpty(strSiteName.SiteName) Then
			Me.Title = strSiteName.SiteName & " » Настройки"
		End If
		If IsPostBack Then
			' Устанавливаем печенье
			' Скрытие левого фрейма
			m_Settings.HideLeftFrame = lstHideLeftFrame.SelectedValue
			' После ответа вернуться в разделу
			m_Settings.MoveToThread = lstMoveToThread.SelectedValue
			' Скрыть имя пользователя
			m_Settings.HideUserName = lstHideUserNameJabberTheme.SelectedValue
			' Положение формы ответа
			m_Settings.PostFormPosition = lstPostFormPosition.SelectedValue
			' Показывать правила
			m_Settings.ShowRules = lstShowRules.SelectedValue
			' Цензура
			m_Settings.Nsfw = lstShowNSFW.SelectedValue
			For Each c As HttpCookie In SetCookieSetthing(m_Settings)
				Response.Cookies.Add(c)
			Next
		End If
		' Скрытие левого фрейма
		lstHideLeftFrame.SelectedValue = m_Settings.HideLeftFrame
		' После ответа вернуться в разделу
		lstMoveToThread.SelectedValue = m_Settings.MoveToThread
		' Скрыть имя пользователя
		lstHideUserNameJabberTheme.SelectedValue = m_Settings.HideUserName
		'' Положение формы ответа
		lstPostFormPosition.SelectedValue = m_Settings.PostFormPosition
		' Показывать правила
		lstShowRules.SelectedValue = m_Settings.ShowRules
		' Цензура
		lstShowNSFW.SelectedValue = m_Settings.Nsfw
	End Sub

End Class
