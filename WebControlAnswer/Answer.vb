Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

<DefaultProperty("Text"), ToolboxData("<{0}:ServerControl1 runat=""server""></{0}:ServerControl1>")> _
Public Class Answer
	Inherits Control
	Implements IPostBackDataHandler
	Implements INamingContainer
	Implements IPostBackEventHandler

	''' <summary>
	''' Состояние
	''' </summary>
	''' <remarks></remarks>
	Public Enum ThreadState As Integer
		''' <summary>
		''' Ответ в тред
		''' </summary>
		''' <remarks></remarks>
		AnswerToThread
		''' <summary>
		''' Создание треда
		''' </summary>
		''' <remarks></remarks>
		CreateThread
		''' <summary>
		''' Режим каталога
		''' </summary>
		''' <remarks></remarks>
		Catalog
	End Enum

	''' <summary>
	''' Сообщение пользователя
	''' </summary>
	''' <remarks></remarks>
	Public Class MessageInfo
		Inherits EventArgs
		''' <summary>
		''' Имя раздела
		''' </summary>
		''' <remarks></remarks>
		Public BoardName As String
		''' <summary>
		''' Номер треда
		''' </summary>
		''' <remarks></remarks>
		Public ThreadNumber As Long
		''' <summary>
		''' Имя пользователя
		''' </summary>
		''' <remarks></remarks>
		Public UserName As String
		''' <summary>
		''' Электропочта
		''' </summary>
		''' <remarks></remarks>
		Public Email As String
		''' <summary>
		''' Тема
		''' </summary>
		''' <remarks></remarks>
		Public Subject As String
		''' <summary>
		''' Текст сообщения
		''' </summary>
		''' <remarks></remarks>
		Public MessageText As String
		''' <summary>
		''' Список прикреплённых файлов
		''' </summary>
		''' <remarks></remarks>
		Public PostedFiles As Generic.List(Of HttpPostedFile)
		''' <summary>
		''' Отключение файлов
		''' </summary>
		''' <remarks></remarks>
		Public WithoutFiles As Boolean
		''' <summary>
		''' Пароль
		''' </summary>
		''' <remarks></remarks>
		Public Password As String
		''' <summary>
		''' Рейтинг возрастных ограничений
		''' </summary>
		''' <remarks></remarks>
		Public Rating As Long
		''' <summary>
		''' После ответа переместиться к треду
		''' </summary>
		''' <remarks></remarks>
		Public MoveToThread As Boolean
		''' <summary>
		''' Изображение капчи-ведуньи
		''' </summary>
		''' <remarks></remarks>
		Public MagicCaptcha As Bitmap
		''' <summary>
		''' Список ссылок на видео
		''' </summary>
		''' <remarks></remarks>
		Public VideoLinks As Generic.List(Of String)
		''' <summary>
		''' Прикреплённый тред
		''' </summary>
		''' <remarks></remarks>
		Public TopThread As Boolean
		''' <summary>
		''' Номер сообщения
		''' </summary>
		''' <remarks></remarks>
		Public MessageNumber As Long
		''' <summary>
		''' Отметка модератора
		''' </summary>
		''' <remarks></remarks>
		Public Checked As Boolean
		''' <summary>
		''' Печенье пользователя
		''' </summary>
		''' <remarks></remarks>
		Public Cookie As String
		''' <summary>
		''' Настоящее имя пользователя
		''' </summary>
		''' <remarks></remarks>
		Public RealUserName As String
		''' <summary>
		''' Время создания сообщения
		''' </summary>
		''' <remarks></remarks>
		Public DateTime As Date
		''' <summary>
		''' Сообщение удалено
		''' </summary>
		''' <remarks></remarks>
		Public MessageDeleted As Boolean
	End Class

