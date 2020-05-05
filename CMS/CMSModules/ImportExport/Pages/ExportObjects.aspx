<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ImportExport_Pages_ExportObjects"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeBehind="ExportObjects.aspx.cs" %>

<%@ Register Src="~/CMSModules/ImportExport/Controls/ExportWizard.ascx" TagName="ExportWizard"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:ExportWizard ID="wzdExport" ShortID="w" runat="server" />
</asp:Content>
