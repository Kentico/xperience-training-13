<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_MultiFileUploader_MultiFileUploader"
     Codebehind="MultiFileUploader.ascx.cs" %>
<asp:Panel ID="pnlUpload" class="uploader-overlay-div" runat="server">
    <cms:CMSFileUpload ID="uploadFile" type="file" runat="server" CssClass="dont-check-changes" />
</asp:Panel>
