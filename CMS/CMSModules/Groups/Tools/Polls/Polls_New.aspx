<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Polls_Polls_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="New poll"  Codebehind="Polls_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Polls/Controls/PollNew.ascx" TagName="PollNew" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PollNew ID="PollNew" runat="server" IsLiveSite="false" />
</asp:Content>
