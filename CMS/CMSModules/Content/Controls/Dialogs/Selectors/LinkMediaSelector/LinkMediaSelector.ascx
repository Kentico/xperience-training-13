<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_LinkMediaSelector"  Codebehind="LinkMediaSelector.ascx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/ContentTree.ascx" TagName="ContentTree"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/MediaView.ascx"
    TagName="MediaView" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/Menu.ascx"
    TagName="Menu" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/HTMLMediaProperties.ascx"
    TagName="HTMLMediaProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/URLProperties.ascx"
    TagName="URLProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/HTMLLinkProperties.ascx"
    TagName="HTMLLinkProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/BBMediaProperties.ascx"
    TagName="BBMediaProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/BBLinkProperties.ascx"
    TagName="BBLinkProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Properties/DocCopyMoveProperties.ascx"
    TagName="DocCopyMoveProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<div class="Hidden">
    <cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server">
        <ContentTemplate>
            <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
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
</style>
<div class="DialogMainBlock">
    <div class="DialogContent">
        <asp:Panel ID="pnlLeftContent" runat="server" CssClass="DialogLeftBlock">
            <div class="DialogSiteBlock">
                <cms:CMSUpdatePanel ID="pnlUpdateSelectors" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="control-group-inline">
                            <cms:LocalizedLabel ID="lblContentSite" CssClass="input-label" runat="server" ResourceString="general.site"
                                DisplayColon="true" />
                        </div>
                        <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false"
                            StopProcessing="true" UseCodeNameForSelection="false" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
            <asp:PlaceHolder ID="plcTreeArea" runat="server">
                <asp:Panel ID="pnlTreeArea" runat="server" class="DialogTreeArea">
                    <div class="DialogTree">
                        <cms:CMSUpdatePanel ID="pnlUpdateTree" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cms:LocalizedLabel ID="lblTreeInfo" runat="server" CssClass="InfoLabel PageContent" ResourceString="contenttree.failedload" Visible="false" />
                                <cms:ContentTree ID="contentTree" runat="server" UseCMSFileIcons="true" AllowMarks="true" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </asp:Panel>
                <div class="DialogResizerH">
                    <div class="DialogResizerArrowH">
                        &nbsp;
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:Panel>
        <asp:PlaceHolder ID="plcSeparator" runat="server">
            <div class="DialogTreeAreaSeparator">
            </div>
        </asp:PlaceHolder>
        <asp:Panel ID="pnlRightContent" runat="server" CssClass="DialogRightBlock">
            <cms:CMSUpdatePanel ID="pnlUpdateMenu" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <cms:Menu ID="menuElem" runat="server" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
            <cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div id="divDialogView" class="DialogViewContent scroll-area" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateView" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                                <cms:MediaView ID="mediaView" ShortID="m" runat="server" />
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
                    <div id="divDialogProperties" class="DialogProperties" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateProperties" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cms:HTMLMediaProperties ID="htmlMediaProp" runat="server" Visible="false" />
                                <cms:HTMLLinkProperties ID="htmlLinkProp" runat="server" Visible="false" />
                                <cms:BBMediaProperties ID="bbMediaProp" runat="server" Visible="false" />
                                <cms:BBLinkProperties ID="bbLinkProp" runat="server" Visible="false" />
                                <cms:URLProperties ID="urlProp" runat="server" Visible="false" />
                                <cms:DocCopyMoveProperties ID="docCopyMoveProp" runat="server" Visible="false" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
    </div>
</div>

<script type="text/javascript">
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

        if (nodeElem != null) {
            currentNode = nodeElem;
            if (currentNode != null) {
                currentNode.className = 'ContentTreeSelectedItem';
                currentNode.id = 'treeSelectedNode';
            }
        }
    }

    function selectDocument(nodeId, selected) {
        SetAction('selectDocument', "nodeId|" + nodeId + "|isChecked|" + selected);
        RaiseHiddenPostBack();
    }
    //]]>
</script>