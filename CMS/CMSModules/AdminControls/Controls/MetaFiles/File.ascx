<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_MetaFiles_File"
     Codebehind="File.ascx.cs" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:PlaceHolder ID="plcOldUploader" runat="server">
    <cms:Uploader ID="uploader" runat="server" RequireDeleteConfirmation="true" />
</asp:PlaceHolder>
<cms:CMSUpdatePanel ID="updPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlAttachmentList" runat="server">
            <div class="New">
                <asp:PlaceHolder ID="plcUploader" runat="server">
                    <cms:DirectFileUploader ID="newMetafileElem" runat="server" InsertMode="true" UploadMode="DirectSingle" />
                </asp:PlaceHolder>
            </div>
            <asp:Panel ID="pnlGrid" runat="server">
                <cms:UniGrid ID="gridFile" runat="server" ObjectType="cms.metafile" OrderBy="MetaFileName"
                    HideControlForZeroRows="true" ShowObjectMenu="false" ShowActionsMenu="false" Columns="MetaFileID,MetaFileGUID,MetaFileObjectType,MetaFileObjectID,MetaFileGroupName,MetaFileName,MetaFileExtension,MetaFileSize,MetaFileImageWidth,MetaFileImageHeight,MetaFileTitle,MetaFileDescription">
                    <GridActions>
                        <ug:Action Name="edit" ExternalSourceName="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                        <ug:Action Name="delete" ExternalSourceName="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="##ALL##" ExternalSourceName="update" Name="Update" Caption="$general.update$"
                            Wrap="false" Style="text-align: center;" CssClass="unigrid-actions" AllowSorting="false" />
                        <ug:Column Source="##ALL##" ExternalSourceName="name" Caption="$general.filename$"
                            Wrap="false" CssClass="main-column-100" AllowSorting="false" />
                        <ug:Column Source="MetaFileSize" ExternalSourceName="size" Caption="$general.size$"
                            Wrap="false" AllowSorting="false" />
                    </GridColumns>
                    <PagerConfig DisplayPager="false" ShowPageSize="false" />
                </cms:UniGrid>
            </asp:Panel>
            <div>
                <asp:Button ID="hdnPostback" CssClass="HiddenButton" runat="server" EnableViewState="false"
                    OnClick="hdnPostback_Click" />
                <asp:HiddenField ID="hdnField" runat="server" />
            </div>
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Button id="hdnFullPostback" cssclass="HiddenButton" runat="server" enableviewstate="false"
    onclick="hdnPostback_Click" />
