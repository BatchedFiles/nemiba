Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Threading

<ToolboxData("<{0}:BoardThread runat=""server""></{0}:BoardThread>")> _
Public Class BoardPage
	Inherits Control
	Implements INamingContainer
	Implements IPostBackDataHandler

	Private m_BoardName As String
	Private m_ThreadNumber As Long
	Private m_PageNumber As Long
	Private m_RealUserName As String
	Private m_StartPosition As Long
	Private m_CurrentNSFWRating As Long
	Private m_PreModerationEnabled As Boolean
	Private m_ReadOnly As Boolean
	Private m_ThreadManage As Boolean
	Private m_BanUsers As Boolean
	Private m_UserNameEnable As Boolean
	Private m_DefaultUserName As String
	Private m_TimeEnable As Boolean
	Private m_UserCookie As String
	Private m_GetArchiveThread As Boolean
	Private m_ResFolder As String
	Private m_MaxMessagesPerPagePerThread As Long
	Private m_ColHiddenThreads As List(Of Long)
	Private m_MaxThreadsPerPage As Long
	Private m_PostNumber As Long
	Private m_TextAreaId As String

	''' <summary>
	''' Количество изображений для разворота
	''' </summary>
	''' <remarks></remarks>
	Private m_ExpandImagexCount As Long

#Region "Свойства"

	Public Property NewsManage As Boolean

	''' <summary>
	''' Возвращает/устаналивает режим каталога
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property IsCatalog() As Boolean

	''' <summary>
	''' Возвращает/устанавливает класс треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property BoardThread() As ImageBoardThreadBase

	''' <summary>
	''' Возвращает/устанавливает клиентский идентификатор надписи с ответом
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property AnswerToThreadLabelClientId() As String

	''' <summary>
	''' Возвращает/устанавливает клиентский идентификатор поля с номером треда
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadNumberTextClientId() As String

	''' <summary>
	''' Возвращает/устанавливает клиентский идентификатор текстового поля 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property TextAreaClientId() As String
		Get
			Return m_TextAreaId
		End Get
		Set(ByVal value As String)
			m_TextAreaId = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает номер поста для отображения
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property PostNumber() As Long
		Get
			Return m_PostNumber
		End Get
		Set(ByVal value As Long)
			m_PostNumber = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает коллекцию скрытых тредов
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ColHiddenThreads() As List(Of Long)
		Get
			Return m_ColHiddenThreads
		End Get
		Set(ByVal value As List(Of Long))
			m_ColHiddenThreads = value
		End Set
	End Property

	''' <summary>
	''' Устанавливает/возвращает путь к папке хранения файлов
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ResourceDirectory() As String
		Get
			Return m_ResFolder
		End Get
		Set(ByVal value As String)
			m_ResFolder = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает имя раздела
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property BoardName() As String
		Get
			Return m_BoardName
		End Get
		Set(ByVal value As String)
			m_BoardName = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает номер треда для отображения
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadNumber() As Long
		Get
			Return m_ThreadNumber
		End Get
		Set(ByVal value As Long)
			m_ThreadNumber = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает номер страницы
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property PageNumber() As Long
		Get
			Return m_PageNumber
		End Get
		Set(ByVal value As Long)
			m_PageNumber = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает настоящее имя пользователя
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property RealUserName() As String
		Get
			Return m_RealUserName
		End Get
		Set(ByVal value As String)
			m_RealUserName = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает номер начального сообщения для показа
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property StartPosition() As Long
		Get
			Return m_StartPosition
		End Get
		Set(ByVal value As Long)
			m_StartPosition = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает рейтинг сообщений, выше которого они не показываются
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property NSFWRating() As Long
		Get
			Return m_CurrentNSFWRating
		End Get
		Set(ByVal value As Long)
			m_CurrentNSFWRating = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает значение, можно ли управлять тредом
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property ThreadManage() As Boolean
		Get
			Return m_ThreadManage
		End Get
		Set(ByVal value As Boolean)
			m_ThreadManage = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает значение, можно ли банить создателей тредов
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property BanUsers() As Boolean
		Get
			Return m_BanUsers
		End Get
		Set(ByVal value As Boolean)
			m_BanUsers = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает печенье пользователя, загружающего тред
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property UserCookie() As String
		Get
			Return m_UserCookie
		End Get
		Set(ByVal value As String)
			m_UserCookie = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает значение, отображать ли архивные треды
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property GetArchiveThread() As Boolean
		Get
			Return m_GetArchiveThread
		End Get
		Set(ByVal value As Boolean)
			m_GetArchiveThread = value
		End Set
	End Property

