<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Automation_Controls_AutomationMenu"  Codebehind="AutomationMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:PlaceHolder ID="plcMenu" runat="server">
    <cms:CMSPanel ID="pnlContainer" ShortID="pC" CssClass="cms-edit-menu" runat="server">
        <asp:Panel ID="pnlMenu" runat="server">
            <cms:HeaderActions ID="menu" runat="server" />
        </asp:Panel>
        <asp:Label runat="server" ID="lblInfo" Visible="false" EnableViewState="false" />
        <div class="Clear">
        </div>
    </cms:CMSPanel>
    <cms:CMSDocumentPanel ID="pnlDoc" runat="server" ShortID="pD" />
    <asp:HiddenField ID="hdnParam" runat="server" EnableViewState="false" />
</asp:PlaceHolder>
