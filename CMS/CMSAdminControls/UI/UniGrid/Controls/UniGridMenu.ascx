<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UniGridMenu.ascx.cs" Inherits="CMSAdminControls_UI_UniGrid_Controls_UniGridMenu" %>
<asp:Panel runat="server" ID="pnlUniGridMenu" CssClass="PortalContextMenu WebPartContextMenu"
    EnableViewState="false">
    <asp:PlaceHolder runat="server" ID="plcExport">
        <cms:ContextMenuItem runat="server" ID="iExcel" ResourceString="export.exporttoexcel" />
        <cms:ContextMenuItem runat="server" ID="iCSV" ResourceString="export.exporttocsv" />
        <cms:ContextMenuItem runat="server" ID="iXML" ResourceString="export.exporttoxml" />
        <cms:ContextMenuItem runat="server" ID="iAdvanced" ResourceString="export.advancedexport" />
        <cms:ContextMenuSeparator runat="server" ID="sm1" />
    </asp:PlaceHolder>
    <cms:ContextMenuItem runat="server" ID="iFilter" ResourceString="general.showfilter" />
    <cms:ContextMenuItem runat="server" ID="iReset" ResourceString="general.resetview" />
</asp:Panel>
