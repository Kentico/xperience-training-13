<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_GlobalObjects"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="StoreSettings_GlobalObjects.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="SettingsGroupViewer" Src="~/CMSModules/Settings/Controls/SettingsGroupViewer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SettingsGroupViewer ID="SettingsGroupViewer" runat="server" AllowGlobalInfoMessage="false" />
</asp:Content>
