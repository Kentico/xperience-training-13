<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Tools - Newsletters"  Codebehind="Newsletter_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagName="SmartTip" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:SmartTip runat="server" ID="tipHowEMWorks" Visible="true" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="NewsletterDisplayName" IsLiveSite="false"
        ObjectType="newsletter.newsletter" RememberStateByParam=""
        Columns="NewsletterID, NewsletterDisplayName, (SELECT MAX(IssueMailoutTime) FROM Newsletter_NewsletterIssue WHERE IssueNewsletterID = Newsletter_Newsletter.NewsletterID ) AS LastIssue, NewsletterType">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="NewsletterDisplayName" Caption="$Unigrid.Newsletter.Columns.NewsletterDisplayName$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="LastIssue" Caption="$Unigrid.Newsletter.Columns.LastIssue$" Wrap="false" AllowSorting="false" />
            <ug:Column Source="NewsletterType" Caption="$Unigrid.Newsletter.Columns.NewsletterType$" ExternalSourceName="type" Wrap="false" AllowSorting="false" Localize="True" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>