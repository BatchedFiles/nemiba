Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Threading
Imports ImageboardUtils
Imports I2P

Partial Class NemibaWebAPI

	''' <summary>
	''' Получает список зарегистрированных типов файлов
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Получает список зарегистрированных типов файлов"), SoapHeader("sHeader")> _
	Public Function GetMimeList(ByVal BoardName As String) As List(Of MimeInfo)
		Dim objBoard As ImageBoardThreadBase = GetBoardThread()
		objBoard.BoardName = BoardName
		Return objBoard.GetMimeList
	End Function

	''' <summary>
	''' Создаёт миме
	''' </summary>
	''' <param name="Mime"></param>
	''' <param name="Extension"></param>
	''' <param name="BoardName"></param>
	''' <param name="MaxFileLength"></param>
	''' <param name="FileType"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	<WebMethod(Description:="Регистрирует тип файла"), SoapHeader("sHeader")> _
	Public Function CreateMime(ByVal Mime As String, _
	   ByVal Extension As String, _
	 ByVal BoardName As String, _
	 ByVal MaxFileLength As Long, _
	 ByVal FileType As FileType) As ErrorInfo
		Dim objErrorInfo As ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.CreateMime(Mime, Extension, MaxFileLength, FileType)
		Else
			objErrorInfo = New ErrorInfo
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Уладяет зарегистрированный тип файла"), SoapHeader("sHeader")> _
	Public Function RemoveMime(ByVal BoardName As String, ByVal MimeId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.RemoveMime(MimeId)
		Else
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

	<WebMethod(Description:="Изменяет зарегистрированный тип файла"), SoapHeader("sHeader")> _
	Public Function EditMime(ByVal MimeId As Long, ByVal Mime As String, _
	  ByVal Extension As String, _
	  ByVal BoardName As String, _
	  ByVal MaxFileLength As Long, _
	  ByVal FileType As FileType) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		If ValidateUserRoles(MethodsManage.Администратор, BoardName) Then
			Dim objBoard As ImageBoardThreadBase = GetBoardThread()
			objBoard.BoardName = BoardName
			objErrorInfo = objBoard.EditMime(MimeId, Mime, Extension, MaxFileLength, FileType)
		Else
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
			objErrorInfo.ErrorString = "Не положено"
		End If
		Return objErrorInfo
	End Function

End Class
