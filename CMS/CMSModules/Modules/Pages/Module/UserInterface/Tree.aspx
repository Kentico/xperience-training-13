<%@ Page Title="Module edit - User interface - Tree" Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_UserInterface_Tree"
    Theme="Default"  Codebehind="Tree.aspx.cs" %>
<%@ Import Namespace="CMS.Base" %>
<%@ Import Namespace="CMS.Modules" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" TagPrefix="cms" TagName="TreeBorder" %>


<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneMenu" runat="server" Direction="North" RednerAs="Div" SpacingOpen="0">
                <Template>
                    <div class="tree-actions-panel">
                        <div class="tree-actions">
                            <cms:CMSAccessibleButton runat="server" ID="btnNew" IconCssClass="icon-plus" IconOnly="true" />
                            <cms:CMSAccessibleButton runat="server" ID="btnDelete" IconCssClass="icon-bin" IconOnly="true" OnClick="btnDeleteElem_Click" OnClientClick="if (!deleteConfirm()) return false;" />
                            <cms:CMSAccessibleButton runat="server" ID="btnUp" IconCssClass="icon-chevron-up" IconOnly="true" OnClick="btnMoveUp_Click" />
                            <cms:CMSAccessibleButton runat="server" ID="btnDown" IconCssClass="icon-chevron-down" IconOnly="true" OnClick="btnMoveDown_Click" />
                        </div>
                    </div>

                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneTree" runat="server" Direction="Center" RednerAs="Div" SpacingOpen="0">
                <Template>
                    <div class="TreeAreaTree">
                        <cms:UniTree ID="uniTree" ShortID="t" cssclass="ContentTree" runat="server" Localize="true" IsLiveSite="false" />
                    </div>
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="paneSeparator" runat="server" Direction="East" RednerAs="Div" SpacingOpen="0" Resizable="false" Closable="false" Size="8" />
        </Panes>
    </cms:UILayout>

    <asp:HiddenField ID="hidSelectedElem" runat="server" />
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />

    <script type="text/javascript" language="javascript">
        //<![CDATA[
        function SelectNode(elementId, parentId, moduleId, refreshContent) {
            // Set selected item in tree
            $cmsj('span[name=treeNode]').each(function () {
                var jThis = $cmsj(this);
                jThis.removeClass('ContentTreeSelectedItem');
                if (!jThis.hasClass('ContentTreeItem')) {
                    jThis.addClass('ContentTreeItem');
                }
                if (this.id == 'node_' + elementId) {
                    jThis.addClass('ContentTreeSelectedItem');
                }
            });
            // Update frames URLs
            if (refreshContent && (window.parent != null)) {
                if (window.parent.frames['uicontent'] != null) {
                    var currentModule = '<%= ModuleId %>';
                    var query = '&moduleID=' + currentModule + '&objectParentID=' + currentModule + '&elementId=' + elementId + '&objectId=' + elementId;
                    window.parent.frames['uicontent'].location = '<%= ContentUrl %>' + query;
                }
            }
            // Set menu actions value
            var menuElem = $cmsj('#' + '<%= hidSelectedElem.ClientID %>');
            // Set identificators
            menuElem[0].value = elementId;

            // Disable menu items for root element and for elements from other modules
            var menuItems = menuElem.parent().find('.btn-icon');
            setMenuAction(menuItems, parentId, moduleId);
        }

        // Disables menu items for root element and for elements from other modules
        function setMenuAction(menuItems, parent, moduleId) {
            var currentModule = '<%= ModuleId %>';
            var moduleInDevelopment = <%= CurrentModule.ResourceIsInDevelopment.ToString().ToLowerCSafe() %>;
            var disable = (moduleId != currentModule) && <%= UIElementInfoProvider.AllowEditOnlyCurrentModule.ToString().ToLowerCSafe() %>  || !moduleInDevelopment;
            if (menuItems.length > 0) {
                menuItems.each(function (i) {
                    if (i > 0) {
                        var jThis = $cmsj(this);
                        if ((parent == 0) || disable) {
                            jThis.attr('disabled', 'disabled');
                        }
                        else {
                            jThis.removeAttr('disabled');
                        }
                    }
                });
            }
        }

        // If frame is minimized swap resize handlers
        if (document.body.clientWidth === 10) {
            var minimizeElem = document.getElementById('imgMinimize');
            var maximizeElem = document.getElementById('imgMaximize');
            if ((minimizeElem != null) && (maximizeElem != null)) {
                minimizeElem.style.display = 'none';
                maximizeElem.style.display = 'inline';
                originalSizes[0] = '240,*';
            }
        }
        //]]>
    </script>

    <cms:TreeBorder runat="server" ID="TreeBorder" />

</asp:Content>
