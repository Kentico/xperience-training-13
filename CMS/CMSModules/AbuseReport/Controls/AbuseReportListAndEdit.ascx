<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AbuseReport/Controls/AbuseReportListAndEdit.ascx.cs"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportListAndEdit" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportList.ascx" TagName="AbuseReportList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportStatusEdit.ascx" TagName="AbuseReportEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Panel ID="pnlHeader" runat="server" Visible="false">
    <cms:Breadcrumbs id="ucBreadcrumbs" runat="server" EnableViewState="false" HideBreadcrumbs="false" PropagateToMainNavigation="false" />
</asp:Panel>
<cms:abusereportedit id="ucAbuseEdit" runat="server" visible="false" />
<cms:abusereportlist id="ucAbuseReportList" runat="server" />
<asp:HiddenField runat="server" ID="hfEditReport" />
<asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
