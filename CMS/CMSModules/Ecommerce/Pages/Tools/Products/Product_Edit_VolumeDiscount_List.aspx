<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_List"
    Theme="Default" Title="Product edit - volume discount"  Codebehind="Product_Edit_VolumeDiscount_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:LocalizedHeading runat="server" ID="headProductPriceInfo" Level="4" EnableViewState="false" />
    <cms:UniGrid runat="server" ID="UniGrid" IsLiveSite="false" Columns="VolumeDiscountID,VolumeDiscountValue,VolumeDiscountMinCount,VolumeDiscountIsFlatValue"
        ObjectType="ecommerce.volumediscount">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="VolumeDiscountMinCount" Caption="$unigrid.product_edit_volumediscount.columns.volumediscountmincount$"
                Wrap="false" CssClass="TextRight" />
            <ug:Column Source="##ALL##" ExternalSourceName="DiscountValue" Caption="$com.discount.valueperitem$"
                CssClass="TextRight" Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
