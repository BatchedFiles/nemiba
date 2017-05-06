Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function AddRoleToAnswersBoardDeny(ByVal RoleName As String, ByVal DenyForReading As Boolean) As ErrorInfo
		AddRoleToAnswersBoardDeny = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO DenyForBoardAnswers VALUES ($BoardName, $RoleName, $DenyForReading)", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$RoleName", RoleName)
		objCommand.Parameters.AddWithValue("$DenyForReading", DenyForReading)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			AddRoleToAnswersBoardDeny.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function RemoveRoleFromAnswersBoardDeny(ByVal RoleName As String) As ErrorInfo
		RemoveRoleFromAnswersBoardDeny = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM DenyForBoardAnswers WHERE BoardName=$BoardName AND RoleName=$RoleName", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$RoleName", RoleName)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			RemoveRoleFromAnswersBoardDeny.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function EditRoleFromAnswersBoardDeny(ByVal RoleName As String, ByVal DenyForReading As Boolean) As ErrorInfo
		EditRoleFromAnswersBoardDeny = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE DenyForBoardAnswers SET DenyForReading=$DenyForReading WHERE BoardName=$BoardName AND RoleName=$RoleName", dbBoards)
		objCommand.Parameters.AddWithValue("$DenyForReading", DenyForReading)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$RoleName", RoleName)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			EditRoleFromAnswersBoardDeny.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function GetRolesFromAnswersBoardDenyList() As System.Collections.Generic.List(Of DenyRolesForBoardAnswersInfo)
		GetRolesFromAnswersBoardDenyList = New System.Collections.Generic.List(Of DenyRolesForBoardAnswersInfo)
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand(String.Format("SELECT * FROM DenyForBoardAnswers WHERE BoardName=$BoardName", m_BoardName), dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			Dim objGroup As DenyRolesForBoardAnswersInfo = New DenyRolesForBoardAnswersInfo
			With objGroup
				.BoardName = objReader.GetString(0)
				.RoleName = objReader.GetString(1)
				.DenyForReading = objReader.GetBoolean(2)
			End With
			GetRolesFromAnswersBoardDenyList.Add(objGroup)
		Loop
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

End Class
