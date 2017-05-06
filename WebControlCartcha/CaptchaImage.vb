Imports System
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

''' <summary>
''' CAPTCHA image generation class
''' </summary>
''' <remarks>
''' Adapted from the excellent code at 
''' http://www.codeproject.com/aspnet/CaptchaImage.asp
'''
''' Jeff Atwood
''' http://www.codinghorror.com/
''' </remarks>
Public Class CaptchaImage

	Private _height As Integer
	Private _width As Integer
	Private _rand As Random
	Private m_datCaptchaImageGenerated As DateTime
	Private _randomText As String
	Private _randomTextLength As Integer
	Private _randomTextChars As String
	Private _fontFamilyName As String
	Private _fontWarp As FontWarpFactor
	Private _backgroundNoise As BackgroundNoiseLevel
	Private _lineNoise As LineNoiseLevel
	Private _guid As String
	Private _fontWhitelist As String
	Private _TextColored As Boolean


#Region "Public Enums"

	''' <summary>
	''' Amount of random font warping to apply to rendered text
	''' </summary>
	Public Enum FontWarpFactor
		None
		Low
		Medium
		High
		Extreme
	End Enum

	''' <summary>
	''' Amount of background noise to add to rendered image
	''' </summary>
	Public Enum BackgroundNoiseLevel
		None
		Low
		Medium
		High
		Extreme
	End Enum

	''' <summary>
	''' Amount of curved line noise to add to rendered image
	''' </summary>
	Public Enum LineNoiseLevel
		None
		Low
		Medium
		High
		Extreme
	End Enum

#End Region

#Region "Public Properties"

	''' <summary>
	''' Returns a GUID that uniquely identifies this Captcha
	''' </summary>
	Public ReadOnly Property UniqueId() As String
		Get
			Return _guid
		End Get
	End Property

	''' <summary>
	''' Returns the date and time this image was last rendered
	''' </summary>
	Public ReadOnly Property RenderedAt() As DateTime
		Get
			Return m_datCaptchaImageGenerated
		End Get
	End Property

	''' <summary>
	''' Font family to use when drawing the Captcha text. If no font is provided, a random font will be chosen from the font whitelist for each character.
	''' </summary>
	Public Property Font() As String
		Get
			Return _fontFamilyName
		End Get
		Set(ByVal Value As String)
			Try
				Dim font1 As Font = New Font(Value, 12.0!)
				_fontFamilyName = Value
				font1.Dispose()
			Catch ex As Exception
				_fontFamilyName = Drawing.FontFamily.GenericSerif.Name
			End Try
		End Set
	End Property

	''' <summary>
	''' Amount of random warping to apply to the Captcha text.
	''' </summary>
	Public Property FontWarp() As FontWarpFactor
		Get
			Return _fontWarp
		End Get
		Set(ByVal Value As FontWarpFactor)
			_fontWarp = Value
		End Set
	End Property

	''' <summary>
	''' Amount of background noise to apply to the Captcha image.
	''' </summary>
	Public Property BackgroundNoise() As BackgroundNoiseLevel
		Get
			Return _backgroundNoise
		End Get
		Set(ByVal Value As BackgroundNoiseLevel)
			_backgroundNoise = Value
		End Set
	End Property

	Public Property LineNoise() As LineNoiseLevel
		Get
			Return _lineNoise
		End Get
		Set(ByVal value As LineNoiseLevel)
			_lineNoise = value
		End Set
	End Property

	''' <summary>
	''' Âîçâğàùàåò/óñòàíàâëèâàåò öâåòíîé òåêñò
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property TextColored() As Boolean
		Get
			Return _TextColored
		End Get
		Set(ByVal value As Boolean)
			_TextColored = value
		End Set
	End Property

	''' <summary>
	''' Ñòğîêà äîïóñòèìûõ ñèìâîëîâ äëÿ èñïîëüçîâàíèÿ â êàï÷å.
	''' Ïğè ãåíåğàöèè êàï÷è áóäóò èñïîëüçîâàíû ñëó÷àéíûå ñèìâîëû èç ıòîé ñòğîêè
	''' </summary>
	Public Property CaptchaChars() As String
		Get
			Return _randomTextChars
		End Get
		Set(ByVal Value As String)
			_randomTextChars = Value
			_randomText = GenerateRandomText()
		End Set
	End Property

	''' <summary>
	''' Êîëè÷åñòâî ñèìâîëîâ, èñïîëüçóåìûõ äëÿ ãåíåğàöèè êàï÷è
	''' </summary>
	Public Property TextLength() As Integer
		Get
			Return _randomTextLength
		End Get
		Set(ByVal Value As Integer)
			_randomTextLength = Value
			_randomText = GenerateRandomText()
		End Set
	End Property

	''' <summary>
	''' Âîçâğàùàåò òåêñò ñãåíåğèğîâàííîé êàï÷è
	''' </summary>
	Public ReadOnly Property [Text]() As String
		Get
			Return _randomText
		End Get
	End Property

	''' <summary>
	''' Width of Captcha image to generate, in pixels 
	''' </summary>
	Public Property Width() As Integer
		Get
			Return _width
		End Get
		Set(ByVal Value As Integer)
			If (Value <= 60) Then
				Throw New ArgumentOutOfRangeException("width", Value, "width must be greater than 60.")
			End If
			_width = Value
		End Set
	End Property

	''' <summary>
	''' Height of Captcha image to generate, in pixels 
	''' </summary>
	Public Property Height() As Integer
		Get
			Return _height
		End Get
		Set(ByVal Value As Integer)
			If Value <= 30 Then
				Throw New ArgumentOutOfRangeException("height", Value, "height must be greater than 30.")
			End If
			_height = Value
		End Set
	End Property

	''' <summary>
	''' A semicolon-delimited list of valid fonts to use when no font is provided.
	''' </summary>
	Public Property FontWhitelist() As String
		Get
			Return _fontWhitelist
		End Get
		Set(ByVal value As String)
			_fontWhitelist = value
		End Set
	End Property

#End Region

	Public Sub New()
		_rand = New Random
		_fontWarp = FontWarpFactor.Low
		_backgroundNoise = BackgroundNoiseLevel.Low
		_lineNoise = LineNoiseLevel.None
		_width = 180
		_height = 50
		_randomTextLength = 5
		'_randomTextChars = "ACDEFGHJKLNPQRTUVXYZ2346789"
		_randomTextChars = ",.:;""§…®©«»±?¹~@#$%^&*+-{}[]=><ÉÖÓÊÅÍÃØÙÇÕÔÛÂÀÏĞËÎÄÆİß×ÑÌÈÒÜÁŞÚ¨12456789QWRYUSDFGJLZVNM"
		_fontFamilyName = ""
		' -- a list of known good fonts in on both Windows XP and Windows Server 2003
		_fontWhitelist = _
		 "arial;arial black;comic sans ms;courier new;estrangelo edessa;franklin gothic medium;" & _
		 "georgia;lucida console;lucida sans unicode;mangal;microsoft sans serif;palatino linotype;" & _
		 "sylfaen;tahoma;times new roman;trebuchet ms;verdana"
		GenerateNewCaptcha()
	End Sub

	''' <summary>
	''' Ñîçäà¸ò íîâóş êàï÷ó
	''' </summary>
	''' <remarks></remarks>
	Friend Sub GenerateNewCaptcha()
		_randomText = GenerateRandomText()
		m_datCaptchaImageGenerated = DateTime.Now
		_guid = Guid.NewGuid.ToString()
	End Sub

	''' <summary>
	''' Forces a new Captcha image to be generated using current property value settings.
	''' </summary>
	Public Function RenderImage() As Bitmap
		Return GenerateImage(False)
	End Function

	''' <summary>
	''' Returns a random font family from the font whitelist
	''' </summary>
	Private Function RandomFontFamily() As String
		Static ff() As String
		'-- small optimization so we don't have to split for each char
		If ff Is Nothing Then
			ff = _fontWhitelist.Split(";"c)
		End If
		Return ff(_rand.Next(0, ff.Length))
	End Function

	''' <summary>
	''' generate random text for the CAPTCHA
	''' </summary>
	Private Function GenerateRandomText(Optional ByVal Max1 As Integer = 100) As String
		Dim sb As StringBuilder = New StringBuilder(_randomTextLength)
		Dim maxLength As Integer = _randomTextChars.Length
		If _rand.Next(Max1) > 97 Then
			Dim strMass() As String = {">146%", "ØÈNDÀ", "ÒÛÌ¨Ä", _
					 "ÁÈÄLÅ", "ÀĞÁÓİ", "GÓÂÍİ", _
					 "ÊÀÏ×À", "ÀËÎİ?", "×ÓØÊÀ", _
					 "ÄÂÀ×N", "ÏÀÊÅÒ", "2×ÀŞ?", _
					 "ÒËÑÒÎ", "ÒĞËËÜ", "ßNÄÊÑ", _
					 "=È2Ï?", "RÒÓÒ?", "ßÑÇÀÄ", _
					 "ÑËÛØÓ", "NÅÂRÈ", "ßÆÈÂÉ", _
					 "ßÃÂĞŞ", "ÓÁÅÉÑ", "Õ@×ÈÊ", _
					 "ÃËÀÇÀ", "RÀØÊÀ", "RĞßÄÌ", _
					 "ÊĞÈÏÈ", "ÒÛNFÕ", "ÕÓÈÒÀ", _
					 "ÍÅÕ×Ó", "ÍÓÆFÍ", "ÕÓÉÖÛ", _
					 "ÒÛGFÉ", "ÅÒWÈÍ", "ÑÈÑÜÊ", _
					 "ÏFNÈÑ", "SÅÊÑ?", "AÕÀÕÀ", _
					 "ßÏĞÀÂ", "VØÊLÓ", "2+2=?", _
					 "ÌÀNÓË", "ÊÎÒİ?", "ßÃÂĞŞ", _
					 "ÏÄ×ÍÑ", "ÏÛÙ11", "UËÎË?", _
					 "U2CH?", "ÑÎÑÀ4", "ÒÈRÅ4", _
					 "UÇÎÉ?", "ÒYÍÅÖ", "E^5=?", _
					 "?ÄŞÆÅ", "»ÕÀÍŞ", "ÀÍÀËÛ", _
					 "ÏÈÏÅÖ", "ÒÛÃÅÉ", "ÑÎÑÀÒ", _
					 "ÕÀÉÊÓ", "1ÁßÊÀ", "VÊÑÎÌ", _
					 "ÑËÅÆÓ", "ÒÛÕYÉ", "NÎÊÈß", _
					 "ÌÀÒÀN", "ÓÏÎĞÒ", "VÁNÅÒ", _
					 "ÂÀÑÈÊ", "ÄÛËÄÎ", ">:3Ú…", _
					 "ÒĞÈÊÎ", "ÌÉÍÅÇ", "LSD??", _
					 "ÎĞÃNR", "ÓÏĞËÑ", "ØÒÀ?!", _
					 "ÃÄÅ?!", "×ÓÉÊÀ", "ÄÓĞÀ!", _
					 "ÊÓĞÈË", "ÁÓİİİ", ";ÙËß;", _
					 "ÂÎÍßË", "^ÌÛØÈ", "?ÀÍÎÍ", _
					 "ÓĞ×ÈÒ", "ÊÈØÊÈ", "ÂĞÀÒÜ"}
			Return strMass(Math.Min(_rand.Next(strMass.Length + 10), strMass.Length - 1))
		Else
			For n As Integer = 0 To _randomTextLength - 1
				sb.Append(_randomTextChars.Substring(_rand.Next(maxLength), 1))
			Next
			Return sb.ToString
		End If
	End Function

	''' <summary>
	''' Returns a random point within the specified x and y ranges
	''' </summary>
	Private Function RandomPoint(ByVal xmin As Integer, ByVal xmax As Integer, ByRef ymin As Integer, ByRef ymax As Integer) As PointF
		Return New PointF(_rand.Next(xmin, xmax), _rand.Next(ymin, ymax))
	End Function

	''' <summary>
	''' Returns a random point within the specified rectangle
	''' </summary>
	Private Function RandomPoint(ByVal rect As Rectangle) As PointF
		Return RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom)
	End Function

	''' <summary>
	''' Returns a GraphicsPath containing the specified string and font
	''' </summary>
	Private Function TextPath(ByVal s As String, ByVal f As Font, ByVal r As Rectangle) As GraphicsPath
		Dim sf As StringFormat = New StringFormat
		sf.Alignment = StringAlignment.Near
		sf.LineAlignment = StringAlignment.Near
		Dim gp As GraphicsPath = New GraphicsPath
		gp.AddString(s, f.FontFamily, CType(f.Style, Integer), f.Size, r, sf)
		Return gp
	End Function

	''' <summary>
	''' Returns the CAPTCHA font in an appropriate size 
	''' </summary>
	Private Function GetFont() As Font
		Dim fsize As Single
		Dim fname As String = _fontFamilyName
		If fname = "" Then
			fname = RandomFontFamily()
		End If
		Select Case Me.FontWarp
			Case FontWarpFactor.None
				fsize = Convert.ToInt32(_height * 0.7)
			Case FontWarpFactor.Low
				fsize = Convert.ToInt32(_height * 0.8)
			Case FontWarpFactor.Medium
				fsize = Convert.ToInt32(_height * 0.85)
			Case FontWarpFactor.High
				fsize = Convert.ToInt32(_height * 0.9)
			Case FontWarpFactor.Extreme
				fsize = Convert.ToInt32(_height * 0.95)
		End Select
		Return New Font(fname, fsize, FontStyle.Bold)
	End Function

	''' <summary>
	''' Renders the CAPTCHA image
	''' </summary>
	''' <param name="MagicCaptcha">Ãåíåğèğóåò êàï÷ó-âåäóíüş</param>
	Friend Function GenerateImage(ByVal MagicCaptcha As Boolean) As Bitmap
		Dim rect As Rectangle = New Rectangle(0, 0, _width, _height)
		Dim br As Brush = New LinearGradientBrush(rect, GenerateColor(True), GenerateColor(True), _rand.Next(4))
		Dim bmp As Bitmap = New Bitmap(_width, _height)
		Dim gr As Graphics = Graphics.FromImage(bmp)
		gr.SmoothingMode = SmoothingMode.HighQuality
		' Çàïîëíåíèå ïóñòîãî ïğîñòğàíñòâà
		gr.FillRectangle(br, rect)
		br.Dispose()

		Dim charOffset As Integer = 0
		Dim charWidth As Double = _width / _randomTextLength
		' Äîáàâëåíèå øóìà
		AddNoise(gr, rect)

		Dim sb As StringBuilder
		If MagicCaptcha Then
			sb = New StringBuilder(GenerateRandomText(555))
		Else
			sb = New StringBuilder(_randomText)
		End If
		For i As Integer = 0 To sb.Length - 1
			Dim CurrentChar As Char = sb.Chars(i)
			'-- establish font and draw area
			Dim fnt As Font = GetFont()
			Dim rectChar As Rectangle = New Rectangle(Convert.ToInt32(charOffset * charWidth), 0, Convert.ToInt32(charWidth), _height)

			'-- warp the character
			Dim gp As GraphicsPath = TextPath(CurrentChar, fnt, rectChar)
			'
			WarpText(gp, rectChar)

			'-- draw the character
			Dim brFore As LinearGradientBrush
			If _TextColored Then
				brFore = New LinearGradientBrush(rectChar, GenerateColor(False), GenerateColor(False), _rand.Next(4))
			Else
				brFore = New LinearGradientBrush(rectChar, Color.Black, Color.Black, _rand.Next(4))
			End If
			'If CanRotateChar(c) Then
			'	'gr.RotateTransform(90)
			'	'gr.TranslateTransform(rectChar.Height, -rectChar.Width)
			'	'Dim m As Matrix = New Matrix
			'	'm.Translate(1, -1)

			'	'gr.MultiplyTransform(m)

			'	gr.FillPath(brFore, gp)
			'	'm.Translate(-1, 1)
			'	'gr.MultiplyTransform(m)
			'	'gr.TranslateTransform(-rectChar.Height, rectChar.Width)
			'	'gr.RotateTransform(-90)
			'Else
			'	gr.FillPath(brFore, gp)
			'End If
			gr.FillPath(brFore, gp)

			charOffset += 1
			' Î÷èñòêà
			gp.Dispose()
			brFore.Dispose()
			fnt.Dispose()
		Next

		AddNoise(gr, rect)
		AddLine(gr, rect)

		'-- clean up unmanaged resources
		gr.Dispose()

		Return bmp
	End Function

	''' <summary>
	''' Ãåíåğèğóåò ñëó÷àéíûé öâåò
	''' </summary>
	''' <param name="Back">Öâåò ôîíà</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function GenerateColor(ByVal Back As Boolean) As Color
		If Back Then
			Return Color.FromArgb(_rand.Next(100), _rand.Next(100, 255), _rand.Next(100, 255), _rand.Next(100, 255))
		Else
			Return Color.FromArgb(_rand.Next(10, 255), _rand.Next(255), _rand.Next(255), _rand.Next(255))
		End If
	End Function

	Private Function CanRotateChar(ByVal c As Char) As Boolean
		For Each tmpChar As Char In "®©@ÉÖÅØÙÛÂÀÏĞËÄß×ÌÒÜÁŞÚ¨4FGQRY"
			If c = tmpChar Then
				Return True
			End If
		Next
		Return False
	End Function

	''' <summary>
	''' Warp the provided text GraphicsPath by a variable amount
	''' </summary>
	Private Sub WarpText(ByVal textPath As GraphicsPath, ByVal rect As Rectangle)
		Dim WarpDivisor As Single
		Dim RangeModifier As Single

		Select Case _fontWarp
			Case FontWarpFactor.None
				Return
			Case FontWarpFactor.Low
				WarpDivisor = 6
				RangeModifier = 1
			Case FontWarpFactor.Medium
				WarpDivisor = 5
				RangeModifier = 1.3
			Case FontWarpFactor.High
				WarpDivisor = 4.5
				RangeModifier = 1.4
			Case FontWarpFactor.Extreme
				WarpDivisor = 4
				RangeModifier = 1.5
		End Select

		Dim rectF As RectangleF
		rectF = New RectangleF(Convert.ToSingle(rect.Left), 0, Convert.ToSingle(rect.Width), rect.Height)

		Dim hrange As Integer = Convert.ToInt32(rect.Height / WarpDivisor)
		Dim wrange As Integer = Convert.ToInt32(rect.Width / WarpDivisor)
		Dim left As Integer = rect.Left - Convert.ToInt32(wrange * RangeModifier)
		Dim top As Integer = rect.Top - Convert.ToInt32(hrange * RangeModifier)
		Dim width As Integer = rect.Left + rect.Width + Convert.ToInt32(wrange * RangeModifier)
		Dim height As Integer = rect.Top + rect.Height + Convert.ToInt32(hrange * RangeModifier)

		If left < 0 Then left = 0
		If top < 0 Then top = 0
		If width > Me.Width Then width = Me.Width
		If height > Me.Height Then height = Me.Height

		Dim leftTop As PointF = RandomPoint(left, left + wrange, top, top + hrange)
		Dim rightTop As PointF = RandomPoint(width - wrange, width, top, top + hrange)
		Dim leftBottom As PointF = RandomPoint(left, left + wrange, height - hrange, height)
		Dim rightBottom As PointF = RandomPoint(width - wrange, width, height - hrange, height)

		Dim points As PointF() = New PointF() {leftTop, rightTop, leftBottom, rightBottom}
		Dim m As New Matrix
		m.Translate(0, 0)
		textPath.Warp(points, rectF, m, WarpMode.Perspective, 0)
	End Sub

	''' <summary>
	''' Add a variable level of graphic noise to the image
	''' </summary>
	Private Sub AddNoise(ByVal graphics1 As Graphics, ByVal rect As Rectangle)
		Dim density As Integer
		Dim size As Integer

		Select Case _backgroundNoise
			Case BackgroundNoiseLevel.None
				Return
			Case BackgroundNoiseLevel.Low
				density = 30
				size = 40
			Case BackgroundNoiseLevel.Medium
				density = 18
				size = 40
			Case BackgroundNoiseLevel.High
				density = 16
				size = 39
			Case BackgroundNoiseLevel.Extreme
				density = 12
				size = 38
		End Select

		Dim max As Integer = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / size)

		For i As Integer = 0 To Convert.ToInt32((rect.Width * rect.Height) / density)
			Dim br As SolidBrush = New SolidBrush(GenerateColor(False))
			graphics1.FillEllipse(br, _rand.Next(rect.Width), _rand.Next(rect.Height), _
			 _rand.Next(max), _rand.Next(max))
			br.Dispose()
		Next
	End Sub

	''' <summary>
	''' Add variable level of curved lines to the image
	''' </summary>
	Private Sub AddLine(ByVal g As Graphics, ByVal rect As Rectangle)

		Dim length As Integer
		Dim width As Single
		Dim linecount As Integer

		Select Case _lineNoise
			Case LineNoiseLevel.None
				Return
			Case LineNoiseLevel.Low
				length = 4
				width = Convert.ToSingle(_height / 31.25) ' 1.6
				linecount = 1
			Case LineNoiseLevel.Medium
				length = 5
				width = Convert.ToSingle(_height / 27.7777)	' 1.8
				linecount = 1
			Case LineNoiseLevel.High
				length = 3
				width = Convert.ToSingle(_height / 25) ' 2.0
				linecount = 2
			Case LineNoiseLevel.Extreme
				length = 3
				width = Convert.ToSingle(_height / 22.7272)	' 2.2
				linecount = 3
		End Select

		Dim pf(length) As PointF

		For l As Integer = 1 To linecount
			For i As Integer = 0 To length
				pf(i) = RandomPoint(rect)
			Next
			'Dim p As Pen = New Pen(GenerateColor(False), width)
			'g.DrawCurve(p, pf, 1.75)
			'p.Dispose()
			Dim gp As GraphicsPath = New GraphicsPath
			Dim br As LinearGradientBrush = New LinearGradientBrush(rect, GenerateColor(False), GenerateColor(False), _rand.Next(4))
			gp.AddCurve(pf, 1.75)
			g.FillPath(br, gp)
			br.Dispose()
			gp.Dispose()
		Next

	End Sub

End Class