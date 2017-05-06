<%@ Page Language="VB" MasterPageFile="~/masterpage.master" AutoEventWireup="false"
    CodeFile="settings.aspx.vb" Inherits="settings" Title="Настройки" ContentType="application/xhtml+xml" %>

<asp:Content ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ContentPlaceHolderID="cont" runat="Server">
    <div class="theader">
        Настройки</div>
    <div class="postarea">
        <div class="postform">
            <div class="postblock">
                Выбери нужные пункты из списков</div>
            <div class="postbuttons">
                <input type="submit" class="but" /></div>
        </div>
        <div class="postform">
            <div class="postblock">
                Стиль</div>
            <div class="postbuttons">
                <select name="lstTheme" id="lstTheme" class="but">
                    <option value="0" selected="selected">По умолчанию</option>
                    <%--<option value="1">Буричан</option>
                    <option value="2">Футаба</option>
                    <option value="3">Гурочан</option>
                    <option value="4">Мюон</option>--%>
                    <option value="5">Нейтрон</option>
                    <option value="6">Фотон</option>
                    <%--<option value="7">Электрон</option>
                    <option value="8">Позитрон</option>--%>
                    <%--<option value="8">Глюон</option>
    <option value="9">W-бозон</option>
    <option value="10">Z-бозон</option>--%>
                </select></div>
        </div>
        <div class="postform">
            <div class="postblock">
                Скрыть левый «фрейм»</div>
            <div class="postbuttons">
                <asp:DropDownList ID="lstHideLeftFrame" runat="server" CssClass="but">
                    <asp:ListItem Value="0">По умолчанию</asp:ListItem>
                    <asp:ListItem Value="1">Да</asp:ListItem>
                    <asp:ListItem Value="2">Нет</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <%--<tr><td class="postblock">Интерфейс</td><td><select name="lstLanguage" id="lstLanguage" class="but">
    <option value="0" selected="selected">Выбрать</option>
    <option value="1">Русский</option>
    <option value="2">Дореволюціонный боярскій</option>
    </select> </td></tr>--%>
        <div class="postform">
            <div class="postblock">
                После ответа вернуться</div>
            <div class="postbuttons">
                <asp:DropDownList runat="server" ID="lstMoveToThread" CssClass="but">
                    <asp:ListItem Value="0">Указывается в треде</asp:ListItem>
                    <asp:ListItem Value="1">К разделу</asp:ListItem>
                    <asp:ListItem Value="2">К нити</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="postform">
            <div class="postblock">
                Форма ответа</div>
            <div class="postbuttons">
                <asp:DropDownList ID="lstPostFormPosition" CssClass="but" runat="server">
                    <asp:ListItem Value="0">По умолчанию</asp:ListItem>
                    <asp:ListItem Value="1">Сверху</asp:ListItem>
                    <asp:ListItem Value="2">Снизу</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="postform">
            <div class="postblock">
                Скрыть поля имя/джаббер в форме</div>
            <div class="postbuttons">
                <asp:DropDownList runat="server" ID="lstHideUserNameJabberTheme" CssClass="but">
                    <asp:ListItem Value="0">По умолчанию</asp:ListItem>
                    <asp:ListItem Value="1">Нет</asp:ListItem>
                    <asp:ListItem Value="2">Да</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="postform">
            <div class="postblock">
                Показывать правила</div>
            <div class="postbuttons">
                <asp:DropDownList ID="lstShowRules" CssClass="but" runat="server">
                    <asp:ListItem Value="0">По умолчанию</asp:ListItem>
                    <asp:ListItem Value="1">Да</asp:ListItem>
                    <asp:ListItem Value="2">Нет</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="postform">
            <div class="postblock">
                Показывать NSFW</div>
            <div class="postbuttons">
                <asp:DropDownList runat="server" ID="lstShowNSFW" CssClass="but">
                    <asp:ListItem Value="6">Показывать всё</asp:ListItem>
                    <asp:ListItem Value="4">Не показывать 18+</asp:ListItem>
                    <asp:ListItem Value="3">Не показывать 16+</asp:ListItem>
                    <asp:ListItem Value="2">Не показывать 12+</asp:ListItem>
                    <asp:ListItem Value="1">Не показывать 6+</asp:ListItem>
                    <asp:ListItem Value="5">Не показывать NSFW</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
</asp:Content>
