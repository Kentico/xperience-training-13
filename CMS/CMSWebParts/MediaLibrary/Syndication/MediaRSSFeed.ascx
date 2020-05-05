<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_MediaLibrary_Syndication_MediaRSSFeed"  Codebehind="~/CMSWebParts/MediaLibrary/Syndication/MediaRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.MediaLibrary.Web.UI" Assembly="CMS.MediaLibrary.Web.UI" %>

<cms:MediaFileDataSource ID="srcMedia" runat="server" />
<cms:RSSFeed runat="server" ID="rssFeed" />
