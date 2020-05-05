<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Community_Groups_GroupsViewer"  Codebehind="~/CMSWebParts/Community/Groups/GroupsViewer.ascx.cs" %>
<%@ Register Src="~/CMSWebparts/Community/Groups/GroupsFilter_files/GroupsFilterControl.ascx"
    TagName="GroupsFilterControl" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.Community.Web.UI" Assembly="CMS.Community.Web.UI" %>
<cms:GroupsFilterControl ID="filterGroups" runat="server" />
<cms:BasicRepeater ID="repGroups" runat="server" />
<cms:GroupsDataSource ID="srcGroups" runat="server" />
<div class="Pager">
    <cms:UniPager ID="pagerElem" runat="server" />
</div>
