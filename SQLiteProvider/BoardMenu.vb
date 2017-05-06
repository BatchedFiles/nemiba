Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function CreateMenu(ByVal MenuText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO MenuGroups (Description) VALUES ($Description)", dbBoards)
		objCommand.Parameters.AddWithValue("$Description", MenuText)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	Public Overrides Function EditMenu(ByVal MenuId As Long, ByVal MenuText As String) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("UPDATE OR IGNORE MenuGroups SET Description=$Description WHERE MenuGroupId=$MenuGroupId", dbBoards)
		objCommand.Parameters.AddWithValue("$Description", MenuText)
		objCommand.Parameters.AddWithValue("$MenuGroupId", MenuId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

	Public Overrides Function GetMenuList() As System.Collections.Generic.List(Of MenuInfo)
		Dim colMenuGroup As List(Of MenuInfo) = New List(Of MenuInfo)
		' Открываем базу данных
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		' Выборка всех пунктов меню из базы
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM MenuGroups", dbBoards)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			Dim objMenuInfo As MenuInfo = New MenuInfo
			With objMenuInfo
				.MenuNumber = objReader.GetInt64(0)
				.MenuText = objReader.GetString(1)
			End With
			colMenuGroup.Add(objMenuInfo)
		Loop
		objReader.Close()
		objCommand.Dispose()
		' Закрываем базу
		CloseBoardBase(True)
		Return colMenuGroup
	End Function

	Public Overrides Function RemoveMenu(ByVal MenuId As Long) As ErrorInfo
		Dim objErrorInfo As ErrorInfo = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM MenuGroups WHERE MenuGroupId=$MenuGroupId", dbBoards)
		objCommand.Parameters.AddWithValue("$MenuGroupId", MenuId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			objErrorInfo.ErrorStatus = ErrorType.AccessViolation
		End If
		Return objErrorInfo
	End Function

End Class
