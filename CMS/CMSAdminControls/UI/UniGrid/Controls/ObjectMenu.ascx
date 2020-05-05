<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniGrid_Controls_ObjectMenu"
     Codebehind="ObjectMenu.ascx.cs" %>
    
<asp:Panel runat="server" ID="pnlObjectMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <cms:ContextMenuSeparator runat="server" ID="sepCloneDestroy" />
    <cms:ContextMenuItem runat="server" ID="iClone" ResourceString="general.clone" /> 
    <cms:ContextMenuItem runat="server" ID="iDestroy" ResourceString="security.destroy" /> 
    <cms:ContextMenuSeparator runat="server" ID="sepExport" />
    <cms:ContextMenuItem runat="server" ID="iExport" ResourceString="General.Export" />
    <cms:ContextMenuItem runat="server" ID="iBackup" ResourceString="General.Backup" />
    <cms:ContextMenuItem runat="server" ID="iRestore" ResourceString="General.Restore" />
    <cms:ContextmenuSeparator runat="server" ID="sepMove" />
    <cms:ContextMenuItem runat="server" ID="iMoveUp" ResourceString="general.moveup" />
    <cms:ContextMenuItem runat="server" ID="iMoveDown" ResourceString="general.movedown" />
</asp:Panel>
