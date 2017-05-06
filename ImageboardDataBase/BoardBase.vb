Imports Microsoft.VisualBasic

Namespace ImageBoard

	Partial Public MustInherit Class ImageBoardThreadBase

		''' <summary>
		''' Возвращает/устанавливает путь к папке-хранилищу файлов имиджборды
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property ResourceDirectory() As String

		''' <summary>
		''' Отправка жалобы на сообщение
		''' </summary>
		''' <param name="MessageNumber">Номер сообщения, должен быть натуральным числом</param>
		''' <param name="СomplaintText">Текст жалобы</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateComplaint(ByVal MessageNumber As Long, _
		ByVal СomplaintText As String) As ErrorInfo

		''' <summary>
		''' Получение списка жалоб раздела
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetComplaintList() As List(Of ComplaintInfo)

		''' <summary>
		''' Удаление жалобы
		''' </summary>
		''' <param name="СomplaintId"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveComplaint(ByVal СomplaintId As Long) As ErrorInfo

		''' <summary>
		''' Получение списка ролей, запрещённых для ответа в тред
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetRolesFromAnswersThreadDenyList(ByVal ThreadNumber As Long) As List(Of DenyRolesForThreadAnswersInfo)

		''' <summary>
		''' Добавление роли в запрещённые для ответа в тред
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="RoleName"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function AddRoleFromAnswersThreadDeny(ByVal ThreadNumber As Long, _
		  ByVal RoleName As String) As ErrorInfo

		''' <summary>
		''' Удаление группы из списка запрещённых для ответа в тред
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="RoleName"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveRoleFromAnswersThreadDeny(ByVal ThreadNumber As Long, _
		ByVal RoleName As String) As ErrorInfo

		''' <summary>
		''' Редактирование закреплённости треда
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="TopFlag"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditThreadTopFlag(ByVal ThreadNumber As Long, _
		 ByVal TopFlag As Boolean) As ErrorInfo

		''' <summary>
		''' Перенос треда на другой раздел
		''' </summary>
		''' <param name="NewBoardName">Новое имя раздела</param>
		''' <param name="ThreadNumber">Номер треда для переноса</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function MoveThread(ByVal ThreadNumber As Long, ByVal NewBoardName As String) As BoardThreadInfo

		''' <summary>
		''' Создаёт правило раздела
		''' </summary>
		''' <param name="RulesText"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateRules(ByVal RulesText As String) As ErrorInfo

		''' <summary>
		''' Удаляет правило раздела
		''' </summary>
		''' <param name="RulesId"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveRules(ByVal RulesId As Long) As ErrorInfo

		''' <summary>
		'''  Изменяет правило раздела
		''' </summary>
		''' <param name="RulesId"></param>
		''' <param name="RulesText"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditRules(ByVal RulesId As Long, ByVal RulesText As String) As ErrorInfo

		''' <summary>
		''' Получает список правил раздела
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetRulesList() As List(Of RulesInfo)

		''' <summary>
		''' Получает список зарегистрированных типов файлов
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMimeList() As List(Of MimeInfo)

		''' <summary>
		''' Создаёт миме
		''' </summary>
		''' <param name="Mime"></param>
		''' <param name="Extension"></param>
		''' <param name="MaxFileLength"></param>
		''' <param name="FileType"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateMime(ByVal Mime As String, _
		ByVal Extension As String, _
		 ByVal MaxFileLength As Long, _
		 ByVal FileType As FileType) As ErrorInfo

		''' <summary>
		''' Уладяет зарегистрированный тип файла
		''' </summary>
		''' <param name="MimeId"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveMime(ByVal MimeId As Long) As ErrorInfo

		''' <summary>
		''' Изменяет зарегистрированный тип файла
		''' </summary>
		''' <param name="MimeId"></param>
		''' <param name="Mime"></param>
		''' <param name="Extension"></param>
		''' <param name="MaxFileLength"></param>
		''' <param name="FileType"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditMime(ByVal MimeId As Long, _
		  ByVal Mime As String, _
		  ByVal Extension As String, _
		  ByVal MaxFileLength As Long, _
		  ByVal FileType As FileType) As ErrorInfo

		''' <summary>
		''' Добавляет группу в запрещённые для ответов на разделе
		''' </summary>
		''' <param name="RoleName"></param>
		''' <param name="DenyForReading"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function AddRoleToAnswersBoardDeny(ByVal RoleName As String, _
		  ByVal DenyForReading As Boolean) As ErrorInfo

		''' <summary>
		''' Удаляет группу из запрещённых для ответов на разделе
		''' </summary>
		''' <param name="RoleName"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveRoleFromAnswersBoardDeny(ByVal RoleName As String) As ErrorInfo

		''' <summary>
		''' Изменяет группу из запрещённых для ответов на разделе
		''' </summary>
		''' <param name="RoleName"></param>
		''' <param name="DenyForReading"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditRoleFromAnswersBoardDeny(ByVal RoleName As String, _
		  ByVal DenyForReading As Boolean) As ErrorInfo

		''' <summary>
		''' Получает список запрещённых групп для ответов
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetRolesFromAnswersBoardDenyList() As List(Of DenyRolesForBoardAnswersInfo)

		''' <summary>
		''' Добавляет группу в список запрещённых для отправки файлов
		''' </summary>
		''' <param name="MimeId"></param>
		''' <param name="RoleName"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function AddRoleToFilesDeny(ByVal MimeId As Long, _
		 ByVal RoleName As String) As ErrorInfo

		''' <summary>
		''' Удаляет группу из списка запрещённых для отправки файлов
		''' </summary>
		''' <param name="DenyId"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveRoleFromFilesDeny(ByVal DenyId As Long) As ErrorInfo

		''' <summary>
		''' Получает список групп, запрещённых для отправки файлов
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetRolesFromFilesDenyList() As List(Of DenyRolesForFilesInfo)

		''' <summary>
		''' Получает список меню
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMenuList() As List(Of MenuInfo)

		''' <summary>
		''' Создаёт пункт меню
		''' </summary>
		''' <param name="MenuText">Текст меню</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateMenu(ByVal MenuText As String) As ErrorInfo

		''' <summary>
		''' Удаляет пункт меню
		''' </summary>
		''' <param name="MenuId">Идентификатор меню</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveMenu(ByVal MenuId As Long) As ErrorInfo

		''' <summary>
		''' Изменяет пункт меню
		''' </summary>
		''' <param name="MenuId">Идентификатор меню</param>
		''' <param name="MenuText">Текст меню</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditMenu(ByVal MenuId As Long, ByVal MenuText As String) As ErrorInfo

		''' <summary>
		''' Получает настройки раздела
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetBoardInfo() As BoardInfo

		''' <summary>
		''' Получает список всех разделов
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetBoardsInfoList() As List(Of BoardInfo)

		''' <summary>
		''' Получает список регулярных выражений запрещённых слов на разделе
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetBadWordList() As List(Of BadWordInfo)

		''' <summary>
		''' Редактирует регулярное выражение запрещённого слова на разделе
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditBadWord(ByVal BadWordNumber As Long, _
		 ByVal Template As String, _
		 ByVal ReplacementString As String, _
		 ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo

		''' <summary>
		''' Удаляет регулярное выражение запрещённых слов с раздела
		''' </summary>
		''' <param name="BadWordNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveBadWord(ByVal BadWordNumber As Long) As ErrorInfo

		''' <summary>
		''' Создаёт регулярное выражение запрещённых слов на разделе
		''' </summary>
		''' <param name="Template"></param>
		''' <param name="ReplacementString"></param>
		''' <param name="ReplacementFlag"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateBadWord(ByVal Template As String, _
		  ByVal ReplacementString As String, _
		  ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo

		'Public MustOverride Function CreateBoard() As ErrorInfo
		'Public MustOverride Function EditBoard() As ErrorInfo
		'Public MustOverride Function EditBoardParameter() As ErrorInfo
		'Public MustOverride Function RemoveBoard() As ErrorInfo

		''' <summary>
		''' Отправка новых сообщений на электропочту
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property EnableSendNewMessageToEmail() As Boolean

		''' <summary>
		''' Почтовый ящик для отправки новых сообщений
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property SendNewMessageEmailAddress() As String

		''' <summary>
		''' Создаёт раздел
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateBoard(ByVal Description As String, _
		 ByVal MaxMessagesPerPagePerThread As Long, _
		 ByVal DefaultUserName As String, _
		 ByVal MaxThreadsPerPage As Long, _
		 ByVal MaxPagesCount As Long, _
		 ByVal BumpLimit As Long, _
		  ByVal MaxFilesPerMessage As Long, _
		  ByVal NewThreadWithoutFilesCreate As Boolean, _
		  ByVal CaptchaEnable As String, _
		  ByVal TimeEnable As Boolean, _
		  ByVal FilesDeny As Boolean, _
		  ByVal MaxMessageLength As Long, _
		  ByVal IsReadOnly As Boolean, _
		  ByVal Access As AnonymousAccess, _
		  ByVal MaxMessagesCanDeleteThread As Long, _
		  ByVal IsHidden As Boolean, _
		  ByVal MenuGroupId As Long, _
		  ByVal UserNameEnable As Boolean, _
		  ByVal ThumbnailWidth As Long, _
		  ByVal ThumbnailHeight As Long, _
		  ByVal PreModerationEnabled As Boolean, _
		  ByVal ShowFaq As Boolean) As ErrorInfo

		''' <summary>
		''' Создаёт раздел в упрощённом виде
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateBoard(ByVal Description As String, _
		 ByVal MenuGroupId As Long) As ErrorInfo

		''' <summary>
		''' Удаляет раздел и все связанные с ним данные
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveBoard() As ErrorInfo

		''' <summary>
		''' Устанавливает описание раздела
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function SetBoardDescription(ByVal Description As String) As ErrorInfo


		''' <summary>
		''' Изменяет раздел
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditBoard(Optional ByVal Description As String = Nothing, _
		 Optional ByVal MaxMessagesPerPagePerThread As Long = 8, _
		 Optional ByVal DefaultUserName As String = "Anonymous", _
		 Optional ByVal MaxThreadsPerPage As Long = 10, _
		 Optional ByVal MaxPagesCount As Long = 20, _
		 Optional ByVal BumpLimit As Long = 500, _
		 Optional ByVal MaxFilesPerMessage As Long = 1, _
		 Optional ByVal NewThreadWithoutFilesCreate As Boolean = False, _
		 Optional ByVal CaptchaEnable As String = "Enable:1|BackgroundNoise:0|FontWarping:0|LineNoise:0|Colored:0", _
		 Optional ByVal TimeEnable As Boolean = True, _
		 Optional ByVal FilesDeny As Boolean = False, _
		 Optional ByVal MaxMessageLength As Long = 8192, _
		 Optional ByVal IsReadOnly As Boolean = False, _
		 Optional ByVal Access As AnonymousAccess = AnonymousAccess.All, _
		 Optional ByVal MaxMessagesCanDeleteThread As Long = 100, _
		 Optional ByVal IsHidden As Boolean = False, _
		 Optional ByVal MenuGroupId As Long = 1, _
		 Optional ByVal UserNameEnable As Boolean = True, _
		 Optional ByVal ThumbnailWidth As Long = 150, _
		 Optional ByVal ThumbnailHeight As Long = 150, _
		 Optional ByVal PreModerationEnabled As Boolean = False, _
		 Optional ByVal ShowFaq As Boolean = True) As ErrorInfo

		''' <summary>
		''' Выполняет SQL-код над базой данных
		''' </summary>
		''' <param name="Sql"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RunSql(ByVal Sql As String) As ErrorInfo

		''' <summary>
		''' Определяет, является ли раздел системным
		''' </summary>
		''' <param name="BoardName">Имя раздела</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function IsSystemBoard(ByVal BoardName As String) As Boolean
			Select Case BoardName
				Case "default"
				Case "faq"
				Case "news"
				Case "rules"
				Case "terms"
				Case Else
					Return False
			End Select
			Return True
		End Function
	End Class
End Namespace
