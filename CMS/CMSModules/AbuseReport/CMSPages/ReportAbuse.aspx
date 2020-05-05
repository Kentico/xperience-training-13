<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_AbuseReport_CMSPages_ReportAbuse"
    Title="Report abuse" Theme="default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"  Codebehind="ReportAbuse.aspx.cs" %>

<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportEdit.ascx" TagName="AbuseReportEdit"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:AbuseReportEdit ID="reportElem" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnReport" runat="server" ResourceString="abuse.reportabuse"
           ButtonStyle="Primary" EnableViewState="false" />
    </div>
</asp:Content>