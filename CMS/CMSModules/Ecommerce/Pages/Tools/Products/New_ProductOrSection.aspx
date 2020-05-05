<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="New_ProductOrSection.aspx.cs"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_New_ProductOrSection" %>

<%@ Register TagPrefix="cms" TagName="DocTypeSelector" Src="~/CMSModules/Content/Controls/DocTypeSelection.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSPanel runat="server" ID="pnlProductOrSectionType" class="ProductOrSectionType">
        <cms:DocTypeSelector runat="server" ID="ProductTypes" AllowNewABTest="false" AllowNewLink="true"
            Where="ClassIsProduct = 1 OR ClassIsProductSection = 1" RedirectWhenNoChoice="true" />
    </cms:CMSPanel>
</asp:Content>
