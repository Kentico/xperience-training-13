<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="UserSubscriptionList.ascx.cs"
    Inherits="CMSModules_Forums_Controls_Subscriptions_UserSubscriptionList" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" LiveSiteOnly="true" />
<asp:Panel runat="server" ID="pnlSubscriptions" Visible="false">
    <cms:LocalizedLabel ID="lblMessage" runat="server" CssClass="InfoLabel" EnableViewState="false"
        ResourceString="forumsubscripitons.userissubscribed" />
    <cms:UniGrid ID="forumSubscriptions" runat="server" FilterLimit="10" ExportFileName="forum_subscription">
        <GridActions>
            <ug:Action Name="ForumUnsubscribe" Caption="Unsubscribe" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$forum.confirmunsubscribe$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ForumDisplayName" Caption="$unigrid.forums.columns.forumname$" CssClass="main-column-100"
                Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
    <asp:Panel ID="pnlSeparator" runat="server" Visible="true">
        <br />
    </asp:Panel>
    <cms:LocalizedLabel ID="lblMessagePost" runat="server" CssClass="InfoLabel" EnableViewState="false"
        ResourceString="forumpostsubscripitons.userissubscribed" />
    <cms:UniGrid ID="postSubscription" runat="server" FilterLimit="10" ExportFileName="forumpost_subscription">
        <GridActions Parameters="PostID">
            <ug:Action Name="PostUnsubscribe" Caption="Unsubscribe" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$forumpost.confirmunsubscribe$" />
            <ug:Action Name="View" Caption="$forumpost.postpreview$" FontIconClass="icon-eye" FontIconStyle="Allow" OnClick="var ret = true; if (window.ForumPostUnsubscribe) {ret = window.ForumPostUnsubscribe({0});} if (ret == false) {return false;} " />
        </GridActions>
        <GridColumns>
            <ug:Column Source="PostForumID" Caption="$unigrid.forums.columns.forumname$" Wrap="false"
                ExternalSourceName="ForumName" Localize="true">
            </ug:Column>
            <ug:Column Source="PostSubject" Caption="$Forums.PostListing.PostSubject$" CssClass="main-column-100"
                Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Panel>
<asp:Panel runat="server" ID="pnlNoSubscription" Visible="false">
    <cms:LocalizedLabel ID="forumsNoData" CssClass="InfoLabel" runat="server" ResourceString="forumsubscripitons.userhasnosubscriptions" />
</asp:Panel>