#Region "Свойства"

	''' <summary>
	''' Возвращает/устанавливает тему по умолчанию
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property DefaultSubject As String

	''' <summary>
	''' Возвращает/устанавливает текст кнопки при создании треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CreateNewThreadText As String
		Get
			Return If(String.IsNullOrEmpty(m_CreateNewThreadText), "Создать нить", m_CreateNewThreadText)
		End Get
		Set(ByVal value As String)
			m_CreateNewThreadText = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает текст кнопки при ответе в тред
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property AnswerToThreadText As String
		Get
			Return If(String.IsNullOrEmpty(m_AnswerToThreadText), "Ответить в ", m_AnswerToThreadText)
		End Get
		Set(ByVal value As String)
			m_AnswerToThreadText = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает обязательность заполнения поля «Имя пользователя»
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property UserNameMustExists As Boolean

	''' <summary>
	''' Возвращает/устанавливает отображение текстового поля ссылки на видео
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property VideoLinkEnable() As Boolean

	''' <summary>
	''' Набор символов в капче
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CaptchaChars() As String

	''' <summary>
	''' Возвращает клиентский идентификатор надписи с ответом
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public ReadOnly Property AnswerToThreadLabelClientId() As String
		Get
			Return Me.ClientID & ClientIDSeparator & "lblAnswer"
		End Get
	End Property


	''' <summary>
	''' Возвращает клиентский идентификатор текстового поля номера треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public ReadOnly Property ThreadNumberTextClientId() As String
		Get
			Return Me.ClientID & ClientIDSeparator & "txtThreadNumber"
		End Get
	End Property

	''' <summary>
	''' Возвращает клиентский идентификатор текстового поля комментария
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public ReadOnly Property MessageTextClientId() As String
		Get
			Return Me.ClientID & ClientIDSeparator & "txtMessage"
		End Get
	End Property

	''' <summary>
	''' Возвращает/устанавливает права администратора
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadManage() As Boolean

	''' <summary>
	''' Возвращает или устанавливает максимальное количество символов в комментарии
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property MaxMessageLength() As Integer

	''' <summary>
	''' Возвращает/устанавливает текст сообщения
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property MessageText() As String
		Get
			If m_MessageText Is Nothing Then
				Return String.Empty
			Else
				Return m_MessageText
			End If
		End Get
		Set(ByVal value As String)
			m_MessageText = value
		End Set
	End Property

	''' <summary>
	''' Возвращает или устанавливает переключатель Перейти к треду
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property MoveToThread() As Boolean

	''' <summary>
	''' Включает или выключает отображение поля Имя пользователя
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property UserNameVisible() As Boolean

	''' <summary>
	''' Возвращает/устанавливает отображение поля электропочты
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property EMailVisible() As Boolean

	''' <summary>
	''' Номер треда для ответа при отключении режима создания нового треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadNumber() As Long

	''' <summary>
	''' Возвращает/устанавливает пароль, не хранится во ViewState
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Password() As String
		Get
			If m_Password Is Nothing Then
				Return String.Empty
			Else
				Return m_Password
			End If
		End Get
		Set(ByVal value As String)
			m_Password = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает видимость флажка «капча‐ведунья»
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property IsMagicCaptcha As Boolean

	''' <summary>
	''' Возвращает или устанавливает флаги капчи
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CaptchaMode() As String
		Get
			EnsureChildControls()
			' получаем флаги капчи и отправляем строку
			Dim sb As StringBuilder = New StringBuilder
			sb.Append("Enable:1|")
			sb.Append("BackgroundNoise:" & Convert.ToInt32(imgCartcha.CaptchaBackgroundNoise).ToString & "|")
			sb.Append("FontWarping:" & Convert.ToInt32(imgCartcha.CaptchaFontWarping).ToString & "|")
			sb.Append("LineNoise:" & Convert.ToInt32(imgCartcha.CaptchaLineNoise).ToString & "|")
			sb.Append("Colored:")
			If imgCartcha.CaptchaTextColored Then
				sb.Append(1.ToString)
			Else
				sb.Append(0.ToString)
			End If
			Return sb.ToString
		End Get
		Set(ByVal value As String)
			EnsureChildControls()
			' Примерить флаги капчи
			'Enable:1|BackgroundNoise:0|FontWarping:0|LineNoise:0
			For Each strTemp As String In value.Split("|"c)
				Dim astrParams() As String = strTemp.Split(":"c)
				Select Case astrParams(0)
					Case "Enable"
						imgCartcha.Enabled = Convert.ToBoolean(Convert.ToInt32(astrParams(1)))
						imgCartcha.Visible = Convert.ToBoolean(Convert.ToInt32(astrParams(1)))
						'				If CaptchaGroup.Visible Then
						'					' Регистрация возможна
						'				Else
						'					' Регистрация невозможна
						'				End If
					Case "BackgroundNoise"
						imgCartcha.CaptchaBackgroundNoise = CType(Convert.ToInt32(astrParams(1)), WebControlCaptcha.CaptchaImage.BackgroundNoiseLevel)
					Case "FontWarping"
						imgCartcha.CaptchaFontWarping = CType(Convert.ToInt32(astrParams(1)), WebControlCaptcha.CaptchaImage.FontWarpFactor)
					Case "LineNoise"
						imgCartcha.CaptchaLineNoise = CType(Convert.ToInt32(astrParams(1)), WebControlCaptcha.CaptchaImage.LineNoiseLevel)
					Case "Colored"
						' Цветные буквы капчи
						imgCartcha.CaptchaTextColored = Convert.ToBoolean(Convert.ToInt32(astrParams(1)))
				End Select
			Next
		End Set
	End Property

	''' <summary>
	''' Режим создания нового треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadMode() As ThreadState

	''' <summary>
	''' Возвращает/устанавливает тему
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Subject() As String
		Get
			If m_Subject Is Nothing Then
				Return String.Empty
			Else
				Return m_Subject
			End If
		End Get
		Set(ByVal value As String)
			m_Subject = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает список ссылок на видео
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property VideoList() As List(Of String)
		Get
			Return m_VideoList
		End Get
		Set(ByVal value As List(Of String))
			m_VideoList = value
		End Set
	End Property

	''' <summary>
	''' Возвращает или устанавливает название раздела
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property BoardName() As String
		Get
			Dim objBoardName As Object = ViewState.Item("BoardName")
			If objBoardName Is Nothing Then
				Return String.Empty
			Else
				Return objBoardName.ToString
			End If
		End Get
		Set(ByVal value As String)
			If Not String.IsNullOrEmpty(value) Then
				ViewState.Add("BoardName", value)
			End If
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает электропочту
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property EMail() As String
		Get
			Dim objEmail As Object = ViewState.Item("Email")
			If objEmail Is Nothing Then
				Return String.Empty
			Else
				Return objEmail.ToString
			End If
		End Get
		Set(ByVal value As String)
			If Not String.IsNullOrEmpty(value) Then
				ViewState.Add("Email", value)
			End If
		End Set
	End Property

	''' <summary>
	''' Возвращает или устанавливает максимальное количество файлов, которых можно загрузить
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property MaxFiles() As Integer
		Get
			Dim FilesCount As Object = ViewState.Item("MaxFiles")
			If FilesCount Is Nothing Then
				Return 0
			Else
				Dim m As Integer
				Integer.TryParse(FilesCount.ToString, m)
				Return m
			End If
		End Get
		Set(ByVal value As Integer)
			If value > 0 Then
				ViewState.Add("MaxFiles", value)
			End If
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает видимость списка с рейтингом
	''' </summary>
	''' <remarks></remarks>
	Public Property RatingListVisible() As Boolean
		Get
			Dim objEMailVisible As Object = ViewState.Item("RatingListVisible")
			If objEMailVisible Is Nothing Then
				Return True
			Else
				Dim b As Boolean
				Boolean.TryParse(objEMailVisible.ToString, b)
				Return b
			End If
		End Get
		Set(ByVal value As Boolean)
			If value Then
				ViewState.Add("RatingListVisible", value)
			End If
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает имя пользователя
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property UserName() As String
		Get
			Dim objUserName As Object = ViewState.Item("UserName")
			If objUserName Is Nothing Then
				Return String.Empty
			Else
				Return objUserName.ToString
			End If
		End Get
		Set(ByVal value As String)
			If Not String.IsNullOrEmpty(value) Then
				ViewState.Add("UserName", value)
			End If
		End Set
	End Property

