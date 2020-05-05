<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Forums_ForumPostsViewer"  Codebehind="~/CMSWebParts/Forums/ForumPostsViewer.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Forums.Web.UI" Assembly="CMS.Forums.Web.UI" %>
<cms:BasicRepeater runat="server" ID="repLatestPosts" />
<cms:ForumPostsDataSource runat="server" ID="forumDataSource" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
