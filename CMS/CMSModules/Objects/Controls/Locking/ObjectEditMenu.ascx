<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Objects_Controls_Locking_ObjectEditMenu"
     Codebehind="ObjectEditMenu.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<cms:CMSPanel ID="pnlContainer" ShortID="pC" runat="server" CssClass="header-actions-container">
    <cms:HeaderActions ID="menu" runat="server" UseSmallIcons="true"  PanelCssClass="header-actions-main" />
    <asp:Panel ID="pnlRight" runat="server" CssClass="header-actions-additional-indented btn-actions">
        <asp:PlaceHolder ID="plcAdditionalControls" runat="server" Visible="false"></asp:PlaceHolder>
        <asp:PlaceHolder ID="plcDevices" runat="server" Visible="false"></asp:PlaceHolder>
    </asp:Panel>
    <div class="Clear">
    </div>
</cms:CMSPanel>
<asp:Panel ID="pnlInfoWrapper" runat="server" CssClass="object-edit-menu-info-wrapper"
    EnableViewState="false" Visible="false">
    <asp:Panel ID="pnlInfo" runat="server" CssClass="object-edit-menu-info" EnableViewState="false">
        <asp:Label runat="server" ID="lblInfo" Visible="false" />
    </asp:Panel>
</asp:Panel>
