<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CustomTables_AlternativeForms_AlternativeForms_Layout"
    Theme="Default"  Codebehind="AlternativeForms_Layout.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/Layout.ascx" TagName="Layout"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:Layout ID="layoutElem" runat="server" IsLiveSite="false" />
</asp:Content>
