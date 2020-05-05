<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Community_Forums_GroupForumSearchResults"  Codebehind="~/CMSWebParts/Community/Forums/GroupForumSearchResults.ascx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumDivider.ascx" TagName="ForumFlatView"
    TagPrefix="cms" %>
<cms:ForumFlatView ID="forumElem" runat="server" />
<asp:Label ID="lblNoResults" runat="server" Visible="false" EnableViewState="false" />
