<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_FormControls_MembershipGroupSelector"  Codebehind="MembershipGroupSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ObjectType="community.group" SelectionMode="MultipleTextBox" ReturnColumnName="GroupName"
            OrderBy="GroupDisplayName" ResourcePrefix="groups" runat="server" AllowEmpty="false"
            ID="usGroups" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
