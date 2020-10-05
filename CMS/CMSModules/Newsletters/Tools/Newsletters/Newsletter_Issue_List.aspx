<%@ Page Language="C#" AutoEventWireup="false" Title="Tools - Newsletter issues"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="Newsletter_Issue_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:AlertLabel runat="server" ID="lblUrlWarning" AlertType="Warning" Visible="False" CssClass="hide" EnableViewState="false" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" IsLiveSite="false" OrderBy="IssueStatus, CASE WHEN IssueMailoutTime IS NULL THEN 0 ELSE 1 END, IssueMailoutTime DESC"
        ObjectType="newsletter.issue" Columns="IssueID, IssueDisplayName, IssueMailoutTime, IssueSentEmails, IssueOpenedEmails, IssueUnsubscribed, IssueBounces, IssueIsABTest, IssueStatus, IssueVariantOfIssueID, IssueForAutomation"
        RememberStateByParam="">
        <GridActions Parameters="IssueID">
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="##ALL##" ExternalSourceName="IssueDisplayName" Caption="$unigrid.newsletter_issue.columns.issuedisplayname$" Wrap="false">
                <Filter Type="text" Source="IssueDisplayName" />
            </ug:Column>
            <ug:Column Source="IssueStatus" Caption="$newsletters.issuestatus$" ExternalSourceName="IssueStatus" Wrap="false" />
            <ug:Column Source="IssueMailoutTime" Caption="$unigrid.newsletter_issue.columns.issuemailouttime$" Wrap="false" />
            <ug:Column Source="IssueSentEmails" Caption="$unigrid.newsletter_issue.columns.issuesentemails$" Wrap="false" CssClass="TableCell" ExternalSourceName="IssueSentEmails" />

            <ug:Column Source="##ALL##" Caption="$newsletter.issue.delivered$" Wrap="false" CssClass="TableCell" ExternalSourceName="Delivered" Name="delivered" />
            <ug:Column Source="##ALL##" Caption="$newsletters.issuedeliveryrate$" Wrap="false" CssClass="TableCell" ExternalSourceName="DeliveryRate" Name="deliveryrate" />

            <ug:Column Source="##ALL##" ExternalSourceName="IssueOpenedEmails" Caption="$newsletters.issueopens$" Wrap="false" CssClass="TableCell" Name="openedemails" />
            <ug:Column Source="##ALL##" ExternalSourceName="IssueOpenedEmailsRate" Caption="$newsletters.issueopenrate$" Wrap="false" CssClass="TableCell" Name="openedemailsrate" />

            <ug:Column Source="##ALL##" ExternalSourceName="IssueClickedLinks" Caption="$newsletters.issueclicks$" Wrap="false" CssClass="TableCell" Name="issueclickedlinks" />
            <ug:Column Source="##ALL##" ExternalSourceName="IssueClickedLinksRate" Caption="$newsletters.issueclickrate$" Wrap="false" CssClass="TableCell" Name="issueclickedlinksrate" />

            <ug:Column Source="##ALL##" Caption="$newsletter.issue.unsubscriptions$" Wrap="false" CssClass="TableCell" ExternalSourceName="unsubscribtions" />
            <ug:Column Source="##ALL##" Caption="$newsletters.issueunsubscriberate$" Wrap="false" CssClass="TableCell" ExternalSourceName="unsubscriberate" />

            <ug:Column Source="IssueForAutomation" Caption="$newsletters.issueforautomation$" Wrap="false" CssClass="TableCell" ExternalSourceName="#yesno" AllowSorting="false">
                <Filter Type="bool" Source="IssueForAutomation" />
            </ug:Column>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
