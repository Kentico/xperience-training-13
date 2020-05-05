<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Forums_ForumSecurity"  Codebehind="ForumSecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix" TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover permission-matrix">
</asp:Table>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="SecurityMatrix.RolesAvailability" CssClass="listing-title" EnableViewState="false" />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="Forums.ForumRole.getpermissionMatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName" RowItemDisplayNameColumn="RoleDisplayName"
    ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
    ColumnItemTooltipColumn="PermissionDescription" ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription"
    AddGlobalObjectSuffix="true" SiteIDColumnName="SiteID" MaxFilterLength="100" />
<cms:CMSCheckBox runat="server" ID="chkChangeName" ResourceString="forum_security.changename"
    AutoPostBack="true" />
