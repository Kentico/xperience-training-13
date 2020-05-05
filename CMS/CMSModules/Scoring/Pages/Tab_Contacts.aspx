<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Contacts.aspx.cs" Inherits="CMSModules_Scoring_Pages_Tab_Contacts" Title="Contacts list"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register Src="~/CMSModules/Scoring/Controls/UI/Contact/List.ascx"
    TagName="ScoreList" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ScoreList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
