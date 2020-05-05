<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="UnlockUserAccount.aspx.cs"
    MasterPageFile="~/CMSMasterPages/LiveSite/SimplePage.master" Inherits="CMSModules_Membership_CMSPages_UnlockUserAccount"
    Theme="Default" %>
<%@ Register Src="~/CMSModules/Membership/Controls/UnlockUserAccount.ascx" TagName="UnlockAccount" TagPrefix="cms" %>
<asp:Content ID="cnt" ContentPlaceHolderID="plcContent" runat="server" >
    <cms:UnlockAccount ID="unlockAccount" runat="server" />
</asp:Content>