<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Templates_Tab_Newsletters"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Newsletter template edit - Newsletters"
     Codebehind="Tab_Newsletters.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="newslettertemplate.pernewslettertemplate"
        CssClass="listing-title" DisplayColon="true" EnableViewState="false" />
    <cms:CMSUpdatePanel runat="server" ID="pnlAvailability">
        <ContentTemplate>
            <cms:AlertLabel runat="server" AlertType="Error" Visible="False" ID="lblErrorMessage"></cms:AlertLabel>
            <cms:UniSelector ID="usNewsletters" runat="server" IsLiveSite="false" ObjectType="Newsletter.Newsletter"
                SelectionMode="Multiple" ResourcePrefix="newsletterselect" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
