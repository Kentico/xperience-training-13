<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Macros_ConditionBuilderDialog"
    ValidateRequest="false" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Edit macro condition"  Codebehind="ConditionBuilder.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroDesigner.ascx" TagName="MacroDesigner"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MacroDesigner runat="server" ID="designerElem" ShortID="d" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
