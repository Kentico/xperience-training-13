<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="false"  Codebehind="ShippingOption_Edit_ShippingCosts.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:UniGrid runat="server" ID="gridElem" ObjectType="ecommerce.shippingcost" IsLiveSite="false"
        OrderBy="ShippingCostMinWeight" WhereCondition="ShippingCostShippingOptionID = {%EditedObjectParent.ID%}"
        EditActionUrl="ShippingOption_Edit_ShippingCosts_Edit.aspx?shippingCostID={0}&objectid={%EditedObjectParent.ID%}&siteId={?siteId?}"
        OnOnExternalDataBound="gridElem_OnExternalDataBound">
        <GridActions>
            <ug:Action Name="edit" ExternalSourceName="Edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="ShippingCostMinWeight" Caption="$com.ui.shippingcost.edit_minweight$"
                Wrap="false" CssClass="TextRight">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="ShippingCostValue" ExternalSourceName="ShippingCostValue" Caption="$com.ui.shippingcost.edit_cost$"
                Wrap="false" CssClass="TextRight" />
            <ug:Column CssClass="filling-column" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
    <script type="text/javascript">
        //<![CDATA[
        // Refreshes current page when properties are changed in modal dialog window
        function RefreshPage() {
            window.location.replace(window.location.href);
        }
        //]]>
    </script>
</asp:Content>
