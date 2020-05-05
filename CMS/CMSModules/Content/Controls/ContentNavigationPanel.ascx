<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContentNavigationPanel.ascx.cs" Inherits="CMSModules_Content_Controls_ContentNavigationPanel" %>

<div class="ContentMenuGroup">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="menu" runat="server" Direction="North" Closable="false" Resizable="false"
                SpacingOpen="0" ControlPath="~/CMSModules/Content/Controls/TreeActionsPanel.ascx" />
            <cms:UILayoutPane ID="tree" runat="server" Direction="Center" PaneClass="TreeBody"
                ControlPath="~/CMSModules/Content/Controls/CMSDeskTree.ascx"/>
            <cms:UILayoutPane ID="language" runat="server" Direction="South" Closable="false" Resizable="false" SpacingOpen="0"
                 ControlPath="~/CMSModules/Content/Controls/TreeLanguageMenu.ascx" PaneClass="tree-bottom-actions-panel" />
        </Panes>
    </cms:UILayout>
</div>
