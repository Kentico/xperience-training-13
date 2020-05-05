<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Forums_Forum_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="Forum_New.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumNew.ascx" TagName="ForumNew" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumNew ID="forumNew" runat="server" />
</asp:Content>