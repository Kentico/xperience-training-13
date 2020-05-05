<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_MetaFiles_FileList"
     Codebehind="FileList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="updPanel" runat="server">
    <ContentTemplate>
        <%-- Uploaders --%>
        <div class="content-block-50">
            <asp:PlaceHolder ID="plcUploader" runat="server">
                <cms:CMSFileUpload ID="uploader" runat="server" />
                <cms:CMSButton ID="btnUpload" runat="server" OnClick="btnUpload_Click" ButtonStyle="Default"
                    EnableViewState="false" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcDirectUploder" runat="server">
                <cms:DirectFileUploader ID="newMetafileElem" runat="server" ShortID="dfu" InsertMode="true" UploadMode="DirectSingle" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcUploaderDisabled" runat="server">
                <cms:LocalizedButton ResourceString="attach.uploadfile" runat="server" class="btn btn-default" Enabled="False" EnableViewState="false"></cms:LocalizedButton>
                <div class="Clear"></div>
            </asp:PlaceHolder>
        </div>
        <asp:Label ID="lblError" runat="server" Visible="false" CssClass="ErrorLabel" EnableViewState="false" />
        <%-- Meta files list --%>
        <div class="content-block-50">
            <asp:PlaceHolder runat="server" ID="plcGridFiles" Visible="true">
                <cms:UniGrid runat="server" ID="gridFiles" ShortID="g" GridName="~/CMSModules/AdminControls/Controls/MetaFiles/FileList.xml"
                    OrderBy="MetaFileName" Columns="MetaFileID,MetaFileGUID,MetaFileObjectType,MetaFileObjectID,MetaFileGroupName,MetaFileName,MetaFileExtension,MetaFileSize,MetaFileImageWidth,MetaFileImageHeight,MetaFileTitle,MetaFileDescription" />
            </asp:PlaceHolder>
            <asp:Button ID="hdnPostback" runat="server" CssClass="HiddenButton" EnableViewState="false"
                OnClick="hdnPostback_Click" />
            <asp:HiddenField ID="hdnField" runat="server" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Button ID="hdnFullPostback" CssClass="HiddenButton" runat="server" EnableViewState="false"
    OnClick="hdnPostback_Click" />