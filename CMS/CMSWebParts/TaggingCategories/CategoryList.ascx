<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_TaggingCategories_CategoryList"
     Codebehind="~/CMSWebParts/TaggingCategories/CategoryList.ascx.cs" %>
<asp:Label ID="lblInfo" runat="server" Visible="false" CssClass="InfoLabel" />
<cms:BasicRepeater ID="rptCategoryList" runat="server" OnItemDataBound="rptCategoryList_ItemDataBound" />
<asp:Literal ID="ltlList" runat="server"></asp:Literal>