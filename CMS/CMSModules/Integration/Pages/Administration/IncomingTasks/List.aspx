<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Integration task list" Inherits="CMSModules_Integration_Pages_Administration_IncomingTasks_List"
    Theme="Default"  Codebehind="List.aspx.cs" %>

<%@ Register Src="~/CMSModules/Integration/Controls/UI/IntegrationTask/List.ascx"
    TagName="IntegrationTaskList" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="ConnectorSelector" Src="~/CMSModules/Integration/FormControls/ConnectorSelector.ascx" %>
<asp:Content runat="server" ID="cntSiteSelector" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConnector" runat="server" ResourceString="integration.connector"
                    EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:ConnectorSelector ID="connectorSelector" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:IntegrationTaskList ID="listElem" runat="server" IsLiveSite="false" TasksAreInbound="true" />
</asp:Content>
