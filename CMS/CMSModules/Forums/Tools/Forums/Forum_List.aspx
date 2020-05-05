<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Forums_Forum_List" 
MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="Forum_List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumList.ascx" TagName="ForumList" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumList ID="forumList" runat="server" Visible="true" />
</asp:Content>