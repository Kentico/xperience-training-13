<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_ProductDocuments"
     Codebehind="ProductDocuments.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Filters/DocumentFilter.ascx" TagName="DocumentFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Documents"
    TagPrefix="cms" %>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="com.product.documentstitle" CssClass="listing-title" />
<asp:PlaceHolder ID="plcFilter" runat="server">
    <cms:DocumentFilter ID="filterDocuments" runat="server" LoadSites="true" />
</asp:PlaceHolder>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:Documents ID="docElem" runat="server" ListingType="ProductDocuments" IsLiveSite="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
