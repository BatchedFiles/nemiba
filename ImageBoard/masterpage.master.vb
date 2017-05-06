Imports System.Xml
Imports ImageboardUtils
Imports System.Threading
Imports I2P

Partial Class MasterPage
	Inherits System.Web.UI.MasterPage

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		' Проверяем печенье
		Dim objSettings As CookieSettings = GetCookieSettings(Request.Cookies)
		If objSettings.HideLeftFrame > 1 Then
			mnuLeft.Visible = True
			Article.Attributes.Add("class", "article")
		End If
		' Имя раздела
		Dim BoardName As String = Request.QueryString.Item("board")

		Dim objBoardThread As ImageBoardThreadBase = GetBoardThread()

		' Список меню
		Dim lstMenuList As List(Of MenuInfo)
		Dim oMenu As Object = Application("Menu")
		If oMenu Is Nothing Then
			lstMenuList = objBoardThread.GetMenuList
			Application("Menu") = lstMenuList
		Else
			lstMenuList = CType(oMenu, List(Of MenuInfo))
		End If

		' Список разделов
		Dim lstBoards As List(Of BoardInfo)
		Dim oBoards As Object = Application("Boards")
		If oBoards Is Nothing Then
			lstBoards = objBoardThread.GetBoardsInfoList()
			Application("Boards") = lstBoards
		Else
			lstBoards = CType(oBoards, List(Of BoardInfo))
		End If

		' Коллекция разделов (название + описание), принадлежащих одному пункту меню
		Dim colBoards As New SortedDictionary(Of Long, SortedDictionary(Of String, String))
		For Each objMenu As MenuInfo In lstMenuList
			colBoards.Add(objMenu.MenuNumber, New SortedDictionary(Of String, String))
		Next

		' Выборка всех разделов из базы данных
		For Each objBoard As BoardInfo In lstBoards
			If colBoards.ContainsKey(objBoard.MenuGroupId) Then
				' Если раздел не скрыт
				If Not Convert.ToBoolean(objBoard.IsHidden) Then
					' Добавляем в коллекцию
					colBoards.Item(objBoard.MenuGroupId).Add(objBoard.BoardName, objBoard.Description)
				End If
			End If
		Next

		For Each objMenu As MenuInfo In lstMenuList
			' Получили заголовок группы
			mnuHorizontal.Controls.Add(New LiteralControl("<span class=""navhorbrakets"">[</span>"))
			'mnuVertiсal.Controls.Add(New LiteralControl("<ul class=""navvertical""><li>" & objMenu.MenuText & "</li><li><ul>"))
			mnuLeft.Controls.Add(New LiteralControl("<ul class=""navvertical""><li>" & objMenu.MenuText & "</li><li><ul>"))

			For Each strBoardName As String In colBoards.Item(objMenu.MenuNumber).Keys
				Dim strDescription As String = colBoards.Item(objMenu.MenuNumber).Item(strBoardName)
				mnuHorizontal.Controls.Add(New LiteralControl(" "))

				'mnuVertiсal.Controls.Add(New LiteralControl("<li>"))
				mnuLeft.Controls.Add(New LiteralControl("<li>"))
				If BoardName = strBoardName Then
					mnuHorizontal.Controls.Add(New LiteralControl("<span class=""navhorizontal"">/" & strBoardName & "/</span>"))
					'mnuVertiсal.Controls.Add(New LiteralControl("/" & strBoardName & "/ — " & strDescription))
					mnuLeft.Controls.Add(New LiteralControl("/" & strBoardName & "/ — " & strDescription))
				Else
					Dim lnkUrlHorizontal As HyperLink = New HyperLink
					lnkUrlHorizontal.CssClass = "navhorizontal"
					lnkUrlHorizontal.NavigateUrl = "~/" & strBoardName & "/"
					lnkUrlHorizontal.ToolTip = strDescription
					lnkUrlHorizontal.Text = "/" & strBoardName & "/"
					mnuHorizontal.Controls.Add(lnkUrlHorizontal)

					Dim lnkUrlVertical As HyperLink = New HyperLink
					lnkUrlVertical.NavigateUrl = "~/" & strBoardName & "/"
					lnkUrlVertical.Text = "/" & strBoardName & "/ — " & strDescription
					'mnuVertiсal.Controls.Add(lnkUrlVertical)
					mnuLeft.Controls.Add(lnkUrlVertical)
				End If
				'mnuVertiсal.Controls.Add(New LiteralControl("</li>"))
				mnuLeft.Controls.Add(New LiteralControl("</li>"))
			Next
			mnuHorizontal.Controls.Add(New LiteralControl("<span class=""navhorbrakets""> ] </span>"))
			'mnuVertiсal.Controls.Add(New LiteralControl("</ul></li></ul>"))
			mnuLeft.Controls.Add(New LiteralControl("</ul></li></ul>"))
		Next

		' Устанавливаем пароль удаления файлов в печенье
		If String.IsNullOrEmpty(objSettings.Password) Then
			objSettings.Password = Guid.NewGuid.ToString
			For Each c In SetCookieSetthing(objSettings)
				Response.Cookies.Add(c)
			Next
		End If
	End Sub

End Class

