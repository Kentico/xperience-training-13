<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Roles_Role_Edit_Users" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Role Edit - Users"  Codebehind="Role_Edit_Users.aspx.cs" %>


<%@ Register Src="~/CMSModules/Membership/Controls/Roles/RoleUsers.ascx" TagName="RoleUsers" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">    
    <cms:RoleUsers Id="roleUsersElem" runat="server" />        
</asp:Content>
