<%@ Control Language="C#" Inherits="CMS.DocumentEngine.Web.UI.CMSAbstractTransformation" %>
<%@ Import Namespace="CMS.UIControls" %>

<div class="attachment-main">
    <%= DataHelper.GetSizeString(((AttachmentWithVariants)DataItem).Attachment.AttachmentSize) %>
</div>
<% if (((AttachmentWithVariants)DataItem).Variants.Any()) { %>
<div class="attachment-variants attachment-variants-noheader">
    <ul>
        <% foreach (var variant in ((AttachmentWithVariants)DataItem).Variants) { %>
        <li><%= DataHelper.GetSizeString(variant.AttachmentSize) %></li>
        <% } %>
    </ul>
</div>
<% } %>