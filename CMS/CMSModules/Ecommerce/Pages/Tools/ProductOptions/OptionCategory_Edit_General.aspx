<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Option Category - Edit"
     Codebehind="OptionCategory_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/ProductOptions/OptionCategoryEdit.ascx"
    TagName="OptionCategoryEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:optioncategoryedit id="OptionCategoryEditElem" runat="server" shortid="oce"
        islivesite="false" />
</asp:Content>
