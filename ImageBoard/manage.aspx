<%@ Page Language="VB" MasterPageFile="~/masterpage.master" AutoEventWireup="false" CodeFile="manage.aspx.vb" Inherits="internetscreator" Title="Управление" ContentType="application/xhtml+xml" %>
<%@ Register Assembly="WebControlCaptcha" Namespace="WebControlCaptcha" TagPrefix="cc1" %>
<asp:Content ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ContentPlaceHolderID="cont" runat="Server">
    <asp:LoginView runat="server">
        <AnonymousTemplate>
            <asp:Login runat="server" />
        </AnonymousTemplate>
        <LoggedInTemplate>
            <h2>
                Приветствую тебя, создатель интернетов!</h2>
            <p>
                Сохрани строку
                <%=m_UserName.ToString%>
                и не теряй. Используй её в качестве имени для входа.</p>
            <div>
            </div> 
            <asp:MultiView runat="server" ActiveViewIndex="0">
                <asp:View runat="server" ID="View1">
                    <asp:Button runat="server" CommandArgument="View2" CommandName="SwitchViewByID" Text="Вперёд" />
                    <asp:ChangePassword runat="server" />
                </asp:View>
                <asp:View runat="server" ID="View2">
                    <asp:Button runat="server" CommandArgument="View1" CommandName="SwitchViewByID" Text="Назад" />
                    <asp:Button runat="server" CommandArgument="View2" CommandName="SwitchViewByID" Text="Вперёд" />
                    <p>F[f[f[1</p>
                </asp:View>
                
            </asp:MultiView>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
