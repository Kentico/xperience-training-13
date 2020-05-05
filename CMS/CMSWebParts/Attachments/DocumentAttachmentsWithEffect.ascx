<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Attachments_DocumentAttachmentsWithEffect"  Codebehind="~/CMSWebParts/Attachments/DocumentAttachmentsWithEffect.ascx.cs" %>

<cms:AttachmentsDataSource ID="ucDataSource" runat="server" />
<cms:BasicRepeaterWithEffect ID="ucRepeater" runat="server" />
<cms:UniPager ID="ucPager" runat="server" />

