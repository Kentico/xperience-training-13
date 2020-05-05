<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_Dialogs_LinkMediaSelector"  Codebehind="LinkMediaSelector.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibraryTree.ascx"
    TagName="MediaLibraryTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaView.ascx" TagName="MediaView"
    TagPrefix="cms" %>
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
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/Dialogs/AdvancedMediaLibrarySelector.ascx"
    TagName="LibrarySelector" TagPrefix="cms" %>

<script type="text/javascript" language="javascript">
    function insertItem() {
        SetAction('insertItem', '');
        RaiseHiddenPostBack();
    }
</script>

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
<div class="DialogMainBlock">
    <div class="DialogContent">
        <asp:Panel ID="pnlLeftContent" runat="server" CssClass="DialogLeftBlock">
            <div class="DialogMediaLibraryBlock">
                <cms:CMSUpdatePanel ID="pnlUpdateSelectors" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cms:LibrarySelector ID="librarySelector" runat="server" OnLibraryChanged="librarySelector_LibraryChanged" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
            <asp:Panel runat="server" ID="pnlTreeAreaSep" CssClass="DialogMediaLibraryTreeAreaSep">
                <cms:CMSUpdatePanel ID="pnlUpdateTreeSeparator" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <cms:LocalizedLabel runat="server" ID="lblNoLibraries" ResourceString="dialogs.libraries.nolibraries"
                            EnableViewState="false" Visible="false" CssClass="InfoLabel" />
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
            <asp:Panel ID="pnlTreeArea" runat="server" CssClass="DialogTreeArea">
                <div class="DialogTree">
                    <cms:CMSUpdatePanel ID="pnlUpdateTree" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <asp:Panel ID="pnlTree" runat="server">
                                <cms:MediaLibraryTree ID="folderTree" runat="server" GenerateIDs="true" />
                            </asp:Panel>
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
        <div class="DialogTreeAreaSeparator">
        </div>
        <asp:Panel ID="pnlRightContent" runat="server" CssClass="DialogRightBlock">
            <cms:CMSUpdatePanel ID="pnlUpdateMenu" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <cms:Menu ID="menuElem" runat="server" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
            <cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div id="divDialogView" class="DialogViewContent scroll-area" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateView" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                                <cms:LocalizedLabel ID="lblNoLibrary" runat="server" Visible="false" CssClass="InfoLabel" ResourceString="dialogs.libraries.nolibrary" />
                                <cms:MediaView ID="mediaView" runat="server" />
                                <asp:HiddenField ID="hdnLastSearchedValue" runat="server" />
                                <asp:HiddenField ID="hdnLastSelectedPath" runat="server" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                        <asp:Literal ID="ltlColorizeScript" runat="server"></asp:Literal>
                    </div>
                    <div id="divDialogResizer" class="DialogResizerVLine" runat="server" EnableViewState="false">
                        <div class="DialogResizerV">
                            <div class="DialogResizerArrowV">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                    <div id="divDialogProperties" class="DialogProperties" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateProperties" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="pnlEmpty" runat="server" CssClass="DialogInfoArea">
                                    <cms:LocalizedLabel ID="lblEmpty" runat="server" CssClass="InfoLabel" EnableViewState="false"></cms:LocalizedLabel>
                                </asp:Panel>
                                <cms:HTMLMediaProperties ID="htmlMediaProp" runat="server" Visible="false" SourceType="MediaLibraries" />
                                <cms:HTMLLinkProperties ID="htmlLinkProp" runat="server" Visible="false" />
                                <cms:BBMediaProperties ID="bbMediaProp" runat="server" Visible="false" />
                                <cms:BBLinkProperties ID="bbLinkProp" runat="server" Visible="false" />
                                <cms:URLProperties ID="urlProp" runat="server" Visible="false" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
    </div>
</div>
