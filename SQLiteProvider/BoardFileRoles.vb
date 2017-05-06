Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function RemoveRoleFromFilesDeny(ByVal DenyId As Long) As ErrorInfo
		RemoveRoleFromFilesDeny = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM DenyGroupsForFiles WHERE DenyId=$DenyId", dbBoards)
		objCommand.Parameters.AddWithValue("$DenyId", DenyId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			RemoveRoleFromFilesDeny.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function GetRolesFromFilesDenyList() As System.Collections.Generic.List(Of DenyRolesForFilesInfo)
		GetRolesFromFilesDenyList = New List(Of DenyRolesForFilesInfo)
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM DenyGroupsForFiles", dbBoards)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			Dim objGroup As DenyRolesForFilesInfo = New DenyRolesForFilesInfo
			With objGroup
				.DenyId = objReader.GetInt64(0)
				.MimeId = objReader.GetInt64(1)
				.RoleName = objReader.GetString(2)
			End With
			GetRolesFromFilesDenyList.Add(objGroup)
		Loop
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

	Public Overrides Function AddRoleToFilesDeny(ByVal MimeId As Long, ByVal RoleName As String) As ErrorInfo
		AddRoleToFilesDeny = New ErrorInfo
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO DenyGroupsForFiles(MimeId, RoleName) VALUES ($MimeId, $RoleName)", dbBoards)
		objCommand.Parameters.AddWithValue("$MimeId", MimeId)
		objCommand.Parameters.AddWithValue("$RoleName", RoleName)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			AddRoleToFilesDeny.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

End Class
