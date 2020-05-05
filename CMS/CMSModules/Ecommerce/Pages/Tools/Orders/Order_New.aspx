<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_New" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Orders - new order"  Codebehind="Order_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/ShoppingCart/ShoppingCart.ascx"
    TagName="ShoppingCart" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="GlobalWizard">
        <cms:ShoppingCart ID="Cart" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
