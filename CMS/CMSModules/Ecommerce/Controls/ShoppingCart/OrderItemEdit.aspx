<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_OrderItemEdit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Shopping cart - Order item edit"  CodeBehind="OrderItemEdit.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:UIForm runat="server" ID="EditForm" AlternativeFormName="ShoppingCartItemEdit" ObjectType="ecommerce.shoppingcartitem" />
    </asp:Panel> 
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <cms:FormSubmitButton runat="server" ID="btnSave" ResourceString="general.saveandclose" />
</asp:Content>