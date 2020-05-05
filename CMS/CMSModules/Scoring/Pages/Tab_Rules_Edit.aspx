<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Rules_Edit.aspx.cs" Inherits="CMSModules_Scoring_Pages_Tab_Rules_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Rule properties"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Scoring/Controls/UI/Rule/Edit.ascx" TagName="Edit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Edit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
