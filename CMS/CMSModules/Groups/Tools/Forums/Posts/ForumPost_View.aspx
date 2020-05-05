<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Posts_ForumPost_View"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" ValidateRequest="false"  Codebehind="ForumPost_View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Posts/PostView.ascx" TagName="PostView"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:PostView ID="postView" runat="server" />
</asp:Content>
