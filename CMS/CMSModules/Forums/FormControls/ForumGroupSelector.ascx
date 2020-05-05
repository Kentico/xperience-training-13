<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_FormControls_ForumGroupSelector"  Codebehind="ForumGroupSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ObjectType="Forums.ForumGroup" SelectionMode="SingleDropDownList"  AllowEditTextBox="true" />        
    </ContentTemplate>
</cms:CMSUpdatePanel>
