<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectMembership.ascx.cs"
    Inherits="CMSModules_Membership_FormControls_Membership_SelectMembership" %>
<%@ Register TagPrefix="cms" TagName="UniSelector" Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelectorElem" runat="server" ObjectType="CMS.Membership"
            DisplayNameFormat="{%MembershipDisplayName%}" SelectionMode="SingleDropDownList" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
