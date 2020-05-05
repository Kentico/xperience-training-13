<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemTree"
     Codebehind="FileSystemTree.ascx.cs" %>
<asp:Label runat="server" ID="lblError"></asp:Label>
<asp:Panel runat="server" ID="pnlTree" CssClass="TreeMain">
    <cms:uitreeview id="treeFileSystem" shortid="t" runat="server" ontreenodepopulate="treeFileSystem_TreeNodePopulate"
        cssclass="ContentTree" />
</asp:Panel>
<asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