#End Region

	Public Event AnswerToThread(ByVal Sender As Object, ByVal mInfo As MessageInfo)
	Public Event ThreadCreateNeeded(ByVal Sender As Object, ByVal mInfo As MessageInfo)
	Public Event RegisterUserNeeded(ByVal Sender As Object, ByVal mInfo As MessageInfo)
	Public Event CaptchaNotValid(ByVal Sender As Object, ByVal ErrorMessage As String)

	Private m_RatingListVisible As Boolean
	Private m_Info As New MessageInfo
	Private m_Password As String
	Private m_VideoList As List(Of String)
	Private m_MessageText As String
	Private m_Subject As String
	Private m_CreateNewThreadText As String
	Private m_AnswerToThreadText As String

	''' <summary>
	''' Капча
	''' </summary>
	''' <remarks></remarks>
	Private imgCartcha As WebControlCaptcha.CaptchaControl

	''' <summary>
	''' Коллекция правил
	''' </summary>
	''' <remarks></remarks>
	Private m_RulesInfo As Generic.List(Of RulesInfo)

	Private Structure RulesInfo
		Public Simple As Boolean
		Public Text1 As String
		Public Text2 As String
		Public RulesLinkText As String
		Public RulesLinkUrl As String
	End Structure

	Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
		'<div class="theader">
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "theader")
		writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "lblAnswer")
		writer.RenderBeginTag(HtmlTextWriterTag.Div)

		Select Case ThreadMode
			Case ThreadState.AnswerToThread
				writer.Write("Ответ в " & ThreadNumber.ToString)
			Case ThreadState.CreateThread
				writer.Write("Новая нить")
			Case ThreadState.Catalog
				writer.Write("Каталог")
		End Select
		'</div>theader
		writer.RenderEndTag()

		If ThreadMode <> ThreadState.Catalog Then
			'<div class="postarea">
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postarea")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "fraPostArea")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)

			If UserNameVisible Then
				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<div class="postblock">Имя</div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.Write("Имя")
				writer.RenderEndTag()

				'<div><input type="text" id="txtUserName" runat="server" class="inp" />&nbsp;(оставь это поле пустым)</td>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtUserName")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtUserName")
				writer.AddAttribute(HtmlTextWriterAttribute.Value, UserName)
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				writer.Write(HtmlTextWriter.SpaceChar)

				writer.AddAttribute(HtmlTextWriterAttribute.Class, "usernamemustbeempty")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				If UserNameMustExists Then
					writer.Write(" (обязательно)")
				Else
					writer.Write(" (оставь это поле пустым)")
				End If
				writer.RenderEndTag()

				writer.RenderEndTag()
				writer.RenderEndTag()
			End If

			If EMailVisible Then
				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<div class="postblock">Электропочта</div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.Write("Электропочта")
				writer.RenderEndTag()

				'<div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<input type="text" id="txtEmail" runat="server" class="inp" />
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtEmail")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtEmail")
				writer.AddAttribute(HtmlTextWriterAttribute.Value, EMail)
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				'</div>
				writer.RenderEndTag()
				writer.RenderEndTag()
			End If

			'<div class="postform">
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<div class="postblock">Тема</td>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.Write("Тема")
			writer.RenderEndTag()

			'<div><input type="text" id="txtSubject" runat="server" class="inp" />
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
			writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtSubject")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtSubject")
			writer.AddAttribute(HtmlTextWriterAttribute.Value, Subject)
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()
			'&nbsp;
			writer.Write(HtmlTextWriter.SpaceChar)
			'<input type="submit" id="cmdSubmit" name="cmdSubmit" value="Создать нить" class="but" />
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit")
			writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID)
			Select Case ThreadMode
				Case ThreadState.AnswerToThread
					writer.AddAttribute(HtmlTextWriterAttribute.Value, AnswerToThreadText & ThreadNumber.ToString)
				Case ThreadState.CreateThread
					writer.AddAttribute(HtmlTextWriterAttribute.Value, CreateNewThreadText)
			End Select
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()
			'</div>
			writer.RenderEndTag()
			writer.RenderEndTag()

			'<div class="postform">
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<div class="postblock">Комментарий<br />
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "lblComment")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write("Комментарий")
			writer.RenderEndTag()
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "lblSymbolsCount")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.RenderEndTag()
			'</div>
			writer.RenderEndTag()

			'<div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<textarea id="txtMessage" runat="server" cols="25" rows="10">
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
			writer.AddAttribute(HtmlTextWriterAttribute.Cols, "25")
			writer.AddAttribute(HtmlTextWriterAttribute.Rows, "10")
			writer.AddAttribute("onkeyup", String.Format("txtMessage_onkeyup({0}, {1}, {2}, {3})", Me.ClientID & ClientIDSeparator & "lblComment", Me.ClientID & ClientIDSeparator & "lblSymbolsCount", Me.ClientID & ClientIDSeparator & "txtMessage", MaxMessageLength))
			writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtMessage")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtMessage")
			writer.RenderBeginTag(HtmlTextWriterTag.Textarea)
			'</textarea>
			writer.RenderEndTag()
			'</div>
			writer.RenderEndTag()
			writer.RenderEndTag()

			If MaxFiles > 0 Then
				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<div class="postblock">
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				' Обновляем надпись Файл или Файлы
				If MaxFiles > 1 Then
					writer.Write("Файлы")
				Else
					writer.Write("Файл")
				End If
				'</div>
				writer.RenderEndTag()

				'<div class="postbuttons">
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				' Добавление формы для ввода файлов
				For i As Integer = 0 To MaxFiles - 1
					writer.RenderBeginTag(HtmlTextWriterTag.Div)

					writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
					writer.AddAttribute(HtmlTextWriterAttribute.Type, "file")
					writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtFile")
					'writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtFile" & i.ToString)
					writer.RenderBeginTag(HtmlTextWriterTag.Input)
					writer.RenderEndTag()

					If VideoLinkEnable Then
						' Добавление текстового поля для ссылки на видео
						writer.WriteBreak()
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
						writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
						writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtVideo")
						writer.RenderBeginTag(HtmlTextWriterTag.Input)
						writer.RenderEndTag()
						writer.Write(HtmlTextWriter.SpaceChar & "(YouTube)")
					End If
					'writer.RenderBeginTag(HtmlTextWriterTag.Span)
					'writer.Write(" описание ")
					'writer.RenderEndTag()

					'writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
					'writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
					'writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtFileDescription")
					'writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtFileDescription")
					'writer.RenderBeginTag(HtmlTextWriterTag.Input)
					'writer.RenderEndTag()
					writer.RenderEndTag()
				Next

				writer.RenderBeginTag(HtmlTextWriterTag.Label)
				'<input type="checkbox" id="chkWithoutFile" runat="server" />Без файла</td>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "chkWithoutFile")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "chkWithoutFile")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				writer.Write("Без файла")
				'</label>
				writer.RenderEndTag()
				'</div>postbuttons </div>postform
				writer.RenderEndTag()
				writer.RenderEndTag()
			End If

			' Показываем капчу
			If imgCartcha IsNot Nothing Then
				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				If imgCartcha.Visible Then
					'<div class="postblock">Введи капчу</div>
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
					writer.RenderBeginTag(HtmlTextWriterTag.Div)
					writer.Write("Введи капчу")
					writer.RenderEndTag()

					'<div>
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
					writer.RenderBeginTag(HtmlTextWriterTag.Div)

					' Вывод капчи
					imgCartcha.RenderControl(writer)

					If IsMagicCaptcha Then
						'<div><label>
						writer.RenderBeginTag(HtmlTextWriterTag.Div)
						writer.RenderBeginTag(HtmlTextWriterTag.Label)
						'<input type="checkbox" id="chkCaptchaMagik" runat="server" />
						'<span>Капча-ведунья</span>
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
						writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
						writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "chkCaptchaMagik")
						writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "chkCaptchaMagik")
						writer.RenderBeginTag(HtmlTextWriterTag.Input)
						writer.RenderEndTag()
						writer.Write("Капча‐ведунья")
						'</label></div>
						writer.RenderEndTag()
						writer.RenderEndTag()
					End If
					'</div>
					writer.RenderEndTag()
				End If
				writer.RenderEndTag()
			End If

			'<div class="postform">
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<div class="postblock">Пароль</div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.Write("Пароль")
			writer.RenderEndTag()

			'<div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<input type="password" runat="server" id="txtPassword" class="inp" />
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "password")
			writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtPassword")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtPassword")
			writer.AddAttribute(HtmlTextWriterAttribute.Value, Password)
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()

			'&nbsp;(Для удаления нитей и файлов)
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.Write(" (Для удаления нитей и файлов)")
			writer.RenderEndTag()

			'</div>
			writer.RenderEndTag()
			writer.RenderEndTag()

			If RatingListVisible Then
				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<div class="postblock">Рейтинг цензуры</div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.Write("Рейтинг цензуры")
				writer.RenderEndTag()

				'<div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<asp:DropDownList ID="lstRating" runat="server" CssClass="but">
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "lstRating")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "lstRating")
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.RenderBeginTag(HtmlTextWriterTag.Select)
				'<asp:ListItem Value="0">SFW</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "0")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("SFW")
				writer.RenderEndTag()

				'<asp:ListItem Value="1">6+</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "1")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("6+")
				writer.RenderEndTag()
				'<asp:ListItem Value="2">12+</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "2")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("12+")
				writer.RenderEndTag()
				'<asp:ListItem Value="3">16+</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "3")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("16+")
				writer.RenderEndTag()
				'<asp:ListItem Value="4">18+</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "4")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("18+")
				writer.RenderEndTag()
				'<asp:ListItem Value="5" Selected="True">NSFW</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "5")
				writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("NSFW")
				writer.RenderEndTag()

				'</asp:DropDownList>
				writer.RenderEndTag()

				'</div>
				writer.RenderEndTag()
				writer.RenderEndTag()
			End If

			'<div class="postform">
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<div class="postblock">После ответа</div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.Write("После ответа")
			writer.RenderEndTag()

			'<div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			'<label>
			writer.RenderBeginTag(HtmlTextWriterTag.Label)
			'<input type="checkbox" id="chkMoveToThread" runat="server" />переместиться к нити
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
			If MoveToThread Then
				writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
			End If
			writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "chkMoveToThread")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "chkMoveToThread")
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()
			writer.Write("переместиться к нити")
			'</label>
			writer.RenderEndTag()
			'</div>
			writer.RenderEndTag()
			writer.RenderEndTag()

			If ThreadManage Then

				' Закрепление нитей

				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<div class="postblock">Закрепление нитей</div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.Write("Закрепление нитей")
				writer.RenderEndTag()

				'<div>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				'<label>
				writer.RenderBeginTag(HtmlTextWriterTag.Label)
				'<input type="checkbox" id="chkTopThread" runat="server" />Нить будет только сверху
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "inp")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "chkTopThread")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "chkTopThread")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				writer.Write("Нить будет только сверху")
				'</label>
				writer.RenderEndTag()
				writer.RenderEndTag() '</div> postbuttons
				writer.RenderEndTag()

				' Установка анонимного доступа к треду

				'<div class="postform">
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postblock")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.Write("Анонимный доступ к треду")
				writer.RenderEndTag()

				writer.AddAttribute(HtmlTextWriterAttribute.Class, "postbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "lstAnonAccess")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "lstAnonAccess")
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.RenderBeginTag(HtmlTextWriterTag.Select)
				'<asp:ListItem Value="0">SFW</asp:ListItem>
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "0")
				writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("Доступ для чтения и ответов")
				writer.RenderEndTag()
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "1")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("Доступ только для чтения")
				writer.RenderEndTag()
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "2")
				writer.RenderBeginTag(HtmlTextWriterTag.Option)
				writer.Write("Доступ запрещён")
				writer.RenderEndTag() 'Option

				writer.RenderEndTag() 'Select
				writer.RenderEndTag() 'div postbuttons
				writer.RenderEndTag()

				'UNDONE Поле для редактирования списка запрещённых ролей
			End If
			'</div> postarea
			writer.RenderEndTag()

			If m_RulesInfo.Count > 0 Then
				'<ul class="rules">
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "rules")
				writer.RenderBeginTag(HtmlTextWriterTag.Ul)
				For i As Integer = 0 To m_RulesInfo.Count - 1
					writer.RenderBeginTag(HtmlTextWriterTag.Li)
					If m_RulesInfo.Item(i).Simple Then
						'<li>RulesItem</li>
						writer.Write(m_RulesInfo.Item(i).Text1)
					Else
						'<li>Text1<a href="Ссылка">Ссылка</a>Text2</li>
						writer.Write(m_RulesInfo.Item(i).Text1)
						writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(m_RulesInfo.Item(i).RulesLinkUrl))
						writer.RenderBeginTag(HtmlTextWriterTag.A)
						writer.Write(m_RulesInfo.Item(i).RulesLinkText)
						writer.RenderEndTag()
						writer.Write(m_RulesInfo.Item(i).Text2)
					End If
					writer.RenderEndTag()
				Next i

				'<li>
				writer.RenderBeginTag(HtmlTextWriterTag.Li)
				'<a href="ссылка">Каталог</a>
				writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(String.Format("~/{0}/catalog.aspx", BoardName)))
				writer.RenderBeginTag(HtmlTextWriterTag.A)
				writer.Write("Просмотр каталога")
				writer.RenderEndTag()
				'</li>
				writer.RenderEndTag()

				'</ul>
				writer.RenderEndTag()
			End If
		End If
	End Sub

	''' <summary>
	''' Загружает данные
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function LoadPostData(ByVal postDataKey As String, _
	  ByVal Values As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
		' Прежде чем отправлять событие щелчка,
		' нужно проверить введённые пользователем данные
		BoardName = CutString64(BoardName)
		If BoardName.Length > 0 Then
			Dim strWF As String
			EMail = CutString64(Values.Item(UniqueID & IdSeparator & "txtEmail"), 1024)
			MessageText = Values.Item(UniqueID & IdSeparator & "txtMessage")
			Subject = CutString64(Values.Item(UniqueID & IdSeparator & "txtSubject"))
			UserName = CutString64(Values.Item(UniqueID & IdSeparator & "txtUserName"), 32)
			With m_Info
				.PostedFiles = New Generic.List(Of HttpPostedFile)
				.VideoLinks = New Generic.List(Of String)
				' Строки месте с экранированием символов
				'.BoardName = GetSafeString(BoardName)
				'.Email = GetSafeString(EMail)
				'.MessageText = GetSafeString(MessageText)
				'.Subject = GetSafeString(Subject)
				.BoardName = BoardName
				.Email = EMail
				.MessageText = MessageText
				.Subject = If(String.IsNullOrEmpty(Subject), DefaultSubject, Subject)
				' имя и пароль не экранированы
				.UserName = UserName
				.Password = CutString64(Values.Item(UniqueID & IdSeparator & "txtPassword"))

				strWF = Values.Item(UniqueID & IdSeparator & "txtVideo")
				If Not String.IsNullOrEmpty(strWF) Then
					m_VideoList.AddRange(strWF.Split(","c))
				End If
				' Булёвые переменные
				strWF = Values.Item(UniqueID & IdSeparator & "chkWithoutFile")
				If Not String.IsNullOrEmpty(strWF) Then
					If strWF = "on" Then
						.WithoutFiles = True
					End If
				End If
				strWF = Values.Item(UniqueID & IdSeparator & "chkMoveToThread")
				If Not String.IsNullOrEmpty(strWF) Then
					If strWF = "on" Then
						.MoveToThread = True
					End If
				End If
				strWF = Values.Item(UniqueID & IdSeparator & "chkCaptchaMagik")
				If Not String.IsNullOrEmpty(strWF) Then
					If strWF = "on" Then
						' Капча-ведунья
						If imgCartcha IsNot Nothing Then
							.MagicCaptcha = imgCartcha.GenerateMagicCaptcha
						End If
					End If
				End If
				strWF = Values.Item(UniqueID & IdSeparator & "chkTopThread")
				If Not String.IsNullOrEmpty(strWF) Then
					If strWF = "on" Then
						.TopThread = True
					End If
				End If
				' Добавляем файлы в коллекцию
				' только если не установлен флажок
				If .MagicCaptcha Is Nothing AndAlso Not .WithoutFiles Then
					For i As Integer = 0 To Page.Request.Files.Count - 1
						' Не добавляем файлы меньше 6 байт
						If Page.Request.Files.Item(i).ContentLength > 5 Then
							.PostedFiles.Add(Page.Request.Files.Item(i))
						End If
					Next i
				End If
				'ViewState.Add("MaxFiles", m_Files.Count)

				' Числовые переменные
				'.ThreadNumber = ThreadNumber
				'ThreadNumber = .ThreadNumber
				Long.TryParse(Values.Item(Me.ClientID & ClientIDSeparator & "txtThreadNumber"), ThreadNumber)
				.ThreadNumber = ThreadNumber
				Long.TryParse(Values.Item(UniqueID & IdSeparator & "lstRating"), .Rating)
			End With
			Return True
		Else
			Return False
		End If
	End Function

	Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
		If UserNameMustExists AndAlso m_Info.UserName.Length = 0 Then
			Return
		End If
		If imgCartcha IsNot Nothing Then
			If imgCartcha.IsValid Then
				' Вызываем события
				If m_Info.ThreadNumber > 0 Then
					RaiseEvent AnswerToThread(Me, m_Info)
				Else
					If m_Info.Subject = "$регистрация" AndAlso m_Info.UserName.Length > 0 AndAlso m_Info.Password.Length > 0 Then
						RaiseEvent RegisterUserNeeded(Me, m_Info)
					Else
						RaiseEvent ThreadCreateNeeded(Me, m_Info)
					End If
				End If
			Else
				RaiseEvent CaptchaNotValid(Me, imgCartcha.ErrorMessage)
			End If
		Else
			' Вызываем события
			If Convert.ToBoolean(m_Info.ThreadNumber) Then
				RaiseEvent AnswerToThread(Me, m_Info)
			Else
				If m_Info.Subject = "$регистрация" AndAlso Convert.ToBoolean(m_Info.UserName.Length) AndAlso Convert.ToBoolean(m_Info.Password.Length) Then
					RaiseEvent RegisterUserNeeded(Me, m_Info)
				Else
					RaiseEvent ThreadCreateNeeded(Me, m_Info)
				End If
			End If
		End If
	End Sub

	Public Overloads Function IsLiteralContent() As Boolean
		Return False
	End Function

	''' <summary>
	''' Добавляет правило
	''' </summary>
	''' <param name="RulesItem"></param>
	''' <remarks></remarks>
	Public Sub AddRulesRow(ByVal RulesItem As String)
		Dim mRules As RulesInfo = New RulesInfo
		With mRules
			.Simple = True
			.Text1 = RulesItem
		End With
		m_RulesInfo.Add(mRules)
	End Sub

	''' <summary>
	''' Добавляет правило со ссылкой
	''' </summary>
	''' <param name="strText1"></param>
	''' <param name="RulesLinkText"></param>
	''' <param name="RulesLinkUrl"></param>
	''' <param name="strText2"></param>
	''' <remarks></remarks>
	Public Sub AddRulesRow(ByVal strText1 As String, ByVal RulesLinkText As String, ByVal RulesLinkUrl As String, ByVal strText2 As String)
		Dim mRules As RulesInfo = New RulesInfo
		With mRules
			.Text1 = strText1
			.Text2 = strText2
			' Гиперссылка
			.RulesLinkText = RulesLinkText
			.RulesLinkUrl = RulesLinkUrl
		End With
		m_RulesInfo.Add(mRules)
	End Sub

	Private Sub Answer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		If Not Page.ClientScript.IsClientScriptIncludeRegistered("WebImageBoardControlBoardPage") Then
			Page.ClientScript.RegisterClientScriptInclude(GetType(Page), "WebImageBoardControlBoardPage", Me.Page.ClientScript.GetWebResourceUrl(GetType(Answer), "WebImageBoardControls.BoardPage.js"))
		End If
		' Добавляю скрытое поле с номером ответа
		Page.ClientScript.RegisterHiddenField(Me.ClientID & ClientIDSeparator & "txtThreadNumber", ThreadNumber.ToString)
	End Sub

	' В этом месте нужно создавать дочерние элементы управления
	Protected Overrides Sub CreateChildControls()
		imgCartcha = New WebControlCaptcha.CaptchaControl
		With imgCartcha
			.TextBoxCssClass = "inp"
			.ToolTip = "Кириллическая няшная капча"
			'CaptchaBackgroundNoise = "Low"
			.Visible = False
		End With
		imgCartcha.CaptchaChars = CaptchaChars
		Me.Controls.Add(imgCartcha)
	End Sub

	Public Sub New()
		m_RulesInfo = New Generic.List(Of RulesInfo)
		m_VideoList = New Generic.List(Of String)
		UserNameVisible = True
		EMailVisible = True
		m_RatingListVisible = True
		ThreadMode = ThreadState.CreateThread
	End Sub

	Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
		Page.RegisterRequiresControlState(Me)
		MyBase.OnInit(e)
	End Sub

	Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent

	End Sub

	Private Function CutString64(ByVal strToTrim As String, Optional ByVal Length As Integer = 64) As String
		Dim sb As StringBuilder = New StringBuilder(strToTrim)
		sb.Length = Math.Min(sb.Length, Length)
		Return sb.ToString
	End Function
End Class
