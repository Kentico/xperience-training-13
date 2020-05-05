<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Banned IPs" Inherits="CMSModules_BannedIP_Tools_BannedIP_List" Theme="Default"
     Codebehind="BannedIP_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntBefore" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <asp:PlaceHolder ID="plcSites" runat="server">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblSite" runat="server" CssClass="control-label" AssociatedControlID="siteSelector"
                        EnableViewState="false" ResourceString="general.site" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="true"
                        OnlyRunningSites="false" GlobalRecordValue="0" AllowGlobal="true" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:DisabledModule runat="server" ID="ucDisabledModule" ReloadUIWhenModuleEnabled="False" TestSettingKeys="CMSBannedIPEnabled" />
            <cms:UniGrid runat="server" ID="UniGrid" ObjectType="cms.bannedip" Columns="IPAddressID, IPAddress, IPAddressBanType, IPAddressAllowed, IPAddressBanEnabled, IPAddressSiteID"
                IsLiveSite="false" OrderBy="IPAddress" GridName="BannedIP_List.xml" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>