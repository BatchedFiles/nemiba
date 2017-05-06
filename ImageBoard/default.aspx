<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false"
    CodeFile="~/default.aspx.vb" Inherits="DefaultPage" Title="Безымянная страница"
    EnableEventValidation="false" EnableViewState="true" ContentType="application/xhtml+xml" %>

<%@ Register Assembly="WebImageBoardControls" Namespace="WebImageBoardControls" TagPrefix="uc" %>
<asp:Content ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ContentPlaceHolderID="cont" runat="Server">
    <h1>
        <%=DivLogo%></h1>
    <%--Ссылки навигации по глагне--%>
    <p>
        <span class="navhorbrakets">[ </span>
        <asp:hyperlink cssclass="navhorizontal" visible="<%$ AppSettings: EnableNewsLink %>"
            runat="server" NavigateUrl="~/?board=news"
            text="<%$ AppSettings: NewsLinkText %>"></asp:hyperlink>
            &#32;
        <asp:hyperlink cssclass="navhorizontal" visible="<%$ AppSettings: EnableFaqLink %>"
            runat="server" NavigateUrl="~/?board=faq"
            text="<%$ AppSettings: FaqLinkText %>"></asp:hyperlink>
            &#32;
        <asp:hyperlink cssclass="navhorizontal" visible="<%$ AppSettings: EnableRulesLink %>"
             runat="server" NavigateUrl="~/?board=rules"
            text="<%$ AppSettings: RulesLinkText %>"></asp:hyperlink>
            &#32;
        <asp:hyperlink cssclass="navhorizontal" visible="<%$ AppSettings: EnableTermsLink %>"
            runat="server" NavigateUrl="~/?board=terms"
            text="<%$ AppSettings: TermsLinkText %>"></asp:hyperlink>
        <span class="navhorbrakets"> ]</span>
    </p>
    <%--Логотип имиджборды--%>
    <svg:svg width="100" height="100">
        <svg:rect x="1" y="1" width="99" height="99" fill="none" />
    </svg:svg>
    <%--Девиз, слоган--%>
    <p>
        <asp:Literal runat="server" Text="<%$ AppSettings: SiteSlogan %>" /></p>
    <%--Форма создания тредов--%>
    <asp:LoginView runat="server" ID="kgkgkg">
        <RoleGroups>
            <asp:RoleGroup Roles="Зой, Редактор">
                <ContentTemplate>
                    <uc:Answer runat="server" ThreadMode="CreateThread" OnCaptchaNotValid="ucAnswer_CaptchaNotValid"
                        OnThreadCreateNeeded="ucAnswer_ThreadCreateNeeded" OnRegisterUserNeeded="ucAnswer_RegisterUserNeeded"
                        CaptchaChars="<%$ AppSettings: CartchaChars %>" />
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
    <%--Треды--%>
    <uc:BoardPage runat="server" ID="divThreads" />
    <%--Навигация и удаление тредов--%>
    <h2>
        Навигация</h2>
    <div class="navigationanddelete">
        <uc:PageNavigator runat="server" BoardName="<%=BoardName %>" ID="divNavigate" />
        <asp:LoginView runat="server">
            <RoleGroups>
                <asp:RoleGroup Roles="Редактор, Зой">
                    <ContentTemplate>
                        <div class="userdelete">
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
                    </ContentTemplate>
                </asp:RoleGroup>
            </RoleGroups>
        </asp:LoginView>
    </div>
</asp:Content>
