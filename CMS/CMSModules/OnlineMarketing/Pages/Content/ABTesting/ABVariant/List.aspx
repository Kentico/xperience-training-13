<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Variant list" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_List"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/ABVariant/List.ascx" TagName="VariantList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSABTestingEnabled" />
    </asp:Panel>
    <cms:VariantList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
