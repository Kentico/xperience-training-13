<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ResetPassword.aspx.cs" Inherits="CMSModules_Membership_CMSPages_ResetPassword"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"
    Title="ResetPassword" %>

<%@ Register Src="~/CMSModules/Membership/Controls/ResetPassword.ascx" TagName="ResetPassword"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:ResetPassword ID="resetPasswordItem" runat="server" IsLiveSite="false" />
</asp:Content>
