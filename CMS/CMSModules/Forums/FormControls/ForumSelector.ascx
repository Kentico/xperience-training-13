<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_FormControls_ForumSelector"  Codebehind="ForumSelector.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" ObjectType="Forums.Forum" SelectionMode="SingleDropDownList"  AllowEditTextBox="true" />        
    </ContentTemplate>
</cms:CMSUpdatePanel>
