<%@ Control Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Membership_Controls_Roles_RoleList"  Codebehind="RoleList.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" tagname="UniGrid" tagprefix="cms" %>
    
<cms:UniGrid runat="server" ID="gridElem" GridName="~/CMSModules/Membership/Controls/Roles/Role_List.xml" OrderBy="RoleDisplayName" />
