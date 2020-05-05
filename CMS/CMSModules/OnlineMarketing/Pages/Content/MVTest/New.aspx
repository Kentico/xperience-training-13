<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_OnlineMarketing_Pages_Content_MVTest_New" Theme="Default" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/Mvtest/Edit.ascx" TagName="MvtestEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSMVTEnabled" />
    </asp:Panel>
    <cms:MvtestEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
