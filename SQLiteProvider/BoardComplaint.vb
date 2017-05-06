Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function Removecomplaint(ByVal СomplaintId As Long) As ErrorInfo
		Removecomplaint = New ErrorInfo
		' Найти раздел
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		' Раздел существует
		Dim objCommand As SQLiteCommand = New SQLiteCommand("DELETE FROM Complaint WHERE СomplaintId=$СomplaintId", dbBoards)
		objCommand.Parameters.AddWithValue("$СomplaintId", СomplaintId)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			Removecomplaint.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

	Public Overrides Function GetcomplaintList() As System.Collections.Generic.List(Of ComplaintInfo)
		GetcomplaintList = New List(Of ComplaintInfo)
		' Найти раздел
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, True)
		Dim objCommand As SQLiteCommand = New SQLiteCommand("SELECT * FROM Complaint WHERE BoardName=$BoardName", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		Dim objReader As SQLiteDataReader = objCommand.ExecuteReader()
		Do While objReader.Read
			Dim objСomplaintInfo As ComplaintInfo = New ComplaintInfo
			With objСomplaintInfo
				.СomplaintId = objReader.GetInt64(0)
				.MessageNumber = objReader.GetInt64(1)
				.СomplaintText = objReader.GetString(2)
			End With
			GetcomplaintList.Add(objСomplaintInfo)
		Loop
		objReader.Close()
		objCommand.Dispose()
		CloseBoardBase(True)
	End Function

	Public Overrides Function Createcomplaint(ByVal MessageNumber As Long, ByVal СomplaintText As String) As ErrorInfo
		Createcomplaint = New ErrorInfo
		' Найти раздел
		Dim dbBoards As SQLiteConnection = OpenBoardBase(ConnectionString, False)
		' Раздел существует
		Dim objCommand As SQLiteCommand = New SQLiteCommand("INSERT INTO Complaint (BoardName, MessageNumber, ComplaintText) VALUES ($BoardName, $MessageNumber, $ComplaintText)", dbBoards)
		objCommand.Parameters.AddWithValue("$BoardName", m_BoardName)
		objCommand.Parameters.AddWithValue("$MessageNumber", MessageNumber)
		objCommand.Parameters.AddWithValue("$ComplaintText", СomplaintText)
		Dim intRows As Integer = objCommand.ExecuteNonQuery()
		objCommand.Dispose()
		CloseBoardBase(False)
		If intRows = 0 Then
			Createcomplaint.ErrorStatus = ErrorType.AccessViolation
		End If
	End Function

End Class
