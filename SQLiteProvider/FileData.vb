Friend Class FileData
	Implements IDisposable

	Public Sub New()
		FilesData = New List(Of FileData)
	End Sub

	'''' <summary>
	'''' Изобращение капчи-ведуньи
	'''' </summary>
	'''' <remarks></remarks>
	'Public MagicCaptcha As Bitmap
	''' <summary>
	''' Коллекция отправленных пользователем файлов
	''' </summary>
	''' <remarks></remarks>
	Public FilesData As List(Of FileData)

	''' <summary>
	''' Тип ошибки
	''' </summary>
	''' <remarks></remarks>
	Public ErrorStatus As ErrorType
	''' <summary>
	''' Текстовое описание ошибки
	''' </summary>
	''' <remarks></remarks>
	Public ErrorString As String

	Private disposedValue As Boolean = False ' Чтобы обнаружить избыточные вызовы

	' IDisposable
	Protected Overridable Sub Dispose(ByVal disposing As Boolean)
		If Not Me.disposedValue Then
			If disposing Then
				' освободить другие состояния (управляемые объекты).
				For Each objFileData As FileData In FilesData
					objFileData.Dispose()
				Next
				FilesData.Clear()
				'If MagicCaptcha IsNot Nothing Then
				'	MagicCaptcha.Dispose()
				'End If
			End If

			' освободить собственные состояния (неуправляемые объекты).
			' задать большие поля как null.
		End If
		Me.disposedValue = True
	End Sub

#Region " IDisposable Support "
	' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
	Public Sub Dispose() Implements IDisposable.Dispose
		' Не изменяйте этот код. Разместите код очистки выше в Dispose(ByVal disposing As Boolean).
		Dispose(True)
		GC.SuppressFinalize(Me)
	End Sub
#End Region


	Public Class FileData
		Implements IDisposable
		''' <summary>
		''' Тип файла
		''' </summary>
		''' <remarks></remarks>
		Public FileType As FileType
		''' <summary>
		''' Ссылка на отправленный файл
		''' </summary>
		''' <remarks></remarks>
		Public fs As ImageboardPostedFile
		'''' <summary>
		'''' Капча-ведунья
		'''' </summary>
		'''' <remarks></remarks>
		'Public MagicCaptcha As Bitmap
		''' <summary>
		''' Размер файла какпчи-ведуньи
		''' </summary>
		''' <remarks></remarks>
		Public MagicCaptchaLength As Long
		''' <summary>
		''' 
		''' </summary>
		''' <remarks></remarks>
		Public ThumbBitmap As Bitmap


		Public UrlFilePath As String

		Public FileLength As Long
		Public FileWidth As Integer
		Public FileHeight As Integer

		Public ThumbUrlFilePath As String
		Public ThumbWidth As Integer
		Public ThumbHeight As Integer
		Public ShortFileName As String
		Public MediaType As String
		Public ShortMediaType As String
		'Public BoardName As String
		Private disposedValue As Boolean = False		' Чтобы обнаружить избыточные вызовы

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					'If MagicCaptcha IsNot Nothing Then
					'	MagicCaptcha.Dispose()
					'End If
					If ThumbBitmap IsNot Nothing Then
						ThumbBitmap.Dispose()
					End If
				End If

				' освободить собственные состояния (неуправляемые объекты).
				' задать большие поля как null.
			End If
			Me.disposedValue = True
		End Sub

#Region " IDisposable Support "
		' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
		Public Sub Dispose() Implements IDisposable.Dispose
			' Не изменяйте этот код. Разместите код очистки выше в Dispose(ByVal disposing As Boolean).
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
#End Region

	End Class

End Class

