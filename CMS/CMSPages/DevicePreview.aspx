<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="DevicePreview.aspx.cs" Inherits="CMSPages_DevicePreview"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/DeviceProfiles/Controls/DeviceView.ascx" TagName="DeviceView"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DeviceView ID="ucView" runat="server" UseStarupScriptInitializer="true" />
</asp:Content>
