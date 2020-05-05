<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="NewPage.aspx.cs" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_NewPage"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ABVariant/NewPage.ascx"
    TagName="NewPage" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSABTestingEnabled" />
    </asp:Panel>
    <cms:NewPage runat="server" ID="ucNewPage" />
</asp:Content>
