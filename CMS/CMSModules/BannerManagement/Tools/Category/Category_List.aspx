<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Category_Category_List"
    Title="Banner Management - Category List" Theme="Default"  Codebehind="Category_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcSiteSelector" runat="server">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSites" runat="server" DisplayColon="true" ResourceString="general.site" CssClass="control-label" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" ShortID="c" runat="server" PostbackOnDropDownChange="True" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid ID="gridBannerManagement" runat="server" IsLiveSite="false" OrderBy="BannerCategoryDisplayName" ObjectType="cms.bannercategory" Columns="BannerCategoryID, BannerCategoryDisplayName, BannerCategoryEnabled, BannerCategorySiteID, (SELECT COUNT(BannerID) FROM CMS_Banner WHERE BannerCategoryID = CMS_BannerCategory.BannerCategoryID AND BannerEnabled = 1) BannerCounts, (SELECT COUNT(BannerID) FROM CMS_Banner WHERE BannerCategoryID = CMS_BannerCategory.BannerCategoryID) BannerCountsTotal" RememberStateByParam="">
                <GridActions Parameters="BannerCategoryID">
                    <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                    <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" Confirmation="$general.confirmdelete$" ExternalSourceName="delete" FontIconStyle="Critical" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="BannerCategoryDisplayName" Caption="$general.name$" Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="BannerCategoryEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$" Wrap="false">
                        <Filter Type="bool" />
                    </ug:Column>
                    <ug:Column Source="BannerCounts" Caption="$banner.enabledbanners$" Wrap="false" AllowSorting="false">
                        <Filter Type="integer" Format="(SELECT COUNT(BannerID) FROM CMS_Banner WHERE BannerCategoryID = CMS_BannerCategory.BannerCategoryID AND BannerEnabled = 1) {1} {2}" />
                    </ug:Column>
                    <ug:Column Source="BannerCountsTotal" Caption="$banner.totalcount$" Wrap="false" AllowSorting="false">
                        <Filter Type="integer" Format="(SELECT COUNT(BannerID) FROM CMS_Banner WHERE BannerCategoryID = CMS_BannerCategory.BannerCategoryID) {1} {2}" />
                    </ug:Column>
                    <ug:Column CssClass="filling-column">
                    </ug:Column>
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>

    <asp:Button ID="btnHiddenPostBackButton" runat="server" CssClass="HiddenButton" />
</asp:Content>
