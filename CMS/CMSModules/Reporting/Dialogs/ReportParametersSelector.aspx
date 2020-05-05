<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/Reporting/Dialogs/ReportParametersSelector.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Inherits="CMSModules_Reporting_Dialogs_ReportParametersSelector"
    Theme="Default" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="hdnGuid" runat="server" />
    <cms:BasicForm runat="server" ID="bfParameters" />
</asp:Content>