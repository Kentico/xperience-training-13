<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Integration connector properties" Inherits="CMSModules_Integration_Pages_Administration_Connectors_Edit" Theme="Default"  Codebehind="Edit.aspx.cs" %>

<%@ Register Src="~/CMSModules/Integration/Controls/UI/Connectors/Edit.ascx"
    TagName="IntegrationConnectorEdit" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:IntegrationConnectorEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
