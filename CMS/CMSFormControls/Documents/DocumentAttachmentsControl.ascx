<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="DocumentAttachmentsControl.ascx.cs"
    Inherits="CMSFormControls_Documents_DocumentAttachmentsControl" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DocumentAttachmentsList.ascx"
    TagName="DocumentAttachmentsList" TagPrefix="cms" %>
<cms:DocumentAttachmentsList ID="documentAttachments" runat="server" />
