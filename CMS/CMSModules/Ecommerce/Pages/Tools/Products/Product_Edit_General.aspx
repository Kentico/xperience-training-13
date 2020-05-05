<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_General" Theme="Default"
    Title="Product - edit - general"  Codebehind="Product_Edit_General.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="ProductEdit" Src="~/CMSModules/Ecommerce/Controls/UI/ProductEdit.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <%-- Product edit --%>
    <cms:ProductEdit ID="productEditElem" runat="server" ShortID="p" FormMode="Update"
        IsLiveSite="false" ShowApplyWorkflow="false"/>
</asp:Content>
