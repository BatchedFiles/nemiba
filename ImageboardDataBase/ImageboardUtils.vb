Imports System.Threading
Imports System.Security.Cryptography
Imports System.IO

Namespace ImageBoard

#Region "Структуры данных"

	''' <summary>
	''' Отправленный клиентом файл
	''' </summary>
	''' <remarks></remarks>
	Public Structure ImageboardPostedFile
		''' <summary>
		''' Длина файла
		''' </summary>
		''' <remarks>Размер файла в байтах</remarks>
		Public ContentLength As Integer
		''' <summary>
		''' Тип миме
		''' </summary>
		''' <remarks></remarks>
		Public ContentType As String
		''' <summary>
		''' Имя файла на клиенте
		''' </summary>
		''' <remarks></remarks>
		Public FileName As String
		''' <summary>
		''' Поток данных
		''' </summary>
		''' <remarks></remarks>
		Public InputStream As Stream

		''' <summary>
		''' Сохраняет файл на диск
		''' </summary>
		''' <param name="strFileName"></param>
		''' <remarks></remarks>
		Public Sub SaveAs(ByVal strFileName As String)
			Dim objFS As FileStream = New FileStream(strFileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read)
			InputStream.Seek(0, SeekOrigin.Begin)
			Dim bytes(4095) As Byte
			Dim numBytes As Integer
			' Читаем
			numBytes = InputStream.Read(bytes, 0, 4096)
			Do While numBytes > 0
				objFS.Write(bytes, 0, numBytes)
				numBytes = InputStream.Read(bytes, 0, 4096)
			Loop
			objFS.Close()
		End Sub
	End Structure

	''' <summary>
	''' :Жалоба на сообщение
	''' </summary>
	''' <remarks></remarks>
	Public Structure ComplaintInfo
		''' <summary>
		''' Идентификатор жалобы
		''' </summary>
		''' <remarks></remarks>
		Public СomplaintId As Long
		''' <summary>
		''' Номер сообщения, к которому относится жалоба
		''' </summary>
		''' <remarks></remarks>
		Public MessageNumber As Long
		''' <summary>
		''' Текст жалобы
		''' </summary>
		''' <remarks></remarks>
		Public СomplaintText As String
	End Structure

	''' <summary>
	''' Информация о треде
	''' </summary>
	''' <remarks></remarks>
	Public Structure BoardThreadInfo
		Public ThreadNumber As Long
		Public ErrorInfo As ErrorInfo
	End Structure

	''' <summary>
	''' Анонимный доступ к разделу, треду, типу файлов
	''' </summary>
	''' <remarks></remarks>
	Public Enum AnonymousAccess As Integer
		''' <summary>
		''' Разрешён доступ для чтения и записи
		''' </summary>
		All
		''' <summary>
		''' Разрешён доступ только для создания тредов, но не ответов в них
		''' </summary>
		OnlyCreateThread
		''' <summary>
		''' Разрешён доступ только для чтения
		''' </summary>
		OnlyForRead
		''' <summary>
		''' Доступ запрещён (можно только зарегистрированным пользователям)
		''' </summary>
		Hidden
	End Enum

	''' <summary>
	''' Код ошибки 
	''' </summary>
	''' <remarks></remarks>
	Public Enum ErrorType As Integer
		''' <summary>
		''' Ошибки нет
		''' </summary>
		''' <remarks></remarks>
		None = 0
		''' <summary>
		''' Нарушение доступа
		''' </summary>
		''' <remarks></remarks>
		AccessViolation
		''' <summary>
		''' Раздел предназначен только для чтения
		''' </summary>
		''' <remarks></remarks>
		ReadOnlyBoard
		''' <summary>
		''' Раздел не найден
		''' </summary>
		''' <remarks></remarks>
		BoardNotFound
		''' <summary>
		''' Тред не найден
		''' </summary>
		''' <remarks></remarks>
		ThreadNotFound
		''' <summary>
		''' Необходим файл
		''' </summary>
		''' <remarks></remarks>
		FileRequired
		''' <summary>
		''' Дублирование
		''' </summary>
		''' <remarks></remarks>
		DublicateFile
		''' <summary>
		''' Капча не валидна
		''' </summary>
		''' <remarks></remarks>
		CaptchaNotValid
		''' <summary>
		''' Сообщение слишком длинное
		''' </summary>
		''' <remarks></remarks>
		MessageTooLong
		''' <summary>
		''' Слишком много файлов
		''' </summary>
		''' <remarks></remarks>
		TooManyFiles
		''' <summary>
		''' Это текстовый раздел, файлы запрещены к загрузке
		''' </summary>
		''' <remarks></remarks>
		OnlyText
		''' <summary>
		''' Сообщение содержит запрещённые фразы
		''' </summary>
		''' <remarks></remarks>
		DenyStringFound
		''' <summary>
		''' Файл замаскирован под изображение
		''' </summary>
		''' <remarks></remarks>
		FakeFileType
		''' <summary>
		''' Файл этого типа не разрешён к загрузке
		''' </summary>
		''' <remarks></remarks>
		DenyMime
		''' <summary>
		''' Это системный раздел
		''' </summary>
		''' <remarks></remarks>
		IsSystemBoard
		''' <summary>
		''' Необходим файл
		''' </summary>
		''' <remarks></remarks>
		MessageIsEmpty
	End Enum

	''' <summary>
	'''  Информация об ошибке
	''' </summary>
	''' <remarks></remarks>
	Public Structure ErrorInfo
		''' <summary>
		''' Тип ошибки
		''' </summary>
		''' <remarks></remarks>
		Public ErrorStatus As ErrorType
		''' <summary>
		''' Строка описания ошибки
		''' </summary>
		''' <remarks></remarks>
		Public ErrorString As String
	End Structure

	''' <summary>
	''' Тип файла
	''' </summary>
	''' <remarks></remarks>
	Public Enum FileType As Long
		''' <summary>
		''' Конкретного типа нет, просто файл
		''' </summary>
		''' <remarks></remarks>
		None
		''' <summary>
		''' Файл-изображение
		''' </summary>
		''' <remarks></remarks>
		Image
		''' <summary>
		''' Капча-ведунья
		''' </summary>
		''' <remarks></remarks>
		MagicCaptcha
		''' <summary>
		''' Ссылка на видео
		''' </summary>
		''' <remarks></remarks>
		VideoLink
		''' <summary>
		''' Векторное изображение в формате SVG
		''' </summary>
		''' <remarks></remarks>
		Svg
	End Enum

	''' <summary>
	''' Флаги замены плохих слов по шаблону
	''' </summary>
	''' <remarks></remarks>
	Public Enum ReplacementFlag
		''' <summary>
		''' Ничего не делаем
		''' </summary>
		''' <remarks></remarks>
		None
		''' <summary>
		''' Заменяем
		''' </summary>
		''' <remarks></remarks>
		Replace
		''' <summary>
		''' Выходим без замены
		''' </summary>
		''' <remarks></remarks>
		[Error]
		''' <summary>
		''' Просто убираем
		''' </summary>
		''' <remarks></remarks>
		Remove
	End Enum

	''' <summary>
	''' Информация о пункте меню
	''' </summary>
	''' <remarks></remarks>
	Public Structure MenuInfo
		''' <summary>
		''' Внутренний идентификатор (номер пункта из базы данных)
		''' </summary>
		''' <remarks></remarks>
		Public MenuNumber As Long
		''' <summary>
		''' Текст меню
		''' </summary>
		''' <remarks></remarks>
		Public MenuText As String
	End Structure

	''' <summary>
	''' Информация о разделе
	''' </summary>
	''' <remarks></remarks>
	Public Structure BoardInfo
		Public BoardName As String
		Public Description As String
		Public MaxMessagesPerPagePerThread As Long
		Public DefaultUserName As String
		Public MaxThreadsPerPage As Long
		Public MaxPagesCount As Long
		Public BumpLimit As Long
		Public MaxFilesPerMessage As Long
		Public NewThreadWithoutFilesCreate As Boolean
		Public CaptchaEnable As String
		Public TimeEnable As Boolean
		Public FilesDeny As Boolean
		Public MaxMessageLength As Long
		Public IsReadOnly As Boolean
		Public MaxMessagesCanDeleteThread As Long
		Public IsHidden As Boolean
		Public MenuGroupId As Long
		Public UserNameEnable As Boolean
		Public ThumbnailWidth As Long
		Public ThumbnailHeight As Long
		Public PreModerationEnabled As Boolean
		Public ShowFaq As Boolean
		Public AnonymousAccess As AnonymousAccess
	End Structure

	''' <summary>
	''' Информация о миме
	''' </summary>
	''' <remarks></remarks>
	Public Structure MimeInfo
		Public MimeTypeId As Long
		Public Mime As String
		Public Extension As String
		Public BoardName As String
		Public MaxFileLength As Long
		Public FileType As FileType
		Public AnonymousAccess As AnonymousAccess
	End Structure

	''' <summary>
	''' Запрещённые роли для отправки файлов
	''' </summary>
	''' <remarks></remarks>
	Public Structure DenyRolesForFilesInfo
		Public DenyId As Long
		Public MimeId As Long
		Public RoleName As String
	End Structure

	''' <summary>
	''' Запрещённые роли для ответов на разделе
	''' </summary>
	''' <remarks></remarks>
	Public Structure DenyRolesForBoardAnswersInfo
		Public BoardName As String
		Public RoleName As String
		Public DenyForReading As Boolean
	End Structure

	''' <summary>
	''' Запрещённые роли для ответов в треде
	''' </summary>
	''' <remarks></remarks>
	Public Structure DenyRolesForThreadAnswersInfo
		Public ThreadNumber As Long
		Public RoleName As String
		Public DenyForReading As Boolean
	End Structure

	''' <summary>
	''' Забаненные
	''' </summary>
	''' <remarks></remarks>
	Public Structure BanInfo
		Public BanId As Long
		'Public IpAddress As String
		Public Reason As String
		Public ExpiresDate As Date
		Public BoardName As String
	End Structure

	''' <summary>
	''' Запрещённые слова
	''' </summary>
	''' <remarks></remarks>
	Public Structure BadWordInfo
		Public BadWordId As Long
		'Public BoardName As String
		Public Template As String
		Public ReplacementString As String
		Public ReplacementFlag As ReplacementFlag
	End Structure

	''' <summary>
	''' Правила раздела
	''' </summary>
	''' <remarks></remarks>
	Public Structure RulesInfo
		Public RulesId As Long
		Public BoardName As String
		Public RulesText As String
	End Structure

	''' <summary>
	''' Информация о треде
	''' </summary>
	''' <remarks></remarks>
	Public Structure ThreadInfo
		''' <summary>
		''' Номер треда
		''' </summary>
		''' <remarks></remarks>
		Public ThreadNumber As Long
		'''' <summary>
		'''' Номер последнего сообщения в треде
		'''' </summary>
		'''' <remarks></remarks>
		'Public LastMessageNumber As Long
		''' <summary>
		''' Флаг закреплённости треда, если установлен в <c>True</c>, то тред закреплён наверху страницы
		''' </summary>
		''' <remarks></remarks>
		Public IsTop As Boolean
		''' <summary>
		''' Флаг архивного треда, если установлен в <c>True</c>, то тред находится в архиве
		''' </summary>
		''' <remarks></remarks>
		Public IsArchive As Boolean
		''' <summary>
		''' Анонимный доступ к треду
		''' </summary>
		''' <remarks></remarks>
		Public AnonymousAccess As AnonymousAccess
		''' <summary>
		''' Количество сообщений в треде
		''' </summary>
		''' <remarks></remarks>
		Public MessagesCount As Long
		''' <summary>
		''' Список сообщений в треде
		''' </summary>
		''' <remarks></remarks>
		Public Messages As List(Of MessageInfo)
	End Structure

	''' <summary>
	''' Рейтинг
	''' </summary>
	''' <remarks></remarks>
	Public Enum NsfwRating
		''' <summary>
		''' Абсолютно безопасно
		''' </summary>
		''' <remarks></remarks>
		Sfw = 0
		''' <summary>
		''' Только для лиц старше шести лет
		''' </summary>
		''' <remarks></remarks>
		R6
		''' <summary>
		''' Только для лиц старше двенадцати лет
		''' </summary>
		''' <remarks></remarks>
		R12
		''' <summary>
		''' Только для лиц старше шестнадцати лет
		''' </summary>
		''' <remarks></remarks>
		R16
		''' <summary>
		''' Только для лиц старше восемнадцати лет
		''' </summary>
		''' <remarks></remarks>
		R18
		''' <summary>
		''' Небезопасно (гуро, нигра, расчленёнка)
		''' </summary>
		''' <remarks></remarks>
		Nsfw
	End Enum

	''' <summary>
	''' Ответ на сообщение
	''' </summary>
	''' <remarks></remarks>
	Public Structure AnswerInfo
		Public MessageNumber As Long
		Public AnswerMessageNumber As Long
		Public AnswerBoardName As String
		Public External As Boolean
	End Structure

	''' <summary>
	''' Информация о файлах
	''' </summary>
	''' <remarks></remarks>
	Public Structure ImageboardFileInfo
		''' <summary>
		''' Номер треда
		''' </summary>
		''' <remarks></remarks>
		Public ThreadNumber As Long
		''' <summary>
		''' Номер сообщения
		''' </summary>
		''' <remarks></remarks>
		Public MessageNumber As Long
		Public UrlFilePath As String
		Public FileLength As Long
		Public ImageWidth As Long
		Public ImageHeight As Long
		Public ThumbUrlFilePath As String
		Public ThumbWidth As Long
		Public ThumbHeight As Long
		Public FileDeleted As Boolean
		Public FileType As FileType
		Public ShortFileName As String
		Public MediaType As String
		Public ShortMediaType As String
	End Structure

	''' <summary>
	''' Информация о сообщении
	''' </summary>
	''' <remarks></remarks>
	Public Structure MessageInfo
		''' <summary>
		''' Номер сообщения
		''' </summary>
		''' <remarks>Натуральное число</remarks>
		Public MessageNumber As Long
		''' <summary>
		''' Номер треда, к которому относится сообщение
		''' </summary>
		''' <remarks>Натуральное число</remarks>
		Public ThreadNumber As Long
		''' <summary>
		''' Флаг проверенности сообщения модератором
		''' </summary>
		''' <remarks></remarks>
		Public Checked As Boolean
		''' <summary>
		''' Идентификатор сессии пользователя, создавшего сообщение
		''' </summary>
		''' <remarks></remarks>
		Public Cookie As String
		''' <summary>
		''' Рейтинг цензуры сообщения
		''' </summary>
		''' <remarks></remarks>
		Public Rating As NsfwRating
		''' <summary>
		''' Имя пользователя
		''' </summary>
		''' <remarks>Не соответствует настоящему имени пользователя в базе данных</remarks>
		Public UserName As String
		''' <summary>
		''' Электронная почта
		''' </summary>
		''' <remarks></remarks>
		Public EMail As String
		''' <summary>
		''' Тема сообщения
		''' </summary>
		''' <remarks></remarks>
		Public Subject As String
		''' <summary>
		''' Тема сообщения
		''' </summary>
		''' <remarks></remarks>
		Public MessageText As String
		''' <summary>
		''' Время создания сообщения
		''' </summary>
		''' <remarks></remarks>
		Public DateTime As Date
		''' <summary>
		''' Настоящее имя пользователя-создателя сообщения
		''' </summary>
		''' <remarks></remarks>
		Public RealUserName As String
		''' <summary>
		''' Флаг удалённого сообщения
		''' </summary>
		''' <remarks></remarks>
		Public MessageDeleted As Boolean
		''' <summary>
		''' Список ответов на сообщение
		''' </summary>
		''' <remarks>Карта ответов</remarks>
		Public Answers As List(Of AnswerInfo)
		''' <summary>
		''' Список файлов сообщения
		''' </summary>
		''' <remarks></remarks>
		Public Files As List(Of ImageboardFileInfo)
	End Structure

#End Region
End Namespace
