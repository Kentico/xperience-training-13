<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Users_SelectUser"
     Codebehind="SelectUser.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="usUsers" ShortID="s" runat="server" ObjectType="cms.userlist" SelectionMode="SingleTextBox"
            AllowEditTextBox="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
