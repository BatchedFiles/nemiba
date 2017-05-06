Imports System
Imports System.Web
Imports System.Drawing
Imports System.IO
Imports System.Globalization

''' <summary>
''' Captcha image stream HttpModule. Retrieves CAPTCHA objects from cache, renders them to memory, 
''' and streams them to the browser.
''' </summary>
''' <remarks>
''' You *MUST* enable this HttpHandler in your web.config, like so:
'''
'''	  &lt;httpHandlers&gt;
'''		  &lt;add verb="GET" path="CaptchaImage.aspx" type="WebControlCaptcha.CaptchaImageHandler, WebControlCaptcha" /&gt;
'''	  &lt;/httpHandlers&gt;
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
'''</remarks>
Public Class CaptchaImageHandler
	Implements IHttpHandler

	Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
		Dim app As HttpApplication = context.ApplicationInstance

		'-- get the unique GUID of the captcha; this must be passed in via the querystring
		Dim guid As String = app.Request.QueryString.Item("guid")
		Dim ci As CaptchaImage = Nothing

		If Not String.IsNullOrEmpty(guid) Then
			If String.IsNullOrEmpty(app.Request.QueryString.Item("s")) Then
				ci = CType(HttpRuntime.Cache.Get(guid), CaptchaImage)
			Else
				ci = CType(HttpContext.Current.Session.Item(guid), CaptchaImage)
			End If

		End If

		Using ms As New MemoryStream
			If ci Is Nothing Then
				app.Response.StatusCode = 410
				app.Response.ContentType = "application/xhtml+xml; charset=utf-8"
				app.Response.AddHeader("Content-Language", "ru-RU,ru")
				Dim xml = "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?><!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN"" ""http://www.w3.org/2002/04/xhtml-math-svg/xhtml-math-svg.dtd""><html version=""-//W3C//DTD XHTML 1.1//EN"" xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.w3.org/1999/xhtml http://www.w3.org/MarkUp/SCHEMA/xhtml11.xsd""><head><meta name=""viewport"" content=""width=device-width, initial-scale=1"" /><title>Not Found</title></head><body><h1>Server Error in &quot;/&quot; Application</h1><h2>HTTP Error 404 - Not Found</h2><p>Файл не найден и не найдётся в будущем.</p></body></html>"
				Dim bytes = System.Text.Encoding.UTF8.GetBytes(xml)
				ms.Write(bytes, 0, bytes.Length)
			Else
				Using b = ci.RenderImage
					b.Save(ms, Drawing.Imaging.ImageFormat.Jpeg)
				End Using
				app.Response.StatusCode = 200
				app.Response.ContentType = "image/jpeg"
				Dim dCurrent = Date.Now
				app.Response.AddHeader("Last-Modified", dCurrent.ToUniversalTime.ToString("R", DateTimeFormatInfo.InvariantInfo))
				app.Response.AddHeader("ETag", """" & dCurrent.ToBinary.ToString & """")
				app.Response.AddHeader("Expires", dCurrent.ToUniversalTime.AddSeconds(120).ToString("R", DateTimeFormatInfo.InvariantInfo))
				app.Response.AddHeader("Cache-Control", "public, max-age=120")
			End If
			ms.Seek(0, SeekOrigin.Begin)
			ms.WriteTo(app.Context.Response.OutputStream)
		End Using
		context.ApplicationInstance.CompleteRequest()
	End Sub

	Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
		Get
			Return True
		End Get
	End Property

End Class