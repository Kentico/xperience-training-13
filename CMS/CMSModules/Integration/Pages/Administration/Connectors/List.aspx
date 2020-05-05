<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Integration connector list"
    Inherits="CMSModules_Integration_Pages_Administration_Connectors_List" Theme="Default"  Codebehind="List.aspx.cs" %>
<%@ Register Src="~/CMSModules/Integration/Controls/UI/Connectors/List.ascx" TagName="IntegrationConnectorList" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:IntegrationConnectorList ID="listElem" runat="server" IsLiveSite="false" />
</asp:Content>
