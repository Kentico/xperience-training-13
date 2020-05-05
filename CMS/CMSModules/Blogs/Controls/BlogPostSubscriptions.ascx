<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Blogs_Controls_BlogPostSubscriptions"  Codebehind="BlogPostSubscriptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" LiveSiteOnly="true" />
<div class="BlogPostSubscriptions">
    <cms:LocalizedLabel ID="lblMessage" runat="server" CssClass="InfoLabel" EnableViewState="false"
        ResourceString="blogsubscripitons.userissubscribed" />
    <cms:UniGrid ID="gridElem" runat="server" GridName="~/CMSModules/Blogs/Controls/BlogPostSubscriptions.xml"
        FilterLimit="10" OrderBy="SubscriptionEmail" ExportFileName="blog_postsubscription" />
</div>
