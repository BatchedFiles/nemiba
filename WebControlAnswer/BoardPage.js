// Подсчёт количества символов
function txtMessage_onkeyup(lblComment, lblSymbolsCount, txtComment, MaxMessageLength)
{
    lblComment.innerHTML = "<br />Символов: " + txtComment.value.length.toString() + "<br />";
    if (txtComment.value.length > MaxMessageLength) {
        lblSymbolsCount.innerHTML = "Комментарий слишком толстый";
    }
    else {
        lblSymbolsCount.innerHTML = "Комментарий";
    }
}
// Функция быстрого ответа. Параметры:
// Номер ответа
// Идентификатор скрытого поля с номером ответа
// Идентификатор текстового поля для вставки ответа
function QuickReply(ThreadNumber, txtThreadNumber, txtComment, lblAnswer)
{
    txtThreadNumber.value = ThreadNumber;
    if (txtComment.value)
        txtComment.value += "\n";
    txtComment.value += ">>" + ThreadNumber;
    txtComment.value += "\n";
    if (ThreadNumber) {
        lblAnswer.innerHTML = "Ответ в " + ThreadNumber;
    }
    else {
        lblAnswer.innerHTML = "Новая нить";
    }
}
// Разворачивание и сворачивание изображений
// imgImage — идентификатор изображения
// txtValue — массив, где хранятся значения
function ExpandImage(imgImage, txtValue)
{
    var tmpWidth = imgImage.width;
    var tmpHeight = imgImage.height;
    var tmpSrc = imgImage.src;
    var tmpMime = imgImage.type;
    imgImage.width = txtValue[0];
    imgImage.height = txtValue[1];
    imgImage.src = txtValue[2];
    txtValue[0] = tmpWidth;
    txtValue[1] = tmpHeight;
    txtValue[2] = tmpSrc;
}
// Создание окна-просмотра
function ShowToolTipPost(event, el, BoardName, MessageNumber, WebServiceSite)
{
	// Будущие координаты кохихтора
    var x = event.clientX + document.documentElement.scrollLeft;
    var y = event.clientY + document.documentElement.scrollTop;
	// Идентификатор кохихтора
    var divThreadId = "idKohihtorConversion" + BoardName + MessageNumber;
	// Ищем кохихтор, если найден, то меняем координаты
	// иначе просто создаём заново
    var divThread = document.getElementById(divThreadId);
	if (divThread)
	{
	}
	else
	{
	    divThread = document.createElement("div");
		// Обработчик закрытия окна просмотра
		//divThread.setAttribute("position", "fixed")
	    divThread.setAttribute("class", "reply");
	    divThread.setAttribute("onclick", "HideToolTipPost(this)");
	    divThread.setAttribute("id", divThreadId);
	    divThread.setAttribute("style", "position: absolute;left: " + x + "px;top: " + y + "px;");
		// Первое появление кохихтора
	    var strCohihtorHtml = "<p>Загружаю сообщение…</p>";
	    divThread.innerHTML = strCohihtorHtml;
		// Объект запроса к службе
	    var xmlhttp = getXmlHttp();
	    xmlhttp.open("POST", WebServiceSite, true);
	    xmlhttp.onreadystatechange = function () {
	        if (xmlhttp.readyState != 4) {
	            // Не удалось соединиться с сервером
	            divThread.innerHTML = "<p>Не смог соединиться с сервером.</p>";
	            return;
	        }
	        // очистить таймаут при наступлении readyState 4
	        clearTimeout(timeout);
	        if (xmlhttp.status == 200) {
	            // Все ок
	            var objResponse = xmlhttp.responseXML;
	            // Массив узлов
	            var chapters = objResponse.getElementsByTagName("GetPostHtmlResult");
	            // Текст первого узла массиве
	            divThread.removeAttribute("class");
	            divThread.innerHTML = chapters[0].textContent;
	        }
	        else {
	            divThread.setAttribute("class", "reply");
	            divThread.innerHTML = "<p>" + xmlhttp.status + " " + xmlhttp.statusText + "</p>";
	        }
	    };
		// Тело запроса
	    var SendData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
	    SendData += "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">";
	    SendData += "<soap12:Body><GetPostHtml xmlns=\"http://sethi.su/\">";
	    SendData += "<BoardName>" + BoardName + "</BoardName>";
	    SendData += "<MessageNumber>" + MessageNumber + "</MessageNumber>";
	    SendData += "</GetPostHtml></soap12:Body></soap12:Envelope>";
		
		// Заголовки запроса
	    xmlhttp.setRequestHeader("Content-Type", "application/soap+xml; charset=utf-8");
	    xmlhttp.setRequestHeader("Content-Length", SendData.length);
		// Отправка данных
	    xmlhttp.send(SendData);
		// Таймаут 60 секунд
	    var timeout = setTimeout(function () { xmlhttp.abort(); }, 60000);

	    var PARENT = document.forms["aspnetForm"];
		// пока элемент не отображается на странице
		// Добавляем созданный элемент к элементу PARENT
	    PARENT.appendChild(divThread);
	}
}
// Закрытие окна просмотра
function HideToolTipPost(el)
{
    el.parentNode.removeChild(el);
}
// Возвращает XmlHttpRequest
function getXmlHttp()
{
    var xmlhttp;
	try
	{
	    xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
	}
	catch(e)
	{
		try
		{
		    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (E)
		{
		    xmlhttp = false;
		}
	}
	if (!xmlhttp && typeof XMLHttpRequest!="undefined")
	{
	    xmlhttp = new XMLHttpRequest();
	}
	return xmlhttp;
}