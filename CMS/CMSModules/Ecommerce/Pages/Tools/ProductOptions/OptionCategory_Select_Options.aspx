<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Select_Options"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Option Category - Select options"
     Codebehind="OptionCategory_Select_Options.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContacts" runat="server">
        <div class="form-group">
            <cms:CMSRadioButtonList ID="rbAllowAllOption" runat="server" AutoPostBack="true" RepeatDirection="Vertical" CssClass="first-on-page" UseResourceStrings="True">
                <asp:ListItem Text="com.optioncategory.allowall" Value="0" />
                <asp:ListItem Text="com.optioncategory.allowselected" Value="1" />
            </cms:CMSRadioButtonList>
        </div>
        <cms:UniGrid ID="ugOptions" runat="server" IsLiveSite="false" OrderBy="SKUOrder" Columns="SKUID,SKUName,SKUPrice,SKUEnabled,SKUSiteID" ObjectType="ecommerce.skuoption" ShowObjectMenu="false" ShowActionsMenu="false">
            <GridColumns>
                <ug:Column Source="SKUName" Caption="$General.Name$" Wrap="false" Name="SKUName" />
                <ug:Column Source="##ALL##" ExternalSourceName="SKUPrice" Name="SKUPrice" Caption="$Unigrid.ProductOptions.ProductPriceAdjustment$" Wrap="false" CssClass="TextRight" />
                <ug:Column Source="SKUEnabled" Name="SKUEnabled" ExternalSourceName="#yesno" Caption="$com.sku.enabled$" Wrap="false" />
                <ug:Column CssClass="filling-column" />
            </GridColumns>
            <GridOptions ShowSelection="true" SelectionColumn="SKUID" />
        </cms:UniGrid>
    </asp:Panel>
</asp:Content>