<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Forums_Forum_View"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" ValidateRequest="false"   Codebehind="Forum_View.aspx.cs" %>
<%@ Register Src="~/CMSModules/Forums/Controls/ForumDivider.ascx" TagName="ForumFlatView" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumFlatView ID="ForumFlatView1" runat="server" RedirectToUserProfile="false"  />
</asp:Content>
