<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_FormControls_PageTemplates_LevelTree"  Codebehind="LevelTree.ascx.cs" %>

<div class="level-tree">
    <asp:TreeView ID="treeElem" runat="server" ShowCheckBoxes="All" ShowLines="true"
        ShowExpandCollapse="false" CssClass="level-tree-view">
        <NodeStyle CssClass="checkbox" />
    </asp:TreeView>
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />