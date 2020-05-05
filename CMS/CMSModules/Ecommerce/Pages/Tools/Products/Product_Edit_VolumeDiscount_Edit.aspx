<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_Edit"
    Theme="Default" Title="Product edit - volume discount edit"  Codebehind="Product_Edit_VolumeDiscount_Edit.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:LocalizedHeading runat="server" ID="headProductPriceInfo" Level="4" EnableViewState="false" />
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.volumediscount" CssClass="VolumeDiscountEditForm"
        RedirectUrlAfterCreate="Product_Edit_VolumeDiscount_Edit.aspx?volumediscountid={%EditedObject.VolumeDiscountID%}&siteId={?siteId?}&nodeId={?nodeId?}&productId={%EditedObject.VolumeDiscountSKUID%}&dialog={?dialog?}&saved=1" />
</asp:Content>
