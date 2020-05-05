<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UpDownMenu.ascx.cs"
    Inherits="CMSAdminControls_UI_UniGrid_Controls_UpDownMenu" %>
<asp:Panel runat="server" ID="pnlObjectMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <cms:ContextMenuItem runat="server" ID="iTop" />
    <cms:ContextMenuItem runat="server" ID="iUp" />
    <cms:ContextMenuSeparator runat="server" ID="sep1" />
    <cms:ContextMenuItem runat="server" ID="iDown" />
    <cms:ContextMenuItem runat="server" ID="iBottom" />
</asp:Panel>
