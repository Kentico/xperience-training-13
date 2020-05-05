<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_Forums_ForumModerators"  Codebehind="ForumModerators.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/securityAddUsers.ascx" TagName="SelectUser" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:CMSCheckBox ID="chkForumModerated" runat="server" CssClass="CheckBoxMovedLeft"
    EnableViewState="true" AutoPostBack="true" OnCheckedChanged="chkForumModerated_CheckedChanged" />
<cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" ResourceString="Forum_Edit.Moderators" EnableViewState="false" />
<cms:SelectUser ID="userSelector" runat="server" />
