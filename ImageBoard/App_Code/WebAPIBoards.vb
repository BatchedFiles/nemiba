Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Удаляет раздел
	''' </summary>
	''' <param name="BoardName">Имя раздела</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаляет раздел"), SoapHeader("sHeader")> _
	Public Function RemoveBoard(ByVal BoardName As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveBoard()
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Получаем список разделов
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает список разделов"), SoapHeader("sHeader")> _
	Public Function GetBoardInfoList() As List(Of BoardInfo)
		' Нужно создать массив чтобы его потом вернуть
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		Return objBoard.GetBoardsInfoList
	End Function

	''' <summary>
	''' Получает параметры раздела
	''' </summary>
	''' <param name="BoardName">Имя раздела</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает параметры раздела"), SoapHeader("sHeader")> _
	Public Function GetBoardInfo(ByVal BoardName As String) As BoardInfo
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		Return objBoard.GetBoardInfo
	End Function

End Class
