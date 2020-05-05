<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Overview"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeBehind="Newsletter_Issue_Overview.aspx.cs" Title="Newsletter - Issue - Reports - Overview" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/IssueLinks.ascx" TagName="IssueLinks" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSPanel ID="pnlContent" runat="server" Visible="True">
        
        <div class="row">
            <div class="col-md-6">
                <cms:UIForm ID="issueForm" runat="server" ObjectType="newsletter.issue" AlternativeFormName="IssueOverview" FieldCaptionCellCssClass="editing-form-label-cell-narrow" FieldValueCellCssClass="editing-form-value-cell-narrow" MarkRequiredFields="False" />
            </div>
            <div class="col-md-6 overview-funnel-wrapper">
                <asp:Panel runat="server" ID="pnlFunnel" CssClass="overview-funnel" />
            </div>
        </div>
        
        <asp:Panel runat="server" ID="pnlDelivery">
            <cms:LocalizedHeading runat="server" Level="3" ResourceString="Delivery" />
            <cms:UniGrid runat="server" ID="ugDelivery" ShortID="dg" IsLiveSite="false" PageSize="0" ShowActionsMenu="False">
                <GridColumns>
                    <ug:Column Source="sent" Name="sent" Caption="$unigrid.newsletter_issue.columns.issuesentemails$" AllowSorting="False" CssClass="TableCell" />
                    <ug:Column Source="bounces" Name="bounces" Caption="$om.contact.bounces$" AllowSorting="False" CssClass="TableCell" />
                    <ug:Column Source="bouncerate" Name="bouncerate" ExternalSourceName="bouncerate" Caption="$newsletter.issue.bouncerate$" AllowSorting="False" CssClass="TableCell" />
                    <ug:Column Source="delivered" Name="delivered" Caption="$newsletter.issue.delivered$" AllowSorting="False" CssClass="TableCell" />
                    <ug:Column Source="deliveryrate" Name="deliveryrate" ExternalSourceName="deliveryrate" Caption="$newsletters.issuedeliveryrate$" AllowSorting="False" CssClass="TableCell" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
            </cms:UniGrid>
        </asp:Panel>

        <cms:LocalizedHeading runat="server" Level="3" ResourceString="newsletter.issue.engagement" />
        <cms:UniGrid runat="server" ID="ugEngagement" ShortID="g" IsLiveSite="false" PageSize="0" ShowActionsMenu="False">
            <GridColumns>
                <ug:Column Source="sent" Name="sent" Caption="$unigrid.newsletter_issue.columns.issuesentemails$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="delivered" Name="delivered" Caption="$newsletter.issue.delivered$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="deliveryrate" Name="deliveryrate" ExternalSourceName="deliveryrate" Caption="$newsletters.issuedeliveryrate$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="opens" Name="opens" ExternalSourceName="opens" Caption="$newsletter.issue.uniqueopens$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="openrate" Name="openrate" ExternalSourceName="openrate" Caption="$newsletter.issue.uniqueopenrate$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="totalclicks" Name="totalclicks" Caption="$unigrid.newsletter_issue_trackedlinks.columns.totalclicks$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="clicks" Name="clicks" ExternalSourceName="clicks" Caption="$unigrid.newsletter_issue_trackedlinks.columns.uniqueclicks$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="clickrate" Name="clickrate" ExternalSourceName="clickrate" Caption="$newsletter.issue.uniqueclickrate$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>
        
        <cms:LocalizedHeading runat="server" Level="3" ResourceString="Contact loss" />
        <cms:UniGrid runat="server" ID="ugContactLoss" ShortID="cg" IsLiveSite="false" PageSize="0" ShowActionsMenu="False">
            <GridColumns>
                <ug:Column Source="sent" Name="sent" Caption="$unigrid.newsletter_issue.columns.issuesentemails$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="delivered" Name="delivered" Caption="$newsletter.issue.delivered$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="deliveryrate" Name="deliveryrate" ExternalSourceName="deliveryrate" Caption="$newsletters.issuedeliveryrate$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="unsubscriptions" Name="unsubscriptions" ExternalSourceName="unsubscriptions" Caption="$newsletter.issue.unsubscriptions$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column Source="unsubscriptionrate" Name="unsubscriptionrate" ExternalSourceName="unsubscriptionrate" Caption="$newsletters.issueunsubscriberate$" AllowSorting="False" CssClass="TableCell" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
        </cms:UniGrid>

        <cms:LocalizedHeading runat="server" Level="3" ResourceString="Most clicks" />       
        <cms:IssueLinks runat="server" ID="issueLinks" IncludeAllVariants="True" TopN="5" DisplayOnlyClickedLinks="True"/>
        
        <div class="pull-right">
            <cms:LocalizedLinkButton ID="lnkAllLinks" ResourceString="newsletter.issue.showalllinks" runat="server" OnClick="lnkAllLinks_OnClick"/>
        </div>

    </cms:CMSPanel>
</asp:Content>
