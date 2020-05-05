<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="false"  Codebehind="PageLayouts.aspx.cs" Inherits="CMSModules_DeviceProfiles_Pages_Development_PageLayouts"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/DeviceProfiles/Controls/PageLayouts.ascx" TagPrefix="cms" TagName="PageLayouts" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PageLayouts runat="server" ID="PageLayouts" />
</asp:Content>
