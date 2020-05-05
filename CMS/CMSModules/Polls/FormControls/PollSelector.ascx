<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_FormControls_PollSelector"  Codebehind="PollSelector.ascx.cs" %>

<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ResourcePrefix="pollsselect" ObjectType="polls.poll"
    OrderBy="PollDisplayName" AllowEditTextBox="true" SelectionMode="SingleTextBox" ShortID="s" AddGlobalObjectSuffix="true" />