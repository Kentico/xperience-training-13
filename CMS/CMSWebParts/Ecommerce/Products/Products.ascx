<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Ecommerce/Products/Products.ascx.cs" Inherits="CMSWebParts_Ecommerce_Products_Products" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" WrapperControlID="pnlMessages" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="ecommerce.skulist" RememberDefaultState="true" IsLiveSite="false"
    Columns="SKUID, SKUName, SKUOptionCategoryID, SKUNumber, SKUPrice, SKUDepartmentID, SKUManufacturerID, SKUSupplierID, SKUPublicStatusID,
    SKUInternalStatusID, SKUReorderAt, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUParentSKUID, SKUBrandID, SKUCollectionID, SKUTaxClassID,">
    <GridColumns>
        <ug:Column Source="##ALL##" ExternalSourceName="SKUName"
            Caption="$product_list.productname$" Wrap="false" Localize="true" CssClass="main-column-100" />
        <ug:Column Name="OptionCategory" Source="##ALL##" ExternalSourceName="OptionCategory"
            Caption="$com.productswidget.optioncategory$" Wrap="false" Localize="true" />
        <ug:Column Name="Number" Source="##ALL##" ExternalSourceName="SKUNumber" Sort="SKUNumber"
            Caption="$product_list.productnumber$" Wrap="false" />
        <ug:Column Name="Price" Source="##ALL##" ExternalSourceName="SKUPrice" Sort="SKUPrice"
            Caption="$product_list.productprice$" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="Department" Source="##ALL##" ExternalSourceName="SKUDepartmentID"
            Caption="$com.productswidget.department$" Wrap="false" />
        <ug:Column Name="Brand" Source="##ALL##" ExternalSourceName="SKUBrandID"
            Caption="$com.productswidget.brand$" Wrap="false" Localize="true" />
        <ug:Column Name="Manufacturer" Source="##ALL##" ExternalSourceName="SKUManufacturerID"
            Caption="$com.productswidget.manufacturer$" Wrap="false" Localize="true" />
        <ug:Column Name="Supplier" Source="##ALL##" ExternalSourceName="SKUSupplierID"
            Caption="$com.productswidget.supplier$" Wrap="false" Localize="true" />
        <ug:Column Name="Collection" Source="##ALL##" ExternalSourceName="SKUCollectionID"
            Caption="$com.productswidget.collection$" Wrap="false" Localize="true" />
        <ug:Column Name="TaxClass" Source="##ALL##" ExternalSourceName="SKUTaxClassID"
            Caption="$com.productswidget.taxclass$" Wrap="false" Localize="true" />
        <ug:Column Name="PublicStatus" Source="##ALL##" ExternalSourceName="SKUPublicStatusID"
            Caption="$product_list.grid.storestatus$" Wrap="false" Localize="true" />
        <ug:Column Name="InternalStatus" Source="##ALL##" ExternalSourceName="SKUInternalStatusID"
            Caption="$product_list.grid.internalstatus$" Wrap="false" Localize="true" />
        <ug:Column Name="ReorderAt" Source="SKUReorderAt" Caption="$com.sku.reorderat$" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="AvailableItems" Source="##ALL##" ExternalSourceName="SKUAvailableItems"
            Sort="SKUAvailableItems" Caption="$product_list.productavailableitems$" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="ItemsToBeReordered" Source="##ALL##" ExternalSourceName="ItemsToBeReordered"
            Caption="$com.productswidget.itemstobereordered$" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="AllowForSale" Source="SKUEnabled" ExternalSourceName="#yesno" Caption="$com.productlist.allowforsale$"
            Wrap="false" />
        <ug:Column Source="SKUID" Sort="SKUSiteID" Name="SKUSiteID" ExternalSourceName="#transform: ecommerce.sku: {% (ToInt(SKUSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$" Wrap="false"  />
    </GridColumns>
</cms:UniGrid>
