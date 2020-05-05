<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PermissionsMatrix.ascx.cs"
    Inherits="CMSModules_Permissions_Controls_PermissionsMatrix" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdMat" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlMat" runat="server">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
            <cms:UniMatrix ID="gridMatrix" ShortID="um" runat="server" QueryName="cms.permission.getpermissionMatrix"
                RowItemIDColumn="PermissionID" ColumnItemIDColumn="RoleID" RowItemDisplayNameColumn="PermissionDisplayName"
                ColumnItemDisplayNameColumn="RoleDisplayName" RowTooltipColumn="PermissionDescription" FirstColumnClass="first-column"
                ColumnTooltipColumn="RoleDisplayName" ItemTooltipColumn="PermissionDescription" AddFillingColumn="true" MaxFilterLength="100" />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
