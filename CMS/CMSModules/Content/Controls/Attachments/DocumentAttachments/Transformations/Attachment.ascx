<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Attachments_DocumentAttachments_Transformations_Attachment" CodeBehind="Attachment.ascx.cs" %>
<div class="attachment-main">
    <%= GetAttachmentHtml(Attachment.Attachment, null, Attachment.VersionHistoryID) %>
</div>
<% if (Attachment.Variants.Any()) { %>
<div class="attachment-variants">
    <cms:LocalizedHeading runat="server" ID="h" Level="4" ResourceString="ImageVariants.Heading" />
    <ul>
        <% foreach (var variant in Attachment.Variants) { %>
        <li><%= GetAttachmentHtml(variant, Attachment.Attachment, Attachment.VersionHistoryID) %></li>
        <% } %>
    </ul>
</div>
<% } %>
