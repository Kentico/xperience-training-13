<%@ Control Language="C#" AutoEventWireup="true"  Inherits="CMSApp.CMSFormControls.MedioClinic.NewsletterLinkSelector" Codebehind="NewsletterLinkSelector.ascx.cs"
     %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <div class="form-group">Newsletter: 
            <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter" SelectionMode="SingleDropDownList" 
            AllowEmpty="true" 
            ResourcePrefix="newsletterselect" /></div>
        <div class="form-group">Issue: 
            <cms:UniSelector ID="usIssues" runat="server" IsLiveSite="false" ObjectType="Newsletter.Issue" SelectionMode="SingleDropDownList" AllowEmpty="true" ResourcePrefix="NewsletterIssueSelect" AdditionalColumns="IssueSubject,IssueIsABTest,IssueVariantName,IssueVariantOfIssueID" OrderBy="IssueMailoutTime DESC" /></div>
        <div class="form-group">Link: 
            <cms:UniSelector ID="usLinks" runat="server" IsLiveSite="false" ObjectType="Newsletter.Link" SelectionMode="SingleDropDownList" AllowEmpty="true" ResourcePrefix="NewsletterIssueSelect" AdditionalColumns="LinkTarget,LinkDescription" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>