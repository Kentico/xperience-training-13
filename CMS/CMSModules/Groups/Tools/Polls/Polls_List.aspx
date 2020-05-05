<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Polls_Polls_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Group polls - list"
    Theme="Default"  Codebehind="Polls_List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollsList.ascx" TagName="PollsList"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PollsList ID="pollsList" runat="server" DelayedReload="false" />
</asp:Content>
