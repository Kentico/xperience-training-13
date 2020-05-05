<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="MVTest list" Inherits="CMSModules_OnlineMarketing_Pages_Content_MVTest_List"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/Mvtest/List.ascx" TagName="MvtestList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSMVTEnabled" />
    </asp:Panel>
    <cms:MvtestList ID="listElem" runat="server" IsLiveSite="false" ShowPageColumn="false" EditPage="Frameset.aspx" />
</asp:Content>
