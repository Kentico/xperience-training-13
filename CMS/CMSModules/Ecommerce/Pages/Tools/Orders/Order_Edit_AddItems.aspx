<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AddItems"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Ecommerce - Add order item"  Codebehind="Order_Edit_AddItems.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/ProductOptions/ShoppingCartItemSelector.ascx"
    TagName="ShoppingCartItemSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        function AddProducts(productIDs, quantities, options, price) {
            wopener.AddProduct(productIDs, quantities, options, price);
            CloseDialog();
            return false;
        }
        function RefreshCart() {
            wopener.RefreshCart();
            CloseDialog();
            return false;
        }
        //]]>
    </script>
    <%-- Products --%>
    <asp:PlaceHolder ID="plcProducts" runat="server">
        <cms:UniGrid runat="server" ID="gridProducts" ShortID="g" OrderBy="SKUName" IsLiveSite="false"
            ObjectType="ecommerce.sku">
            <GridColumns>
                <ug:Column Source="##ALL##" ExternalSourceName="SKUName" Caption="$Order_Edit_AddItems.GridProductName$"
                    Sort="SKUName" CssClass="main-column-100" Wrap="false" Style="text-align: left;">                    
                </ug:Column>
                <ug:Column Source="SKUNumber" Caption="$Order_Edit_AddItems.GridProductCode$" Wrap="false"
                    Style="text-align: left;" />
                <ug:Column Source="##ALL##" ExternalSourceName="Price" Caption="$Order_Edit_AddItems.GridUnitPrice$"
                    Wrap="false" Style="text-align: right;" Sort="SKUPrice" />
                <ug:Column Source="SKUID" ExternalSourceName="Quantity" Caption="$Order_Edit_AddItems.GridQuantity$"
                    Wrap="false" Style="text-align: center;" AllowSorting="false" />
                <ug:Column Source="SKUID" Visible="false" />
            </GridColumns>
            <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/Ecommerce/Controls/Filters/SimpleProductFilter.ascx" />            
            <PagerConfig DefaultPageSize="10" />
        </cms:UniGrid>
    </asp:PlaceHolder>
    <%-- Shopping cart item selector --%>
    <asp:PlaceHolder ID="plcSelector" runat="server">        
            <cms:LocalizedHeading runat="server" Level="3" ID="headSKUName" EnableViewState="false" />
            <cms:LocalizedLabel runat="server" ID="lblTitle" CssClass="InfoLabel" />
            <cms:ShoppingCartItemSelector ID="cartItemSelector" runat="server" ShowProductOptions="true"
                ShowUnitsTextBox="true" ShowTotalPrice="true" DialogMode="True" IsLiveSite="false" />      
    </asp:PlaceHolder>
</asp:Content>
