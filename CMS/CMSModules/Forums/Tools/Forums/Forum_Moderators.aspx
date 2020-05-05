<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Tools_Forums_Forum_Moderators" 
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="default" Title="Forums - Forum security"  Codebehind="Forum_Moderators.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumModerators.ascx" TagName="ForumModerators" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumModerators ID="forumModerators" runat="server" />
</asp:Content>