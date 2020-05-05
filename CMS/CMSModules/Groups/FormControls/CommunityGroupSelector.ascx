<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_FormControls_CommunityGroupSelector"  Codebehind="CommunityGroupSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ObjectType="community.group" SelectionMode="SingleDropDownList"
            ReturnColumnName="GroupName" OrderBy="GroupDisplayName" ResourcePrefix="group"
            AllowEmpty="false" runat="server" ID="usGroups" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
