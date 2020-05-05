<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SocialMarketing_FormControls_FacebookAutoPost"  Codebehind="FacebookAutoPost.ascx.cs" %>
<%@ Import Namespace="CMS.MacroEngine" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>
<%@ Register Src="~/CMSModules/SocialMarketing/FormControls/AvailableUrlShortenerSelector.ascx" TagPrefix="cms" TagName="UrlShortenerSelector" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-horizontal">
            <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
            <cms:CMSCheckBox runat="server" ID="chkPostToFacebook" AutoPostBack="True" OnCheckedChanged="chkPostToFacebook_OnCheckedChanged" ResourceString="sm.facebook.autopost.createpost" />
            <cms:CMSPanel runat="server" ID="pnlForm" Visible="False">
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="pageSelector" ResourceString="sm.facebook.posts.account" DisplayColon="True" />
                </div>
                <div class="control-group-inline-forced">
                    <cms:UniSelector runat="server" ID="pageSelector" ObjectType="sm.facebookaccount" ReturnColumnName="FacebookAccountID" SelectionMode="SingleDropDownList" AllowEmpty="False" />
                    <span class="info-icon">
                        <cms:LocalizedLabel runat="server" ToolTipResourceString="sm.facebook.autopost.pagetooltip" CssClass="sr-only"></cms:LocalizedLabel>
                        <i aria-hidden="true" class="icon-question-circle" title="<%=GetString("sm.facebook.autopost.pagetooltip") %>"></i>
                    </span>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="txtPost" ResourceString="sm.facebook.posts.content" DisplayColon="True" />
                </div>
                <div>
                    <cms:CMSTextArea runat="server" ID="txtPost" Rows="8" />
                    <div class="sm-related-margin-top form-text">
                        <% = MacroResolver.Resolve(GetString("sm.facebook.autopost.macrohint"))%>
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="urlShortenerSelector" ResourceString="sm.facebook.posts.urlshortener" DisplayColon="True" />
                </div>
                <div>
                    <cms:UrlShortenerSelector runat="server" ID="urlShortenerSelector" SocialNetworkName="Facebook" />
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="publishDateTime" ResourceString="sm.facebook.posts.scheduledpublish" DisplayColon="True" />
                </div>
                <div>
                    <div>
                        <cms:FormControl runat="server" ID="publishDateTime" FormControlName="CalendarControl" />
                    </div>
                    <div class="sm-related-margin-top">
                        <cms:CMSCheckBox runat="server" ID="chkPostAfterDocumentPublish" ResourceString="sm.facebook.autopost.postondocumentpublish" AutoPostBack="true" OnCheckedChanged="chkPostAfterDocumentPublish_OnCheckedChanged" />
                    </div>
                </div>
                <div class="sm-inner-control-label">
                    <cms:LocalizedLabel runat="server" AssociatedControlID="campaingSelector" ResourceString="sm.facebook.posts.campaign" DisplayColon="True" />
                </div>
                <div>
                    <cms:UniSelector runat="server" ID="campaingSelector" ObjectType="Analytics.Campaign" ReturnColumnName="CampaignID" SelectionMode="SingleDropDownList" AllowEmpty="True" OrderBy="CampaignDisplayName" />
                </div>
            </cms:CMSPanel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
