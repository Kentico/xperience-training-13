<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_MessageBoards_Syndication_MessageBoardRSSFeed"  Codebehind="~/CMSWebParts/MessageBoards/Syndication/MessageBoardRSSFeed.ascx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.MessageBoards.Web.UI" Assembly="CMS.MessageBoards.Web.UI" %>

<cms:BoardMessagesDataSource ID="srcMessages" runat="server" />
<cms:RSSFeed runat="server" ID="rssFeed" />
