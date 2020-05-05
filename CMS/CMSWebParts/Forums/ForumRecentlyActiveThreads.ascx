<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Forums_ForumRecentlyActiveThreads"  Codebehind="~/CMSWebParts/Forums/ForumRecentlyActiveThreads.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Forums.Web.UI" Assembly="CMS.Forums.Web.UI" %>
<cms:BasicRepeater runat="server" ID="repLatestPosts" />
<cms:ForumPostsDataSource runat="server" ID="forumDataSource" />
