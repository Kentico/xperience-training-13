<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Variant_Edit.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Variant_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Variant properties" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="editForm" ObjectType="ecommerce.sku" AlternativeFormName="UpdateVariant" CssClass="ui-form label-width-250"  DefaultFieldLayout="TwoColumns"
        RedirectUrlAfterCreate="Product_Edit_Variant_Edit.aspx?variantId={%EditedObject.ID%}&saved=1" />
</asp:Content>
