<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Controls_Forums_ForumGroupList"
     Codebehind="ForumGroupList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumPostsWaitingForApproval.ascx"
    TagName="ApprovePosts" TagPrefix="cms" %>
<asp:Panel ID="pnlGroups" runat="server">
    <cms:UniGrid runat="server" ID="gridGroups" GridName="~/CMSModules/Forums/Controls/Forums/Group_List.xml"
        OrderBy="GroupOrder" DelayedReload="true" />
</asp:Panel>
<br />
<%--Approve/Reject--%>
<asp:Panel runat="server" ID="pnlApprove">
    <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader">
        <cms:PageTitle runat="server" ID="titleElem" TitleCssClass="SubTitleHeader" />
    </asp:Panel>
    <asp:Panel ID="pnlApproveRejectGrid" runat="server">
        <br />
        <br />
        <cms:ApprovePosts ID="ucPostApprove" runat="server" />
    </asp:Panel>
</asp:Panel>
