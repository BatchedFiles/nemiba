Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Text

''' <summary>
''' CAPTCHA ASP.NET 2.0 user control
''' </summary>
''' <remarks>
''' add a reference to this DLL and add the CaptchaControl to your toolbox;
''' then just drag and drop the control on a web form and set properties on it.
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
''' </remarks>
Public Class CaptchaControl
	Inherits Control

	Implements INamingContainer
	Implements IPostBackDataHandler
	Implements IValidator
	Implements ICallbackEventHandler

	Private _timeoutSecondsMax As Integer = 120
	Private _timeoutSecondsMin As Integer = 10
	Private _userValidated As Boolean
	Private _font As String = ""
	Private m_Captcha As CaptchaImage
	Private _prevguid As String
	Private _errorMessage As String = ""
	Private _cacheStrategy As CacheType
	Private _inputCssClass As String = ""
	Private m_ToolTip As String = ""
	Private m_Enabled As Boolean = True
	Private m_UserEntry As String


#Region "Public Properties"

	Public Enum CacheType
		HttpRuntime
		Session
	End Enum

	<Browsable(False), _
	Bindable(True), _
	Category("Appearance"), _
	DefaultValue("The text you typed does not match the text in the image."), _
	Description("Message to display in a Validation Summary when the CAPTCHA fails to validate.")> _
	Public Property ErrorMessage() As String Implements System.Web.UI.IValidator.ErrorMessage
		Get
			If Not _userValidated Then
				Return _errorMessage
			Else
				Return ""
			End If
		End Get
		Set(ByVal value As String)
			_errorMessage = value
		End Set
	End Property

	<Browsable(False), _
	Category("Behavior"), _
	DefaultValue(True), _
	Description("Is Valid"), _
	DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
	Public Property IsValid() As Boolean Implements System.Web.UI.IValidator.IsValid
		Get
			If Not Visible OrElse Not Enabled Then
				Return True
			Else
				Return _userValidated
			End If
		End Get
		Set(ByVal value As Boolean)
			_userValidated = value
		End Set
	End Property

	Public Property Enabled() As Boolean
		Get
			Return m_Enabled
		End Get
		Set(ByVal value As Boolean)
			m_Enabled = value
			' When a validator is disabled, generally, the intent is not to
			' make the page invalid for that round trip.
			If Not value Then
				_userValidated = True
			End If
		End Set
	End Property

	<DefaultValue(GetType(CaptchaControl.CacheType), "HttpRuntime"), _
 Description("Determines if CAPTCHA codes are stored in HttpRuntime (fast, but local to current server) or Session (more portable across web farms)."), _
 Category("Captcha")> _
 Public Property CacheStrategy() As CacheType
		Get
			Return _cacheStrategy
		End Get
		Set(ByVal value As CacheType)
			_cacheStrategy = value
		End Set
	End Property

	'<Description("Returns True if the user was CAPTCHA validated after a postback."), _
	'Category("Captcha")> _
	'Public ReadOnly Property UserValidated() As Boolean
	'	Get
	'		ValidateCaptcha(HttpContext.Current.Request.Form.Item(UniqueID & IdSeparator & "txtCaptcha"))
	'		Return _userValidated
	'	End Get
	'End Property

	<DefaultValue(""), _
	Description("Font used to render CAPTCHA text. If font name is blank, a random font will be chosen."), _
	Category("Captcha")> _
	Public Property CaptchaFont() As String
		Get
			Return _font
		End Get
		Set(ByVal Value As String)
			_font = Value
			m_Captcha.Font = _font
		End Set
	End Property

	<DefaultValue(""), _
	Description("Characters used to render CAPTCHA text. A character will be picked randomly from the string."), _
	Category("Captcha")> _
	Public Property CaptchaChars() As String
		Get
			Return m_Captcha.CaptchaChars
		End Get
		Set(ByVal Value As String)
			m_Captcha.CaptchaChars = Value
		End Set
	End Property

	<DefaultValue(""), _
	Description("Класс стиля для текстового поля"), _
	Category("Captcha")> _
	Public Property TextBoxCssClass() As String
		Get
			Return _inputCssClass
		End Get
		Set(ByVal Value As String)
			_inputCssClass = Value
		End Set
	End Property

	<DefaultValue(5), _
	Description("Number of CaptchaChars used in the CAPTCHA text"), _
	Category("Captcha")> _
	Public Property CaptchaLength() As Integer
		Get
			Return m_Captcha.TextLength
		End Get
		Set(ByVal Value As Integer)
			m_Captcha.TextLength = Value
		End Set
	End Property

	<DefaultValue(5), _
	Description("Minimum number of seconds CAPTCHA must be displayed before it is valid. If you're too fast, you must be a robot. Set to zero to disable."), _
	Category("Captcha")> _
	Public Property CaptchaMinTimeout() As Integer
		Get
			Return _timeoutSecondsMin
		End Get
		Set(ByVal Value As Integer)
			If Value > 15 Then
				Throw New ArgumentOutOfRangeException("CaptchaTimeout", "Timeout must be less than 15 seconds. Humans aren't that slow!")
			End If
			_timeoutSecondsMin = Value
		End Set
	End Property

	<DefaultValue(120), _
 Description("Maximum number of seconds CAPTCHA will be cached and valid. If you're too slow, you may be a CAPTCHA hack attempt. Set to zero to disable."), _
 Category("Captcha")> _
 Public Property CaptchaMaxTimeout() As Integer
		Get
			Return _timeoutSecondsMax
		End Get
		Set(ByVal Value As Integer)
			If Value < 15 And Value <> 0 Then
				Throw New ArgumentOutOfRangeException("CaptchaTimeout", "Timeout must be greater than 15 seconds. Humans can't type that fast!")
			End If
			_timeoutSecondsMax = Value
		End Set
	End Property

	<DefaultValue(50), _
	Description("Height of generated CAPTCHA image."), _
	Category("Captcha")> _
	Public Property CaptchaHeight() As Integer
		Get
			Return m_Captcha.Height
		End Get
		Set(ByVal Value As Integer)
			m_Captcha.Height = Value
		End Set
	End Property

	<DefaultValue(180), _
	Description("Width of generated CAPTCHA image."), _
	Category("Captcha")> _
	Public Property CaptchaWidth() As Integer
		Get
			Return m_Captcha.Width
		End Get
		Set(ByVal Value As Integer)
			m_Captcha.Width = Value
		End Set
	End Property

	<DefaultValue(GetType(CaptchaImage.FontWarpFactor), "Low"), _
	Description("Amount of random font warping used on the CAPTCHA text"), _
	Category("Captcha")> _
	Public Property CaptchaFontWarping() As CaptchaImage.FontWarpFactor
		Get
			Return m_Captcha.FontWarp
		End Get
		Set(ByVal Value As CaptchaImage.FontWarpFactor)
			m_Captcha.FontWarp = Value
		End Set
	End Property

	<DefaultValue(GetType(CaptchaImage.BackgroundNoiseLevel), "Low"), _
	Description("Amount of background noise to generate in the CAPTCHA image"), _
	Category("Captcha")> _
	Public Property CaptchaBackgroundNoise() As CaptchaImage.BackgroundNoiseLevel
		Get
			Return m_Captcha.BackgroundNoise
		End Get
		Set(ByVal Value As CaptchaImage.BackgroundNoiseLevel)
			m_Captcha.BackgroundNoise = Value
		End Set
	End Property

	<DefaultValue(GetType(CaptchaImage.LineNoiseLevel), "None"), _
	Description("Add line noise to the CAPTCHA image"), _
	Category("Captcha")> _
	Public Property CaptchaLineNoise() As CaptchaImage.LineNoiseLevel
		Get
			Return m_Captcha.LineNoise
		End Get
		Set(ByVal Value As CaptchaImage.LineNoiseLevel)
			m_Captcha.LineNoise = Value
		End Set
	End Property

	<DefaultValue(GetType(Boolean), "True"), _
	Description("Устанавливает цветной текст капчи"), _
	Category("Captcha")> _
	Public Property CaptchaTextColored() As Boolean
		Get
			Return m_Captcha.TextColored
		End Get
		Set(ByVal Value As Boolean)
			m_Captcha.TextColored = Value
		End Set
	End Property

	<DefaultValue(""), _
	  Description("Tool tip text"), _
	  Category("Captcha")> _
	Public Property ToolTip() As String
		Get
			Return m_ToolTip
		End Get
		Set(ByVal Value As String)
			m_ToolTip = Value
		End Set
	End Property

#End Region

	Public Sub Validate() Implements System.Web.UI.IValidator.Validate
		If Visible AndAlso Enabled Then
			'-- retrieve the previous captcha from the cache to inspect its properties
			Dim ci As CaptchaImage = GetCachedCaptcha(_prevguid)
			If ci Is Nothing Then
				_errorMessage = "Слоупоки такие слоупоки. Тред ещё не начался, а твоя капча за " & Me.CaptchaMaxTimeout & " секунд уже протухла."
				_userValidated = False
				Return
			End If

			'--  was it entered too quickly?
			If Me.CaptchaMinTimeout > 0 Then
				If (ci.RenderedAt.AddSeconds(_timeoutSecondsMin) > Now) Then
					_userValidated = False
					_errorMessage = "Ты слишком быстро вводишь капчу. В следующий раз жди " & Me.CaptchaMinTimeout & " секунд."
					RemoveCachedCaptcha(_prevguid)
					Return
				End If
			End If

			If String.Compare(m_UserEntry, ci.Text, True) <> 0 Then
				_errorMessage = "Не угадал <span class=""spoiler"">&#62;:3</span> Правильный ответ " & ci.Text
				_userValidated = False
				RemoveCachedCaptcha(_prevguid)
				Return
			End If
		End If
		_userValidated = True
		RemoveCachedCaptcha(_prevguid)
	End Sub

	''' <summary>
	''' Генерирует случайное изображение капчи-ведуньи
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GenerateMagicCaptcha() As Bitmap
		Return m_Captcha.GenerateImage(True)
	End Function

	Private Function GetCachedCaptcha(ByVal guid As String) As CaptchaImage
		If _cacheStrategy = CacheType.HttpRuntime Then
			Return CType(HttpRuntime.Cache.Get(guid), CaptchaImage)
		Else
			Return CType(HttpContext.Current.Session.Item(guid), CaptchaImage)
		End If
	End Function

	Private Sub RemoveCachedCaptcha(ByVal guid As String)
		If Not String.IsNullOrEmpty(guid) Then
			If _cacheStrategy = CacheType.HttpRuntime Then
				HttpRuntime.Cache.Remove(guid)
			Else
				HttpContext.Current.Session.Remove(guid)
			End If
		End If
	End Sub

	''' <summary>
	''' render raw control HTML to the page
	''' </summary>
	Protected Overrides Sub Render(ByVal Output As HtmlTextWriter)
		With Output
			' master DIV
			.AddAttribute(HtmlTextWriterAttribute.Class, "captcha")
			.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "divCaptcha")
			.RenderBeginTag(HtmlTextWriterTag.Div)

			.AddAttribute(HtmlTextWriterAttribute.Class, "captchaimage")
			.RenderBeginTag(HtmlTextWriterTag.Div)

			' Картинка с капчой
			.AddAttribute(HtmlTextWriterAttribute.Onclick, "ChangeCaptcha()")
			.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "imgCaptcha")
			.AddAttribute(HtmlTextWriterAttribute.Src, ResolveUrl(GetImageLink))
			.AddAttribute(HtmlTextWriterAttribute.Alt, "Щёлкни здесь, чтобы обновить капчу")
			.AddAttribute(HtmlTextWriterAttribute.Width, m_Captcha.Width.ToString)
			.AddAttribute(HtmlTextWriterAttribute.Height, m_Captcha.Height.ToString)
			.RenderBeginTag(HtmlTextWriterTag.Img)
			.RenderEndTag()

			.RenderEndTag()

			.AddAttribute(HtmlTextWriterAttribute.Class, "captchainputtext")
			.RenderBeginTag(HtmlTextWriterTag.Div)

			' Текстовое поле ввода значений
			.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
			.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID)
			.AddAttribute(HtmlTextWriterAttribute.Type, "text")
			If String.IsNullOrEmpty(_inputCssClass) Then
				.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
			Else
				.AddAttribute(HtmlTextWriterAttribute.Class, _inputCssClass)
			End If
			.RenderBeginTag(HtmlTextWriterTag.Input)
			.RenderEndTag()

			.RenderEndTag()

			' master DIV
			.RenderEndTag()
		End With
	End Sub

	''' <summary>
	''' generate a new captcha and store it in the ASP.NET Cache by unique GUID
	''' </summary>
	Private Sub SaveCaptchaInCache()
		If Not MyBase.DesignMode Then
			If _cacheStrategy = CacheType.HttpRuntime Then
				HttpRuntime.Cache.Add(m_Captcha.UniqueId, m_Captcha, Nothing, _
				 DateTime.Now.AddSeconds(Convert.ToDouble(IIf(Me.CaptchaMaxTimeout = 0, 120, Me.CaptchaMaxTimeout))), _
				 TimeSpan.Zero, Caching.CacheItemPriority.NotRemovable, Nothing)
			Else
				HttpContext.Current.Session.Add(m_Captcha.UniqueId, m_Captcha)
			End If
		End If
	End Sub

	''' <summary>
	''' Retrieve the user's CAPTCHA input from the posted data
	''' </summary>
	Protected Function LoadPostData(ByVal PostDataKey As String, ByVal Values As NameValueCollection) As Boolean Implements IPostBackDataHandler.LoadPostData
		' Текст пользователя
		m_UserEntry = Values.Item(UniqueID)
		' Идентификатор капчи
		_prevguid = Values.Item(Me.ClientID & ClientIDSeparator & "Guid")
		' Проверка на валидность
		Validate()
		Return True
	End Function

	Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

	End Sub

	'Protected Overrides Function SaveControlState() As Object
	'	Return _captcha.UniqueId
	'End Function

	'Protected Overrides Sub LoadControlState(ByVal state As Object)
	'	If state IsNot Nothing Then
	'		_prevguid = CType(state, String)
	'	End If
	'End Sub

	Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
		MyBase.OnInit(e)
		'	Page.RegisterRequiresControlState(Me)
		Page.Validators.Add(Me)
	End Sub

	Protected Overrides Sub OnUnload(ByVal e As System.EventArgs)
		If Not (Page Is Nothing) Then
			Page.Validators.Remove(Me)
		End If
		MyBase.OnUnload(e)
	End Sub

	Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
		If Me.Visible Then
			SaveCaptchaInCache()
		End If
		MyBase.OnPreRender(e)
	End Sub

	Public Sub New()
		m_Captcha = New CaptchaImage
	End Sub

	Private Sub Me_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim cs As ClientScriptManager = Page.ClientScript
		If Me.Visible AndAlso Me.Enabled Then
			' Добавляю скрытое поле с идентификатором капчи
			Page.ClientScript.RegisterHiddenField(Me.ClientID & ClientIDSeparator & "Guid", m_Captcha.UniqueId.ToString)

			If Not cs.IsClientScriptBlockRegistered("WebCaptchaControl") Then
				Dim cbReference As String = Page.ClientScript.GetCallbackEventReference(Me, "arg", "ReceiveServerData", "context")

				Dim sb As New StringBuilder
				sb.Append("function ChangeCaptcha()")
				sb.Append("{")
				sb.Append(String.Format("CallServer(document.getElementById(""{0}"").value, ""value"");", Me.ClientID & ClientIDSeparator & "Guid"))
				sb.Append("};")

				sb.Append("function CallServer(arg, context)")
				sb.Append("{")
				sb.Append(cbReference)
				sb.Append("};")

				sb.Append("function ReceiveServerData(GiudAndImagesrc)")
				sb.Append("{")
				sb.Append("var simpleArray = GiudAndImagesrc.split("" "");")
				sb.Append(String.Format("var txtCapt = document.getElementById(""{0}"");", Me.ClientID & ClientIDSeparator & "Guid"))
				sb.Append("txtCapt.value = simpleArray[0];")
				sb.Append(String.Format("var imgCapt = document.getElementById(""{0}"");", Me.ClientID & ClientIDSeparator & "imgCaptcha"))
				sb.Append("imgCapt.src = simpleArray[1];")
				sb.Append("};")

				Page.ClientScript.RegisterClientScriptBlock(GetType(Page), "WebCaptchaControl", sb.ToString, True)
			End If
		End If
	End Sub

	Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
		' Создаём новую капчу
		m_Captcha.GenerateNewCaptcha()
		SaveCaptchaInCache()
		' Возвращаем ссылку на изображение и Guid
		Return String.Format("{0} {1}", m_Captcha.UniqueId, ResolveUrl(GetImageLink()))
	End Function

	' Пришедние данные от клиента
	Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent

	End Sub

	Private Function GetImageLink() As String
		' Здесь нужна ссылка на изображение
		Dim strLink As String = String.Empty
		If Not Me.DesignMode Then
			'MyBase.ImageUrl = "~/CaptchaImage.aspx?guid=" & _captcha.UniqueId
			strLink = "~/CaptchaImage.aspx?guid=" & m_Captcha.UniqueId
		End If
		If Me.CacheStrategy = CacheType.Session Then
			'MyBase.ImageUrl &= "&s=1"
			strLink &= "&amp;s=1"
		End If
		Return strLink
	End Function

End Class