<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_Options" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Option category - list of options"  Codebehind="OptionCategory_Edit_Options.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="ugOptions"
        OrderBy="SKUOrder" IsLiveSite="false" Columns="SKUID,SKUOptionCategoryID,SKUName,SKUNumber,SKUDepartmentID,SKUPrice,SKUTrackInventory,SKUAvailableItems,SKUEnabled,SKUSiteID,SKUReorderAt" ObjectType="ecommerce.skuoption">
        <GridActions>
            <ug:Action Name="#move" Externalsourcename="move" Caption="$General.DragMove$" FontIconClass="icon-dots-vertical"/>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" Externalsourcename="delete" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="SKUName" Caption="$Unigrid.ProductOptions.Name$" Wrap="false" Name="SKUName" />
            <ug:Column Source="SKUNumber" Caption="$Unigrid.ProductOptions.Number$" Wrap="false" Name="SKUNumber" />
            <ug:Column Source="SKUDepartmentID" Externalsourcename="#transform: ecommerce.department.departmentdisplayname" 
                Caption="$com.productoptionscategory.skudepartment$" Wrap="false" Name="SKUDepartment" />
            <ug:Column Source="##ALL##" Externalsourcename="SKUPrice" Name="SKUPrice" Caption="$Unigrid.ProductOptions.ProductPriceAdjustment$" Wrap="false" Sort="SKUPrice" CssClass="TextRight" />
            <ug:Column Source="##ALL##" Externalsourcename="SKUAvailableItems" Caption="$Unigrid.ProductOptions.AvailableItems$" Wrap="false" Name="SKUAvailableItems" CssClass="TextRight" />
            <ug:Column Source="SKUEnabled" Externalsourcename="#yesno"  Caption="$com.productedit.allowforsale$" Wrap="false" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="false" />
    </cms:UniGrid>
</asp:Content>