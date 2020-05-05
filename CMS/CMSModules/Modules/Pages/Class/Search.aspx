<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Class_Search"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Class - Search"
    Theme="Default"  Codebehind="Search.aspx.cs" %>

<%@ Register Src="~/CMSModules/SmartSearch/Controls/Edit/SearchFields.ascx"
    TagName="SearchFields" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:ObjectCustomizationPanel runat="server" ID="pnlCustomization">
        <cms:SearchFields ID="searchFields" runat="server" IsLiveSite="false" />
    </cms:ObjectCustomizationPanel>
</asp:Content>
