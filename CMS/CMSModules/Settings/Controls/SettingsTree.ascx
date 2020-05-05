<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SettingsTree.ascx.cs" Inherits="CMSModules_Settings_Controls_SettingsTree" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    var selectedTreeNode = '';

    function SelectNode(elementName) {
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

    //]]>
</script>
<asp:PlaceHolder ID="plcActionSelectionScript" runat="server" Visible="false">
    <script type="text/javascript" language="javascript">
        //<![CDATA[
        function NodeSelected(elementName, elementId, siteId, parentId, moduleId) {
            // Update frames URLs
            if (window.parent != null) {
                if (window.parent.frames['settingsmain'] != null) {
                    var query = '&categoryId=' + elementId + '&objectId=' + elementId + '&moduleid=' + selectedModuleId + '&parentObjectId=' + selectedModuleId;
                    if (window.tabIndex) {
                        query += '&tabIndex=' + window.tabIndex;
                    }
                    window.parent.frames['settingsmain'].location = frameURL + query;
                }
            }

            // Set menu actions value
            enableMenu(elementId, parentId, moduleId);
        }

        function setMenuAction(elementId, menuItems, moduleId) {
            if (menuItems.length > 0) {
                menuItems.each(function (i) {
                    if ((!developmentMode && !resourceInDevelopment) || (i > 0)) {
                        var jThis = $cmsj(this);
                        if ((rootId == elementId) || ((moduleId != selectedModuleId) && !developmentMode) || (!resourceInDevelopment && !developmentMode)) {
                            if (jThis.hasClass('btn-icon')) {
                                jThis.attr('disabled', 'disabled');
                            } 

                            jThis.attr('_onclick', jThis.attr('onclick'));
                            jThis.removeAttr('onclick');
                        }
                        else {
                            if (jThis.hasClass('btn-icon')) {
                                jThis.removeAttr('disabled');
                            } 

                            jThis.attr('onclick', jThis.attr('_onclick'));
                            jThis.removeAttr('_onclick');
                        }
                    }
                });
            }
        }

        function enableMenu(elementId, parentId, moduleId) {
            // Set menu actions value
            var hidVal = $cmsj('#' + '<%=hidSelectedElem.ClientID%>');
            if (hidVal.length > 0) {
                hidVal[0].value = elementId + '|' + parentId;
                if (window.tabIndex) {
                    hidVal[0].value += '|' + window.tabIndex;
                }
            }

            // Disable menu items for root element
            var menuItems = $cmsj('.js-settings-tree .btn-icon');
            setMenuAction(elementId, menuItems, moduleId);
        }

        function setTab(tabIndex) {
            window.tabIndex = tabIndex;
            var hidVal = $cmsj('#' + '<%=hidSelectedElem.ClientID%>');
            if (hidVal.length == 1) {
                var menuValue = hidVal[0].value.match(/^\d+\|\d+/, '');
                hidVal[0].value = menuValue + '|' + tabIndex;
            }
        }

        $cmsj().ready(function() {
            var menuItems = $cmsj('.js-settings-tree .btn-icon');
            setMenuAction(postParentId, menuItems, <%=CategoryModuleID%>);
        });

        //]]>
    </script>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcSelectionScript" runat="server" Visible="false">
    <script type="text/javascript" language="javascript">
        //<![CDATA[

        var selectedItemId = 0;
        var selectedItemParent = 0;

        function NodeSelected(elementName, categoryId, siteId, parentId) {
            selectedItemId = categoryId;
            selectedItemParent = parentId;
            // Update frames URLs
            if ((window.parent != null) && (window.parent.frames['keys'] != null)) {
                if (doNotReloadContent) {
                    doNotReloadContent = false;
                } else {
                    var contentFrame = window.parent.frames['keys'];
                    var url = categoryURL + (categoryURL.indexOf('?') === -1 ? '?' : '&') + 'categoryid=' + categoryId + '&parentid=' + parentId;

                    if (siteId > 0) {
                        url = url + "&siteid=" + siteId;
                    }

                    // If helper method exists
                    if (contentFrame.GetSearchValues != null) {
                        var searchSettings = contentFrame.GetSearchValues();

                        if ((searchSettings != null) && (searchSettings[0] == categoryId)) {
                            // Set search settings to url
                            url = url + "&search=" + searchSettings[1] + "&description=" + searchSettings[2];
                        }
                    }

                    document.getElementById('selectedCategoryId').value = categoryId;
                    contentFrame.location = url;
                }
            }
        }
        //]]>
    </script>
</asp:PlaceHolder>
<div class="js-settings-tree">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneMenu" runat="server" Direction="North" RednerAs="Div" PaneClass="tree-menu-buttons" Resizable="false" Closable="false" SpacingOpen="0">
                <Template>
                    <asp:Panel ID="pnlSite" CssClass="tree-buttons-panel" runat="server">
                        <strong>
                            <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="ContentLabel" EnableViewState="false" ResourceString="general.site" DisplayColon="true" />
                        </strong>
                        <br />
                        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" UseCodeNameForSelection="False" AllowAll="False" OnlyRunningSites="False" DropDownSingleSelect-AutoPostBack="True" />
                    </asp:Panel>
                    <asp:Panel ID="pnlActions" CssClass="tree-actions-panel" runat="server">
                        <div class="tree-actions">
                            <cms:CMSAccessibleButton runat="server" ID="btnNew" IconCssClass="icon-plus" IconOnly="true" />
                            <cms:CMSAccessibleButton runat="server" ID="btnDelete" OnClick="btnDeleteElem_Click" IconCssClass="icon-bin" IconOnly="true" />
                            <cms:CMSAccessibleButton runat="server" ID="btnUp" OnClick="btnMoveUp_Click" IconCssClass="icon-chevron-up" IconOnly="true" />
                            <cms:CMSAccessibleButton runat="server" ID="btnDown" OnClick="btnMoveDown_Click" IconCssClass="icon-chevron-down" IconOnly="true" />
                        </div>
                    </asp:Panel>
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneTree" runat="server" Direction="Center" RednerAs="Div" PaneClass="ContentTreeArea" SpacingOpen="0">
                <Template>
                    <div class="TreeAreaTree">
                        <cms:UniTree runat="server" ID="treeElem" ShortID="t" Localize="true" IsLiveSite="false" />
                    </div>
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneSeparator" runat="server" Direction="East" RednerAs="Div" SpacingOpen="0" Resizable="false" Closable="false" Size="8" />
        </Panes>
    </cms:UILayout>
    <asp:HiddenField ID="hidSelectedElem" runat="server" />
    <input type="hidden" id="selectedCategoryId" name="selectedCategoryId" value="0" />
</div>
