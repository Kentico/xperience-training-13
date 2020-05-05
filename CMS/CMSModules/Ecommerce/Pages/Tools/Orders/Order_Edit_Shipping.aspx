<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Shipping"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order edit - Shipping"
     Codebehind="Order_Edit_Shipping.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">   
    <cms:UIForm runat="server" ID="orderShippingForm"  ObjectType="ecommerce.order" AlternativeFormName="UpdateShipping" MarkRequiredFields="false"/>
</asp:Content>

