<%@ Page Title="Module edit - User interface - Roles" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_UserInterface_Roles"
    Theme="Default"  Codebehind="Roles.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector"
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
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" AllowGlobal="true" OnlyRunningSites="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="cntMatrix" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel id="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSPersonalizeUserInterface" />
                <asp:PlaceHolder ID="plcUser" runat="server">
                    <div class="form-horizontal form-filter">
                        <div class="form-group">
                            <div class="filter-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblUser" runat="server" ResourceString="Administration-Permissions_Header.User"
                                    DisplayColon="false" EnableViewState="false" AssociatedControlID="userSelector" />
                            </div>
                            <div class="filter-form-value-cell">
                                <cms:UserSelector ID="userSelector" runat="server" SelectionMode="SingleDropDownList" IsLiveSite="false" ShowSiteFilter="false" />
                                <cms:CMSUpdatePanel ID="pnlUpdateUsers" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <cms:CMSCheckBox ID="chkUserOnly" runat="server" AutoPostBack="true" />
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
            <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" EnableViewState="false" />
            <asp:PlaceHolder ID="plcUpdate" runat="server">
                <cms:UniMatrix ID="gridMatrix" CssClass="permission-matrix" runat="server" QueryName="CMS.UIElement.getpermissionmatrix"
                    RowItemIDColumn="RoleID" ColumnItemIDColumn="ElementID" RowItemDisplayNameColumn="RoleDisplayName"
                    ColumnItemDisplayNameColumn="ElementDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
                    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" AddFillingColumn="true" MaxFilterLength="100" />
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>