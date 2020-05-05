<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Option Categories - List"
     Codebehind="OptionCategory_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
            <cms:UniGrid runat="server" ID="OptionCategoryGrid" ObjectType="ecommerce.optioncategory"
                IsLiveSite="false" Columns="CategoryID, CategoryDisplayName, CategoryDisplayPrice, CategoryType, CategorySelectionType, CategoryEnabled, CategorySiteID, CategoryLiveSiteDisplayName">
                <GridActions>
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical"
                        Confirmation="$General.ConfirmDelete$" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="##ALL##" ExternalSourceName="CategoryDisplayName" Caption="$general.name$" Wrap="false" Sort="CategoryDisplayName">
                        <Filter Type="custom" Path="~/CMSModules/Ecommerce/Controls/Filters/OptionCategoryNameFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="CategoryType" ExternalSourceName="CategoryType" Caption="$com.productoptions.categorytype$"
                        Wrap="false">
                        <Filter Type="custom" Path="~/CMSModules/Ecommerce/Controls/Filters/OptionCategoryTypeFilter.ascx" />
                    </ug:Column>
                    <ug:Column Source="CategorySelectionType" ExternalSourceName="CategorySelectionType" Caption="$Unigrid.OptionCategory.Columns.CategorySelectionType$"
                        Wrap="false" />
                    <ug:Column Source="CategoryDisplayPrice" ExternalSourceName="#yesno" Caption="$optioncategory_edit.categorydisplayprice$"
                        Wrap="false" />
                    <ug:Column Source="CategoryEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                        Wrap="false" />
                    <ug:Column Source="CategoryID" Name="CategorySiteID" Sort="CategorySiteID" ExternalSourceName="#transform: ecommerce.optioncategory: {% (ToInt(CategorySiteID, 0) == 0) ? GetResourceString(&quot;com.globally&quot;) : GetResourceString(&quot;com.onthissiteonly&quot;) %}" Caption="$com.available$"
                        Wrap="false" />
                    <ug:Column CssClass="filling-column" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
