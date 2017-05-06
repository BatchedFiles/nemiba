<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false"
    CodeFile="board.aspx.vb" Inherits="BoardPage" Title="Безымянная страница" EnableEventValidation="false"
    ContentType="application/xhtml+xml" %>

<%@ Register Assembly="WebImageBoardControls" Namespace="WebImageBoardControls" TagPrefix="uc" %>
<asp:Content ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ContentPlaceHolderID="cont" runat="Server">
    <h1>
        <%=DivLogo%></h1>
    <uc:BoardPage runat="server" ID="divThreads" />
    <asp:PlaceHolder ID="plhBottom" runat="server"></asp:PlaceHolder>
    <h2>
        Навигация</h2>
    <div class="navigationanddelete">
        <uc:PageNavigator runat="server" ID="divNavigate" />
    </div>
    <hr />
    <uc:Answer runat="server" ID="ucAnswer" CaptchaChars="<%$ AppSettings: CartchaChars %>" CreateNewThreadText="<%$ AppSettings: CreateNewThreadText %>" AnswerToThreadText="<%$ AppSettings: AnswerToThreadText %>" UserNameMustExists="<%$ AppSettings: UserNameMustExists %>" DefaultSubject="<%$ AppSettings: DefaultSubject %>" />
    <hr />
    <div class="userdelete" runat="server" id="tdDeletePost">
        <div>
            Удалить сообщения</div>
        <div>
            <input type="submit" class="but" /></div>
        <div>
            Пароль</div>
        <div>
            <input id="txtPostPassword" name="txtPostPassword" type="password" class="inp" value="<%=PostPassword%>" /></div>
        <div>
            <label>
                [<input name="chkFileOnly" id="chkFileOnly" value="on" type="checkbox" />Только
                файл(ы)]</label></div>
        <div>
            Отправить жалобу</div>
        <div>
            <input type="text" class="inp" name="txtReportReason" id="txtReportReason" /></div>
    </div>
</asp:Content>
