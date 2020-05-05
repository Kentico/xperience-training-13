<%@ Page Title="Post detail" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_Pages_FacebookPostDetailDialog"  Codebehind="FacebookPostDetailDialog.aspx.cs" Theme="Default" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlPostDetail" CssClass="sm-post-detail form-horizontal">
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="sm.facebook.post"></cms:LocalizedHeading>
        <div class="form-group">
            <asp:Label runat="server" ID="lblPostStatus"></asp:Label>
            <cms:CMSPanel runat="server" ID="pnlCampaign">
                <cms:LocalizedLabel runat="server" ResourceString="sm.facebook.posts.campaign" DisplayColon="True"></cms:LocalizedLabel>
                <asp:Label runat="server" ID="lblCampaign"></asp:Label>
            </cms:CMSPanel>
        </div>
        <div class="form-group">
            <asp:Label runat="server" ID="lblPostText"></asp:Label>
        </div>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="sm.facebook.posts.detail.stats"></cms:LocalizedHeading>
        <div class="form-group">
            <div>
                <asp:Label runat="server" ID="lblPeopleReachedValue"></asp:Label>
                <%=GetString("sm.facebook.posts.detail.reach") %>
            </div>
            <div class="sm-related-margin-top">
                <div class="sm-three-columns">
                    <asp:Label runat="server" ID="lblPostLikesValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.likes") %></div>
                <div class="sm-three-columns">
                    <asp:Label runat="server" ID="lblPostCommentsValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.comments") %></div>
                <div class="sm-three-columns">
                    <asp:Label runat="server" ID="lblPostSharesValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.shares") %></div>
                <div class="clearfix"></div>
            </div>

        </div>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="sm.facebook.posts.detail.total"></cms:LocalizedHeading>
        <div class="form-group">
            <div>
                <div class="sm-two-columns">
                    <asp:Label runat="server" ID="lblTotalLikesValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.likes") %></div>
                <div class="sm-two-columns">
                    <asp:Label runat="server" ID="lblTotalCommentsValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.comments") %></div>
            </div>
            <div class="clearfix"></div>
        </div>
        <cms:LocalizedHeading runat="server" Level="4" ResourceString="sm.facebook.posts.detail.negative"></cms:LocalizedHeading>
        <div class="form-group">
            <div class="sm-two-columns">
                <div>
                    <asp:Label runat="server" ID="lblHidePostValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.hidepost") %></div>
                <div class="sm-related-margin-top">
                    <asp:Label runat="server" ID="lblReportSpamValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.reportspam") %></div>
            </div>
            <div class="sm-two-columns">
                <div>
                    <asp:Label runat="server" ID="lblHideAllPostsValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.hideallposts") %></div>
                <div class="sm-related-margin-top">
                    <asp:Label runat="server" ID="lblUnlikePageValue"></asp:Label>
                    <%=GetString("sm.facebook.posts.detail.unlikepage") %></div>
            </div>
            <div class="clearfix"></div>
        </div>
    </asp:Panel>
</asp:Content>


