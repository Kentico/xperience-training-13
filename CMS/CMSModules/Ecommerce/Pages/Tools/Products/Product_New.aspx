<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Product properties"
     Codebehind="Product_New.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="DocTypeSelection" Src="~/CMSModules/Content/Controls/DocTypeSelection.ascx" %>
<%@ Register TagPrefix="cms" TagName="ProductEdit" Src="~/CMSModules/Ecommerce/Controls/UI/ProductEdit.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <%-- Document type selection --%>
    <asp:PlaceHolder ID="plcDocTypeSelection" runat="server" Visible="false">
        <cms:DocTypeSelection ID="docTypeElem" runat="server" IsLiveSite="false" />
    </asp:PlaceHolder>
    <%-- Product edit --%>
    <asp:PlaceHolder ID="plcProductEdit" runat="server" Visible="false">
        <cms:ProductEdit ID="productEditElem" runat="server" FormMode="Insert" IsLiveSite="false" />
    </asp:PlaceHolder>
</asp:Content>
