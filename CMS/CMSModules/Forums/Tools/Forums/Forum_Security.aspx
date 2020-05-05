<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Forums_Forum_Security"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="default" Title="Forums - Forum security"  Codebehind="Forum_Security.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumSecurity.ascx" TagName="ForumSecurity" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumSecurity ID="forumSecurity" runat="server" />    
</asp:Content>