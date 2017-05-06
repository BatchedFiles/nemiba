Imports System.Web
Imports System.Drawing

Namespace ImageBoard

	Public MustInherit Class ImageBoardThreadBase

		''' <summary>
		''' Имя пользователя, прошедшего авторизацию
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property UserName() As String

		''' <summary>
		''' Строка подключения к базе данных
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property ConnectionString() As String

		''' <summary>
		''' Имя приложения
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property ApplicationName() As String

		''' <summary>
		''' Имя раздела
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property BoardName() As String

		''' <summary>
		''' Блокирует базу данных тредов от изменения
		''' </summary>
		''' <remarks></remarks>
		Public MustOverride Sub LockThreadBase()

		''' <summary>
		''' Снимает блокировку с базы данных тредов
		''' </summary>
		''' <remarks></remarks>
		Public MustOverride Sub UnLockThreadBase()

		''' <summary>
		''' Удаление сообщения из нити
		''' </summary>
		''' <param name="MessageNumber">Номер сообщения</param>
		''' <param name="Password">Пароль сообщения</param>
		''' <param name="FilesOnly">True, если удаляем только файлы</param>
		''' <param name="Admin">Административное удаление</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveMessage(ByVal MessageNumber As Long, ByVal Password As String, ByVal FilesOnly As Boolean, ByVal Admin As Boolean) As ErrorInfo

		''' <summary>
		''' Удаление архивного треда
		''' </summary>
		''' <param name="ThreadNumber">Номер треда для удаления</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function RemoveArchiveThread(ByVal ThreadNumber As Long) As ErrorInfo

		''' <summary>
		''' Создание треда
		''' </summary>
		''' <param name="Cookie"></param>
		''' <param name="Rating"></param>
		''' <param name="strUserName"></param>
		''' <param name="strEmail"></param>
		''' <param name="strSubject"></param>
		''' <param name="strMessage"></param>
		''' <param name="strPassword"></param>
		''' <param name="TopThread"></param>
		''' <param name="strClientIP"></param>
		''' <param name="PostedFiles"></param>
		''' <param name="VideoLinks"></param>
		''' <param name="MagicCaptcha"></param>
		''' <param name="RealUserName"></param>
		''' <param name="NewsManage"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateThread(ByVal Cookie As String, _
		  ByVal Rating As NsfwRating, _
		  ByVal strUserName As String, _
		  ByVal strEmail As String, _
		  ByVal strSubject As String, _
		  ByVal strMessage As String, _
		  ByVal strPassword As String, _
		  ByVal TopThread As Boolean, _
		  ByVal strClientIP As String, _
		  ByVal PostedFiles As Generic.List(Of ImageboardPostedFile), _
		  ByVal VideoLinks As Generic.List(Of String), _
		  ByVal MagicCaptcha As Bitmap, _
		  ByVal RealUserName As String, _
		  ByVal NewsManage As Boolean) As BoardThreadInfo

		''' <summary>
		''' Создание треда
		''' </summary>
		''' <param name="Rating">Рейтинг цензуры (NSFW-содержимого)</param>
		''' <param name="NewsManage">Флаг создателя новостей</param>
		''' <param name="Cookie">Строка с печеньем создателя сообщения</param>
		''' <param name="strUserName">Имя пользователя</param>
		''' <param name="strEmail">Электропочта</param>
		''' <param name="strSubject">Тема</param>
		''' <param name="strMessage">Сообщение</param>
		''' <param name="TopThread">Если флаг установлен в True, то создаёт закреплённую нить</param>
		''' <param name="strPassword">Пароль для удаления треда</param>
		''' <param name="PostedFiles">Файловый поток</param>
		''' <param name="strClientIP">Используемый клиентом айпишник</param>
		''' <param name="RealUserName">Номер зарегистрированного пользователя</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CreateThread(ByVal Cookie As String, _
		  ByVal Rating As NsfwRating, _
		  ByVal strUserName As String, _
		  ByVal strEmail As String, _
		  ByVal strSubject As String, _
		  ByVal strMessage As String, _
		  ByVal strPassword As String, _
		  ByVal TopThread As Boolean, _
		  ByVal strClientIP As String, _
		  ByVal PostedFiles As Generic.List(Of HttpPostedFile), _
		  ByVal VideoLinks As Generic.List(Of String), _
		  ByVal MagicCaptcha As Bitmap, _
		  ByVal RealUserName As String, _
		  ByVal NewsManage As Boolean) As BoardThreadInfo

		''' <summary>
		''' Добавление сообщения в тред
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="Cookie"></param>
		''' <param name="Rating"></param>
		''' <param name="strUserName"></param>
		''' <param name="strEmail"></param>
		''' <param name="strSubject"></param>
		''' <param name="strMessage"></param>
		''' <param name="strPassword"></param>
		''' <param name="strClientIP"></param>
		''' <param name="PostedFiles"></param>
		''' <param name="VideoLinks"></param>
		''' <param name="MagicCaptcha"></param>
		''' <param name="RealUserName"></param>
		''' <param name="NewsManage"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function AddMessageToThread(ByVal ThreadNumber As Long, _
		 ByVal Cookie As String, _
		 ByVal Rating As NsfwRating, _
		 ByVal strUserName As String, _
		 ByVal strEmail As String, _
		 ByVal strSubject As String, _
		 ByVal strMessage As String, _
		 ByVal strPassword As String, _
		 ByVal strClientIP As String, _
		 ByVal PostedFiles As Generic.List(Of ImageboardPostedFile), _
		 ByVal VideoLinks As Generic.List(Of String), _
		 ByVal MagicCaptcha As Bitmap, _
		 ByVal RealUserName As String, _
		 ByVal NewsManage As Boolean) As BoardThreadInfo

		''' <summary>
		''' Добавление сообщения в тред
		''' </summary>
		''' <param name="ThreadNumber">Номер тредя для ответа в него</param>
		''' <param name="Rating">Рейтинг цензуры (NSFW-содержимого)</param>
		''' <param name="NewsManage">Флаг создателя новостей</param>
		''' <param name="Cookie">Строка с печеньем создателя сообщения</param>
		''' <param name="strUserName">Имя пользователя</param>
		''' <param name="strEmail">Электропочта</param>
		''' <param name="strSubject">Тема</param>
		''' <param name="strMessage">Сообщение</param>
		''' <param name="strPassword">Пароль для удаления треда</param>
		''' <param name="PostedFiles">Файловый поток</param>
		''' <param name="strClientIP">Используемый клиентом айпишник</param>
		''' <param name="RealUserName">Номер зарегистрированного пользователя</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function AddMessageToThread(ByVal ThreadNumber As Long, _
		 ByVal Cookie As String, _
		 ByVal Rating As NsfwRating, _
		 ByVal strUserName As String, _
		 ByVal strEmail As String, _
		 ByVal strSubject As String, _
		 ByVal strMessage As String, _
		 ByVal strPassword As String, _
		 ByVal strClientIP As String, _
		 ByVal PostedFiles As Generic.List(Of HttpPostedFile), _
		 ByVal VideoLinks As Generic.List(Of String), _
		 ByVal MagicCaptcha As Bitmap, _
		 ByVal RealUserName As String, _
		 ByVal NewsManage As Boolean) As BoardThreadInfo

		''' <summary>
		''' Возвращает количество страниц на разделе
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <param name="ArchiveThread">Если установлен в <c>True</c>, то возвращает количество страниц только архивных тредов</param>
		Public MustOverride Function GetPagesCount(ByVal ArchiveThread As Boolean) As Long

		''' <summary>
		''' Возвращает список тредов, доступных для чтения
		''' </summary>
		''' <param name="PageNumber">Номер страницы. Если меньше нуля, то возвращает все треды в разделе</param>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <param name="ArchiveThread">Если установлен в <c>True</c>, то возвращает список только архивных тредов</param>
		Public MustOverride Function GetThreadList(ByVal PageNumber As Long, _
		 ByVal ArchiveThread As Boolean) As Generic.List(Of ThreadInfo)

		''' <summary>
		''' Возвращает список всех сообщений в треде
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMessageList(ByVal ThreadNumber As Long) As List(Of Long)

		''' <summary>
		''' Получает количество сообщений в треде
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMessagesCount(ByVal ThreadNumber As Long) As Long

		''' <summary>
		''' Возвращает/устанавливает номер следующего сообщения
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Property NextMessageNumber() As Long

		''' <summary>
		''' Возвращает список всех сообщений в треде начиная с некоторого номера сообщения
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="StartMessageNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMessageListStartAt(ByVal ThreadNumber As Long, _
		ByVal StartMessageNumber As Long) As List(Of Long)

		''' <summary>
		''' Возвращает список всех сообщений в треде на странице предпросмотра
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMessageListInPage(ByVal ThreadNumber As Long) As List(Of Long)

		''' <summary>
		''' Возвращает сообщение со списком ответов и файлов
		''' </summary>
		''' <param name="MessageNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetMessage(ByVal MessageNumber As Long) As MessageInfo

		''' <summary>
		''' Возвращает список ответов по номеру собщения
		''' </summary>
		''' <param name="MessageNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetAnswerList(ByVal MessageNumber As Long) As List(Of AnswerInfo)

		''' <summary>
		''' Возвращает список файлов по номеру сообщения
		''' </summary>
		''' <param name="MessageNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetFileList(ByVal MessageNumber As Long) As List(Of ImageboardFileInfo)

		''' <summary>
		''' Возвращает номер треда по сообщению
		''' </summary>
		''' <param name="MessageNumber">Номер сообщения</param>
		''' <param name="Archive">Если установлен в True, то получаем номер треда из архива</param>
		''' <param name="DenyForWriting">Запрет на ответ в тред</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function GetThreadNumber(ByVal MessageNumber As Long, _
		  ByVal DenyForWriting As Boolean, _
		  ByVal Archive As Boolean) As Long

		''' <summary>
		''' Редактирование закреплённости треда
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="TopFlag"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditThreadAlwaysOnTopFlag(ByVal ThreadNumber As Long, ByVal TopFlag As Boolean) As ErrorInfo

		''' <summary>
		''' Редактирование анонимного доступа к треду
		''' </summary>
		''' <param name="ThreadNumber"></param>
		''' <param name="Access"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditThreadAnonymousAccess(ByVal ThreadNumber As Long, ByVal Access As AnonymousAccess) As ErrorInfo

		''' <summary>
		''' Редактирование анонимного доступа к разделу
		''' </summary>
		''' <param name="Access"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditBoardAnonymousAccess(ByVal Access As AnonymousAccess) As ErrorInfo

		''' <summary>
		''' Редактирование анонимного доступа к типу файлов
		''' </summary>
		''' <param name="MimeId"></param>
		''' <param name="Access"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function EditFileAnonymousAccess(ByVal MimeId As Long, ByVal Access As AnonymousAccess) As ErrorInfo

		''' <summary>
		''' Отмечает сообщение как проверенное
		''' </summary>
		''' <param name="MessageNumber"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public MustOverride Function CheckMessage(ByVal MessageNumber As Long) As ErrorInfo

	End Class

End Namespace
