<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"  Codebehind="Role_Edit_Permissions_Matrix.aspx.cs" 
Inherits="CMSModules_Membership_Pages_Roles_Role_Edit_Permissions_Matrix" Theme="Default" %>
<%@ Register Src="~/CMSModules/Permissions/Controls/PermissionsFilter.ascx" TagName="PermissionsHeader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Permissions/Controls/PermissionsMatrix.ascx" TagName="PermissionsMatrix" TagPrefix="cms" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PermissionsHeader ID="prmhdrHeader" runat="server"/>
    <cms:PermissionsMatrix ID="prmMatrix" runat="server" QueryName="cms.permission.getpermissionMatrix" ColumnItemTooltipColumn="RoleDisplayName" 
        RowItemIDColumn="PermissionID" ColumnItemIDColumn="RoleID" RowItemDisplayNameColumn="PermissionDisplayName" ColumnItemDisplayNameColumn="RoleDisplayName" 
        RowItemTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" PermissionsAsRows="true" />    
</asp:Content>
