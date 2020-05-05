<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/Forums/Controls/Forums/ForumPostsWaitingForApproval.ascx.cs"
    Inherits="CMSModules_Forums_Controls_Forums_ForumPostsWaitingForApproval" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlApprove">
    <asp:Panel ID="pnlApproveRejectGrid" runat="server">
        <asp:Label runat="server" ID="lblInfo" Visible="false" EnableViewState="false" />
        <cms:UniGrid runat="server" ID="gridApprove" GridName="~/CMSModules/Forums/Controls/Forums/Approve_List.xml"
            DelayedReload="true" OrderBy="PostID" />
    </asp:Panel>
</asp:Panel>
