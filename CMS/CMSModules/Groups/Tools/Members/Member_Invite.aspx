<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Groups_Tools_Members_Member_Invite" Title="Group nembers invite"  Codebehind="Member_Invite.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupInvite.ascx" TagName="GroupInvite"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:GroupInvite ID="groupInviteElem" runat="server" IsLiveSite="false"  />
</asp:Content>
