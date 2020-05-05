<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="AB test list" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_List"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/AbTest/List.ascx" TagName="AbTestList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule"/>
    </asp:Panel>
    <cms:SmartTip runat="server" ID="tipHowToListing" Visible="true" />
    <cms:AbTestList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
