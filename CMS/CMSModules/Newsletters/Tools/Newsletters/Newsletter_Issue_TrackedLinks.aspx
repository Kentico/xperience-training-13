<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_TrackedLinks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Tools - Link tracking"  CodeBehind="Newsletter_Issue_TrackedLinks.aspx.cs" %>

<%@ Register Src="~/CMSModules/Newsletters/Controls/IssueLinks.ascx" TagName="IssueLinks" TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSPanel ID="pnlContent" runat="server">
        <cms:IssueLinks runat="server" ID="issueLinks" ShowFilter="true" IncludeWinnerStatistics="true" AllowHidingClickRateColumnForAbTest="true" />
    </cms:CMSPanel>
</asp:Content>
