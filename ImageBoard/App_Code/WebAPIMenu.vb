Imports Microsoft.VisualBasic
Imports ImageboardUtils
Imports System.Web.Services.Protocols
Imports System.Web.Services
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Получаем список меню
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает список пунктов меню"), SoapHeader("sHeader")> _
	Public Function GetMenuList() As List(Of MenuInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		Return objBoard.GetMenuList
	End Function

	''' <summary>
	''' Создаёт пункт меню
	''' </summary>
	''' <param name="MenuText">Отображаемое имя группы меню</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Создаёт пункт меню"), SoapHeader("sHeader")> _
	Public Function CreateMenu(ByVal MenuText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, String.Empty) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.CreateMenu(MenuText)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Удаляет пункт меню
	''' </summary>
	''' <param name="MenuNumber">Номер пункта меню</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаляет пункт меню"), SoapHeader("sHeader")> _
	Public Function RemoveMenu(ByVal MenuNumber As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, String.Empty) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.RemoveMenu(MenuNumber)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Изменяет пункт меню
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Изменяет меню"), SoapHeader("sHeader")> _
	Public Function EditMenu(ByVal MenuNumber As Long, ByVal MenuText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, String.Empty) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.EditMenu(MenuNumber, MenuText)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

End Class
