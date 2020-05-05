<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_AbuseReport_AbuseReport_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="AbuseReport_List.aspx.cs" %>
<%@ Register Src="~/CMSModules/AbuseReport/Controls/AbuseReportList.ascx" TagName="AbuseReportList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:AbuseReportList ID="ucAbuseReportList" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
