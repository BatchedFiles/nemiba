Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI

	<WebMethod(Description:="Добавляет группу в запрещённые для ответов на разделе"), SoapHeader("sHeader")> _
	Public Function AddRoleToAnswersBoardDeny(ByVal BoardName As String, _
	  ByVal RoleName As String, _
	  ByVal DenyForReading As Boolean) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.AddRoleToAnswersBoardDeny(RoleName, DenyForReading)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Удаляет группу из запрещённых для ответов на разделе"), SoapHeader("sHeader")> _
	Public Function RemoveRoleFromAnswersBoardDeny(ByVal BoardName As String, _
	  ByVal RoleName As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveRoleFromAnswersBoardDeny(RoleName)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Изменяет группу из запрещённых для ответов на разделе"), SoapHeader("sHeader")> _
	Public Function EditRoleFromAnswersBoardDeny(ByVal BoardName As String, _
	  ByVal RoleName As String, _
	  ByVal DenyForReading As Boolean) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.EditRoleFromAnswersBoardDeny(RoleName, DenyForReading)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Получает список запрещённых групп для ответов на разделе"), SoapHeader("sHeader")> _
	Public Function GetRolesFromAnswersBoardDenyList(ByVal BoardName As String) As List(Of DenyRolesForBoardAnswersInfo)
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			Return objBoard.GetRolesFromAnswersBoardDenyList
		Else
			Return New List(Of DenyRolesForBoardAnswersInfo)
		End If
	End Function

	<WebMethod(Description:="Добавляет группу в список запрещённых для отправки файлов")> _
	Public Function AddRoleToFilesDeny(ByVal BoardName As String, ByVal MimeId As Long, _
	 ByVal RoleName As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.AddRoleToFilesDeny(MimeId, RoleName)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Удаляет группу из списка запрещённых для отправки файлов")> _
	Public Function RemoveRoleFromFilesDeny(ByVal BoardName As String, ByVal DenyId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.RemoveRoleFromFilesDeny(DenyId)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Получает список групп, запрещённых для отправки файлов")> _
	Public Function GetRolesFromFilesDenyList(ByVal BoardName As String) As List(Of DenyRolesForFilesInfo)
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			Return objBoard.GetRolesFromFilesDenyList
		Else
			Return New List(Of DenyRolesForFilesInfo)
		End If
	End Function

End Class
