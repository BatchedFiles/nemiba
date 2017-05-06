Imports Microsoft.VisualBasic
Imports ImageboardUtils
Imports System.Threading
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Флаги VCard
	''' </summary>
	''' <remarks></remarks>
	Public Enum VCardFlags
		RealName
	End Enum

	''' <summary>
	''' Дополнительная информация о пользователе
	''' </summary>
	''' <remarks></remarks>
	Public Structure VCardInfo
		Public Key As VCardFlags
		Public Value As String
	End Structure

	''' <summary>
	''' Информация о группе
	''' </summary>
	''' <remarks></remarks>
	Public Structure RoleInfo
		Public RoleName As Long
		Public Description As String
		Public Enabled As Boolean
	End Structure

	''' <summary>
	''' Флаги групп
	''' </summary>
	''' <remarks></remarks>
	Public Structure FlagInfo
		''' <summary>
		''' Идентификатор флага
		''' </summary>
		''' <remarks></remarks>
		Public FlagID As Long
		''' <summary>
		''' Имя флага
		''' </summary>
		''' <remarks></remarks>
		Public FlagName As MethodsManage
		''' <summary>
		''' Управляемая флагом группа
		''' </summary>
		''' <remarks></remarks>
		Public GroupId As Long
	End Structure

	''' <summary>
	''' Список разделов для управления на основе флагов
	''' </summary>
	''' <remarks></remarks>
	Public Structure BoardFlagInfo
		''' <summary>
		''' Идентификатор флага
		''' </summary>
		''' <remarks></remarks>
		Public FlagId As Long
		''' <summary>
		''' Управляемый флагом раздел
		''' </summary>
		''' <remarks></remarks>
		Public BoardName As String
	End Structure

	''' <summary>
	''' Флаги прав групп
	''' </summary>
	''' <remarks></remarks>
	Public Enum MethodsManage
		''' <summary>
		''' Полное управление сайтом, флаг зоя
		''' </summary>
		''' <remarks></remarks>
		Зой
		''' <summary>
		''' Бот (зарезервировано)
		''' </summary>
		''' <remarks></remarks>
		Бот
		''' <summary>
		''' Может править пользователями и группами
		''' </summary>
		''' <remarks></remarks>
		Ролевик
		''' <summary>
		''' Администратор может менять настройки всех разделов
		''' </summary>
		''' <remarks></remarks>
		ДваждыАдминистратор
		''' <summary>
		''' Администратор может менять настройки раздела из списка
		''' </summary>
		''' <remarks></remarks>
		Администратор
		''' <summary>
		''' Мобератор может править треды и их параметры на всех разделах
		''' </summary>
		''' <remarks></remarks>
		ДваждыВыверяющ
		''' <summary>
		''' Мобератор может править треды и их параметры на разделах из списка
		''' </summary>
		''' <remarks></remarks>
		Выверяющ
		''' <summary>
		''' Мобератор может банить пользователей на всех разделах
		''' </summary>
		''' <remarks></remarks>
		ДваждыДосмот
		''' <summary>
		''' Мобератор может банить пользователей на досках из списка
		''' </summary>
		''' <remarks></remarks>
		Досмот
		''' <summary>
		''' Добавление новостей, фака и правил на глагне
		''' </summary>
		''' <remarks></remarks>
		ДваждыРедактор
		''' <summary>
		''' Создание тредов на системных разделах
		''' </summary>
		''' <remarks></remarks>
		Редактор
		''' <summary>
		''' Мобератор может создавать инвайты
		''' </summary>
		''' <remarks></remarks>
		Спамер
		''' <summary>
		''' Может читать и удалять список жалоб на всех разделах
		''' </summary>
		''' <remarks></remarks>
		ДваждыКляузник
		''' <summary>
		''' Может читать и удалять список жалоб на разделах из списка
		''' </summary>
		''' <remarks></remarks>
		Кляузник
	End Enum

	''' <summary>
	''' Удаляет регистрацию пользователя
	''' </summary>
	''' <param name="UserName"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаляет пользователя"), SoapHeader("sHeader")> _
	Public Function UnregisterUser(ByVal UserName As String) As ErrorInfo
		UnregisterUser = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Membership.DeleteUser(UserName)
		End If
	End Function

	''' <summary>
	''' UNDONE Создание инвайта
	''' </summary>
	''' <returns>Ссылка с возможностью залогиниться</returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Создаёт инвайт пользователя"), SoapHeader("sHeader")> _
	Public Function CreateInvite(ByVal BoardName As String) As String
		' Генерируем имя пользователя
		CreateInvite = GenerateRandomString()
		If ValidateUserRoles(MethodsManage.Спамер, BoardName) Then
			' Печенье
			Dim strCookie As String = GenerateRandomString()
			' Пароль
			Dim strPassword As String = GenerateRandomString()
			'Dim dbUsers As SQLite.SQLiteConnection = OpenBoardBase(False)
			'Dim objCommand As SQLite.SQLiteCommand = New SQLite.SQLiteCommand(String.Format("INSERT INTO ""Users"" (""UserName"", ""Password"", ""Cookie"", ""CookieExpires"") VALUES (""{0}"", ""{1}"", ""{2}"", {3})", CreateInvite, GetHash(strPassword), GetHash(strCookie), Date.Now.AddMinutes(GetSessionTime).Ticks), dbUsers)
			'objCommand.ExecuteNonQuery()
			'objCommand.Dispose()
			'' Добавление группы
			'objCommand = New SQLite.SQLiteCommand(String.Format("INSERT INTO ""UsersToGroups"" (""UserName"", ""GroupId"") VALUES (""{0}"", 2)", CreateInvite), dbUsers)
			'objCommand.ExecuteNonQuery()
			'objCommand.Dispose()
			'CloseBoardBase(dbUsers, False)
			' Формирование ссылки
			Return "manage.aspx?" & HttpUtility.HtmlEncode(String.Format("user={0}&password={1}", CreateInvite, strPassword))
		End If
	End Function

	''' <summary>
	''' Получение списка пользователей
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает список пользователей"), SoapHeader("sHeader")> _
	Public Function GetUserList() As List(Of MembershipUser)
		Dim colUsers As List(Of MembershipUser) = New List(Of MembershipUser)
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			For Each objUser As MembershipUser In Membership.GetAllUsers
				colUsers.Add(objUser)
			Next
		End If
		Return colUsers
	End Function

	<WebMethod(Description:="Получает список групп"), SoapHeader("sHeader")> _
 Public Function GetGroupList() As RoleInfo()
		Dim colUsers As List(Of RoleInfo) = New List(Of RoleInfo)
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			colUsers.AddRange(Roles.GetAllRoles)
		End If
		Return colUsers.ToArray
	End Function

	<WebMethod(Description:="Получает список групп пользователя"), SoapHeader("sHeader")> _
 Public Function GetUserGroupList(ByVal UserName As String) As List(Of RoleInfo)
		Dim colUsers As List(Of RoleInfo) = New List(Of RoleInfo)
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			colUsers.AddRange(Roles.GetRolesForUser(UserName))
		End If
		Return colUsers
	End Function

	<WebMethod(Description:="Создание группы"), SoapHeader("sHeader")> _
 Public Function CreateGroup(ByVal RoleName As String) As ErrorInfo
		CreateGroup = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Roles.CreateRole(RoleName)
		End If
	End Function

	<WebMethod(Description:="Удаление группы"), SoapHeader("sHeader")> _
 Public Function RemoveGroup(ByVal RoleName As String) As ErrorInfo
		RemoveGroup = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Roles.DeleteRole(RoleName, True)
		End If
	End Function

	<WebMethod(Description:="Добавление пользователя к группе"), SoapHeader("sHeader")> _
  Public Function AddUserToGroup(ByVal UserName As String, ByVal RoleName As String) As ErrorInfo
		AddUserToGroup = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Roles.AddUserToRole(UserName, RoleName)
		End If
	End Function

	<WebMethod(Description:="Удаление пользователя из группы"), SoapHeader("sHeader")> _
  Public Function RemoveUserFromGroup(ByVal UserName As String, ByVal RoleName As String) As ErrorInfo
		RemoveUserFromGroup = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Roles.RemoveUserFromRole(UserName, RoleName)
		End If
	End Function

	<WebMethod(Description:="Получение списка пользователей группы"), SoapHeader("sHeader")> _
  Public Function GetUserListFromGroup(ByVal RoleName As String) As List(Of String)
		GetUserListFromGroup = New List(Of String)
		If ValidateUserRoles(MethodsManage.Ролевик, String.Empty) Then
			Roles.GetUsersInRole(RoleName)
		End If
	End Function
End Class
