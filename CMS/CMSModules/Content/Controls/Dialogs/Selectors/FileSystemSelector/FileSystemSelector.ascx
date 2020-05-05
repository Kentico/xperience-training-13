<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemSelector"
     Codebehind="FileSystemSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemTree.ascx"
    TagName="FileSystemTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemView.ascx"
    TagName="FileSystemView" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/FileSystemPathProperties.ascx"
    TagName="PathProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/Menu.ascx"
    TagName="Menu" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FolderActions.ascx"
    TagName="FolderActions" TagPrefix="cms" %>

<div class="Hidden">
    <cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server">
        <ContentTemplate>
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false"></asp:Literal>
            <asp:HiddenField ID="hdnAction" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="hdnArgument" runat="server"></asp:HiddenField>
            <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton"
                EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
<style type="text/css">
    body {
        margin: 0;
        padding: 0;
        height: 100%;
        width: 100%;
    }

    a:hover, a:active, a:focus {
        outline: none;
    }
</style>
<div class="DialogMainBlock">
    <div class="DialogContent">
        <asp:Panel ID="pnlLeftContent" runat="server" CssClass="DialogLeftBlock">
            <asp:PlaceHolder runat="server" ID="plcFolderActions">
                <cms:FolderActions ID="folderActions" ShortID="fa" runat="server" />
            </asp:PlaceHolder>
            <asp:Panel ID="pnlTreeArea" runat="server" class="DialogTreeArea">
                <div class="DialogTree">
                    <cms:CMSUpdatePanel ID="pnlUpdateTree" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cms:FileSystemTree ID="treeFileSystem" ShortID="t" runat="server" />
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </asp:Panel>
            <div class="DialogResizerH">
                <div class="DialogResizerArrowH">
                    &nbsp;
                </div>
            </div>
        </asp:Panel>
        <asp:PlaceHolder ID="plcSeparator" runat="server">
            <div class="DialogTreeAreaSeparator">
            </div>
        </asp:PlaceHolder>
        <asp:Panel ID="pnlRightContent" runat="server" CssClass="DialogRightBlock">
            <asp:PlaceHolder runat="server" ID="plcMenu">
                <cms:CMSUpdatePanel ID="pnlUpdateMenu" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cms:Menu ID="menuElem" runat="server" IsLiveSite="false" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:PlaceHolder>
            <cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional"
                ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div id="divDialogView" class="DialogViewContent scroll-area" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateView" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cms:FileSystemView ID="fileSystemView" ShortID="f" runat="server" />
                                <asp:HiddenField ID="hdnLastNodeSlected" runat="server" />
                                <asp:HiddenField ID="hdnLastSearchedValue" runat="server" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                    <div id="divDialogResizer" class="DialogResizerVLine" runat="server" enableviewstate="false">
                        <div class="DialogResizerV">
                            <div class="DialogResizerArrowV">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                    <div id="divDialogProperties" class="DialogProperties" style="height: 180px" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateProperties" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cms:PathProperties runat="server" ID="pathProperties" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
    </div>
</div>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    function insertItem() {
        SetAction('insertItem', '');
        RaiseHiddenPostBack();
    }

    function SelectNode(nodeId, nodeElem) {
        // Select node action
        var currentNode = document.getElementById('treeSelectedNode');
        var currentNodeId = 0;

        if ((currentNode != null) && (nodeElem != null) && (nodeId != currentNodeId)) {
            currentNode.className = 'ContentTreeItem';
            currentNode.id = '';
        }

        currentNodeId = nodeId;

        if (nodeElem != null) {
            currentNode = nodeElem;
            if (currentNode != null) {
                currentNode.className = 'ContentTreeSelectedItem';
                currentNode.id = 'treeSelectedNode';
            }
        }
    }
    //]]>
</script>
