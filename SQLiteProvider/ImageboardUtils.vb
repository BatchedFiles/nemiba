Imports System.Threading
Imports System.Web.Configuration
Imports System.Security.Cryptography

Friend Module ImageboardUtils

	Private m_BaseLock As ReaderWriterLock = New ReaderWriterLock
	Private m_Connection As SQLiteConnection
	Private m_Connections As Integer

	''' <summary>
	''' Возвращает хэш строки
	''' </summary>
	''' <param name="strToHash">Строка для хэширования</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetHash(ByVal strToHash As String) As String
		Dim sb As New StringBuilder
		Using ms As New MemoryStream
			Using br As New BinaryWriter(ms)
				' Создаём объект хэширования с пустым ключом
				Using objHash As New HMACMD5({})
					' записываем хэш и устанавливаем указатель на начало
					br.Write(strToHash)
					ms.Seek(0, SeekOrigin.Begin)
					Dim HashValue() As Byte = objHash.ComputeHash(ms)
					For Each b As Byte In HashValue
						sb.Append(b.ToString("x"))
					Next
					Erase HashValue
					objHash.Clear()
				End Using
			End Using
		End Using
		With sb
			.Replace("1", "G")
			.Replace("2", "H")
			.Replace("3", "K")
			.Replace("4", "M")
			.Replace("5", "N")
			.Replace("6", "g")
			.Replace("7", "h")
			.Replace("8", "k")
			.Replace("9", "m")
			.Replace("0", "r")
		End With
		Return sb.ToString
	End Function

	''' <summary>
	''' Открывает базу на чтение или запись
	''' </summary>
	''' <returns>Открытое подключение <c>SQLiteConnection</c> к базе данных.</returns>
	''' <param name="ForReading">Если база данных открывается на чтение, необходимо передать <c>True</c>, если на запись, то <c>False</c>.</param>
	''' <remarks></remarks>
	Public Function OpenBoardBase(ByVal strConnectionString As String, ByVal ForReading As Boolean) As SQLiteConnection
		If ForReading Then
			m_BaseLock.AcquireReaderLock(Timeout.Infinite)
		Else
			m_BaseLock.AcquireWriterLock(Timeout.Infinite)
		End If
		m_Connections += 1
		If m_Connection Is Nothing Then
			m_Connection = New SQLiteConnection(strConnectionString)
		End If
		If m_Connection.State = ConnectionState.Closed Then
			m_Connection.Open()
		End If
		Return m_Connection
	End Function

	''' <summary>
	''' Закрывает базу и снимает блокировку
	''' </summary>
	''' <param name="blnForReading"></param>
	''' <remarks></remarks>
	Public Sub CloseBoardBase(ByVal blnForReading As Boolean)
		' Закрываем базу
		m_Connections -= 1
		If m_Connections = 0 Then
			m_Connection.Close()
			m_Connection.Dispose()
			m_Connection = Nothing
		End If

		' Снимаем блокировку
		If blnForReading Then
			m_BaseLock.ReleaseReaderLock()
		Else
			m_BaseLock.ReleaseWriterLock()
		End If

	End Sub

	''' <summary>
	''' Повышает уровень блокировки чтения до блокировки записи
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function UpgradeToWriterLock() As LockCookie
		Return m_BaseLock.UpgradeToWriterLock(Timeout.Infinite)
	End Function

	''' <summary>
	''' Вобзвращает состояние потока к тому, которое было до вызова повышения уровня блокировки
	''' </summary>
	''' <param name="Value"></param>
	''' <remarks></remarks>
	Public Sub DowngradeFromWriterLock(ByVal Value As LockCookie)
		m_BaseLock.DowngradeFromWriterLock(Value)
	End Sub

	''' <summary>
	''' Возвращает экранированную строку, безопасную для добавления в базу данных и отображении на странице
	''' </summary>
	''' <param name="strToSafe"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function GetSafeString(ByVal strToSafe As String) As String
		If String.IsNullOrEmpty(strToSafe) Then
			Return String.Empty
		Else
			Dim sb As New StringBuilder(strToSafe)
			' Необходимо найти и экранировать символы '"&<>
			With sb
				.Replace("&", "&amp;")
				.Replace("'", "&apos;")
				.Replace("""", "&quot;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
			End With
			Return sb.ToString
		End If
	End Function

End Module
