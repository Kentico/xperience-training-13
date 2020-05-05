<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CategorySelectionDialog.ascx.cs"
    Inherits="CMSModules_Categories_Controls_CategorySelectionDialog" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<asp:Literal ID="ltlTreeScript" runat="server" EnableViewState="false" />
<script type="text/javascript" language="javascript">
    //<![CDATA[
    var selectedTreeNode = '';

    function SelectNode(elementName, allowedActions) {
        // Set selected item in tree
        selectedTreeNode = elementName;
        $cmsj('span[name=treeNode]').each(function () {
            var jThis = $cmsj(this);
            jThis.removeClass('ContentTreeSelectedItem');
            if (!jThis.hasClass('ContentTreeItem')) {
                jThis.addClass('ContentTreeItem');
            }
            if (this.id == 'node_' + elementName) {
                jThis.addClass('ContentTreeSelectedItem');
            }
        });
    }

    function NodeSelected(elementId, parentId) {
        // Set menu actions value
        var menuElem = $cmsj('#' + menuHiddenId);
        if (menuElem.length = 1) {
            menuElem[0].value = elementId + '|' + parentId;
        }

        RaiseHiddenPostBack();
    }

    function GetSelectedId() {
        // Set menu actions value
        var menuElem = $cmsj('#' + menuHiddenId);
        if (menuElem.length = 1) {
            var ids = menuElem[0].value.split('|');
            if (ids.length = 2) {
                return ids[0];
            }
        }
        return '';
    }
    //]]>
</script>
<cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server" RenderMode="Inline">
    <ContentTemplate>
        <asp:HiddenField ID="hidSelectedElem" runat="server" />
        <asp:HiddenField ID="hidParam" runat="server" />
        <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton"
            EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlBody" runat="server" CssClass="UniSelectorDialogBody Categories">
    <asp:Panel ID="pnlFilter" runat="server" CssClass="header-panel" Visible="false">
    </asp:Panel>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlContent">
                <div class="ContentTree">
                    <cms:CMSUpdatePanel ID="pnlUpdateTrees" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlTreeArea" runat="server" CssClass="TreeAreaTree">
                                <cms:UniTree runat="server" ID="treeElemG" ShortID="tg" Localize="true" IsLiveSite="false" />
                                <cms:UniTree runat="server" ID="treeElemP" ShortID="tp" Localize="true" IsLiveSite="false" />
                                <div class="ClearBoth">
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel runat="server" ID="pnlHidden" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hidItem" runat="server" EnableViewState="false" />
            <asp:HiddenField ID="hidName" runat="server" EnableViewState="false" />
            <asp:HiddenField ID="hidHash" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
<asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
