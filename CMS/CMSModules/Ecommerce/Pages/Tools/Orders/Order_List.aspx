<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_List" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Order list"  Codebehind="Order_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/OrderList.ascx" TagName="OrderList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="Orders">
        <cms:OrderList ID="orderListElem" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
