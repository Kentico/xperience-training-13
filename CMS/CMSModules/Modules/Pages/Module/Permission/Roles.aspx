<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Roles.aspx.cs"
    Theme="Default" Inherits="CMSModules_Modules_Pages_Module_Permission_Roles"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>

<asp:Content ID="cntBeforeBody" ContentPlaceHolderID="plcBeforeBody" runat="Server">
</asp:Content>
<asp:Content ID="cntFilter" ContentPlaceHolderID="plcSiteSelector" runat="Server">
    <asp:PlaceHolder runat="server" ID="plcSiteSelector">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSites" runat="server" ResourceString="general.site" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowAll="false" AllowGlobal="true" OnlyRunningSites="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
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
                <cms:UniMatrix ID="gridMatrix" runat="server" CssClass="permission-matrix" QueryName="CMS.Permission.GetRolePermissionMatrix"
                    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemDisplayNameColumn="RoleDisplayName"
                    ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
                    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" AddFillingColumn="true" MaxFilterLength="100" />
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>