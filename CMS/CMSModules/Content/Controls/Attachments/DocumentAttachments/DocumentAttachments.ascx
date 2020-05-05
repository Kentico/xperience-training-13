<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Attachments_DocumentAttachments_DocumentAttachments"  Codebehind="DocumentAttachments.ascx.cs" %>
<cms:BasicRepeater ID="ucRepeater" runat="server" />
<cms:AttachmentsDataSource ID="ucDataSource" runat="server" />
<div class="Pager">
    <cms:UniPager ID="ucPager" runat="server" />
</div>