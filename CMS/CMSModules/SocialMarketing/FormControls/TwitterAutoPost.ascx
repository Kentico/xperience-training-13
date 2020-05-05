<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_TwitterAutoPost"  Codebehind="TwitterAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagPrefix="cms" tagName="UniSelector" %>
<%@ Register src="~/CMSModules/SocialMarketing/FormControls/AvailableUrlShortenerSelector.ascx" tagPrefix="cms" tagName="UrlShortenerSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
            <cms:CMSCheckBox runat="server" ID="chkPostToTwitter" AutoPostBack="True" OnCheckedChanged="chkPostToTwitter_OnCheckedChanged" ResourceString="sm.twitter.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="channelSelector" ResourceString="sm.twitter.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:UniSelector runat="server" ID="channelSelector" ObjectType="sm.twitteraccount" ReturnColumnName="TwitterAccountID" SelectionMode="SingleDropDownList" AllowEmpty="False" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.twitter.autopost.pagetooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.twitter.autopost.pagetooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="tweetContent" ResourceString="sm.twitter.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:FormControl runat="server" ID="tweetContent" FormControlName="SMTwitterPostTextArea" />
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.twitter.autopost.macrohint"))%>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="urlShortenerSelector" ResourceString="sm.twitter.posts.urlshortener" DisplayColon="True" />
                </div>
                <div>
                    <cms:UrlShortenerSelector runat="server" ID="urlShortenerSelector" SocialNetworkName="Twitter" />
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.twitter.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.twitter.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.twitter.posts.campaign" DisplayColon="True" />
                </div>
                <div>
                    <cms:UniSelector runat="server" ID="campaingSelector" ObjectType="Analytics.Campaign" ReturnColumnName="CampaignID" SelectionMode="SingleDropDownList" AllowEmpty="True" OrderBy="CampaignDisplayName"/>
                </div>
            </cms:CMSPanel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