#End Region

	Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
		' Если подключение установлено, то отображаем
		If BoardThread Is Nothing Then
			' Показываем сообщение, что подключение отсутствует
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write("Подключение к базе данных не установлено")
			writer.RenderEndTag()
		Else
			' Получить настройки раздела
			Dim colBoardInfo As BoardInfo = BoardThread.GetBoardInfo()
			With colBoardInfo
				m_PreModerationEnabled = .PreModerationEnabled
				If ImageBoardThreadBase.IsSystemBoard(m_BoardName) AndAlso Not NewsManage Then
					m_ReadOnly = True
				Else
					m_ReadOnly = If(.IsReadOnly, True, .AnonymousAccess = AnonymousAccess.OnlyCreateThread)
				End If

				m_UserNameEnable = .UserNameEnable
				m_DefaultUserName = .DefaultUserName
				m_TimeEnable = .TimeEnable
				m_MaxMessagesPerPagePerThread = .MaxMessagesPerPagePerThread
				m_MaxThreadsPerPage = .MaxThreadsPerPage
			End With
			BoardThread.LockThreadBase()
			If m_PostNumber > 0 Then
				' Отобразить пост
				Dim intThreadNumber As Long = BoardThread.GetThreadNumber(m_PostNumber, False, False)
				If intThreadNumber > 0 Then
					' Загрузить пост-сообщение
					Dim objMessage As MessageInfo = BoardThread.GetMessage(m_PostNumber)
					If ShowNotVerifiedMessage(objMessage.MessageDeleted, objMessage.Checked, objMessage.Cookie, objMessage.Rating) Then
						LoadMessage(writer, objMessage)
					Else
						writer.RenderBeginTag(HtmlTextWriterTag.Span)
						writer.Write("Ничего не найдено")
						writer.RenderEndTag()
					End If
				Else
					writer.RenderBeginTag(HtmlTextWriterTag.Span)
					writer.Write("Ничего не найдено")
					writer.RenderEndTag()
				End If
			Else
				If IsCatalog Then
					LoadPage(writer)
				Else
					If m_ThreadNumber > 0 Then
						LoadThread(writer)
					Else
						LoadPage(writer)
					End If
				End If
			End If
			BoardThread.UnLockThreadBase()
		End If
	End Sub

	''' <summary>
	''' Загружает тред на страницу
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function LoadThread(ByVal writer As HtmlTextWriter) As Boolean
		' Получаем номер треда
		Dim intThreadNumber As Long = BoardThread.GetThreadNumber(m_ThreadNumber, False, False)
		If intThreadNumber > 0 Then
			Dim strOPThreadNumber As String = intThreadNumber.ToString
			' Проверяем на возможность загрузки ОП-поста
			' если загружен, то загружаем остальное
			If LoadOPMessage(writer, intThreadNumber) Then
				' UNDONE Получение списка сообщений с определённого номера
				Dim colMessages As List(Of Long) = BoardThread.GetMessageList(intThreadNumber)
				For i As Integer = 1 To colMessages.Count - 1
					Dim objMessage As MessageInfo = BoardThread.GetMessage(colMessages.Item(i))	'  GetMessage(objReader)
					If ShowNotVerifiedMessage(objMessage.MessageDeleted, objMessage.Checked, objMessage.Cookie, objMessage.Rating) Then
						' Загружаем сообщение
						LoadMessage(writer, objMessage)
					Else
						' Нужно как-то сигнализировать, что сообщение не загружено
					End If
				Next
				'("<hr />"))
				writer.RenderBeginTag(HtmlTextWriterTag.Hr)
				writer.RenderEndTag()
				Return True
			End If
		End If
		Return False
	End Function

	''' <summary>
	''' Загружает страницу
	''' </summary>
	''' <remarks></remarks>
	Private Function LoadPage(ByVal writer As HtmlTextWriter) As Boolean
		Dim colThreadsInPage As Generic.List(Of ThreadInfo)
		If IsCatalog Then
			colThreadsInPage = BoardThread.GetThreadList(-1, m_GetArchiveThread)
		Else
			colThreadsInPage = BoardThread.GetThreadList(m_PageNumber, m_GetArchiveThread)
		End If
		If colThreadsInPage.Count = 0 Then
			writer.RenderBeginTag(HtmlTextWriterTag.P)
			writer.Write("Въ салонѣ тихо, пыльно и пусто. Только въ одномъ изъ угловъ вышиваетъ гладью пожилая княжна.")
			writer.RenderEndTag()
		Else
			For Each objThread As ThreadInfo In colThreadsInPage
				Dim strOPThreadNumber As String = objThread.ThreadNumber.ToString
				' Главный див треда
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "thread")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "thread" & ClientIDSeparator & m_BoardName & ClientIDSeparator & strOPThreadNumber)
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				' Проверим тред на скрытие
				If m_ColHiddenThreads.Contains(objThread.ThreadNumber) Then
					' Тред скрыт, не отображаем
					'<p class="phiddenthread">Нить 
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "phiddenthread")
					writer.RenderBeginTag(HtmlTextWriterTag.P)
					writer.Write("Нить ")

					'<a href="/b/src/1.aspx">1</a>
					writer.AddAttribute(HtmlTextWriterAttribute.Class, "linktohiddenthread")
					writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(String.Format("~/{0}/src/{1}.aspx", m_BoardName, strOPThreadNumber)))
					writer.RenderBeginTag(HtmlTextWriterTag.A)
					writer.Write(objThread.ThreadNumber)
					writer.RenderEndTag()

					writer.Write(" скрыта.")
					'</p>
					writer.RenderEndTag()
				Else
					Dim intMessagesCount As Long = BoardThread.GetMessagesCount(objThread.ThreadNumber)

					' Проверяем на возможность загрузки ОП-поста
					' если загружен, то загружаем остальное
					If LoadOPMessage(writer, objThread.ThreadNumber) Then
						' взять количество последних сообщений из конфигурации
						If intMessagesCount > m_MaxMessagesPerPagePerThread + 1 Then
							'<div class="omittedposts">
							writer.AddAttribute(HtmlTextWriterAttribute.Class, "omittedposts")
							writer.RenderBeginTag(HtmlTextWriterTag.Div)
							writer.Write(String.Format("Показываю последние {0} сообщений, щёлкни «Ответить», чтобы увидеть нить целиком.", m_MaxMessagesPerPagePerThread - 1))
							'</div>
							writer.RenderEndTag()
						Else
						End If
						If Not IsCatalog Then
							Dim colMessages As List(Of Long) = BoardThread.GetMessageListInPage(objThread.ThreadNumber)
							For i As Integer = 1 To colMessages.Count - 1
								Dim objMessage As MessageInfo = BoardThread.GetMessage(colMessages.Item(i))
								If ShowNotVerifiedMessage(objMessage.MessageDeleted, objMessage.Checked, objMessage.Cookie, objMessage.Rating) Then
									' Загружаем сообщение
									LoadMessage(writer, objMessage)
								Else
									' Сигнализируем, что сообщение не загружено
									'<div class="reply">
									writer.AddAttribute(HtmlTextWriterAttribute.Class, "reply")
									writer.RenderBeginTag(HtmlTextWriterTag.Div)
									'<div>
									writer.AddAttribute(HtmlTextWriterAttribute.Class, "msgbody")
									writer.RenderBeginTag(HtmlTextWriterTag.Div)
									'Мамка в комнате.
									writer.Write(String.Format("Сообщение {0} скрыто настройками цензуры", objMessage.MessageNumber))
									'</div>
									writer.RenderEndTag()
									'</div>
									writer.RenderEndTag()
								End If
							Next
						End If
					Else
						' Сигнализируем, что тред не загружен
						'<div>Мамка в комнате.</div>
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "reply")
						writer.RenderBeginTag(HtmlTextWriterTag.Div)
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "msgbody")
						writer.RenderBeginTag(HtmlTextWriterTag.Div)
						writer.Write(String.Format("Сообщение {0} скрыто настройками цензуры", strOPThreadNumber))
						writer.RenderEndTag()
						writer.RenderEndTag()
					End If
				End If
				'<hr />
				writer.RenderBeginTag(HtmlTextWriterTag.Hr)
				writer.RenderEndTag()
				'</div>
				writer.RenderEndTag()
			Next objThread
		End If
		Return False
	End Function

	Private Function LoadCatalog() As Integer
		Return 0
	End Function

	Private Sub LoadMessage(ByVal writer As HtmlTextWriter, ByVal objMessage As MessageInfo)
		Dim strMessageNumber As String = objMessage.MessageNumber.ToString
		' Загружаем ответы на ОП

		'<div class="reply">
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "reply")
		writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "Kohihtor" & ClientIDSeparator & m_BoardName & ClientIDSeparator & strMessageNumber)
		writer.RenderBeginTag(HtmlTextWriterTag.Div)

		If objMessage.MessageDeleted Then
			'<span>Создатель интернетов удалил это сообщение.</span>
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write("Создатель интернетов удалил это сообщение.")
			writer.RenderEndTag()
		Else
			If Not String.IsNullOrEmpty(objMessage.Subject) Then
				'<h4> & objMessage.Subject & </h4>
				writer.RenderBeginTag(HtmlTextWriterTag.H3)
				writer.Write(objMessage.Subject)
				writer.RenderEndTag()
			End If

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "msghead")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)

			'<a id=""i" & strMessageNumber & """></a>
			writer.AddAttribute(HtmlTextWriterAttribute.Id, "i" & strMessageNumber)
			writer.RenderBeginTag(HtmlTextWriterTag.A)
			writer.RenderEndTag()

			'<label> Для установки галочки щелчком по теме сообщения
			writer.RenderBeginTag(HtmlTextWriterTag.Label)

			'<input name=""chkDeleteMessage"" value=""" & strMessageNumber & """ type=""checkbox"" />"))
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "chkDeleteMessage")
			writer.AddAttribute(HtmlTextWriterAttribute.Value, strMessageNumber)
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()

			' Имя пользователя
			If m_UserNameEnable Then
				LoadUserName(writer, objMessage.UserName, objMessage.EMail, objMessage.UserName)
			End If
			' Дата
			If m_TimeEnable Then
				AddDate(writer, objMessage.DateTime)
			End If

			'</label>
			writer.RenderEndTag()

			If Not m_ReadOnly Then
				' Быстрый ответ
				'writer.Write(String.Format("<svg:svg version=""1.1"" baseProfile=""full"" width=""16px"" height=""16px""><svg:circle onclick=""QuickReply({0}, {1}, {2}, {3})"" cx=""8px"" cy=""8px"" r=""5px"" fill=""#ff0000"" stroke=""#000000"" stroke-width=""2px""/></svg:svg> ", strMessageNumber, m_HiddenTextId, m_TextAreaId, m_AnswerToThreadLabelClientId))
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "button")
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, String.Format("QuickReply({0}, {1}, {2}, {3})", strMessageNumber, ThreadNumberTextClientId, m_TextAreaId, AnswerToThreadLabelClientId))
				writer.AddAttribute(HtmlTextWriterAttribute.Title, "Быстрый ответ на " & strMessageNumber)
				writer.AddAttribute(HtmlTextWriterAttribute.Value, ">>")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				writer.Write(XhtmlTextWriter.SpaceChar)
			End If

			' Рейтинг сообщения
			writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "Rating" & ClientIDSeparator & m_BoardName & ClientIDSeparator & strMessageNumber)
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "rating")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write(objMessage.Rating.ToString)
			writer.RenderEndTag()
			writer.Write(XhtmlTextWriter.SpaceChar)

			'<span class="reflink">
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "reflink")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write("№" & strMessageNumber)
			'</span>
			writer.RenderEndTag()

			'</div> Контейнер заголовка сообщения
			writer.RenderEndTag()

			' Файлы
			LoadFiles(writer, objMessage.MessageNumber, objMessage.Files)

			' Загружаем сообщение
			LoadComment(writer, objMessage.MessageText)

			' Загружаем ответы
			LoadAnswers(writer, objMessage.MessageNumber, objMessage.Answers)

			' UNDONE Админские кнопочки
			If m_ThreadManage Then

				writer.AddAttribute(HtmlTextWriterAttribute.Class, "adminbuttons")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)

				' Права доступа к треду
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "button")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "txtUserName")
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "txtUserName")
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "Права доступа к треду")
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()

				' Установка рейтинга и отметить сообщение
				writer.Write("Текущий рейтинг " & objMessage.Rating.ToString)

				' Отметить сообщение
				' Быстрое удаление сообщения
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "button")
				writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID & IdSeparator & "cmdRemove" & m_BoardName & objMessage.MessageNumber)
				writer.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "cmdRemove" & m_BoardName & objMessage.MessageNumber)
				writer.AddAttribute(HtmlTextWriterAttribute.Value, "Удалить")
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()

				'</div>
				writer.RenderEndTag()
			End If

			' UNDONE  Забанить пользователя
			End If
			'</div> див ответа
			writer.RenderEndTag()
	End Sub

	''' <summary>
	''' Загружаем текст сообщения
	''' </summary>
	''' <param name="writer"></param>
	''' <param name="strMessageText"></param>
	''' <remarks></remarks>
	Private Sub LoadComment(ByVal writer As HtmlTextWriter, ByVal strMessageText As String)
		If Not String.IsNullOrEmpty(strMessageText) Then
			'<div class="blockquote">Контейнер текста сообщения</div>
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "msgbody")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)

			ConvertToLink(writer, strMessageText)

			writer.RenderEndTag()
		End If
	End Sub

	''' <summary>
	''' Загружаем Оп-сообщение
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function LoadOPMessage(ByVal writer As HtmlTextWriter, ByVal MessageNumber As Long) As Boolean
		Dim objMessage As MessageInfo = BoardThread.GetMessage(MessageNumber)

		If ShowNotVerifiedMessage(False, objMessage.Checked, objMessage.Cookie, objMessage.Rating) Then

			' Загружаем ОП-пост 
			'<div class="oppost">
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "oppost")
			writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "Kohihtor" & ClientIDSeparator & m_BoardName & ClientIDSeparator & MessageNumber)
			writer.RenderBeginTag(HtmlTextWriterTag.Div)

			'<h2 class="filetitle">objMessage.Subject</h2>
			writer.RenderBeginTag(HtmlTextWriterTag.H2)
			If String.IsNullOrEmpty(objMessage.Subject) Then
				writer.Write("Нить без темы")
			Else
				writer.Write(objMessage.Subject)
			End If
			writer.RenderEndTag()
			'writer.Write(XhtmlTextWriter.SpaceChar)

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "msghead")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)

			'<a id="i" & strOPThreadNumber & ">
			writer.AddAttribute(HtmlTextWriterAttribute.Id, "i" & MessageNumber)
			writer.RenderBeginTag(HtmlTextWriterTag.A)
			'</a>
			writer.RenderEndTag()

			'<label>
			writer.RenderBeginTag(HtmlTextWriterTag.Label)
			'<input name="chkDeleteMessage" value=" & strOPThreadNumber & " type="checkbox" />
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "chkDeleteMessage")
			writer.AddAttribute(HtmlTextWriterAttribute.Value, MessageNumber.ToString)
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox")
			writer.RenderBeginTag(HtmlTextWriterTag.Input)
			writer.RenderEndTag()

			' Добавляем имя пользователя
			If m_UserNameEnable Then
				LoadUserName(writer, objMessage.UserName, objMessage.EMail, objMessage.RealUserName)
			End If
			If m_TimeEnable Then
				AddDate(writer, objMessage.DateTime)
			End If
			'</label>
			writer.RenderEndTag()

			' UNDONE Кнопка Скрыть нить

			' UNDONE Кнопка Наблюдать за тредом

			If m_ReadOnly Then
			Else
				' Быстрый ответ
				'writer.Write(String.Format("<svg:svg version=""1.1"" baseProfile=""full"" width=""16px"" height=""16px""><svg:circle onclick=""QuickReply({0}, {1}, {2}, {3})"" cx=""8px"" cy=""8px"" r=""5px"" fill=""#ff0000"" stroke=""#000000"" stroke-width=""2px""/></svg:svg> ", MessageNumber, m_HiddenTextId, m_TextAreaId, m_AnswerToThreadLabelClientId))
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "but")
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "button")
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, String.Format("QuickReply({0}, {1}, {2}, {3})", MessageNumber, ThreadNumberTextClientId, m_TextAreaId, AnswerToThreadLabelClientId))
				writer.AddAttribute(HtmlTextWriterAttribute.Title, "Быстрый ответ на " & MessageNumber)
				writer.AddAttribute(HtmlTextWriterAttribute.Value, ">>")
				writer.RenderBeginTag(HtmlTextWriterTag.Input)
				writer.RenderEndTag()
				writer.Write(XhtmlTextWriter.SpaceChar)

				' Рейтинг сообщения
				writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID & ClientIDSeparator & "Rating" & ClientIDSeparator & m_BoardName & ClientIDSeparator & MessageNumber)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "rating")
				writer.RenderBeginTag(HtmlTextWriterTag.Span)
				writer.Write(objMessage.Rating.ToString)
				writer.RenderEndTag()
				writer.Write(XhtmlTextWriter.SpaceChar)

				writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(String.Format("~/{0}/src/{1}.aspx", m_BoardName, MessageNumber)))
				writer.RenderBeginTag(HtmlTextWriterTag.A)
				writer.Write("Ответить")
				writer.RenderEndTag()
				writer.Write(XhtmlTextWriter.SpaceChar)
			End If

			' Номер сообщения как текст
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "reflink")
			'<span class="reflink">	
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write("№" & MessageNumber.ToString)
			'</span>
			writer.RenderEndTag()

			'</div> Контейнер заголовка сообщения
			writer.RenderEndTag()

			' Загружаем список файлов
			LoadFiles(writer, objMessage.MessageNumber, objMessage.Files)

			' Загружаем сообщение
			LoadComment(writer, objMessage.MessageText)

			' Загружаем ответы
			LoadAnswers(writer, objMessage.MessageNumber, objMessage.Answers)

			' Добавляем админские кнопочки
			If m_ThreadManage Then
				'UNDONE Закрепление треда

				'UNDONE Настройка прав доступа к треду

				'UNDONE Удаление треда
			End If
			' Добавить кнопочку Забанить пользователя
			If m_BanUsers Then
				'UNDONE Запломбировать пользователя
			End If

			'</div>
			writer.RenderEndTag()
			Return True
		End If
		Return False
	End Function

	''' <summary>
	''' Разбивает сообщение на фрагменты и вставляет между каждыми ссылку на сообщение и номер треда
	''' </summary>
	''' <param name="writer"></param>
	''' <param name="strTemp"></param>
	''' <remarks></remarks>
	Private Sub ConvertToLink(ByVal writer As HtmlTextWriter, ByVal strTemp As String, Optional ByVal Deep As Integer = 0)
		' Ищем вот это
		'<a><board>test</board><thread>9897</thread></a>
		' Получаем параметры в виде названия раздела и номера треда
		' Разбиваем строку
		' Добавляем первую часть строки,
		' Потом создаём ссылку на параметры
		' Рекурсивно вызываем самих себя
		If Deep > 20 Then
			' Нет смысла в бесконечной рекурсии, просто добавляем
			'divThreads.Controls.Add(New LiteralControl(strTemp))
			writer.Write(strTemp)
		Else
			Dim reg As Regex = New Regex("<a><external>(?<external>[0|1])</external><board>(?<board>\S+)</board><thread>(?<thread>[\d]+)</thread></a>")
			Dim m As Match = reg.Match(strTemp)
			If m.Success Then
				' Нашли
				Dim strBoardName As String = m.Groups("board").ToString
				Dim intThreadNumber As Long
				Long.TryParse(m.Groups("thread").ToString, intThreadNumber)
				' Теперь можно разбить на две части
				' После первой части добавить ссылку
				' А вторую часть пустить на нас
				Dim intStart As Integer = m.Index
				'divThreads.Controls.Add(New LiteralControl(strTemp.Substring(0, m.Index)))
				writer.Write(strTemp.Substring(0, m.Index))

				'Dim lnkAnswer As HyperLink = New HyperLink
				If intThreadNumber > 0 Then
					'lnkAnswer.NavigateUrl = "~/" & strBoardName & "/src/" & intThreadNumber & ".aspx#i" & intThreadNumber
					writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(String.Format("~/{0}/src/{1}.aspx#i{1}", strBoardName, intThreadNumber)))
					writer.AddAttribute("onmouseover", String.Format("ShowToolTipPost(event, this, ""{0}"", {1}, ""{2}"")", strBoardName, intThreadNumber, ResolveUrl("~/webapi.asmx?op=GetPostHtml")))
					writer.RenderBeginTag(HtmlTextWriterTag.A)

					Dim blnExternal As Long
					Long.TryParse(m.Groups("external").ToString, blnExternal)
					If Convert.ToBoolean(blnExternal) Then
						'lnkAnswer.Text = "&gt;&gt;/" & strBoardName & "/" & intThreadNumber.ToString
						writer.Write("&gt;&gt;/" & strBoardName & "/" & intThreadNumber.ToString)
					Else
						'lnkAnswer.Text = "&gt;&gt;" & intThreadNumber.ToString
						writer.Write("&gt;&gt;" & intThreadNumber.ToString)
					End If
					writer.RenderEndTag()
				Else
					'lnkAnswer.NavigateUrl = "~/" & strBoardName & "/"
					'lnkAnswer.Text = "/" & strBoardName & "/"
					writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(String.Format("~/{0}/", strBoardName)))
					writer.RenderBeginTag(HtmlTextWriterTag.A)
					writer.Write(String.Format("/{0}/", strBoardName))
					writer.RenderEndTag()
				End If
				'divThreads.Controls.Add(lnkAnswer)
				' Отправляем на поиски далее
				Deep += 1
				ConvertToLink(writer, strTemp.Substring(m.Index + m.Length, strTemp.Length - (m.Index + m.Length)), Deep)
			Else
				' Не нашли
				' Просто добавлем
				'divThreads.Controls.Add(New LiteralControl(strTemp))
				writer.Write(strTemp)
			End If
		End If
	End Sub

	''' <summary>
	''' Загружаем ответы на сообщение
	''' </summary>
	''' <param name="writer"></param>
	''' <param name="intMessageNumber"></param>
	''' <remarks></remarks>
	Private Sub LoadAnswers(ByVal writer As HtmlTextWriter, ByVal intMessageNumber As Long, ByVal colAnswers As List(Of AnswerInfo))
		If colAnswers.Count > 0 Then
			'<div class="answers">Ответы:
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "answers")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			writer.Write("Ответы:")

			For Each objAnswer As AnswerInfo In colAnswers
				writer.Write(XhtmlTextWriter.SpaceChar)
				writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("~/" & objAnswer.AnswerBoardName & "/src/" & objAnswer.AnswerMessageNumber & ".aspx#i" & objAnswer.AnswerMessageNumber))
				writer.AddAttribute("onmouseover", String.Format("ShowToolTipPost(event, this, ""{0}"", {1}, ""{2}"")", objAnswer.AnswerBoardName, objAnswer.AnswerMessageNumber, ResolveUrl("~/webapi.asmx?op=GetPostHtml")))
				writer.RenderBeginTag(HtmlTextWriterTag.A)
				If m_BoardName = objAnswer.AnswerBoardName Then
					writer.Write("&gt;&gt;" & objAnswer.AnswerMessageNumber)
				Else
					writer.Write("&gt;&gt;/" & objAnswer.AnswerBoardName & "/" & objAnswer.AnswerMessageNumber)
				End If
				writer.RenderEndTag()
			Next
			'</div>
			writer.RenderEndTag()
		End If
	End Sub

	''' <summary>
	''' Проверяет непроверенное сообщение на возможность показа
	''' </summary>
	''' <param name="Verified"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function ShowNotVerifiedMessage(ByVal Deleted As Boolean, _
	  ByVal Verified As Boolean, _
	  ByVal strMessageCookie As String, _
	  ByVal intNSFWMessageRating As Long) As Boolean
		' Нужно как-то разграничить запрет отображения NSFW пользователем
		' от непроверенных сообщений
		If Deleted Then
			Return True
		Else
			If m_PreModerationEnabled Then
				If Verified Then
					If Convert.ToBoolean(m_CurrentNSFWRating) Then
						If m_CurrentNSFWRating > intNSFWMessageRating Then
							Return True
						End If
					Else
						Return True
					End If
				Else
					If m_BanUsers OrElse m_ThreadManage Then
						Return True
					Else
						If m_UserCookie = strMessageCookie Then
							If Convert.ToBoolean(m_CurrentNSFWRating) Then
								If m_CurrentNSFWRating > intNSFWMessageRating Then
									Return True
								End If
							Else
								Return True
							End If
						End If
					End If
				End If
			Else
				If Convert.ToBoolean(m_CurrentNSFWRating) Then
					If m_CurrentNSFWRating > intNSFWMessageRating Then
						Return True
					End If
				Else
					Return True
				End If
			End If
		End If
		Return False
	End Function

	''' <summary>
	''' Добавляет имя пользователя в сообщение
	''' </summary>
	''' <param name="strUserName"></param>
	''' <param name="strEmail"></param>
	''' <param name="strRegisteredUserName"></param>
	''' <remarks></remarks>
	Private Sub LoadUserName(ByVal writer As HtmlTextWriter, _
	  ByVal strUserName As String, _
	  ByVal strEmail As String, _
	  ByVal strRegisteredUserName As String)
		If strUserName.Length = 0 Then
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postername")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			writer.Write(m_DefaultUserName)
			'</span>
			writer.RenderEndTag()
			'&nbsp;"
			writer.Write(XhtmlTextWriter.SpaceChar)
		Else
			'"<span class=""postertrip"">" 
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "postertrip")
			writer.RenderBeginTag(HtmlTextWriterTag.Span)
			'& strUserName 
			writer.Write(strUserName)
			'& "</span>&nbsp;"
			writer.RenderEndTag()
			writer.Write(XhtmlTextWriter.SpaceChar)
		End If
		If strEmail.Length > 0 Then
			'<a href=""mailto:" & strEmail & """>"
			writer.AddAttribute(HtmlTextWriterAttribute.Href, "mailto:" & strEmail)
			writer.RenderBeginTag(HtmlTextWriterTag.A)
			' & strDefaultUserName & 
			writer.Write(strEmail)
			'"</a>&nbsp;"
			writer.RenderEndTag()
			writer.Write(XhtmlTextWriter.SpaceChar)
		End If
	End Sub

	''' <summary>
	''' Загружает список файлов на страницу
	''' </summary>
	''' <param name="intMessageNumber"></param>
	''' <remarks></remarks>
	Private Sub LoadFiles(ByVal writer As HtmlTextWriter, ByVal intMessageNumber As Long, ByVal colFiles As List(Of ImageboardFileInfo))
		For Each oFile As ImageboardFileInfo In colFiles
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "fileblock")
			writer.RenderBeginTag(HtmlTextWriterTag.Div)
			' Увеличиваем счётчик изображений для разворота
			m_ExpandImagexCount += 1
			Dim strArrayName As String = Me.ClientID & ClientIDSeparator & "txtExp" & m_ExpandImagexCount

			If oFile.FileDeleted Then
				'<span class="thumbnailmsg">Файл удалён</span>
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "thumbnailmsg")
				writer.RenderBeginTag(HtmlTextWriterTag.Span)
				writer.Write("Файл удалён")
				writer.RenderEndTag()
			Else
				Select Case oFile.FileType
					Case FileType.None
						' Файл
						'<span class="filesize">
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "filesize")
						writer.RenderBeginTag(HtmlTextWriterTag.Span)

						writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath))
						writer.RenderBeginTag(HtmlTextWriterTag.A)
						writer.Write(oFile.ShortFileName)
						writer.RenderEndTag()
						writer.Write(XhtmlTextWriter.SpaceChar)

						writer.Write(oFile.FileLength.ToString & " байт")
						'</span>
						writer.RenderEndTag()

						AddImage(writer, "img", oFile.ThumbWidth, oFile.ThumbHeight, oFile.UrlFilePath, Me.Page.ClientScript.GetWebResourceUrl(GetType(BoardPage), "WebImageBoardControls.generic.png"), Nothing)

					Case FileType.Image
						' Изображение

						If oFile.ImageHeight > oFile.ThumbHeight OrElse oFile.ImageWidth > oFile.ThumbWidth Then
							'<span class="filesize">
							writer.AddAttribute(HtmlTextWriterAttribute.Class, "filesize")
							writer.RenderBeginTag(HtmlTextWriterTag.Span)

							writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath))
							writer.RenderBeginTag(HtmlTextWriterTag.A)
							writer.Write(oFile.ShortFileName)
							writer.RenderEndTag()
							writer.Write(XhtmlTextWriter.SpaceChar)

							writer.Write(oFile.FileLength.ToString & " байт, " & oFile.ImageWidth.ToString & "x" & oFile.ImageHeight.ToString)
							'</span>
							writer.RenderEndTag()

							'<span class="thumbnailmsg">
							writer.AddAttribute(HtmlTextWriterAttribute.Class, "thumbnailmsg")
							writer.RenderBeginTag(HtmlTextWriterTag.Span)
							writer.Write("Показываю уменьшенную копию")
							'</span>
							writer.RenderEndTag()

							AddImage(writer, "img", oFile.ThumbWidth, oFile.ThumbHeight, oFile.ShortFileName, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.ThumbUrlFilePath), strArrayName)

							If m_PostNumber = 0 Then
								Page.ClientScript.RegisterArrayDeclaration(strArrayName, String.Format("""{0}"", ""{1}"", ""{2}""", oFile.ImageWidth, oFile.ImageHeight, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath)))
							End If
						Else
							AddImage(writer, "img", oFile.ThumbWidth, oFile.ThumbHeight, oFile.ShortFileName, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath), Nothing)
						End If

					Case FileType.MagicCaptcha
						' Капча-ведунья
						'<span class="filesize">
						writer.AddAttribute(HtmlTextWriterAttribute.Class, "filesize")
						writer.RenderBeginTag(HtmlTextWriterTag.Span)

						writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath))
						writer.RenderBeginTag(HtmlTextWriterTag.A)
						writer.Write("Капча-ведунья")
						writer.RenderEndTag()
						writer.Write(XhtmlTextWriter.SpaceChar)

						writer.Write(oFile.FileLength.ToString & " байт, " & oFile.ImageWidth.ToString & "x" & oFile.ImageHeight.ToString)
						'</span>
						writer.RenderEndTag()

						AddImage(writer, "img", oFile.ThumbWidth, oFile.ThumbHeight, "Капча-ведунья.jpg", ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & oFile.UrlFilePath), Nothing)

					Case FileType.VideoLink
						'UNDINE Вставка ссылки на видео
					Case FileType.Svg
						'UNDONE Вставка векторного рисунка
				End Select
			End If
			writer.RenderEndTag()
		Next oFile
	End Sub

	Private Sub AddImage(ByVal writer As HtmlTextWriter,
	   ByVal ClassName As String,
	   ByVal Width As Long,
	   ByVal Height As Long,
	   ByVal AltText As String,
	   ByVal Src As String,
	   ByVal ArrayName As String)
		With writer
			.AddAttribute(HtmlTextWriterAttribute.Class, ClassName)
			.AddAttribute(HtmlTextWriterAttribute.Width, Width.ToString)
			.AddAttribute(HtmlTextWriterAttribute.Height, Height.ToString)
			.AddAttribute(HtmlTextWriterAttribute.Alt, AltText)
			.AddAttribute(HtmlTextWriterAttribute.Src, Src)
			.AddAttribute(HtmlTextWriterAttribute.Id, Me.ClientID & ClientIDSeparator & "imgExp" & m_ExpandImagexCount)
			If Not String.IsNullOrEmpty(ArrayName) Then
				.AddAttribute(HtmlTextWriterAttribute.Onclick, String.Format("ExpandImage({0}, {1})", Me.ClientID & ClientIDSeparator & "imgExp" & m_ExpandImagexCount, ArrayName))
			End If
			.RenderBeginTag(HtmlTextWriterTag.Img)
			.RenderEndTag()
		End With
	End Sub

	Private Sub AddDate(ByVal writer As HtmlTextWriter, ByVal mDate As Date)
		'<span class="date"> & datDate.ToString & </span>&nbsp;
		With writer
			.AddAttribute(HtmlTextWriterAttribute.Class, "date")
			.RenderBeginTag(HtmlTextWriterTag.Span)
			.Write(mDate)
			.RenderEndTag()
			.Write(XhtmlTextWriter.SpaceChar)
		End With
	End Sub

	''' <summary>
	''' Добавляет ссылку на файл
	''' </summary>
	''' <param name="writer"></param>
	''' <remarks></remarks>
	Private Sub AddFileLink(ByVal writer As XhtmlTextWriter, ByVal strUrl As String)
		With writer
			.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(m_ResFolder & Path.AltDirectorySeparatorChar & m_BoardName & Path.AltDirectorySeparatorChar & strUrl))
			.RenderBeginTag(HtmlTextWriterTag.A)
			.Write("oFile.ShortFileName")
			.RenderEndTag()
		End With
	End Sub

	''' <summary>
	''' Щелчок по кнопке «Скрыть/показать тред»
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	''' <remarks></remarks>
	Private Sub cmdHide_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
		' Необходимо тред со страницы
		' и добавить номер треда в печенье
		Dim aTemp() As String = e.CommandArgument.ToString.Split(":"c)
		If aTemp.Length = 2 Then
			Dim BoardName As String = aTemp(0)
			Dim ThreadNumber As Long
			Long.TryParse(aTemp(1), ThreadNumber)
			Select Case e.CommandName
				Case "hide"
					' Скрыть нить
				Case "show"
					' Показать нить
			End Select
			'HideThread(HttpContext.Current.Request.Cookies.Get(HidddenThreadsCookie), BoardName, ThreadNumber)
			'Response.Redirect("~/" & strBoardName & "/" & intPageNumber.ToString & ".aspx", True)
		End If
	End Sub

	Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
		Return True
	End Function

	Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

	End Sub

	Private Sub BoardPage_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		' Регистрация необходимых скриптов
		If Not Page.ClientScript.IsClientScriptIncludeRegistered("WebImageBoardControlBoardPage") Then
			Page.ClientScript.RegisterClientScriptInclude(GetType(Page), "WebImageBoardControlBoardPage", Me.Page.ClientScript.GetWebResourceUrl(GetType(BoardPage), "WebImageBoardControls.BoardPage.js"))
		End If
	End Sub

End Class
