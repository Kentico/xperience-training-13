<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Discount_Discount_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Discounts"
     Codebehind="Discount_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="ugDiscounts" IsLiveSite="false" Columns="DiscountID, DiscountDisplayName, DiscountIsFlat, DiscountValue, DiscountSiteID, DiscountOrderAmount, DiscountValidTo, DiscountValidFrom, DiscountEnabled, DiscountOrder, DiscountApplyFurtherDiscounts, DiscountUsesCoupons"
                OrderBy="DiscountOrder" ObjectType="ecommerce.discount">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="#delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="DiscountDisplayName" Name="DisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="##ALL##" Name="Value" Caption="$general.value$" Wrap="false" ExternalSourceName="value" CssClass="TextRight" />
                    <ug:Column Source="##ALL##" Name="Status" Caption="$com.discount.status$" ExternalSourceName="status" Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/Ecommerce/Controls/Filters/DiscountFilter.ascx">
                            <CustomFilterParameters>
                                <ug:FilterParameter name="DiscountType" value="Discount" />
                            </CustomFilterParameters>
                        </Filter>
                    </ug:Column>
                    <ug:Column Name="ValidFrom" Source="DiscountValidFrom" ExternalSourceName="#userdatetimegmt" Caption="$com.discount.validfrom$" Wrap="false" />
                    <ug:Column Name="ValidTo" Source="DiscountValidTo" ExternalSourceName="#userdatetimegmt" Caption="$com.discount.validto$" Wrap="false" />
                    <ug:Column Name="Application" Source="##ALL##" Caption="$com.couponcode.numberofuses$" ExternalSourceName="application" Wrap="false" />
                    <ug:Column Name="OrderAmount" Source="##ALL##" ExternalSourceName="OrderAmount" Caption="$com.discount.discountorderamount$" CssClass="TextRight" Wrap="false" />
                    <ug:Column Source="##ALL##" Name="Order" Caption="$com.discount.priority$" Wrap="false" ExternalSourceName="DiscountOrder" CssClass="TextRight" />
                    <ug:Column Name="ApplyFurtherDiscounts" Source="DiscountApplyFurtherDiscounts" Caption="$com.discount.applyfurtherdiscountsshort$" ExternalSourceName="#yesno" Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
