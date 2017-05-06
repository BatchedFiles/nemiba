Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function GetBadWordList() As List(Of BadWordInfo)
		GetBadWordList = New List(Of BadWordInfo)
		' Открываем базу данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Выборка всех пунктов меню из базы
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM BadWords WHERE BoardName=$BoardName", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			GetBadWordList.Add(ReadBadWord(objReader))
		Loop
		objReader.Close()
		' Закрываем базу
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

	Public Overrides Function EditBadWord(ByVal BadWordNumber As Long, ByVal Template As String, ByVal ReplacementString As String, ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo
		EditBadWord = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE OR IGNORE BadWords SET Template=$Template, ReplacementString=$ReplacementString, ReplacementFlag=$ReplacementFlag WHERE BadWordId=$BadWordId", dbBoards)
		objCommand.Parameters.AddWithValue("$Template", Template)
		objCommand.Parameters.AddWithValue("$ReplacementString", ReplacementString)
		objCommand.Parameters.AddWithValue("$ReplacementFlag", ReplacementFlag)
		objCommand.Parameters.AddWithValue("$BadWordId", BadWordNumber)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			EditBadWord.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function RemoveBadWord(ByVal BadWordNumber As Long) As ErrorInfo
		RemoveBadWord = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM BadWords WHERE BadWordId=$BadWordId", dbBoards)
		objCommand.Parameters.AddWithValue("$BadWordId", BadWordNumber)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			RemoveBadWord.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function CreateBadWord(ByVal Template As String, ByVal ReplacementString As String, ByVal ReplacementFlag As ReplacementFlag) As ErrorInfo
		CreateBadWord = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO BadWords (BoardName, Template, ReplacementString, ReplacementFlag) VALUES ($BoardName, $Template, $ReplacementString, $ReplacementFlag)", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$Template", Template)
		objCommand.Parameters.AddWithValue("$ReplacementString", ReplacementString)
		objCommand.Parameters.AddWithValue("$ReplacementFlag", ReplacementFlag)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			CreateBadWord.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

End Class
