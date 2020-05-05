<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ClassContactMapping.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_ClassContactMapping"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System tables - on-line marketing" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ObjectCustomizationPanel runat="server" ID="pnlCustomization">
        <asp:PlaceHolder ID="plcMapping" runat="server" />
    </cms:ObjectCustomizationPanel>
</asp:Content>
