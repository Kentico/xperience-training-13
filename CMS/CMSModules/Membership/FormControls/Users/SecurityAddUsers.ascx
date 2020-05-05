<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Users_SecurityAddUsers"
     Codebehind="SecurityAddUsers.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/selectuser.ascx" TagName="SelectUser"
    TagPrefix="cms" %>
<cms:SelectUser ID="usUsers" runat="server" SelectionMode="MultipleButton" />
