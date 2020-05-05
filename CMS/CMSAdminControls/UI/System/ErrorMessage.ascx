<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSAdminControls_UI_System_ErrorMessage"  Codebehind="ErrorMessage.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>

    <div class="PageBody">
        <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader">
            <cms:PageTitle ID="ptTitle" runat="server" />
        </asp:Panel>
        <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
            <asp:Label ID="lblMessage" runat="server" CssClass="ErrorLabel" />
        </asp:Panel>
    </div>