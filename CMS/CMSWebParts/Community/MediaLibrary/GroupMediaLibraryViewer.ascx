<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_MediaLibrary_GroupMediaLibraryViewer"  Codebehind="~/CMSWebParts/Community/MediaLibrary/GroupMediaLibraryViewer.ascx.cs" %>
<%@ Register Assembly="CMS.MediaLibrary.Web.UI" Namespace="CMS.MediaLibrary.Web.UI" TagPrefix="cms" %>
<cms:BasicRepeater ID="repLibraries" runat="server" />
<cms:MediaLibraryDataSource ID="srcLib" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
