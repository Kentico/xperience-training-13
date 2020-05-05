<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_List" Theme="Default"
    MaintainScrollPositionOnPostback="true" Title="Product list"  Codebehind="Product_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/DocumentList.ascx" TagName="DocumentList"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:PlaceHolder runat="server" ID="plcSKUListing">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid ID="gridData" runat="server" ShortID="g" RememberStateByParam=""
                    OrderBy="SKUName" IsLiveSite="false" Columns="SKUID, SKUName, SKUNumber, SKUPrice, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUReorderAt, SKUPublicStatusID, SKUTrackInventory"
                    ObjectType="ecommerce.sku">
                    <GridActions>
                        <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                        <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconstyle="Critical"
                            Confirmation="$General.ConfirmDelete$" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="SKUName" Caption="$product_list.productname$" Wrap="false" CssClass="main-column-100" />
                        <ug:Column Source="##ALL##" ExternalSourceName="SKUNumber" Sort="SKUNumber" Caption="$product_list.productnumber$" Wrap="false" />
                        <ug:Column Source="##ALL##" ExternalSourceName="SKUPrice" Sort="SKUPrice" Caption="$product_list.productprice$" Wrap="false" CssClass="TextRight" />
                        <ug:Column Source="##ALL##" ExternalSourceName="SKUAvailableItems" Caption="$product_list.productavailableitems$" Wrap="false" CssClass="TextRight" />
                        <ug:Column Source="##ALL##" ExternalSourceName="PublicStatusID" Caption="$product_list.grid.storestatus$" Wrap="false" />
                        <ug:Column Source="SKUEnabled" ExternalSourceName="#yesno" Sort="SKUEnabled" Caption="$com.productlist.allowforsale$" Wrap="false" />
                        <ug:Column Source="SKUID" Sort="SKUSiteID" Name="SKUSiteID" ExternalSourceName="#transform: ecommerce.sku: {% (ToInt(SKUSiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$" Wrap="false" />
                        <ug:Column Source="SKUNumber" Visible="false" />
                        <ug:Column CssClass="filling-column" />
                    </GridColumns>
                    <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/Ecommerce/Controls/UI/ProductFilter.ascx" />
                </cms:UniGrid>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcDocumentListing">
        <asp:Panel ID="pnlListingInfo" runat="server" EnableViewState="false">
            <asp:Label ID="lblListingInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        </asp:Panel>
        <cms:DocumentList runat="server" ID="docList" ShowAllLevels="true" ShowDocumentTypeIcon="true"
            DocumentNameAsLink="false" ShowDocumentTypeIconTooltip="true" />
    </asp:PlaceHolder>
</asp:Content>
