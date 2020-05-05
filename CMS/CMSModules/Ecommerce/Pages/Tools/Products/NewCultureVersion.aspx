<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_NewCultureVersion"
    Theme="Default"  Title="New culture version" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="NewCultureVersion.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/NewCultureVersion.ascx"
    TagName="NewCultureVersion" TagPrefix="cms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:NewCultureVersion runat="server" ID="newCultureVersion" />
</asp:Content>