<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaLibraryTree"
     Codebehind="MediaLibraryTree.ascx.cs" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
<asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false" />
<div id="<%=ClientID%>" class="ContentTree MediaLibraryTree">
    <cms:UITreeView ID="treeElem" ShortID="t" runat="server" ShowLines="true" ShowExpandCollapse="true" OnTreeNodePopulate="treeElem_TreeNodePopulate"
        ExpandDepth="1">
        <HoverNodeStyle CssClass="HoveredFolder" />
        <RootNodeStyle CssClass="RootFolder" />
        <LeafNodeStyle CssClass="LeafFolder" />
        <NodeStyle CssClass="Folder ContentTreeItem" />
        <ParentNodeStyle CssClass="ParentFolder" />
    </cms:UITreeView>
    <asp:HiddenField ID="hdnPath" runat="server" />
    <asp:HiddenField ID="hdnFolderSelected" runat="server" Value="false" />
</div>
