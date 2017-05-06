Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports System.Security.Principal
Imports I2P

''' <summary>
''' Класс заголовка SOAP
''' </summary>
''' <remarks></remarks>
Public Class Authenticator
	Inherits SoapHeader
	Public UserName As String
	Public Password As String
End Class

<WebService(Namespace:="http://sethi.su/", Description:="Функции управления двигателем имиджборды Немиба")> _
Public Class NemibaWebAPI
	Inherits System.Web.Services.WebService

	''' <summary>
	''' Заголовки SOAP для аутентификации
	''' </summary>
	''' <remarks></remarks>
	Public sHeader As Authenticator

	<WebMethod(Description:="Создание раздела в упрощённом виде"), SoapHeader("sHeader")> _
	Public Function CreateBoardSimple(ByVal BoardName As String, _
   ByVal Description As String, _
   ByVal MenuGroupId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Зой, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.CreateBoard(Description, MenuGroupId)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Обновление описания раздела"), SoapHeader("sHeader")> _
	Public Function SetBoardDescription(ByVal BoardName As String, _
   ByVal Description As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo

		If ValidateUserRoles(MethodsManage.Зой, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.SetBoardDescription(Description)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Отметка сообщения как проверенного"), SoapHeader("sHeader")> _
	Public Function CheckMessage(ByVal BoardName As String, _
   ByVal MessageNumber As Long, _
   ByVal NsfwRating As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.CheckMessage(MessageNumber)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Получение списка архивных тредов")> _
	Public Function GetArchiveThreadList(ByVal BoardName As String, _
   ByVal PageNumber As Long) As List(Of ThreadInfo)
		GetArchiveThreadList = New List(Of ThreadInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		Return objBoard.GetThreadList(PageNumber, True)
	End Function

	<WebMethod(Description:="Удаление треда из архива"), SoapHeader("sHeader")> _
	Public Function RemoveArchiveThread(ByVal BoardName As String, _
 ByVal ThreadNumber As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Выверяющ, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveArchiveThread(ThreadNumber)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Проверяет права пользователя
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function ValidateUserRoles(ByVal intMethod As MethodsManage, ByVal strBoardName As String) As Boolean
		' Получаем информацию о пользователе
		If sHeader Is Nothing Then
			' Получить пользователя из сессии
			Dim rp As RolePrincipal = User
			If rp.Identity.IsAuthenticated Then
				Return ValidateRoles(rp.Identity.Name, intMethod, strBoardName)
			End If
		Else
			' Получить пользователя из заголовков SOAP
			If Not String.IsNullOrEmpty(sHeader.UserName) Then
				If Membership.ValidateUser(sHeader.UserName, sHeader.Password) Then
					Dim objUser As MembershipUser = Membership.GetUser(sHeader.UserName)
					Return ValidateRoles(objUser.UserName, intMethod, strBoardName)
				End If
			End If
		End If
		Return False
	End Function

End Class
