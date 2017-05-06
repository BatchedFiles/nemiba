Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls


<DefaultProperty("Text"), ToolboxData("<{0}:PageNavigator runat=""server""></{0}:PageNavigator>")> _
Public Class PageNavigator
	Inherits Control

	Private m_PagesCount As Long
	Private m_CurrentPage As Long
	Private m_BoardName As String

	''' <summary>
	''' Возвращает/устанавливает количество страниц
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property PagesCount() As Long
		Get
			Return m_PagesCount
		End Get
		Set(ByVal value As Long)
			m_PagesCount = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает номер текущей страницы
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property CurrentPage() As Long
		Get
			Return m_CurrentPage
		End Get
		Set(ByVal value As Long)
			m_CurrentPage = value
		End Set
	End Property

	''' <summary>
	''' Возвращает/устанавливает имя раздела
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property BoardName() As String
		Get
			Return m_BoardName
		End Get
		Set(ByVal value As String)
			m_BoardName = value
		End Set
	End Property

	Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
		' Как выглядит навигация?
		' [0] [1] [2] [3] [4] и так далее до конца списка
		' У нас есть количество тредов
		writer.AddAttribute(HtmlTextWriterAttribute.Class, "pagenavigator")
		writer.RenderBeginTag(HtmlTextWriterTag.Div)

		If m_PagesCount = 0 Then
			writer.Write("[0]")
		Else
			For intPageNumber As Long = 0 To m_PagesCount
				If intPageNumber = m_CurrentPage Then
					writer.Write(" [" & (intPageNumber).ToString & "]")
				Else
					writer.Write(HtmlTextWriter.SpaceChar)
					writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl("~/" & m_BoardName & "/" & (intPageNumber).ToString & ".aspx"))
					writer.AddAttribute(HtmlTextWriterAttribute.Title, "[" & (intPageNumber).ToString & "]")
					writer.RenderBeginTag(HtmlTextWriterTag.A)
					writer.Write("[" & (intPageNumber).ToString & "]")
					writer.RenderEndTag()
				End If
			Next intPageNumber
		End If
		writer.RenderEndTag()
	End Sub

	Public Sub New()
		m_BoardName = String.Empty
	End Sub
End Class
