<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_ProductOptions"
     Codebehind="ProductOptions.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.optioncategory"
            SelectionMode="MultipleButton" ResourcePrefix="com.ProductOptions" DisplayNameFormat="{%CategoryDisplayName%} ({%CategoryLiveSiteDisplayName%})"
            AdditionalColumns="CategoryType,CategoryLiveSiteDisplayName" DialogGridName="~/CMSModules/Ecommerce/Controls/UI/ProductOptionsSelector.xml" />
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="com.sku.categoriesavailable" CssClass="listing-title" EnableViewState="false" />
        <cms:UniGrid ID="categoryGrid" runat="server" ObjectType="ecommerce.optioncategorylist" ShowObjectMenu="false" OrderBy="CategoryDisplayName">
            <GridActions>
                <ug:Action Name="select" CommandArgument="CategoryID" Caption="$com.ProductOptions.SelectAllowedOptions$" FontIconClass="icon-checklist"
                    ExternalSourceName="SelectOptions" />
                <ug:Action Name="up" CommandArgument="CategoryID" Caption="$com.ProductOptions.Unigrid.Order.Actions.Up$"
                    FontIconClass="icon-chevron-up" />
                <ug:Action Name="down" CommandArgument="CategoryID" Caption="$com.ProductOptions.Unigrid.Order.Actions.Down$"
                    FontIconClass="icon-chevron-down" />
                <ug:Action Name="#edit" CommandArgument="CategoryID" Caption="$general.edit$"
                    FontIconClass="icon-edit" FontIconStyle="Allow" ExternalSourceName="EditItem" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="CategoryID" Caption="$com.ProductOptions.CategoryName$" Wrap="false"
                    ExternalSourceName="CategoryDisplayName" Sort="CategoryDisplayName">
                </ug:Column>
                <ug:Column Source="CategoryType" Caption="$com.ProductOptions.Type$" Wrap="false"
                    ExternalSourceName="CategoryType">
                </ug:Column>
                <ug:Column Source="CategoryID" Caption="$com.ProductOptions.counts$" Wrap="false"
                    ExternalSourceName="OptionsCounts" AllowSorting="false">
                </ug:Column>
                <ug:Column Source="CategoryEnabled" Caption="$com.ProductOptions.Enabled$" Wrap="false"
                    ExternalSourceName="#yesno">
                </ug:Column>
                <ug:Column Wrap="false" CssClass="main-column-100">
                </ug:Column>
            </GridColumns>
            <GridOptions DisplayFilter="false" AllowSorting="false" ShowSelection="true" SelectionColumn="CategoryID" />
        </cms:UniGrid>
        <br />
        <cms:CMSButton ID="btnRemove" runat="server" ButtonStyle="Default" OnClick="btnRemove_Clicked" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
