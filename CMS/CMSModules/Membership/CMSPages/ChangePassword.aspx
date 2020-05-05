<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ChangePassword.aspx.cs" Inherits="CMSModules_Membership_CMSPages_ChangePassword"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Change password" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/ChangePassword.ascx"
    TagName="ChangePassword" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ChangePassword ID="ucChangePassword" runat="server" Visible="true" />
</asp:Content>
