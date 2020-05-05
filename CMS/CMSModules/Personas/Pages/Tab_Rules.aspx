<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Rules.aspx.cs" Inherits="CMSModules_Personas_Pages_Tab_Rules"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Rules list" Theme="Default" %>

<%@ Register Src="~/CMSModules/Scoring/Controls/UI/Rule/List.ascx" TagName="List" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:List ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>