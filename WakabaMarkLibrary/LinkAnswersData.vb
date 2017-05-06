Imports Microsoft.VisualBasic

Public Class LinkAnswersData

	''' <summary>
	''' Список сообщений, номера которых встретились в тексте
	''' </summary>
	''' <remarks></remarks>
	Public LinkMessages As Generic.List(Of LinkMessage)

	''' <summary>
	''' Текст сообщения после трансформации
	''' </summary>
	''' <remarks></remarks>
	Public MessageText As String

	Public Sub New()
		LinkMessages = New Generic.List(Of LinkMessage)
	End Sub

	''' <summary>
	''' Сообщение, на которое ссылается главное
	''' </summary>
	''' <remarks></remarks>
	Public Class LinkMessage
		''' <summary>
		''' Имя раздела, указанное в тексте
		''' </summary>
		''' <remarks></remarks>
		Public BoardName As String
		''' <summary>
		''' Номер сообщения, указанные в тексте
		''' </summary>
		''' <remarks></remarks>
		Public MessageNumber As Long
	End Class

	Public Sub AddAnswer(ByVal BoardName As String, ByVal MessageNumber As Long)
		Dim objAnswer As LinkMessage = New LinkMessage
		With objAnswer
			.BoardName = BoardName
			.MessageNumber = MessageNumber
		End With
		LinkMessages.Add(objAnswer)
	End Sub
End Class
