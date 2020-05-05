<%@ Page Language="C#" AutoEventWireup="false"  Codebehind="InsightsReport.aspx.cs" Inherits="CMSModules_SocialMarketing_Pages_InsightsReport" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx" TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader" TagPrefix="cms" %>
<asp:Content runat="server" ContentPlaceHolderID="plcContent">
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
    <div class="header-panel">
        <cms:GraphType runat="server" ID="ucGraphType" />
    </div>
    <div class="ReportBody">
        <asp:PlaceHolder runat="server" ID="pnlDisplayReport" />
    </div>
</asp:Content>
