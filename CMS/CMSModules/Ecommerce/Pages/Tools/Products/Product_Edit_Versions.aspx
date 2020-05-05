<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Versions.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Versions" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Versions.ascx" TagName="Versions"
    TagPrefix="cms" %>
<asp:Content runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="plcContent">
    <cms:Versions runat="server" ID="versionsElem" IsLiveSite="false" />
</asp:Content>
