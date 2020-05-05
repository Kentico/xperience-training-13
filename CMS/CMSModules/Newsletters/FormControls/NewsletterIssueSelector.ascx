<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_FormControls_NewsletterIssueSelector"  Codebehind="NewsletterIssueSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="newsletter-issue-selector">
            <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter" SelectionMode="SingleDropDownList" AllowEmpty="true" ResourcePrefix="newsletterselect" />
            <cms:UniSelector ID="usIssues" runat="server" IsLiveSite="false" ObjectType="newsletter.issue" SelectionMode="SingleDropDownList" AllowEmpty="true" ResourcePrefix="newsletterissueselect" AdditionalColumns="IssueSubject,IssueIsABTest,IssueVariantName,IssueVariantOfIssueID" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
