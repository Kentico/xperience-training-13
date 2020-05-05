<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Ab test properties" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_New"
    Theme="Default"  Codebehind="New.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/AbTest/Edit.ascx" TagName="AbTestEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSABTestingEnabled"/>        
    </asp:Panel>
    <cms:AbTestEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
