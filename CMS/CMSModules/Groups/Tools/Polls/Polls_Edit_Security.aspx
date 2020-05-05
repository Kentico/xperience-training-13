<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Polls_Polls_Edit_Security" 
    Title="Groups polls - security" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="Polls_Edit_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollSecurity.ascx" TagName="PollSecurity"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PollSecurity ID="PollSecurity" IsLiveSite="false" runat="server" Visible="true" />
</asp:Content>

