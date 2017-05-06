<?xml version="1.0" encoding="UTF-8" standalone="no" ?>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
xsi:schemalocation="http://www.w3.org/1999/xhtml http://www.w3.org/MarkUp/SCHEMA/xhtml11.xsd"
xmlns:svg="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">

<%@ page language="VB" autoeventwireup="false" codefile="api.aspx.vb" inherits="WebAPIHelpGenerator"
     contenttype="application/xhtml+xml" %>

<%@ register assembly="WebImageBoardControls" namespace="WebImageBoardControls" tagprefix="cc1" %>
<head runat="server">
    <title>Управление имиджбордой</title>
</head>
<body>
    <form runat="server" enctype="multipart/form-data">
    <h1>
        NemibaWebAPI</h1>
    <h2>
        WebAPI к Немибе</h2>
        <p><%=FullName %></p>
        <p><%= Types2   %></p>
        <p><%= Methods    %></p>
    <p>
        Следующие операции поддерживаются. Формальное определение смотри в <a href="/webapi.asmx?WSDL">
            Описании службы</a>.</p>
    <asp:LoginView runat="server">
        <RoleGroups>
            <asp:RoleGroup Roles="Зой">
                <ContentTemplate>
                    <div id="content">
                        <ul>
                            <li><a href="webapi.asmx?op=AddBoardToFlag">AddBoardToFlag</a> <span>
                                <br />
                                Добавление раздела к флагу </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=AddFlagToGroup">AddFlagToGroup</a> <span>
                                <br />
                                Добавление флага в группу </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=AnswerToThread">AnswerToThread</a> <span>
                                <br />
                                Ответ в тред </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CheckMessage">CheckMessage</a> <span>
                                <br />
                                Отметка сообщения как проверенного </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateBadWord">CreateBadWord</a> <span>
                                <br />
                                Создаёт шаблон фильтра </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateBan">CreateBan</a> <span>
                                <br />
                                Пломбирует адрес </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateBanFromMessageNumber">CreateBanFromMessageNumber</a>
                                <span>
                                    <br />
                                    Пломбирует адрес по номеру сообщения </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateBoard">CreateBoard</a> <span>
                                <br />
                                Создаёт раздел </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateDenyGroupForAnswers">CreateDenyGroupForAnswers</a>
                                <span>
                                    <br />
                                    Добавляет группу в запрещённые для ответов на разделе </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateDenyGroupForFiles">CreateDenyGroupForFiles</a> <span>
                                <br />
                                Добавляет группу в список запрещённых для отправки файлов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateGroup">CreateGroup</a> <span>
                                <br />
                                Создание группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateInvite">CreateInvite</a> <span>
                                <br />
                                Создаёт инвайт пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateMenu">CreateMenu</a> <span>
                                <br />
                                Создаёт пункт меню </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateMime">CreateMime</a> <span>
                                <br />
                                Регистрирует тип файла </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateRules">CreateRules</a> <span>
                                <br />
                                Создаёт правило раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=CreateThread">CreateThread</a> <span>
                                <br />
                                Создание треда </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=Create%d0%a1omplaint">CreateСomplaint</a> <span>
                                <br />
                                Отправка жалобы на сообщение </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditBadWord">EditBadWord</a> <span>
                                <br />
                                Изменяет шаблон фильтра </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditBoard">EditBoard</a> <span>
                                <br />
                                Изменяет параметры раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditBoardParameter">EditBoardParameter</a> <span>
                                <br />
                                Изменяет параметр раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditDenyForAnswer">EditDenyForAnswer</a> <span>
                                <br />
                                Добавление группы в запрещённые для ответа в тред </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditDenyGroupForAnswers">EditDenyGroupForAnswers</a> <span>
                                <br />
                                Изменяет группу из запрещённых для ответов на разделе </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditGroupEnabled">EditGroupEnabled</a> <span>
                                <br />
                                Блокировка группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditMenu">EditMenu</a> <span>
                                <br />
                                Изменяет меню </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditMime">EditMime</a> <span>
                                <br />
                                Изменяет зарегистрированный тип файла </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditRegisterEnabled">EditRegisterEnabled</a> <span>
                                <br />
                                Установка/снятие запрета саморегистрации пользователей </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditRules">EditRules</a> <span>
                                <br />
                                Изменяет правило раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditThreadTopFlag">EditThreadTopFlag</a> <span>
                                <br />
                                Редактирование закреплённости треда </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditUserEnabled">EditUserEnabled</a> <span>
                                <br />
                                Блокировка/разброкировка пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=EditVCard">EditVCard</a> <span>
                                <br />
                                Редактирование VCard </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetAnswerList">GetAnswerList</a> <span>
                                <br />
                                Возвращает список ответов по номеру собщения </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetArchiveThreadList">GetArchiveThreadList</a> <span>
                                <br />
                                Получение списка архивных тредов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetBadWordList">GetBadWordList</a> <span>
                                <br />
                                Получает список шаблонов фильтра </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetBanList">GetBanList</a> <span>
                                <br />
                                Получает список запломбированных адресов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetBoardInfo">GetBoardInfo</a> <span>
                                <br />
                                Получает параметры раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetBoardInfoList">GetBoardInfoList</a> <span>
                                <br />
                                Получает список разделов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetBoardListFromFlag">GetBoardListFromFlag</a> <span>
                                <br />
                                Получение списка разделов флага </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetDenyForAnswerList">GetDenyForAnswerList</a> <span>
                                <br />
                                Получение списка групп, запрещённых для ответа в тред </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetDenyGroupForAnswersList">GetDenyGroupForAnswersList</a>
                                <span>
                                    <br />
                                    Получает список запрещённых групп для ответов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetDenyGroupForFilesList">GetDenyGroupForFilesList</a> <span>
                                <br />
                                Получает список групп, запрещённых для отправки файлов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetFileList">GetFileList</a> <span>
                                <br />
                                Возвращает список файлов по номеру сообщения </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetGroupFlagList">GetGroupFlagList</a> <span>
                                <br />
                                Получение списка флагов группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetGroupList">GetGroupList</a> <span>
                                <br />
                                Получает список групп </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetMenuList">GetMenuList</a> <span>
                                <br />
                                Получает список пунктов меню </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetMessage">GetMessage</a> <span>
                                <br />
                                Возвращает сообщение со списком ответов и файлов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetMessageList">GetMessageList</a> <span>
                                <br />
                                Возвращает список всех сообщений в треде </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetMessageStartAtList">GetMessageStartAtList</a> <span>
                                <br />
                                Возвращает список всех сообщений в треде начиная с некоторого номера сообщения </span>
                            </li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetMimeList">GetMimeList</a> <span>
                                <br />
                                Получает список зарегистрированных типов файлов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetNextMessageNumber">GetNextMessageNumber</a> <span>
                                <br />
                                Возвращает номер следующего сообщения на разделе </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetPagesCount">GetPagesCount</a> <span>
                                <br />
                                Возвращает количество страниц на разделе </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetPostHtml">GetPostHtml</a> <span>
                                <br />
                                Возвращает сообщение с разметкой в Html </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetRulesList">GetRulesList</a> <span>
                                <br />
                                Получает список правил раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetThreadList">GetThreadList</a> <span>
                                <br />
                                Возвращает список тредов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetUserGroupList">GetUserGroupList</a> <span>
                                <br />
                                Получает список групп пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetUserList">GetUserList</a> <span>
                                <br />
                                Получает список пользователей </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetUserListFromGroup">GetUserListFromGroup</a> <span>
                                <br />
                                Получение списка пользователей группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=GetVCardList">GetVCardList</a> <span>
                                <br />
                                Получает VCard пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=Get%d0%a1omplaintList">GetСomplaintList</a> <span>
                                <br />
                                Получение списка жалоб раздела </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=Login">Login</a> <span>
                                <br />
                                Обеспечивает вход пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=MoveThread">MoveThread</a> <span>
                                <br />
                                Перенос треда на другой раздел </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RegisterUser">RegisterUser</a> <span>
                                <br />
                                Саморегистрация пользователя </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveArchiveThread">RemoveArchiveThread</a> <span>
                                <br />
                                Удаление треда из архива </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveBadWord">RemoveBadWord</a> <span>
                                <br />
                                Удаляет шаблон фильтра </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveBan">RemoveBan</a> <span>
                                <br />
                                Удаляет адрес из блокировок </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveBoard">RemoveBoard</a> <span>
                                <br />
                                Удаляет раздел </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveBoardFromFlag">RemoveBoardFromFlag</a> <span>
                                <br />
                                Удаление раздела из флага </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveDenyForAnswer">RemoveDenyForAnswer</a> <span>
                                <br />
                                Удаление группы из списка запрещённых для ответа в тред </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveDenyGroupForAnswers">RemoveDenyGroupForAnswers</a>
                                <span>
                                    <br />
                                    Удаляет группу из запрещённых для ответов на разделе </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveDenyGroupForFiles">RemoveDenyGroupForFiles</a> <span>
                                <br />
                                Удаляет группу из списка запрещённых для отправки файлов </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveFlagFromGroup">RemoveFlagFromGroup</a> <span>
                                <br />
                                Снятие флага с группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveGroup">RemoveGroup</a> <span>
                                <br />
                                Удаление группы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveMenu">RemoveMenu</a> <span>
                                <br />
                                Удаляет пункт меню </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveMessage">RemoveMessage</a> <span>
                                <br />
                                Удаление сообщения </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveMime">RemoveMime</a> <span>
                                <br />
                                Уладяет зарегистрированный тип файла </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=RemoveRules">RemoveRules</a> <span>
                                <br />
                                Удаляет правило раздела </span></li>
                            <p>
                            </p>
                            
                            <li><a href="webapi.asmx?op=RemoveVCard">RemoveVCard</a> <span>
                                <br />
                                Удаление VCard </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=Remove%d0%a1omplaint">RemoveСomplaint</a> <span>
                                <br />
                                Удаление жалобы </span></li>
                            <p>
                            </p>
                            <li><a href="webapi.asmx?op=SetNextMessageNumber">SetNextMessageNumber</a> <span>
                                <br />
                                Устанавливает номер следующего сообщения на разделе </span></li>
                            <p>
                            </p>
                        </ul>
                    </div>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Администратор">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Бот">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Выверяющ">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Досмот">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Кляузник">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Редактор">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Ролевик">
                <ContentTemplate>
                    <p>
                        <a href="webapi.asmx?op=AddUserToGroup">AddUserToGroup</a></p>
                    <p>
                        Добавление пользователя к группе</p>
                        <p><a href="webapi.asmx?op=RemoveUserFromGroup">RemoveUserFromGroup</a>
                        </p>
                        <p>Удаление пользователя из группы</p>
              </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Спамер">
                <ContentTemplate>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
    <p>
        <a href="webapi.asmx?op=UnregisterUser">UnregisterUser</a></p>
    <p>
        Удаляет пользователя</p>
    </form>
</body>
</html>
