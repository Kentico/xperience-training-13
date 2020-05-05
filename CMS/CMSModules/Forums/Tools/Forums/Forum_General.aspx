<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Forums_Forum_General"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Forum General"  Codebehind="Forum_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/Forums/ForumEdit.ascx" TagName="ForumEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ForumEdit ID="forumEdit" runat="server" />
    <asp:Literal ID="ltlScript" runat="server" />
</asp:Content>