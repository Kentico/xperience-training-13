<%@ Page Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Groups_Tools_Roles_Role_List" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Role list"  Codebehind="Role_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/Controls/Roles/RoleList.ascx" TagName="RoleList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:RoleList Id="roleListElem" runat="server" IsLiveSite="false" />
</asp:Content>
