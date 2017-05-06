Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function RemoveRules(ByVal RulesId As Long) As ErrorInfo
		RemoveRules = New ErrorInfo
		' Берём блокировку на запись общей базы данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM Rules WHERE RulesId=$RulesId", dbBoards)
		objCommand.Parameters.AddWithValue("$RulesId", RulesId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		' Закрываем базу
		CloseBoardBase(False)
		If intRows = 0 Then
			RemoveRules.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function GetRulesList() As System.Collections.Generic.List(Of RulesInfo)
		Dim colRules As List(Of RulesInfo) = New List(Of RulesInfo)
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM Rules WHERE BoardName=$BoardName", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			Dim objRules As RulesInfo
			With objRules
				.RulesId = objReader.GetInt64(0)
				.BoardName = objReader.GetString(1)
				.RulesText = objReader.GetString(2)
			End With
			colRules.Add(objRules)
		Loop
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
		Return colRules
	End Function

	Public Overrides Function EditRules(ByVal RulesId As Long, ByVal RulesText As String) As ErrorInfo
		EditRules = New ErrorInfo
		' Открываем базу данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE OR IGNORE Rules SET RulesItem=$RulesItem WHERE RulesId=$RulesId", dbBoards)
		objCommand.Parameters.AddWithValue("$RulesId", RulesId)
		objCommand.Parameters.AddWithValue("$RulesItem", RulesText)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		' Закрываем базу
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			EditRules.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function CreateRules(ByVal RulesText As String) As ErrorInfo
		CreateRules = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO Rules (BoardName, RulesItem) VALUES ($BoardName, $RulesText)", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$RulesText", RulesText)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			CreateRules.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

End Class
