<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_DocumentTypes_Pages_Development_Scopes_Scopes"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page type edit - Scopes list"
    Theme="Default"  Codebehind="Scopes.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntSiteSelector" ContentPlaceHolderID="plcSiteSelector" runat="server">
    <asp:Panel ID="pnlSite" runat="server">
        <div class="form-horizontal form-filter scopes-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" DisplayColon="true" ResourceString="general.site" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector runat="server" ID="selectSite" IsLiveSite="false" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" EnableViewState="false" ResourceString="scopes.info" DisplayColon="true" />
    <cms:UniSelector ID="usScopes" runat="server" ObjectType="cms.documenttypescope" SelectionMode="Multiple" ResourcePrefix="scope" AdditionalColumns="ScopeSiteID" />
</asp:Content>
