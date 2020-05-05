<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MyDesk_MyProfile_MyProfile_ChangePassword" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MyProfile - Change password"  Codebehind="MyProfile_ChangePassword.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/ChangePassword.ascx"
    TagName="ChangePassword" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ChangePassword ID="ucChangePassword" runat="server" Visible="true" IsLiveSite="false" />
</asp:Content>
