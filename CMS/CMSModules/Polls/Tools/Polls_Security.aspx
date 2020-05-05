<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Polls - security" Inherits="CMSModules_Polls_Tools_Polls_Security" Theme="Default"  Codebehind="Polls_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollSecurity.ascx" TagName="PollSecurity"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PollSecurity ID="PollSecurity" runat="server" Visible="true" />
</asp:Content>
