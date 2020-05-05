<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderTree"
     Codebehind="FolderTree.ascx.cs" %>
<asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false" />
<div class="ContentTree MediaLibraryTree">
    <cms:uitreeview id="treeElem" runat="server" showlines="true" showexpandcollapse="true" shortid="t">
    <HoverNodeStyle CssClass="HoveredFolder" />
    <RootNodeStyle CssClass="RootFolder" />
    <LeafNodeStyle CssClass="LeafFolder" />
    <NodeStyle CssClass="Folder ContentTreeItem" />
    <ParentNodeStyle CssClass="ParentFolder" />
    <SelectedNodeStyle CssClass="SelectedFolder ContentTreeSelectedItem" />
</cms:uitreeview>
</div>
<asp:HiddenField ID="hdnPath" runat="server" />
<div style="clear: both">
</div>
