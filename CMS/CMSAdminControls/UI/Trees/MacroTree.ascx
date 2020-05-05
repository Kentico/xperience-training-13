<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Trees_MacroTree"
    EnableViewState="false"  Codebehind="MacroTree.ascx.cs" %>
<asp:Panel runat="server" ID="pnlPriorityTree" EnableViewState="false" CssClass="TreeMain">
    <cms:UITreeView runat="server" ID="treeElemPriority" ShortID="tp" ShowLines="true" EnableViewState="false" CssClass="macro-tree" />
</asp:Panel>
<asp:Panel runat="server" CssClass="TreeMain">
    <cms:UITreeView runat="server" ID="treeElem" ShortID="t" ShowLines="true" EnableViewState="false" CssClass="macro-tree" />
</asp:Panel>
