<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="AB test properties - General" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_Tab_General"
    Theme="Default"  Codebehind="Tab_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/OnlineMarketing/Controls/UI/AbTest/Edit.ascx" TagPrefix="cms" TagName="AbTestEdit" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <cms:MessagesPlaceHolder runat="server" ID="plcMess" />
    <cms:AbTestEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
