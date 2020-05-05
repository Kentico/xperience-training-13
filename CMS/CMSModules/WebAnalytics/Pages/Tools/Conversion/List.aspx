<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Conversion list"
    Inherits="CMSModules_WebAnalytics_Pages_Tools_Conversion_List" Theme="Default"  Codebehind="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/UI/Conversion/List.ascx" TagName="ConversionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms" TagName="SmartTip" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SmartTip runat="server" ID="tipConversionsListing" />
    <cms:ConversionList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
