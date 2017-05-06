Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function EditMime(ByVal MimeId As Long, ByVal Mime As String, ByVal Extension As String, ByVal MaxFileLength As Long, ByVal FileType As FileType) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		' Открываем базу данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE OR IGNORE MimeTypes SET Mime=$Mime, Extension=$Extension, MaxFileLength=$MaxFileLength, FileType=$FileType WHERE MimeTypeId=$MimeTypeId", dbBoards)
		objCommand.Parameters.AddWithValue("$Mime", Mime)
		objCommand.Parameters.AddWithValue("$Extension", Extension)
		objCommand.Parameters.AddWithValue("$MaxFileLength", MaxFileLength)
		objCommand.Parameters.AddWithValue("$FileType", FileType)
		objCommand.Parameters.AddWithValue("$MimeTypeId", MimeId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		' Закрываем базу
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	Public Overrides Function GetMimeList() As List(Of MimeInfo)
		Dim colMime As List(Of MimeInfo) = New List(Of MimeInfo)
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Выборка всех пунктов миме из базы
		Dim objCommand As SQLiteCommand
		If String.IsNullOrEmpty(m_BoardName) Then
			objCommand = New SQLiteCommand("SELECT * FROM MimeTypes", dbBoards)
		Else
			objCommand = New SQLiteCommand("SELECT * FROM MimeTypes WHERE BoardName=$BoardName", dbBoards)
			objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		End If
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			colMime.Add(ReadMime(objReader))
		Loop
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
		Return colMime
	End Function

	Public Overrides Function RemoveMime(ByVal MimeId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM MimeTypes WHERE MimeTypeId=$MimeTypeId", dbBoards)
		objCommand.Parameters.AddWithValue("$MimeTypeId", MimeId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	Public Overrides Function CreateMime(ByVal Mime As String, ByVal Extension As String, ByVal MaxFileLength As Long, ByVal FileType As FileType) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		' точка для расширения, необходима
		If Not Extension.StartsWith(".") Then
			Extension = "." & Extension
		End If
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO MimeTypes (Mime, Extension, BoardName, MaxFileLength, FileType) VALUES ($Mime, $Extension, $BoardName, $MaxFileLength, $FileType)", dbBoards)
		objCommand.Parameters.AddWithValue("$Mime", Mime)
		objCommand.Parameters.AddWithValue("$Extension", Extension)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MaxFileLength", MaxFileLength)
		objCommand.Parameters.AddWithValue("$FileType", FileType)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		' Закрываем базу
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		' Сообщаем об успешном создании миме
		Return objErrorInfo
	End Function

	Private Function ReadMime(ByVal objReader As SQLiteDataReader) As MimeInfo
		ReadMime = New MimeInfo
		With ReadMime
			.MimeTypeId = objReader.GetInt64(0)
			.Mime = objReader.GetString(1)
			.Extension = objReader.GetString(2)
			.BoardName = objReader.GetString(3)
			.MaxFileLength = objReader.GetInt64(4)
			.FileType = CType(objReader.GetInt64(5), FileType)
			.AnonymousAccess = CType(objReader.GetInt64(6), AnonymousAccess)
		End With
	End Function

End Class
