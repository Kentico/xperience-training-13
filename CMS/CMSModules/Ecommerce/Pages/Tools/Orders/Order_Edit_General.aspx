<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order edit - General"
     Codebehind="Order_Edit_General.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="editOrderGeneral" ObjectType="ecommerce.order" AlternativeFormName="UpdateGeneral" CssClass="" MarkRequiredFields="false" />
</asp:Content>
