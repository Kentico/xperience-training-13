<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaLibrary"  Codebehind="MediaLibrary.ascx.cs" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibraryTree.ascx"
    TagName="LibraryTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/FolderActions/FolderActions.ascx"
    TagName="FolderActions" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/FolderActions/EditFolder.ascx"
    TagName="FolderEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaView.ascx" TagName="MediaView"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/LinkMediaSelector/Menu.ascx"
    TagName="Menu" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileEdit.ascx"
    TagName="FileEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileSingleImport.ascx"
    TagName="SingleImport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileMultipleImport.ascx"
    TagName="MultipleImport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/FolderActions/CopyMoveFolder.ascx"
    TagName="CopyMove" TagPrefix="cms" %>

<div class="Hidden HiddenButton">
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
<cms:FileSystemDataSource ID="fileSystemDataSource" runat="server" IsLiveSite="false" />
<div class="DialogMainBlock">
    <div class="DialogContent">
        <asp:Panel ID="pnlLeftContent" runat="server" CssClass="DialogLeftBlock">
            <asp:PlaceHolder ID="plcFolderActions" runat="server">
                <cms:FolderActions ID="folderActions" ShortID="fa" runat="server" />
            </asp:PlaceHolder>
            <asp:Panel ID="pnlTreeArea" runat="server" class="DialogTreeArea">
                <div class="DialogTree">
                    <cms:CMSUpdatePanel ID="pnlUpdateTree" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlTreeAreaSep" CssClass="DialogMediaLibraryTreeAreaSep">
                                <cms:LocalizedLabel runat="server" ID="lblNoLibraries" ResourceString="dialogs.libraries.nolibraries"
                                    EnableViewState="false" Visible="false" CssClass="InfoLabel" />
                            </asp:Panel>
                            <cms:LibraryTree ID="folderTree" ShortID="t" runat="server" GenerateIDs="true" />
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
                    <cms:Menu ID="menuElem" ShortID="m" runat="server" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
            <cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <div id="divDialogView" class="DialogViewContent scroll-area" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateView" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cms:CMSUpdatePanel ID="pnlMessages" runat="server">
                                    <ContentTemplate>
                                        <cms:MessagesPlaceHolder ID="plcMess" runat="server" WrapperControlID="divDialogView" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                                <cms:MediaView ID="mediaView" ShortID="v" runat="server" DisplayMode="Simple" />
                                <cms:CMSUpdatePanel ID="pnlUpdateViewInfo" runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblBAWarning" EnableViewState="False" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                                <cms:CMSUpdatePanel ID="pnlUpdateViewHidden" runat="server" UpdateMode="Conditional"
                                    ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <asp:HiddenField ID="hdnLastSearchedValue" runat="server" />
                                        <asp:HiddenField ID="hdnLastSelectedPath" runat="server" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                        <asp:Literal ID="ltlColorizeScript" runat="server"></asp:Literal>
                    </div>
                    <div id="divDialogResizer" class="DialogResizerVLine" runat="server" enableviewstate="false">
                        <div class="DialogResizerV">
                            <div class="DialogResizerArrowV">
                                &nbsp;
                            </div>
                        </div>
                    </div>
                    <div id="divDialogProperties" class="DialogProperties MediaProperties" runat="server">
                        <cms:CMSUpdatePanel ID="pnlUpdateProperties" runat="server" UpdateMode="Conditional"
                            ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="plcFileEdit" runat="server">
                                    <cms:FileEdit ID="fileEdit" runat="server" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcEditFolder" runat="server">
                                    <cms:FolderEdit ID="folderEdit" runat="server" ComponentName="MediaFolder" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcImportFile" runat="server">
                                    <cms:SingleImport ID="fileImport" runat="server" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcMultipleImport" runat="server">
                                    <cms:MultipleImport ID="multipleImport" runat="server" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcCopyMove" runat="server">
                                    <cms:CopyMove ID="folderCopyMove" runat="server" />
                                </asp:PlaceHolder>
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
    </div>
</div>
