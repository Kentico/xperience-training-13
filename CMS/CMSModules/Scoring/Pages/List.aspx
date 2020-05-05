<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="List.aspx.cs" Inherits="CMSModules_Scoring_Pages_List" Title="Score list"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Scoring/Controls/UI/Score/List.ascx"
    TagName="ScoreList" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ScoreList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
