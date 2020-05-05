<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_ImportExport_Pages_ImportSite"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeBehind="ImportSite.aspx.cs" %>

<%@ Register Src="~/CMSModules/ImportExport/Controls/ImportWizard.ascx" TagName="ImportWizard" TagPrefix="cms" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server" EnableViewState="false">
    <cms:ImportWizard ID="wzdImport" ShortID="w" runat="server" />
</asp:Content>
