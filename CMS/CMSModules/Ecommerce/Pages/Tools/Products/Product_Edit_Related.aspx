<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Related.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Related" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/FormControls/Relationships/RelatedDocuments.ascx"
    TagName="RelatedDocuments" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <div class="Unsorted">
        <cms:RelatedDocuments ID="relatedDocuments" runat="server" ShowAddRelation="false"
            IsLiveSite="false" PageSize="10,25,50,100,##ALL##" DefaultPageSize="25" />
    </div>
</asp:Content>
