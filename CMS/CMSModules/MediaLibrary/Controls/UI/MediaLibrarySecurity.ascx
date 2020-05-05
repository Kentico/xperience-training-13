<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_UI_MediaLibrarySecurity"
     Codebehind="MediaLibrarySecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:LocalizedLabel ID="lblInfo" runat="server" Visible="false" CssClass="InfoLabel" EnableViewState="false" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover permission-matrix" EnableViewState="false">
</asp:Table>
<cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="SecurityMatrix.RolesAvailability" CssClass="listing-title" EnableViewState="false" />
<cms:UniMatrix ID="gridMatrix" runat="server" QueryName="media.library.getpermissionmatrix"
    RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName"
    RowItemDisplayNameColumn="RoleDisplayName" ColumnItemDisplayNameColumn="PermissionDisplayName"
    ColumnItemTooltipColumn="PermissionDescription" RowTooltipColumn="RowDisplayName"
    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription"
    AddGlobalObjectSuffix="true" SiteIDColumnName="SiteID" FirstColumnClass="first-column" MaxFilterLength="100" />
