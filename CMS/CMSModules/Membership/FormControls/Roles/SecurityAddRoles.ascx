<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Roles_SecurityAddRoles"
     Codebehind="SecurityAddRoles.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="usRoles" runat="server" IsLiveSite="false" ObjectType="cms.role" UseTypeCondition="False"
            ResourcePrefix="addsecurityroles" SelectionMode="MultipleButton" AddGlobalObjectSuffix="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
