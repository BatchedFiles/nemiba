Imports Microsoft.VisualBasic
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Создаёт запрещённое слово
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Создаёт шаблон фильтра"), SoapHeader("sHeader")> _
	Public Function CreateBadWord(ByVal BoardName As String, _
	  ByVal Template As String, _
	  ByVal ReplacementString As String, _
	  ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.CreateBadWord(Template, ReplacementString, ReplacementFlag)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Удаляет запрещённое слово
	''' </summary>
	''' <param name="BadWordNumber">Номер удаляемого запрещённого слова</param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Удаляет шаблон фильтра"), SoapHeader("sHeader")> _
	Public Function RemoveBadWord(ByVal BoardName As String, ByVal BadWordNumber As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.RemoveBadWord(BadWordNumber)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Изменяет запрещённое слово
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Изменяет шаблон фильтра"), SoapHeader("sHeader")> _
	Public Function EditBadWord(ByVal BadWordNumber As Long, _
	 ByVal BoardName As String, _
	 ByVal Template As String, _
	 ByVal ReplacementString As String, _
	 ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objErrorInfo = objBoard.EditBadWord(BadWordNumber, Template, ReplacementString, ReplacementFlag)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	''' <summary>
	''' Получаем список запрещённых слов
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает список шаблонов фильтра"), SoapHeader("sHeader")> _
	Public Function GetBadWordList(ByVal BoardName As String) As List(Of BadWordInfo)
		Dim colBoards As List(Of BadWordInfo)
		If ValidateUserRoles(MethodsManage.Досмот, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			Return objBoard.GetBadWordList
		Else
			colBoards = New List(Of BadWordInfo)
		End If
		Return colBoards
	End Function

End Class
