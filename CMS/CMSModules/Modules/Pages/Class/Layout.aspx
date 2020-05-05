<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Modules_Pages_Class_Layout"
    Title="Class edit layout" Theme="Default"  Codebehind="Layout.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/Layout.ascx" TagName="Layout"
    TagPrefix="cms" %>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:ObjectCustomizationPanel runat="server" ID="pnlCustomization">
        <cms:Layout ID="layoutElem" runat="server" IsLiveSite="false" />
    </cms:ObjectCustomizationPanel>
</asp:Content>
