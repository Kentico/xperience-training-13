<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_OrderItems" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order - Edit - Items list"  Codebehind="Order_Edit_OrderItems.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/ShoppingCart/ShoppingCart.ascx"
    TagName="ShoppingCart" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="GlobalWizard">
        <cms:ShoppingCart ID="Cart" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
