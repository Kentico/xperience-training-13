<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Polls_Polls_Edit_General" 
Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Groups polls edit - general"  Codebehind="Polls_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollProperties.ascx" TagName="PollProperties" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PollProperties ID="PollProperties" runat="server" IsLiveSite="false" />
</asp:Content>
