<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Attachments_DirectFileUploader_DirectFileUploaderControl"
     Codebehind="DirectFileUploaderControl.ascx.cs" %>
<div id="uploaderDiv" style="opacity: 0; filter: alpha(opacity=0);">
    <cms:CMSFileUpload ID="ucFileUpload" runat="server" CssClass="dont-check-changes" />
</div>
<asp:Button ID="btnHidden" runat="server" OnClick="btnHidden_Click" EnableViewState="false" />