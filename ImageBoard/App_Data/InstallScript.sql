BEGIN TRANSACTION;
	
	/* Создание таблиц */
	
	-- Разделы, доступные для модерирования у флага
	CREATE TABLE IF NOT EXISTS AdminBoards
	(
		FlagId INTEGER NOT NULL,
		BoardName TEXT NOT NULL
	);
	CREATE UNIQUE INDEX IF NOT EXISTS AdminBoardsIndex ON AdminBoards (FlagId, BoardName);
	-- Список флагов у группы
	CREATE TABLE IF NOT EXISTS AdminFlags
	(
		FlagId INTEGER PRIMARY KEY AUTOINCREMENT,
		FlagName INTEGER NOT NULL,
		RoleName TEXT NOT NULL
	);
	CREATE UNIQUE INDEX IF NOT EXISTS AdminFlagsIndex ON AdminFlags (FlagId, FlagName, RoleName);
	-- Ответы на сообщения
	CREATE TABLE IF NOT EXISTS Answers
	(
		BoardName TEXT NOT NULL,
		MessageNumber INTEGER NOT NULL DEFAULT 0,
		AnswerBoardName TEXT NOT NULL,
		AnswerMessageNumber INTEGER NOT NULL DEFAULT 0
	);
	-- Баны пользователей
	CREATE TABLE IF NOT EXISTS Bans
	(
		BanId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		IpAddress TEXT NOT NULL,
		Reason TEXT NOT NULL,
		ExpiresDate INTEGER NOT NULL,
		BoardName TEXT NOT NULL
	);
	-- Фильтр слов
	CREATE TABLE IF NOT EXISTS BadWords
	(
		BadWordId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		BoardName TEXT NOT NULL,
		Template TEXT NOT NULL,
		ReplacementString TEXT NOT NULL,
		ReplacementFlag INTEGER NOT NULL DEFAULT 0
	);
	-- Разделы
	CREATE TABLE IF NOT EXISTS Boards
	(
		BoardName TEXT NOT NULL UNIQUE,
		Description TEXT NOT NULL,
		MaxMessagesPerPagePerThread INTEGER NOT NULL DEFAULT 8,
		DefaultUserName TEXT NOT NULL DEFAULT 'Anonymous',
		MaxThreadsPerPage INTEGER NOT NULL DEFAULT 10,
		MaxPagesCount INTEGER NOT NULL DEFAULT 20,
		BumpLimit INTEGER NOT NULL DEFAULT 500,
		MaxFilesPerMessage INTEGER NOT NULL DEFAULT 1,
		NewThreadWithoutFilesCreate BOOL NOT NULL DEFAULT 0,
		CaptchaEnable TEXT NOT NULL DEFAULT 'Enable:1|BackgroundNoise:0|FontWarping:0|LineNoise:0|Colored:0',
		TimeEnable BOOL NOT NULL DEFAULT 1,
		FilesDeny BOOL NOT NULL DEFAULT 0,
		MaxMessageLength INTEGER NOT NULL DEFAULT 8192,
		IsReadOnly BOOL NOT NULL DEFAULT 0,
		AnonymousAnswer INTEGER NOT NULL DEFAULT 0,
		MaxMessagesCanDeleteThread INTEGER NOT NULL DEFAULT 100,
		IsHidden BOOL NOT NULL DEFAULT 0,
		MenuGroupId INTEGER NOT NULL DEFAULT 1,
		UserNameEnable BOOL NOT NULL DEFAULT 1,
		ThumbnailWidth INTEGER NOT NULL DEFAULT 150,
		ThumbnailHeight INTEGER NOT NULL DEFAULT 150,
		PreModerationEnabled BOOL NOT NULL DEFAULT 0,
		ShowFaq BOOL NOT NULL DEFAULT 1
	);
	-- Жалобы на сообщения
	CREATE TABLE IF NOT EXISTS Complaint
	(
		СomplaintId INTEGER PRIMARY KEY NOT NULL UNIQUE,
		BoardName TEXT NOT NULL,
		MessageNumber INTEGER NOT NULL,
		СomplaintText TEXT NOT NULL
	);
	-- Запрещённые группы для ответов в тред
	CREATE TABLE IF NOT EXISTS DenyForThreadAnswers
	(
		BoardName TEXT NOT NULL,
		ThreadNumber INTEGER NOT NULL,
		RoleName TEXT NOT NULL,
		DenyForReading BOOL NOT NULL DEFAULT 0
	);
	CREATE UNIQUE INDEX IF NOT EXISTS DenyForThreadAnswersIndex ON DenyForThreadAnswers (BoardName, ThreadNumber, RoleName);
	-- Запрещённые группы для ответов в разделы
	CREATE TABLE IF NOT EXISTS DenyForBoardAnswers
	(
		BoardName TEXT NOT NULL,
		RoleName TEXT NOT NULL,
		DenyForReading BOOL NOT NULL
	);
	CREATE UNIQUE INDEX IF NOT EXISTS DenyForBoardAnswersIndex ON DenyForBoardAnswers (BoardName, RoleName);
	-- Запрещённые группы для отправки файлов
	CREATE TABLE IF NOT EXISTS DenyForFiles
	(
		DenyId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		MimeId INTEGER NOT NULL,
		RoleName TEXT NOT NULL
	);
	-- Файлы
	CREATE TABLE IF NOT EXISTS Files
	(
		BoardName TEXT NOT NULL,
		MessageNumber INTEGER NOT NULL,
		ThreadNumber INTEGER NOT NULL,
		UrlFilePath TEXT NOT NULL,
		FileLength INTEGER NOT NULL,
		ImageWidth INTEGER NOT NULL,
		ImageHeight INTEGER NOT NULL,
		ThumbUrlFilePath TEXT,
		ThumbWidth INTEGER NOT NULL DEFAULT 0,
		ThumbHeight INTEGER NOT NULL DEFAULT 0,
		FileDeleted BOOL NOT NULL DEFAULT 0,
		FileType INTEGER NOT NULL,
		ShortFileName TEXT NOT NULL,
		MediaType TEXT NOT NULL,
		ShortMediaType TEXT NOT NULL
	);
	CREATE UNIQUE INDEX IF NOT EXISTS FilesIndex ON Files (BoardName, UrlFilePath);
	-- Меню
	CREATE TABLE IF NOT EXISTS MenuGroups
	(
		MenuGroupId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		Description TEXT NOT NULL
	);
	-- Сообщения
	CREATE TABLE IF NOT EXISTS Messages
	(
		BoardName TEXT NOT NULL,
		MessageNumber INTEGER NOT NULL,
		ThreadNumber INTEGER NOT NULL,
		Verified BOOL NOT NULL DEFAULT 0,
		Cookie TEXT NOT NULL DEFAULT Undefined,
		NSFWRating INTEGER NOT NULL DEFAULT 0,
		UserName TEXT,
		RealUserName TEXT,
		Email TEXT,
		Subject TEXT,
		MessageText TEXT,
		Password TEXT,
		DateTime INTEGER NOT NULL,
		Ip TEXT,
		MessageDeleted BOOL NOT NULL DEFAULT 0
	);
	CREATE UNIQUE INDEX IF NOT EXISTS MessagesIndex ON Messages (BoardName, MessageNumber, ThreadNumber);
	-- Зарегистрированные типы файлов
	CREATE TABLE IF NOT EXISTS MimeTypes
	(
		MimeTypeId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		Mime TEXT NOT NULL,
		Extension TEXT NOT NULL,
		BoardName TEXT NOT NULL,
		MaxFileLength INTEGER NOT NULL DEFAULT 4194304,
		FileType INTEGER NOT NULL DEFAULT 1,
		AnonymousAnswer INTEGER NOT NULL DEFAULT 0
	);
	CREATE UNIQUE INDEX IF NOT EXISTS MimeTypesIndex ON MimeTypes (BoardName, Mime);
	-- Счётчик сообщений
	CREATE TABLE IF NOT EXISTS NextMessageNumber
	(
		BoardName TEXT NOT NULL UNIQUE,
		NextNumber INTEGER NOT NULL
	);
	-- Группы пользователей
	CREATE TABLE IF NOT EXISTS Roles
	(
		RoleName TEXT NOT NULL UNIQUE,
		Enabled BOOL NOT NULL DEFAULT 1,
		ApplicationName TEXT NOT NULL
	);
	-- Правила разделов, отображаемые под формой ответа
	CREATE TABLE IF NOT EXISTS Rules
	(
		RulesId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
		BoardName TEXT NOT NULL,
		RulesItem TEXT NOT NULL
	);
	-- Треды
	CREATE TABLE IF NOT EXISTS Threads
	(
		BoardName TEXT NOT NULL,
		ThreadNumber INTEGER NOT NULL,
		LastMessageNumber INTEGER NOT NULL DEFAULT 1,
		IsTop BOOL NOT NULL DEFAULT 0,
		Archive BOOL NOT NULL DEFAULT 0,
		AnonymousAnswer INTEGER NOT NULL DEFAULT 0
	);
	CREATE UNIQUE INDEX IF NOT EXISTS ThreadsIndex ON Threads (BoardName, ThreadNumber);
	-- Пользователи
	CREATE TABLE IF NOT EXISTS Users
	(
		UserName TEXT NOT NULL UNIQUE,
		Password TEXT NOT NULL,
		Enabled BOOL NOT NULL DEFAULT 1,
		PKID TEXT NOT NULL PRIMARY KEY,
		ApplicationName TEXT NOT NULL,
		Email TEXT NOT NULL,
		Comment TEXT,
		PasswordQuestion TEXT,
		PasswordAnswer TEXT,
		IsApproved BOOL NOT NULL, 
		LastActivityDate INTEGER,
		LastLoginDate INTEGER,
		LastPasswordChangedDate INTEGER,
		CreationDate INTEGER, 
		IsOnLine INTEGER,
		LastLockedOutDate INTEGER,
		FailedPasswordAttemptCount INTEGER,
		FailedPasswordAttemptWindowStart INTEGER,
		FailedPasswordAnswerAttemptCount INTEGER,
		FailedPasswordAnswerAttemptWindowStart INTEGER
	);
	-- Данные о пользователях
	CREATE TABLE IF NOT EXISTS UserCards
	(
		UserName TEXT NOT NULL,
		Key INTEGER NOT NULL,
		Value TEXT NOT NULL
	);
	-- Пользователи в группе
	CREATE TABLE IF NOT EXISTS UsersInRoles
	(
		UserName TEXT NOT NULL,
		RoleName TEXT NOT NULL,
		ApplicationName TEXT NOT NULL
	);
	CREATE UNIQUE INDEX IF NOT EXISTS UsersInRolesIndex ON UsersInRoles (UserName, RoleName, ApplicationName);
	
	/* Триггеры */
	
	CREATE TRIGGER IF NOT EXISTS RemoveMime BEFORE DELETE ON MimeTypes FOR EACH ROW
		BEGIN
			DELETE FROM DenyForFiles WHERE DenyId=old.MimeTypeId;
		End;
	CREATE TRIGGER IF NOT EXISTS RemoveMenu BEFORE DELETE ON MenuGroups FOR EACH ROW
		BEGIN
			DELETE FROM Boards WHERE MenuGroupId=old.MenuGroupId;
		End;
	CREATE TRIGGER IF NOT EXISTS InsertBoard AFTER INSERT ON Boards FOR EACH ROW
		BEGIN
			-- Добавить счётчик сообщений
			INSERT OR IGNORE INTO NextMessageNumber (BoardName, NextNumber) VALUES (new.BoardName, 1);
		END;
	CREATE TRIGGER IF NOT EXISTS RemoveBoard BEFORE DELETE ON Boards FOR EACH ROW
		BEGIN
			DELETE FROM Bans WHERE BoardName=old.BoardName;
			DELETE FROM MimeTypes WHERE BoardName=old.BoardName;
			DELETE FROM Rules WHERE BoardName=old.BoardName;
			DELETE FROM DenyForBoardAnswers WHERE BoardName=old.BoardName;
			DELETE FROM BadWords WHERE BoardName=old.BoardName;
			DELETE FROM Threads WHERE BoardName=old.BoardName;
		End;
	CREATE TRIGGER IF NOT EXISTS RemoveRole BEFORE DELETE ON Roles FOR EACH ROW
		BEGIN
			DELETE FROM UsersInRoles WHERE RoleName=old.RoleName;
			DELETE FROM AdminFlags WHERE RoleName=old.RoleName;
		End;
	CREATE TRIGGER IF NOT EXISTS RemoveUser BEFORE DELETE ON Users FOR EACH ROW
		BEGIN
			DELETE FROM UserCards WHERE UserName=old.UserName;
			DELETE FROM UsersInRoles WHERE UserName=old.UserName;
		End;
	-- Отправление треда в архив
	CREATE TRIGGER IF NOT EXISTS InsertThread AFTER INSERT ON Threads FOR EACH ROW
		BEGIN
			-- (MaxPagesCount + 1) * MaxThreadsPerPage
			UPDATE Threads SET Archive=1 WHERE ThreadNumber IN 
			(SELECT ThreadNumber FROM Threads WHERE Archive=0 AND BoardName=new.BoardName ORDER BY LastMessageNumber DESC LIMIT -1 OFFSET 
			(((SELECT MaxPagesCount FROM Boards WHERE BoardName=new.BoardName) + 1) * (SELECT MaxThreadsPerPage FROM Boards WHERE BoardName=new.BoardName)));
		END;
	-- Удаление треда
	CREATE TRIGGER IF NOT EXISTS RemoveThread BEFORE DELETE ON Threads FOR EACH ROW
		BEGIN
			DELETE FROM Messages WHERE BoardName=old.BoardName AND ThreadNumber=old.ThreadNumber;
			UPDATE Files SET FileDeleted=1 WHERE BoardName=old.BoardName AND ThreadNumber=old.ThreadNumber;
			DELETE FROM DenyForThreadAnswers WHERE BoardName=old.BoardName AND ThreadNumber=old.ThreadNumber;
		End;
	-- Удаление сообщения
	CREATE TRIGGER IF NOT EXISTS RemoveMessage BEFORE DELETE ON Messages FOR EACH ROW
		BEGIN
			DELETE FROM Answers WHERE BoardName=old.UserName AND MessageNumber=old.MessageNumber;
		End;
	-- Увеличение счётчика при добавлении сообщений
	CREATE TRIGGER IF NOT EXISTS InsertMessage AFTER INSERT ON Messages
		BEGIN
			UPDATE NextMessageNumber SET NextNumber= 1 + (SELECT NextNumber FROM NextMessageNumber WHERE BoardName=new.BoardName) WHERE BoardName=new.BoardName;
			UPDATE Threads SET LastMessageNumber=new.MessageNumber WHERE BoardName=new.BoardName AND ThreadNumber=new.ThreadNumber AND new.Subject <> 'sage' AND (SELECT COUNT(*) FROM Messages WHERE BoardName=new.BoardName AND ThreadNumber=new.ThreadNumber) < (SELECT BumpLimit FROM Boards WHERE BoardName=new.BoardName);
		END;
	
	/* Группы пользователей */
	
	--INSERT OR IGNORE INTO Roles (RoleName, Description) VALUES (1, Создатели интернетов);
	--INSERT OR IGNORE INTO AdminFlags (FlagId, FlagName, RoleName) VALUES (1, 0, 1);
	--INSERT OR IGNORE INTO Roles (RoleName, Description) VALUES (2, Неймфаги);
	
	/* Создание системных разделов */
	
	INSERT OR IGNORE INTO Boards(BoardName, Description, MaxMessagesPerPagePerThread, NewThreadWithoutFilesCreate, MaxMessageLength, IsHidden, UserNameEnable) VALUES('faq', 'Фак', 1, 1, 131072, 1, 0);
	INSERT OR IGNORE INTO Boards(BoardName, Description, MaxMessagesPerPagePerThread, NewThreadWithoutFilesCreate, MaxMessageLength, IsHidden, UserNameEnable) VALUES('rules', 'Правила', 1, 1, 131072, 1, 0);
	INSERT OR IGNORE INTO Boards(BoardName, Description, MaxMessagesPerPagePerThread, NewThreadWithoutFilesCreate, MaxMessageLength, IsHidden, UserNameEnable) VALUES('terms', 'Условия пользования', 1, 1, 131072, 1, 0);
	INSERT OR IGNORE INTO Boards(BoardName, Description, MaxMessagesPerPagePerThread, NewThreadWithoutFilesCreate, MaxMessageLength, IsHidden, UserNameEnable, ThumbnailWidth, ThumbnailHeight) VALUES('news', 'Горячие новости', 1, 1, 131072, 1, 0, 800, 600);
	INSERT OR IGNORE INTO Boards(BoardName, Description, MaxMessagesPerPagePerThread, NewThreadWithoutFilesCreate, MaxMessageLength, IsHidden, UserNameEnable, ThumbnailWidth, ThumbnailHeight) VALUES('default', 'Глагне', 1, 1, 131072, 1, 0, 800, 600);
	
	/* Добавление миме для системных разделов */
	
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(1, 'image/gif', '.gif', 'default');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(2, 'image/jpeg', '.jpg', 'default');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(3, 'image/png', '.png', 'default');

	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(4, 'image/gif', '.gif', 'faq');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(5, 'image/jpeg', '.jpg', 'faq');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(6, 'image/png', '.png', 'faq');

	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(7, 'image/gif', '.gif', 'rules');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(8, 'image/jpeg', '.jpg', 'rules');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(9, 'image/png', '.png', 'rules');

	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(10, 'image/gif', '.gif', 'terms');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(11, 'image/jpeg', '.jpg', 'terms');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(12, 'image/png', '.png', 'terms');

	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(13, 'image/gif', '.gif', 'news');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(14, 'image/jpeg', '.jpg', 'news');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(15, 'image/png', '.png', 'news');
	
	/* Добавление пунктов меню */
	
	INSERT OR IGNORE INTO MenuGroups VALUES(1, 'Общее');
	-- INSERT OR IGNORE INTO MenuGroups VALUES(2, Тематика);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(3, Творчество);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(4, Игры);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(5, Японская культура);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(6, Взрослым);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(7, Разное);
	-- INSERT OR IGNORE INTO MenuGroups VALUES(8, Пробное);
	
	
	/* Добавление других разделов */
	-- /b/
	INSERT OR IGNORE INTO Boards(BoardName, Description, MenuGroupId) VALUES('b', 'Бред', 1);
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(16, 'image/gif', '.gif', 'b');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(17, 'image/jpeg', '.jpg', 'b');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(18, 'image/png', '.png', 'b');
	
	-- /d/
	INSERT OR IGNORE INTO Boards(BoardName, Description, MenuGroupId) VALUES('d', 'Обсуждения', 1);
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(19, 'image/gif', '.gif', 'd');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(20, 'image/jpeg', '.jpg', 'd');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(21, 'image/png', '.png', 'd');
	
	-- /test/
	INSERT OR IGNORE INTO Boards(BoardName, Description, MenuGroupId) VALUES('test', 'Тестирование', 1);
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(22, 'image/gif', '.gif', 'test');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(23, 'image/jpeg', '.jpg', 'test');
	INSERT OR IGNORE INTO MimeTypes(MimeTypeId, Mime, Extension, BoardName) VALUES(24, 'image/png', '.png', 'test');
	
	-- Добавление ролей, их нельзя удалять или переименовывать
	-- Полное управление сайтом, флаг зоя
	INSERT OR IGNORE INTO Roles VALUES('Зой', 1, 'ImageBoard');
	-- Бот
	INSERT OR IGNORE INTO Roles VALUES('Бот', 1, 'ImageBoard');
	-- Может править пользователями и ролями
	INSERT OR IGNORE INTO Roles VALUES('Ролевик', 1, 'ImageBoard');
	-- Администратор может менять настройки разделов
	INSERT OR IGNORE INTO Roles VALUES('Администратор', 1, 'ImageBoard');
	-- Мобератор может править треды и их параметры
	INSERT OR IGNORE INTO Roles VALUES('Выверяющ', 1, 'ImageBoard');
	-- Мобератор может банить пользователей
	INSERT OR IGNORE INTO Roles VALUES('Досмот', 1, 'ImageBoard');
	-- Создание тредов на системных разделах
	INSERT OR IGNORE INTO Roles VALUES('Редактор', 1, 'ImageBoard');
	-- Мобератор может создавать инвайты
	INSERT OR IGNORE INTO Roles VALUES('Спамер', 1, 'ImageBoard');
	-- Может читать и удалять список жалоб
	INSERT OR IGNORE INTO Roles VALUES('Кляузник', 1, 'ImageBoard');

	-- Для удобства Я скопировал тебе целую спискоту разделов с сосача для быстрого создания
	/*
	<h3>Общее</h3>
	<table>
	<tr class=row1 id=trB runat=server><td>b</td><td>Бред</td><td>Раздел группы «Общее»</td><td><asp:Button ID=cmdCreateB runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trD runat=server><td>d</td><td>Обсуждения</td><td>Обсуждения работы сайта, движка и разделов.</td><td><asp:Button ID=cmdCreateD runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trR runat=server><td>r</td><td>Просьбы</td><td>Выполнение реквестов.</td><td><asp:Button ID=cmdCreateR runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trCp runat=server><td>cp</td><td>Копипаста</td><td>Архив любых интересных паст.</td><td><asp:Button ID=cmdCreateCp runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trInt runat=server><td>int</td><td>International</td><td>Международный раздел для нерусскоязычных.</td><td><asp:Button ID=cmdCreateInt runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trTxt runat=server><td>txt</td><td>Walls of texts</td><td>Текстовый раздел, приклеплять файлы не положено.</td><td><asp:Button ID=cmdCreateTxt runat=server Text=Создать /></td></tr>
	</table>
	<h3>Тематика</h3>
	<table>
	<tr class=row1 id=trAu runat=server><td>au</td><td>Автомобили и транспорт</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateAu runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trBi runat=server><td>bi</td><td>Велосипеды</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateBi runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trBiz runat=server><td>biz</td><td>Бизнес</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateBiz runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trBo runat=server><td>bo</td><td>Книги</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateBo runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trC runat=server><td>с</td><td>Комиксы и мультфильмы</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateC runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trDi runat=server><td>di</td><td>Столовая</td><td>Тематический раздел.</td><td><asp:Button ID=ButtonDi runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trEm runat=server><td>em</td><td>Другие страны и туризм</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateEm runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trEw runat=server><td>ew</td><td>End of World</td><td>Конец света, Большой Пиздец, выживание и самооборона.</td><td><asp:Button ID=cmdCreateEw runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trFa runat=server><td>fa</td><td>Мода и стиль</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateFa runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trFiz runat=server><td>fiz</td><td>Физкультура</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateFiz runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trFl runat=server><td>fl</td><td>Иностранные языки</td><td>Изучение иностранных языков. Тематический раздел.</td><td><asp:Button ID=cmdCreateFl runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trGd runat=server><td>gd</td><td>Gamedev</td><td>Разработка игр. Тематический раздел.</td><td><asp:Button ID=cmdCreateGd runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trHi runat=server><td>hi</td><td>История</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateHi runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trHw runat=server><td>hw</td><td>Железо</td><td>Железо компьтеров и техники.</td><td><asp:Button ID=cmdCreateHw runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trMe runat=server><td>me</td><td>Медицина</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateMe runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trMg runat=server><td>mg</td><td>Магия</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateMg runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trMlp runat=server><td>mlp</td><td>My Little Pony</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateMlp runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trMo runat=server><td>mo</td><td>Мотоциклы</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateMo runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trMu runat=server><td>mu</td><td>Музыка</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateMu runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trNe runat=server><td>ne</td><td>Животные и природа</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreateNe runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trPh runat=server><td>ph</td><td>Философия</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreatePh runat=server Text=Создать /></td></tr>
	<tr class=row2 id=trPo runat=server><td>po</td><td>Политика и новости</td><td>Тематический раздел.</td><td><asp:Button ID=cmdCreatePo runat=server Text=Создать /></td></tr>
	<tr class=row1 id=trPr runat=server><td>pr</td><td>Программирование</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trPsy runat=server><td>psy</td><td>Психология</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trRa runat=server><td>ra</td><td>Радиотехника</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trS runat=server><td>s</td><td>Программы</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trSf runat=server><td>sf</td><td>Научная фантастика</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trSci runat=server><td>sci</td><td>Наука</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trSn runat=server><td>sn</td><td>Паранормальные явления</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trSp runat=server><td>sp</td><td>Спорт</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trSpc runat=server><td>spc</td><td>Космос</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trT runat=server><td>t</td><td>Технологии</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trTr runat=server><td>tr</td><td>Транспорт</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trTv runat=server><td>tv</td><td>Телевизор и фильмы</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trUn runat=server><td>un</td><td>Образование</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trW runat=server><td>w</td><td>Оружие</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row1 id=trWh runat=server><td>wh</td><td>Warhammer</td><td>Тематический раздел.</td><td></td></tr>
	<tr class=row2 id=trWm runat=server><td>wm</td><td>Военная техника</td><td>Тематический раздел.</td><td></td></tr>
	</table>
	<h3>Творчество</h3>
	<table>
	<tr class=row1 id=trDe runat=server><td>de</td><td>Дизайн</td><td>Творчество.</td><td></td></tr>
	<tr class=row2 id=trDiy runat=server><td>diy</td><td>Хобби</td><td>Творчество.</td><td></td></tr>
	<tr class=row1 id=trF runat=server><td>f</td><td>Flash & gif-анимация</td><td>Творчество.</td><td></td></tr>
	<tr class=row2 id=trP runat=server><td>p</td><td>Фото</td><td>Творчество.</td><td></td></tr>
	<tr class=row1 id=trPa runat=server><td>pa</td><td>Живопись и рисование</td><td>Творчество.</td><td></td></tr>
	<tr class=row2 id=trWp runat=server><td>wp</td><td>Обои и высокое рарешение</td><td>Для прикрепления обоев для рабочего стола и картинок с высоким разрешением, рекомендую увеличить стандартный размер загружаемого файла для этого раздела. Творчество.</td><td></td></tr>
	<tr class=row1 id=trTd runat=server><td>td</td><td>Трёхмерная графика</td><td>Творчество</td><td></td></tr>
	</table>
	<h3>Игры</h3>
	<table>
	<tr class=row1 id=trBg runat=server><td>bg</td><td>Настольные игры</td><td></td><td></td></tr>
	<tr class=row2 id=trGb runat=server><td>gb</td><td>Азартные игры</td><td></td><td></td></tr>
	<tr class=row1 id=trMc runat=server><td>mc</td><td>Minecraft</td><td></td><td></td></tr>
	<tr class=row2 id=trMmo runat=server><td>mmo</td><td>Massive Multiplayer Online</td><td></td><td></td></tr>
	<tr class=row1 id=trTes runat=server><td>tes</td><td>The elder scrolls</td><td></td><td></td></tr>
	<tr class=row2 id=trVg runat=server><td>vg</td><td>Видеоигры</td><td></td><td></td></tr>
	<tr class=row1 id=trWr runat=server><td>wr</td><td>Ролевые игры и графомания</td><td></td><td></td></tr>
	</table>
	<h3>Японская культура</h3>
	<table>
	<tr class=row1 id=trA runat=server><td>a</td><td>Аниме</td><td></td><td></td></tr>
	<tr class=row2 id=trAa runat=server><td>aa</td><td>Аниме арт</td><td></td><td></td></tr>
	<tr class=row1 id=trFd runat=server><td>fd</td><td>Фэндом</td><td></td><td></td></tr>
	<tr class=row2 id=trMa runat=server><td>ma</td><td>Манга</td><td></td><td></td></tr>
	<tr class=row1 id=trTo runat=server><td>to</td><td>Touhou</td><td></td><td></td></tr>
	<tr class=row2 id=trVn runat=server><td>vn</td><td>Визуальные новеллы</td><td></td><td></td></tr>
	</table>
	<h3>Взрослым. Лицам до 18 лет посещать эти разделы запрещено.</h3>
	<table>
	<tr class=row1 id=trFg runat=server><td>fg</td><td>Трапы</td><td></td><td></td></tr>
	<tr class=row2 id=trFur runat=server><td>fur</td><td>Фурри</td><td></td><td></td></tr>
	<tr class=row1 id=trG runat=server><td>g</td><td>Девушки</td><td></td><td></td></tr>
	<tr class=row2 id=trGa runat=server><td>ga</td><td>Геи</td><td></td><td></td></tr>
	<tr class=row1 id=trH runat=server><td>h</td><td>Хентай</td><td></td><td></td></tr>
	<tr class=row2 id=trHo runat=server><td>ho</td><td>Прочий хентай</td><td></td><td></td></tr>
	<tr class=row1 id=trLs runat=server><td>ls</td><td>Лоликон</td><td></td><td></td></tr>
	<tr class=row2 id=trPer runat=server><td>per</td><td>Извращения</td><td></td><td></td></tr>
	<tr class=row1 id=trSex runat=server><td>sex</td><td>Секс и отношения</td><td></td><td></td></tr>
	*/
	
END TRANSACTION;