Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI
	<WebMethod(Description:="Создаёт правило раздела"), SoapHeader("sHeader")> _
	Public Function CreateRules(ByVal BoardName As String, ByVal RulesText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.CreateRules(RulesText)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Удаляет правило раздела"), SoapHeader("sHeader")> _
	Public Function RemoveRules(ByVal BoardName As String, ByVal RulesId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveRules(RulesId)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Изменяет правило раздела"), SoapHeader("sHeader")> _
	Public Function EditRules(ByVal BoardName As String, ByVal RulesId As Long, ByVal RulesText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.EditRules(RulesId, RulesText)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Получает список правил раздела"), SoapHeader("sHeader")> _
	Public Function GetRulesList(ByVal BoardName As String) As List(Of RulesInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		Return objBoard.GetRulesList
	End Function

End Class
