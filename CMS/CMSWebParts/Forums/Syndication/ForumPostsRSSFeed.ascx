<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Forums_Syndication_ForumPostsRSSFeed"  Codebehind="~/CMSWebParts/Forums/Syndication/ForumPostsRSSFeed.ascx.cs" %>

<%@ Register TagPrefix="cms" Namespace="CMS.Forums.Web.UI" Assembly="CMS.Forums.Web.UI" %>
<cms:ForumPostsDataSource runat="server" ID="srcElem" />
<cms:RSSFeed runat="server" ID="rssFeed" />
