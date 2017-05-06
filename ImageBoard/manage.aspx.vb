Imports System.Web.Configuration
Imports ImageboardUtils
Imports NemibaWebAPI

Partial Class internetscreator
	Inherits System.Web.UI.Page

	Protected m_UserName As String = String.Empty

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim strSiteName As WebConfigSettings = GetSiteName()
		If Not String.IsNullOrEmpty(strSiteName.SiteName) Then
			Me.Title = strSiteName.SiteName & " » Управление"
		End If
	End Sub

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		ApplyTheme(Me, GetCookieSettings(Request.Cookies).Theme)
	End Sub

End Class
