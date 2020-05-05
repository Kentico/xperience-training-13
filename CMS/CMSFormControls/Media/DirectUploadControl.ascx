<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DirectUploadControl.ascx.cs"
    Inherits="CMSFormControls_Media_DirectUploadControl" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DirectUploader.ascx"
    TagName="DirectUploader" TagPrefix="cms" %>
<cms:DirectUploader ID="directUpload" runat="server" />
