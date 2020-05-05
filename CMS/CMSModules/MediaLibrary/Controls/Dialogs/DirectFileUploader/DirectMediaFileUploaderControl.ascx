<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_Dialogs_DirectFileUploader_DirectMediaFileUploaderControl"
     Codebehind="DirectMediaFileUploaderControl.ascx.cs" %>
<div id="uploaderDiv" style="display: none;">
    <cms:CMSFileUpload ID="ucFileUpload" runat="server" />
</div>
<asp:Button ID="btnHidden" runat="server" OnClick="btnHidden_Click" EnableViewState="false" />
