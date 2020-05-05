<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Billing"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order edit - Billing"
     Codebehind="Order_Edit_Billing.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">   
    <cms:UIForm runat="server" ID="editOrderBilling" ObjectType="ecommerce.order" AlternativeFormName="UpdateBilling" CssClass="" MarkRequiredFields="false" />
</asp:Content>
