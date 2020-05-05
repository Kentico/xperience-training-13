<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_SubscribersClicks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Tools - Participated subscribers"  CodeBehind="Newsletter_Issue_SubscribersClicks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/OpenedByFilter.ascx" TagPrefix="cms"
    TagName="OpenedByFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:OpenedByFilter runat="server" ID="fltOpenedBy" ShortID="f" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" IsLiveSite="false"
        ShowActionsMenu="True">
        <GridColumns>
            <ug:Column Source="ClickedLinkEmail" Caption="$general.email$" 
                Wrap="false"/>
            <ug:Column Source="ClickCount" Caption="$unigrid.newsletter_issue_subscribersclicks.columns.clicks$"
                Wrap="false" CssClass="TableCell"/>
            <ug:Column CssClass="filling-column" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
