<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Blogs_Syndication_BlogCommentsRSSFeed"  Codebehind="~/CMSWebParts/Blogs/Syndication/BlogCommentsRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Blogs.Web.UI" Assembly="CMS.Blogs.Web.UI" %>
<cms:BlogCommentDataSource ID="srcComments" runat="server" />
<cms:RSSFeed ID="rssFeed" runat="server" />
