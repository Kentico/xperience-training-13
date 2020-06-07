<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Automation process – Contacts" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts"
    Theme="Default" Codebehind="Tab_Contacts.aspx.cs" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/Contacts.ascx" TagName="Contacts" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactSelector.ascx" TagName="ContactSelector" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="analytics-main-content">
        <div class="control-group-inline">
            <cms:ContactSelector ID="ucSelector" runat="server" Enabled="false" IsLiveSite="false" />
        </div>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:Contacts ID="listContacts" runat="server" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
    <div class="analytics-right-panel">
        <cms:CMSUpdatePanel ID="pnlUpdateChart" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:CMSButton runat="server" ID="btnDemographics" EnableViewState="false" ButtonStyle="Default" />
                <br />
                <cms:LocalizedHeading runat="server" ID="lblChartHeading" EnableViewState="false" Level="3" />
                <span class="info-icon">
                    <cms:CMSIcon ID="iconHelp" runat="server" CssClass="icon-question-circle" EnableViewState="false" data-html="true" Visible="false" />
                </span>
                <asp:Panel ID="pnlChart" runat="server" CssClass="analytics-chart" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>