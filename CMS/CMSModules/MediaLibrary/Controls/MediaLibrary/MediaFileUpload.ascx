<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUpload"
     Codebehind="MediaFileUpload.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="updPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="AttachmentsList">
            <asp:PlaceHolder ID="plcUploader" runat="server">
                <cms:DirectFileUploader ID="newFileElem" runat="server" InsertMode="true" UploadMode="DirectSingle" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcUploaderDisabled" runat="server">
                <cms:LocalizedButton ResourceString="attach.uploadfile" EnableViewState="false" CssClass="btn btn-default btn-disabled" runat="server"/> 
            </asp:PlaceHolder>
            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
            <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
            <asp:Panel ID="pnlGrid" runat="server">
                <cms:UniGrid ID="gridAttachments" runat="server" GridName="~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaFileUpload.xml" />
            </asp:Panel>
            <div>
                <asp:Button ID="hdnPostback" CssClass="HiddenButton" runat="server" EnableViewState="false" />
                <asp:HiddenField ID="hdnFileName" runat="server" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Button ID="hdnFullPostback" CssClass="HiddenButton" runat="server" EnableViewState="false" />
