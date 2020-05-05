<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Groups_ForumGroup_New"
    Title="Groups New Forum Group" Theme="Default"  Codebehind="ForumGroup_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Groups/GroupNew.ascx" TagName="ForumGroup"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:ForumGroup ID="forumGroup" runat="server" />
</asp:Content>
