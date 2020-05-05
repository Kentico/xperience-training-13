<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_AbuseReport_Dialogs_ReportAbuse"
    Title="Report abuse" Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"  Codebehind="ReportAbuse.aspx.cs" %>

<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportEdit.ascx" TagName="AbuseReportEdit"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AbuseReportEdit ID="reportElem" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnReport" runat="server" ResourceString="abuse.reportabuse"
        ButtonStyle="Primary" EnableViewState="false" />
</asp:Content>