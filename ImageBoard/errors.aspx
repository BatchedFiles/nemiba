<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1 plus MathML 2.0 plus SVG 1.1//EN" "http://www.w3.org/2002/04/xhtml-math-svg/xhtml-math-svg.dtd" [<!ENTITY nbsp "&#160;">]>
<html version="-//W3C//DTD XHTML 1.1//EN" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.w3.org/1999/xhtml http://www.w3.org/MarkUp/SCHEMA/xhtml11.xsd">
<%@ Page Language="VB" AutoEventWireup="false" CodeFile="errors.aspx.vb" Inherits="errors" ContentType="application/xhtml+xml" %>
<head runat="server"><meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Nemiba - <%=ErrorString%></title>
<style type="text/css" id="divStyle" runat="server">
	body
		{
			background: #000099;
			color: #FFFFFF;
			font-family: monospace;
			font-size: larger;
		}
</style>
</head>
<body><div id="divError" runat="server">
<p>A problem has been detected and Nemiba has been shut down to prevent damage<br />
to your computer.</p>
<p><%=ErrorString%></p>
<p>If this is the first time you have seen this error screen,<br />
restart your browser. If this screen appears again, follow<br />
these steps:</p>
<p>Make sure that the page you requested is spelled properly.<br />
If this is the case, press Backspace to return to previous page.</p>
<p>If problems continue, send a message to hiddenchan.i2p/c/res/560.html</p>
<p>Technical information:</p>
<p>*** STOP: <%=ErrorCode%> (<%=ErrorCode1%>, <%=ErrorCode2%>, <%=ErrorCode3%>, <%=ErrorCode3%>)</p>
<p>
<br />
***&nbsp;&nbsp;&nbsp;<%=ErrorPageString%> - Address Nemiba base at, DateStamp <%=DateStamp%></p>
</div>
<div runat="server" id="divException" visible="false">
<h1>Ошибка сервера в приложении</h1>
<p>Немиба получила непойманное исключение при выполнении текущего запроса. Изучите трассировку стека для получения дополнительных сведений об исключении и выбросившем его коде.</p>
<h2>Сведения об исключении</h2>
<p><%=ErrorPageString %></p>
<h2>Трассировка стэка</h2>
<code><%=StackTrace %></code>
<h2>Исходный файл</h2>
<p><%= SouceFile%></p>
<h2>Метод, выбросивший исключение</h2>
<p><%=SourceFunction %></p>
</div>
</body>
</html>
