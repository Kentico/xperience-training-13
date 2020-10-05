<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreeActionsPanel.ascx.cs" Inherits="CMSModules_Content_Controls_TreeActionsPanel" %>
<div class="tree-actions-panel">
    <asp:PlaceHolder runat="server" ID="plcContentButtons">
        <cms:CMSPanel ID="pnlButtons" ShortID="pb" runat="server" CssClass="tree-buttons">
            <div class="btn-group">
                <cms:CMSButton runat="server" ID="btnEdit" ButtonStyle="Default" OnClientClick="return false;" />
                <div class="btn-group cms-split-button">
                    <cms:CMSButton runat="server" ID="btnPreview" ButtonStyle="Default" OnClientClick="return false;" />
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                        <span class="caret"></span>
                        <span class="sr-only"><%= GetString("content.ui.previewoptions") %></span>
                    </button>

                    <ul class="dropdown-menu" role="menu">
                        <li><a href="javascript: GetCurrentNodePreviewUrl()" title='<%= GetString("content.ui.previewinnewtab") %>'><%= GetString("content.ui.previewinnewtab") %></a></li>
                    </ul>
                </div>
                <cms:CMSButton runat="server" ID="btnListing" ButtonStyle="Default" OnClientClick="return false;" />
            </div>
        </cms:CMSPanel>
    </asp:PlaceHolder>

    <script type="text/javascript">
        function OpenInNewTabCallback(url) {
            if (url) {
                var newTab = window.open(url, '_blank');
                // Browser may block pop-up windows which causes newTab is undefined.
                newTab && newTab.focus();
            }
        }
    </script>

    <div class="tree-actions">
        <cms:CMSAccessibleButton runat="server" ID="btnNew" OnClientClick="NewItem(); return false;" IconCssClass="icon-plus" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnDelete" OnClientClick="DeleteItem();  return false;" IconCssClass="icon-bin" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnCopy" OnClientClick="if(CheckChanges()) { CopyRef(GetSelectedNodeId()); }; return false;" IconCssClass="icon-doc-copy" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnMove" OnClientClick="if(CheckChanges()) { MoveRef(GetSelectedNodeId()); }; return false;" IconCssClass="icon-doc-move" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnUp" OnClientClick="MoveUp();  return false;" IconCssClass="icon-chevron-up" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnDown" OnClientClick="MoveDown();  return false;" IconCssClass="icon-chevron-down" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnRefresh" OnClientClick="TreeRefresh(); return false;" IconCssClass="icon-rotate-right" IconOnly="true" />
        <cms:CMSAccessibleButton runat="server" ID="btnSearch" OnClientClick="OpenSearch(); return false;" IconCssClass="icon-magnifier" IconOnly="true" />
    </div>
</div>
