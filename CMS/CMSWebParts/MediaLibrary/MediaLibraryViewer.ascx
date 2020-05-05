<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MediaLibrary_MediaLibraryViewer"  Codebehind="~/CMSWebParts/MediaLibrary/MediaLibraryViewer.ascx.cs" %>
<%@ Register Assembly="CMS.MediaLibrary.Web.UI" Namespace="CMS.MediaLibrary.Web.UI" TagPrefix="cms" %>
<cms:BasicRepeater ID="repMediaLib" runat="server" />
<cms:MediaLibraryDataSource ID="srcMediaLib" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
