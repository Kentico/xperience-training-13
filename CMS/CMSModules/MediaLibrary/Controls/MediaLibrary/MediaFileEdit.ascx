<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileEdit"
     Codebehind="MediaFileEdit.ascx.cs" %>
<%@ Register Src="~/CMSInlineControls/MediaControl.ascx" TagPrefix="cms" TagName="MediaPreview" %>
<%@ Register Src="~/CMSInlineControls/ImageControl.ascx" TagPrefix="cms" TagName="ImagePreview" %>
<%@ Register Src="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileUpload.ascx"
    TagName="FileUpload" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagPrefix="cms" TagName="VersionList" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs">
    <cms:JQueryTab ID="tabGeneral" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlTabFile" runat="server" CssClass="PageContent">
                <cms:CMSUpdatePanel ID="pnlUpdateGeneral" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <asp:PlaceHolder ID="plcDirPath" runat="server">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDirPath" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.dirpath" CssClass="control-label" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <asp:Literal ID="ltrDirPathValue" runat="server" EnableViewState="false" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblPermaLink" runat="server" EnableViewState="false" ResourceString="media.file.permalink" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <asp:Literal ID="ltrPermaLinkValue" runat="server" EnableViewState="false" />
                                </div>
                            </div>
                            <asp:Panel ID="pnlPrew" runat="server" CssClass="form-group">
                                <div class="editing-form-value-cell-offset editing-form-value-cell">
                                    <asp:PlaceHolder ID="plcImagePreview" runat="server" Visible="false">
                                        <cms:ImagePreview ID="imagePreview" runat="server" />
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcMediaPreview" runat="server" Visible="false">
                                        <cms:MediaPreview ID="mediaPreview" runat="server" />
                                    </asp:PlaceHolder>
                                </div>
                            </asp:Panel>
                        </div>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </cms:JQueryTab>
    <cms:JQueryTab ID="tabPreview" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlTabPreview" runat="server" CssClass="PageContent">
                <cms:CMSUpdatePanel ID="pnlUpdatePreviewDetails" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:LocalizedLabel ID="lblEditPreview" runat="server" ResourceString="general.thumbnail"
                                        DisplayColon="true" EnableViewState="false" CssClass="control-label" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:FileUpload ID="fileUplPreview" runat="server" IsMediaThumbnail="true" StopProcessing="true" />
                                </div>
                            </div>
                            <asp:PlaceHolder ID="plcPreview" runat="server" Visible="false">
                                <asp:PlaceHolder ID="plcPrevDirPath" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblPrevDirectLink" runat="server" ResourceString="media.file.dirpath"
                                                EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblPrevDirectLinkVal" runat="server" EnableViewState="false" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblPrevPermaLink" runat="server" ResourceString="media.file.permalink"
                                            EnableViewState="false" CssClass="control-label" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <asp:Label ID="lblPreviewPermaLink" runat="server" EnableViewState="false" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcNoPreview" runat="server" Visible="false">
                                <div class="form-group">
                                    <div class="editing-form-value-cell-offset editing-form-value-cell">
                                        <asp:Label ID="lblNoPreview" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </cms:JQueryTab>
    <cms:JQueryTab ID="tabEdit" runat="server" Style="height: auto">
        <ContentTemplate>
            <cms:CMSUpdatePanel ID="pnlUpdateFileInfo" runat="server" UpdateMode="Conditional"
                ChildrenAsTriggers="true">
                <ContentTemplate>
                    <cms:HeaderActions ID="headerActionsEdit" runat="server" PanelCssClass="cms-edit-menu" />
                    <asp:Panel ID="pnlTabEdit" runat="server" CssClass="PageContent media-file-edit">
                        <cms:MessagesPlaceHolder ID="plcMessEdit" runat="server" ShortID="me" />
                        <div class="layout-2-columns">
                            <div class="col-50">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblEditName" runat="server" ResourceString="general.filename"
                                                DisplayColon="true" EnableViewState="false" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="txtEditName" runat="server" MaxLength="250" EnableViewState="false" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblEditTitle" runat="server" EnableViewState="false" ResourceString="media.file.filetitle" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:LocalizableTextBox ID="txtEditTitle" runat="server" MaxLength="250" EnableViewState="false" AutoSave="False" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblEditDescription" runat="server" EnableViewState="false" ResourceString="general.description" DisplayColon="true" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:LocalizableTextBox ID="txtEditDescription" runat="server" TextMode="MultiLine" EnableViewState="false" AutoSave="False" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                        </div>
                                        <div class="editing-form-value-cell-offset editing-form-value-cell">
                                            <cms:FormSubmitButton ID="btnEdit" runat="server" OnClick="btnEdit_Click" ValidationGroup="MediaFileEdit"
                                                EnableViewState="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-50">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblCreatedBy" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.createdby" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblCreatedByVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblCreatedWhen" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.createdwhen" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblCreatedWhenVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblModified" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.modified" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblModifiedVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="plcFileModified" runat="server" EnableViewState="false" Visible="false">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel ID="lblFileModified" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.filemodified" CssClass="control-label" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <asp:Label ID="lblFileModifiedVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblExtension" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.extension" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblExtensionVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="plcDimensions" runat="server" EnableViewState="false" Visible="false">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel ID="lblDimensions" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.dimensions" CssClass="control-label" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <asp:Label ID="lblDimensionsVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblSize" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.size" CssClass="control-label" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <asp:Label ID="lblSizeVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="plcFileSize" runat="server" EnableViewState="false" Visible="false">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel ID="lblFileSize" runat="server" EnableViewState="false" DisplayColon="true" ResourceString="media.file.filesize" CssClass="control-label" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <asp:Label ID="lblFileSizeVal" runat="server" EnableViewState="false" CssClass="form-control-text" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </ContentTemplate>
    </cms:JQueryTab>
    <cms:JQueryTab ID="tabCustomFields" runat="server">
        <ContentTemplate>
            <cms:CMSUpdatePanel ID="pnlUpdateCustomFields" runat="server" UpdateMode="Conditional"
                ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:Panel ID="pnlActions" runat="server" CssClass="cms-edit-menu" EnableViewState="false">
                        <cms:HeaderActions ID="headerActionsCustom" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlTabCustomFields" runat="server" CssClass="MediaLibraryCustomTab PageContent">
                        <asp:PlaceHolder runat="server" ID="plcMediaFileCustomFields">
                            <cms:MessagesPlaceHolder ID="plcMessCustom" runat="server" ShortID="mc" />
                            <cms:DataForm ID="formMediaFileCustomFields" runat="server" IsLiveSite="false" />
                        </asp:PlaceHolder>
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </ContentTemplate>
    </cms:JQueryTab>
    <cms:JQueryTab ID="tabVersions" runat="server" Visible="True">
        <ContentTemplate>
            <cms:CMSUpdatePanel ID="pnlUpdateVersions" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlTabVersions" CssClass="PageContent media-file-versions" runat="server">
                        <cms:VersionList ID="objectVersionList" runat="server" />
                        <asp:Button ID="btnHidden" runat="server" CssClass="HiddenButton" EnableViewState="false"
                            OnClick="btnHidden_Click" />
                    </asp:Panel>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </ContentTemplate>
    </cms:JQueryTab>
</cms:JQueryTabContainer>
