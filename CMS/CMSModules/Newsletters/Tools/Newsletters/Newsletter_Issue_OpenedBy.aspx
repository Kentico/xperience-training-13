<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_OpenedBy"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Tools - Newsletter issue opened emails"  CodeBehind="Newsletter_Issue_OpenedBy.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/OpenedByFilter.ascx" TagPrefix="cms"
    TagName="OpenedByFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:OpenedByFilter runat="server" ID="fltOpenedBy" ShortID="f" ShowDateFilter="true" />
    <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" IsLiveSite="false" ObjectType="newsletter.openedemail"
        ShowActionsMenu="true" OrderBy="OpenedEmailTime DESC" Columns="OpenedEmailEmail, OpenedEmailTime, OpenedEmailIssueID">
        <GridColumns>
            <ug:Column Source="OpenedEmailEmail" ExternalSourceName="email" Caption="$general.email$"
                Wrap="false" AllowSorting="True" CssClass="main-column-100" />
            <ug:Column Source="OpenedEmailTime" Caption="$unigrid.newsletter_issue_openedby.columns.openedwhen$"
                Wrap="false" AllowSorting="True" />
            <ug:Column Source="OpenedEmailIssueID" Caption="$newsletterissue_send.variantname$" Wrap="false" Name="variants"
                ExternalSourceName="variantname" AllowSorting="false" />
        </GridColumns>
    </cms:UniGrid>
</asp:Content>
