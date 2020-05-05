<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Tools_Polls_General"
    Title="Poll properties" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="Polls_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollProperties.ascx" TagName="PollProperties"
    TagPrefix="cms" %>
    
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PollProperties ID="PollProperties" runat="server" Visible="true" /> 
</asp:Content>
