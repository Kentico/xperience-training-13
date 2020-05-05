<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true"  Codebehind="ShippingOption_Edit_ShippingCosts_Edit.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts_Edit"
    Theme="Default" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.shippingcost"
        RedirectUrlAfterCreate="ShippingOption_Edit_ShippingCosts_Edit.aspx?shippingCostID={%EditedObject.ID%}&objectid={%EditedObjectParent.ID%}&saved=1" />
</asp:Content>