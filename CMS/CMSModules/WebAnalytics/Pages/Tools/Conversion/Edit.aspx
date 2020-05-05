<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Conversion properties" Inherits="CMSModules_WebAnalytics_Pages_Tools_Conversion_Edit" Theme="Default"  Codebehind="Edit.aspx.cs" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/UI/Conversion/Edit.ascx"
    TagName="ConversionEdit" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
<asp:Panel ID="pnlContent" runat="server">
    <cms:ConversionEdit ID="editElem" runat="server" IsLiveSite="false" />
    </asp:Panel>
</asp:Content>