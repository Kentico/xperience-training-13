<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Roles_Role_Edit_General" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Role Edit - General"  Codebehind="Role_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/Controls/Roles/RoleEdit.ascx" TagName="RoleEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:RoleEdit Id="roleEditElem" runat="server" />
</asp:Content>