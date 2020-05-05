<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Tools_Polls_New"
    Title="Polls - New" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"  Codebehind="Polls_New.aspx.cs" %>
<%@ Register Src="~/CMSModules/Polls/Controls/PollNew.ascx" TagName="PollNew"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PollNew ID="PollNew" runat="server" Visible="true" />
</asp:Content>
