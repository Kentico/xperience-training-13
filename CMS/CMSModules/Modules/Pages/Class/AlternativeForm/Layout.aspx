<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Class_AlternativeForm_Layout"
    Theme="Default"  Codebehind="Layout.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/Layout.ascx" TagName="Layout" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ObjectCustomizationPanel runat="server" ID="pnlCustomization">
        <cms:Layout ID="layoutElem" runat="server" IsLiveSite="false" FormLayoutType="SystemTable" IsAlternative="true" />
    </cms:ObjectCustomizationPanel>
</asp:Content>
