Partial Class errors
	Inherits System.Web.UI.Page

	''' <summary>
	''' Время ошибки, будет генерироваться случайно
	''' </summary>
	Public DateStamp As String
	''' <summary>
	''' Код ошибки
	''' </summary>
	Public ErrorCode As String
	''' <summary>
	''' Первый код ошибки
	''' </summary>
	Public ErrorCode1 As String
	''' <summary>
	''' Второй код ошибки
	''' </summary>
	Public ErrorCode2 As String
	''' <summary>
	''' Третий код ошибки
	''' </summary>
	Public ErrorCode3 As String
	''' <summary>
	''' Четвёртый код ошибки
	''' </summary>
	Public ErrorCode4 As String
	''' <summary>
	''' Строка с ошибкой
	''' </summary>
	Public ErrorString As String
	''' <summary>
	''' Имя страницы с ошибкой
	''' </summary>
	''' <remarks></remarks>
	Public ErrorPageString As String
	Public StackTrace As String
	Public SouceFile As String
	Public SourceFunction As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim objRandom As New Random
		Dim Status As String
		If Context.Error Is Nothing Then
			Dim Code As Integer
			Integer.TryParse(Request.QueryString.Item("code"), Code)

			Status = Request.QueryString.Item("status")
			If String.IsNullOrEmpty(Status) Then
				Status = Request.QueryString.Item("aspxerrorpath")
			End If

			Select Case Code
				Case ErrorType.AccessViolation
					ErrorCode = "0x0000009A"
					ErrorString = "SYSTEM_LICENSE_VIOLATION"
					Response.StatusCode = 418
					Response.StatusDescription = "I’m a teapot"
				Case ErrorType.ReadOnlyBoard
					ErrorCode = "0x000000C1"
					ErrorString = "ATTEMPTED_WRITE_TO_READONLY_MEMORY"
					Response.StatusCode = 403
				Case ErrorType.BoardNotFound, ErrorType.ThreadNotFound
					ErrorCode = "0x00000404"
					ErrorString = "PAGE_FAULT_IN_NONPAGED_AREA"
					Response.StatusCode = 404
				Case ErrorType.FileRequired
					ErrorCode = "0x0000007A"
					ErrorString = "KERNEL_DATA_INPAGE_ERROR"
					Response.StatusCode = 411
				Case ErrorType.DublicateFile
					ErrorCode = "0x0000003E"
					ErrorString = "MULTIPROCESSOR_CONFIGURATION_NOT_SUPPORTED"
					Response.StatusCode = 409
				Case ErrorType.CaptchaNotValid
					ErrorCode = "0x000000C4"
					ErrorString = "DRIVER_VERIFIER_DETECTED_VIOLATION"
					Response.StatusCode = 402
				Case ErrorType.MessageTooLong
					ErrorCode = "0x0000004D"
					ErrorString = "NO_PAGES_AVAILABLE"
					Response.StatusCode = 414
				Case ErrorType.TooManyFiles
					ErrorCode = "0x0000004D"
					ErrorString = "NO_PAGES_AVAILABLE"
					Response.StatusCode = 413
				Case ErrorType.OnlyText
					ErrorCode = "0x00000036"
					ErrorString = "DEVICE_REFERENCE_COUNT_NOT_ZERO"
					Response.StatusCode = 415
				Case ErrorType.DenyStringFound
					ErrorCode = "0x00000099"
					ErrorString = "INVALID_REGION_OR_SEGMENT"
					Response.StatusCode = 415
				Case ErrorType.FakeFileType
					ErrorCode = "0x00000079"
					ErrorString = "MISMATCHED_HAL"
					Response.StatusCode = 415
				Case ErrorType.DenyMime
					ErrorCode = "0x00000097"
					ErrorString = "BOUND_IMAGE_UNSUPPORTED"
					Response.StatusCode = 415
				Case Else
					ErrorString = "PAGE_FAULT_IN_NONPAGED_AREA"
					ErrorCode = "0x00000404"
					Status = "0x00000404"
			End Select
		Else
			divException.Visible = True
			divStyle.Visible = False
			divError.Visible = False
			' Вышло исключение
			ErrorString = Context.Error.Message	'"PAGE_FAULT_IN_NONPAGED_AREA"
			ErrorCode = "0x00000500"
			Response.StatusCode = 500

			'Cообщение, описывающее текущее исключение
			Status = Context.Error.Message
			'Возвращает или задает имя приложения или объекта, вызывавшего ошибку
			SouceFile = HttpUtility.HtmlEncode(Context.Error.Source)
			' Трассировка стэка
			StackTrace = String.Join(String.Empty, Context.Error.StackTrace.Split(vbCrLf).Select(Function(x) "<p>" & HttpUtility.HtmlEncode(x) & "</p>"))
			' метод, создавший текущее исключение
			SourceFunction = HttpUtility.HtmlEncode(Context.Error.TargetSite)
		End If

		ErrorCode1 = "0x0016a537"
		ErrorCode2 = "0x0000cafc"
		ErrorCode3 = "0x003fcc65"
		ErrorCode4 = "0x00000050"
		ErrorPageString = HttpUtility.HtmlEncode(Status)
		DateStamp = objRandom.Next.ToString("X")

	End Sub

End Class
