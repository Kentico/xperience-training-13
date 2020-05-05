<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_PostTree"  Codebehind="PostTree.ascx.cs" %>
<div class="PostTree">
    <asp:TreeView ID="treeElem" runat="server" ShowLines="True" CssClass="ForumTree" OnTreeNodePopulate="treeElem_TreeNodePopulate" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</div>
