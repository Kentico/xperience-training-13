<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Shopping cart - Product price detail"  Codebehind="ShoppingCartSKUPriceDetail.aspx.cs" %>

<%@ Register Src="ShoppingCartSKUPriceDetail.ascx" TagName="ShoppingCartSKUPriceDetail"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div>
        <cms:ShoppingCartSKUPriceDetail ID="ucSKUPriceDetail" runat="server" />
    </div>

    <script type="text/javascript">
        //<![CDATA[       
        function Close() {
            CloseDialog();
        }
        //]]>
    </script>

</asp:Content>