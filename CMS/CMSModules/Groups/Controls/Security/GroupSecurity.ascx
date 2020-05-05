<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_Security_GroupSecurity"  Codebehind="GroupSecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover permission-matrix" EnableViewState="false">
</asp:Table>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="SecurityMatrix.RolesAvailability" CssClass="listing-title" EnableViewState="false" />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="Community.GroupRolePermission.getpermissionMatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName" RowItemDisplayNameColumn="RoleDisplayName"
    ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" MaxFilterLength="100" />
