Imports System.Text
Imports System.Text.RegularExpressions

Public Class WakabaMark

	''' <summary>
	''' Списки
	''' </summary>
	''' <remarks></remarks>
	Private Enum Lists
		''' <summary>
		''' Списков нет
		''' </summary>
		''' <remarks></remarks>
		None
		''' <summary>
		''' Маркированный список
		''' </summary>
		''' <remarks></remarks>
		Mark
		''' <summary>
		''' Нумерованный список
		''' </summary>
		''' <remarks></remarks>
		Numeric
	End Enum

	''' <summary>
	''' Изменяет текст сообщения пользователя на соответсткие вакаба-марку
	''' </summary>
	''' <param name="strMessage"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function WakabaMark(ByVal strMessage As String) As String
		Dim blnEmptyLine As Boolean	' Пустая строка
		Dim blnCode As Boolean ' Внутри кода
		Dim intList As Lists ' Списки
		Dim lngLanguage As String ' Язык

		Dim sb As StringBuilder = New StringBuilder()
		' Рзазбить сообщение пользователя на строки
		Dim astrLines() As String = Regex.Split(strMessage, "\r\n")
		For i As Long = 0 To astrLines.Length - 1
			' Если мы внутри кода, то ищем закрытия тэга
			If blnCode Then
				' Ищем </code> с начала строки
				Dim regCodeEnd As Regex = New Regex("^(?<data>.*)&lt;/code&gt;")
				Dim mm As MatchCollection = regCodeEnd.Matches(astrLines(i))
				If mm.Count Then
					' Данные
					Dim strData As String = mm(0).Groups.Item("data").ToString
					' Изменяем строку на окончание кода
					sb.Append(CodeHighLighting(strData) & "</code></div>")
					' Снимаем флаг
					blnCode = False
					lngLanguage = String.Empty
				Else
					' Не нашли: отправляем на обработку подсветки синтаксиса
					sb.Append(CodeHighLighting(astrLines(i)))
				End If
			Else
				' Проверим на наличие <code>
				' Ищем <code> или <code lang="vb"> с начала строки "&lt;code lang=&#32;язык&#32;&gt;"
				Const CodePattern As String = "^*&lt;code(\s+lang(uage)?=&quot;(?<lang>.+)&quot;)?&gt;(?<data>.*)"
				Dim regCodeStart As Regex = New Regex(CodePattern)
				Dim mm As MatchCollection = regCodeStart.Matches(astrLines(i))
				If mm.Count Then
					' Устанавливаем флаг кода
					blnCode = True
					' Язык кода
					Dim objLang As Group = mm(0).Groups.Item("lang")
					If objLang Is Nothing Then
						lngLanguage = "Text"
					Else
						lngLanguage = objLang.ToString
					End If
					' Данные
					Dim strData As String = mm(0).Groups.Item("data").ToString

					' Закрываем абзац
					If blnEmptyLine Then
						sb.Append("</p>")
						blnEmptyLine = False
					End If

					' Закрываем списки
					Select Case intList
						Case Lists.Mark
							sb.Append("</ul>")
						Case Lists.Numeric
							sb.Append("</ol>")
					End Select
					intList = Lists.None

					' Заменяем строку на код
					sb.AppendFormat("<div class=""codebox""><div><span>Код</span> <span class=""codename"">{0}</span></div><code>{1}", lngLanguage.ToString, CodeHighLighting(strData))
				Else
					' Ищем списки
					' Проверим на наличие * и пробела
					' Если есть, то ставим флаг маркированного списка
					If astrLines(i).StartsWith("* ") Then
						Select Case intList
							Case Lists.None
								' Закрываем линию, если есть
								If blnEmptyLine Then
									sb.Append("</p>")
								End If
								' Открываем список
								sb.Append("<ul>")
							Case Lists.Mark
								' Ничего не делаем
							Case Lists.Numeric
								' Закрываем нумерованный список
								' и открываем маркированный
								sb.Append("</ol><ul>")
						End Select
						blnEmptyLine = False
						intList = Lists.Mark
						sb.Append("<li>" & AddMark(astrLines(i).Substring(2)) & "</li>")
						' Проверяем следующую строку
						Continue For
					End If

					' Проверим на наличие # и пробела
					' Если есть, то ставим флаг нумерованного списка
					If astrLines(i).StartsWith("# ") Then
						Select Case intList
							Case Lists.None
								' Закрываем линию, если есть
								If blnEmptyLine Then
									sb.Append("</p>")
								End If
								' Открываем список
								sb.Append("<ol>")
							Case Lists.Mark
								' Закрываем маркированный список
								' и открываем нумерованный
								sb.Append("</ul><ol>")
							Case Lists.Numeric
								' Ничего не делаем
						End Select
						blnEmptyLine = False
						intList = Lists.Numeric
						' UNDONE Устранить баг удаления символов нумерованного списка
						sb.Append("<li>" & AddMark(astrLines(i).Substring(2)) & "</li>")
						' Проверяем следующую строку
						Continue For
					End If
					' Закрываем списки
					Select Case intList
						Case Lists.Mark
							sb.Append("</ul>")
						Case Lists.Numeric
							sb.Append("</ol>")
					End Select
					intList = Lists.None

					' TODO добавить тег форматирования для
					'<i2p name="">key</i2p>
					Const i2pPattern As String = "^&lt;i2p\s+name?=&quot;(?<sitename>\.+?)&quot;?&gt;(?<i2pkey>.+)&lt;/i2p&gt;"
					Dim regi2pStart As Regex = New Regex(i2pPattern)
					'Dim mm As MatchCollection = regi2pStart.Matches(astrLines(i))

					' Разбить на строки, каждую строку обрамить в отдельный <p></p>
					' если между абзацами один символ переноса строки, то использовать <br />
					' Если без пробелов строка не пуста
					If Regex.Replace(astrLines(i), "\s", String.Empty).Length > 0 Then
						' В строке есть текст
						If blnEmptyLine Then
							' Предыдущая строка полная
							' Добавляем разрыв абзаца
							sb.Append("<br />" & AddMark(astrLines(i)))
						Else
							' Предыдущей строки нет
							sb.Append("<p>" & AddMark(astrLines(i)))
							blnEmptyLine = True
						End If
					Else
						If blnEmptyLine Then
							sb.Append("</p>")
							blnEmptyLine = False
						End If
					End If
				End If
			End If
		Next i
		' Закрываем незакрытые тэги
		If blnEmptyLine Then
			sb.Append("</p>")
		Else
			Select Case intList
				Case Lists.None
					If blnCode Then
						' Код не закрыт, закрываем
						sb.Append("</div>")
					End If
				Case Lists.Mark
					sb.Append("</ul>")
				Case Lists.Numeric
					sb.Append("</ol>")
			End Select
		End If
		Return sb.ToString
	End Function

	''' <summary>
	''' Подсвечиваем синтаксис
	''' </summary>
	''' <param name="strLine"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Shared Function CodeHighLighting(ByVal strLine As String) As String
		Return Regex.Replace(Regex.Replace(strLine, "\t", "&#160;&#160;&#160;&#160;"), "\s", "&#32;") & "<br />"
	End Function

	''' <summary>
	''' Преобразуем строку в однострочный вакабамарк
	''' </summary>
	''' <param name="strMark"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Shared Function AddMark(ByVal strMark As String) As String
		' UNDONE Добавить возможность экранирования символов
		' А сейчас здесь работает только грязный хак
		If Regex.IsMatch(strMark, "^&lt;nocode&gt;.*&lt;/nocode&gt;") Then
			Return Regex.Replace(strMark, "^&lt;nocode&gt;(?<text>.*)&lt;/nocode&gt;", "${text}")
		Else
			' 4 и более пробелов — моноширинный
			strMark = Regex.Replace(strMark, "^[\s]{4,}(?<messagetext>.*)", "<span class=""monospace"">${messagetext}</span>")
			' Ищем 1-3 пробела в начале строки - строка с отступом
			strMark = Regex.Replace(strMark, "^[\s]{1,3}(?<messagetext>.*)", "&#160;&#160;&#160;&#160;${messagetext}")
			' Ищем > в начале строки и преобразуем в цитату
			strMark = Regex.Replace(strMark, "^&gt;(?<messagetext>.*)", "<span class=""unkfunc"">&gt;${messagetext}</span>")

			' Ищем	   <s>текст</s> == зачёркнутый
			strMark = Regex.Replace(strMark, "&lt;s&gt;(?<messagetext>.*?)&lt;/s&gt;", "<span class=""strike"">${messagetext}</span>")
			' Ищем жирный
			'strMark = Regex.Replace(strMark, "&lt;b&gt;(?<messagetext>.*?)&lt;/b&gt;", "<span class=""bold"">${messagetext}</span>")
			'strMark = Regex.Replace(strMark, "\*\*(?<messagetext>.*?)\*\*", "<span class=""bold"">${messagetext}</span>")
			'strMark = Regex.Replace(strMark, "__(?<messagetext>.*?)__", "<span class=""bold"">${messagetext}</span>")
			strMark = Regex.Replace(strMark, "&lt;b&gt;(?<messagetext>.*?)&lt;/b&gt;", "<strong>${messagetext}</strong>")
			strMark = Regex.Replace(strMark, "\*\*(?<messagetext>.*?)\*\*", "<strong>${messagetext}</strong>")
			strMark = Regex.Replace(strMark, "__(?<messagetext>.*?)__", "<strong>${messagetext}</strong>")
			' Ищем курсив
			'strMark = Regex.Replace(strMark, "&lt;i&gt;(?<messagetext>.*?)&lt;/i&gt;", "<span class=""italic"">${messagetext}</span>")
			'strMark = Regex.Replace(strMark, "\*(?<messagetext>.*?)\*", "<span class=""italic"">${messagetext}</span>")
			strMark = Regex.Replace(strMark, "&lt;i&gt;(?<messagetext>.*?)&lt;/i&gt;", "<em>${messagetext}</em>")
			strMark = Regex.Replace(strMark, "\*(?<messagetext>.*?)\*", "<em>${messagetext}</em>")
			' Спойлер
			strMark = Regex.Replace(strMark, "&lt;spoiler&gt;(?<messagetext>.*)&lt;/spoiler&gt;", "<span class=""spoiler"">${messagetext}</span>")
			strMark = Regex.Replace(strMark, "%%(?<messagetext>.*?)%%", "<span class=""spoiler"">${messagetext}</span>")
			' Подчёрнутый
			strMark = Regex.Replace(strMark, "_(?<messagetext>.*?)_", "<span class=""underline"">${messagetext}</span>")
			strMark = Regex.Replace(strMark, "&lt;u&gt;(?<messagetext>.*?)&lt;/u&gt;", "<span class=""underline"">${messagetext}</span>")

			'' Надчёркнутый
			'pattern = "(&lt;)o(&gt;)(?<messagetext>.*)(&lt;)/o(&gt;)"
			'strMark(intNumber) = Regex.Replace(strMark(intNumber), pattern, "<o>${messagetext}</o>")

			' Ищем символы ^H
			'Dim reg As Regex = New Regex("(?<messagetext>.)\^H")
			'Dim intReplace As Long
			'If reg.IsMatch(strMark(intNumber)) Then
			'	strMark(intNumber) = reg.Replace(strMark(intNumber), "<s>${messagetext}</s>", 1)
			'	intReplace = 1
			'End If
			'Do While strMark(intNumber).IndexOf("^H") > 0
			'	reg = New Regex("(?<messagetext>.)(?<strikeout>[\<s>.\</s>])\^H")
			'	'Dim strTemp As String = reg.Match(strMark(intNumber)).Value
			'	strMark(intNumber) = reg.Replace(strMark(intNumber), "<s>${messagetext}</s>${strikeout}", 1)
			'	intReplace += 1
			'Loop

			' UNDONE Магнет-ссылка
			' magnet:?xt=urn:btih:ff952751320ca71e686482bf300a4c57977ec20d&dn=Ulf+Soderberg+%26+Sephiroth&tr=http://tracker2.postman.i2p/announce.php
			'strMessage = Regex.Replace(strMessage, "magnet:\?(?<sitename>[^\s]+)", "<a href=""magnet:?"">dn</a>")

			' Ссылка на сайт
			'If Regex.IsMatch() And Not Regex.IsMatch() Then
			strMark = Regex.Replace(strMark, "(?<http>https?)://(?<sitename>[^/\s]+\.i2p)/(?<page>[^\s]+)", "<a href=""${http}://${sitename}/${page}"">${http}://${sitename}/${page}</a>")
			'End If
			'If Regex.IsMatch() And Not Regex.IsMatch() Then
			strMark = Regex.Replace(strMark, "(?<http>https?)://(?<sitename>[^/\s]+\.onion)/(?<page>[^\s]+)", "<a href=""${http}://${sitename}/${page}"">${http}://${sitename}/${page}</a>")
			'End If
			'If Regex.IsMatch() And Not Regex.IsMatch() Then
			strMark = Regex.Replace(strMark, "(?<http>https?)://(?<sitename>[^/\s]+\.[^io][^2n][^pi][^on]?)/(?<page>[^\s]+)", "<a class=""externallink"" href=""${http}://${sitename}/${page}"">${http}://${sitename}/${page}</a>")
			'End If

			'strMark(intNumber) = Regex.Replace(strMark(intNumber), "http://(?<sitename>[^\s]+)", "<a class=""externallink"" href=""http://${sitename}"">http://${sitename}</a>")
			'strMark(intNumber) = Regex.Replace(strMark(intNumber), "https?://(?<sitename>[a-z\-\.]+)(?<i2pflag>.i2p)/[^\s]+", "<a class=""externallink"" href=""https://${sitename}"">https://${sitename}</a>")
			' Электропочта
			'"mailto:([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)"
			'
			Return strMark
		End If
	End Function

	Public Shared Function ReplaceLink(ByVal BoardName As String, ByVal strMessage As String) As LinkAnswersData
		Dim objAnswers As LinkAnswersData = New LinkAnswersData
		' + предыдущий элемент повторяется 1 или более раз
		' ? предыдущий элемент повторяется 0 или 1 раз
		' * предыдущий элемент повторяется 0 или более раз

		' Ищем >>12342
		Dim reg As Regex = New Regex("&gt;&gt;(?<messagenumber>[\d]+)")
		Dim intMessageNumber As Long
		For Each m As Match In reg.Matches(strMessage)
			intMessageNumber = ParseString(m.Groups("messagenumber").ToString)
			If intMessageNumber > 0 Then
				objAnswers.AddAnswer(BoardName, intMessageNumber)
				strMessage = reg.Replace(strMessage, "<a><external>0</external><board>" & BoardName & "</board><thread>${messagenumber}</thread></a>", 1)
			End If
		Next
		' Ищем >>b/1142 или >>/b/634634
		reg = New Regex("&gt;&gt;/?(?<boardname>\S+)/(?<messagenumber>[\d]+)")
		For Each m As Match In reg.Matches(strMessage)
			intMessageNumber = ParseString(m.Groups("messagenumber").ToString)
			If intMessageNumber > 0 Then
				Dim strBoardName As String = m.Groups("boardname").ToString
				objAnswers.AddAnswer(strBoardName, intMessageNumber)
				strMessage = reg.Replace(strMessage, "<a><external>1</external><board>${boardname}</board><thread>${messagenumber}</thread></a>", 1)
			End If
		Next

		' Ищем >>/названиераздела/ и заменяем на ссылку
		reg = New Regex("(&gt;){2}/(?<boardname>\S+)/")
		For Each m As Match In reg.Matches(strMessage)
			Dim strBoardName As String = m.Groups("boardname").ToString
			' Существование доски не проверяем
			strMessage = reg.Replace(strMessage, "<a><external>1</external><board>${boardname}</board><thread>0</thread></a>", 1)
		Next

		objAnswers.MessageText = strMessage
		Return objAnswers
	End Function

	''' <summary>
	''' Конвертирует полученную от пользователя строку в число или возвращает 0 при невозможности
	''' </summary>
	''' <param name="strParse"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Shared Function ParseString(ByVal strParse As String) As Long
		Dim intNumber As Long
		Long.TryParse(strParse, intNumber)
		Return intNumber
	End Function

End Class
