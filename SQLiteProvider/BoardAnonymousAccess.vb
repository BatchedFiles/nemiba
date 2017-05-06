Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function EditThreadAnonymousAccess(ByVal ThreadNumber As Long, ByVal Access As AnonymousAccess) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function EditBoardAnonymousAccess(ByVal Access As AnonymousAccess) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function EditFileAnonymousAccess(ByVal MimeId As Long, ByVal Access As AnonymousAccess) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function EditThreadAlwaysOnTopFlag(ByVal ThreadNumber As Long, ByVal TopFlag As Boolean) As ErrorInfo
		EditThreadAlwaysOnTopFlag = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE OR IGNORE Threads SET IsTop=$IsTop WHERE BoardName=$BoardName AND ThreadNumber=$ThreadNumber", dbBoards)
		objCommand.Parameters.AddWithValue("$IsTop", Convert.ToInt64(TopFlag))
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$ThreadNumber", ThreadNumber)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			EditThreadAlwaysOnTopFlag.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

End Class
