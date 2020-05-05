<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_Parameters"
    Theme="Default" Title="Form User Control Parameters"  Codebehind="Parameters.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register Src="~/CMSModules/FormControls/Controls/FormControlFieldEditor.ascx" TagPrefix="cms" TagName="FormControlFieldEditor" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:FormControlFieldEditor runat="server" id="FormControlFieldEditor"/>
</asp:Content>
