<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="E-commerce Configuration - Order status properties"
     Codebehind="OrderStatus_Edit.aspx.cs" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
     <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.orderstatus" OnOnBeforeDataLoad="EditForm_OnBeforeDataLoad"
         RedirectUrlAfterCreate="OrderStatus_Edit.aspx?orderStatusId={%EditedObject.ID%}&siteId={?SiteID?}&saved=1" />
</asp:Content>
