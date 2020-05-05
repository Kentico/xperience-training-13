<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Option Category - New"
     Codebehind="OptionCategory_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/ProductOptions/OptionCategoryEdit.ascx"
    TagName="OptionCategoryEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:OptionCategoryEdit ID="OptionCategoryEditElem" runat="server" ShortID="oce"
        IsLiveSite="false" />
</asp:Content>
