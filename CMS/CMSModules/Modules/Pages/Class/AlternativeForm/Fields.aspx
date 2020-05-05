<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Fields.aspx.cs" Inherits="CMSModules_Modules_Pages_Class_AlternativeForm_Fields"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="ALternative forms - fields" Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/AlternativeFormFieldEditor.ascx" TagName="AlternativeFormFieldEditor" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AlternativeFormFieldEditor ID="altFormFieldEditor" runat="server" IsLiveSite="false" Mode="AlternativeClassFormDefinition" DisplayedControls="All" />
</asp:Content>
