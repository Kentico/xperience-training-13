<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Alternative Forms - New"
    Inherits="CMSModules_Modules_Pages_Class_AlternativeForm_New"
    Theme="default"  Codebehind="New.aspx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/AlternativeFormEdit.ascx" TagName="AlternativeFormEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AlternativeFormEdit runat="server" ID="altFormEdit" />
</asp:Content>