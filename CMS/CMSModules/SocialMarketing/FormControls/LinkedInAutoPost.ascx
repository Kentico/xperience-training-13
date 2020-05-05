<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_LinkedInAutoPost"  Codebehind="LinkedInAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>
<%@ Register Src="~/CMSModules/SocialMarketing/FormControls/AvailableUrlShortenerSelector.ascx" TagPrefix="cms" TagName="UrlShortenerSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
            <cms:CMSCheckBox runat="server" ID="chkPostToLinkedIn" AutoPostBack="True" OnCheckedChanged="chkPostToLinkedIn_OnCheckedChanged" ResourceString="sm.linkedin.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="companySelector" ResourceString="sm.linkedin.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:UniSelector runat="server" ID="companySelector" ObjectType="sm.linkedinaccount" ReturnColumnName="LinkedInAccountID" SelectionMode="SingleDropDownList" AllowEmpty="False" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.linkedin.autopost.profiletooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.linkedin.autopost.profiletooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="txtPost" ResourceString="sm.linkedin.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:CMSTextArea runat="server" ID="txtPost" Rows="8" />
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.linkedin.autopost.macrohint"))%>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="urlShortenerSelector" ResourceString="sm.linkedin.posts.urlshortener" DisplayColon="True" />
                </div>
                <div>
                    <cms:UrlShortenerSelector runat="server" ID="urlShortenerSelector" SocialNetworkName="LinkedIn" />
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.linkedin.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.linkedin.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.linkedin.posts.campaign" DisplayColon="True" />
                </div>
                <div>
                    <cms:UniSelector runat="server" ID="campaingSelector" ObjectType="Analytics.Campaign" ReturnColumnName="CampaignID" SelectionMode="SingleDropDownList" AllowEmpty="True" OrderBy="CampaignDisplayName" />
                </div>
            </cms:CMSPanel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
