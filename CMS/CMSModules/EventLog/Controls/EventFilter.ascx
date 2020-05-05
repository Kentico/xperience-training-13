<%@ Control Language="C#" AutoEventWireup="false" Inherits="CMSModules_EventLog_Controls_EventFilter"
     Codebehind="EventFilter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<asp:Panel ID="pnl" runat="server" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" ResourceString="general.type" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:EventLogTypeSelector ID="drpEventLogType" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblSource" runat="server" ResourceString="Unigrid.EventLog.Columns.Source"
                    DisplayColon="true" />
            </div>
            <cms:TextSimpleFilter ID="fltSource" runat="server" Column="Source" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEventCode" runat="server" ResourceString="Unigrid.EventLog.Columns.EventCode"
                    DisplayColon="true" />
            </div>

            <cms:TextSimpleFilter ID="fltEventCode" runat="server" Column="EventCode" />
        </div>
        <asp:PlaceHolder ID="plcAdvancedSearch" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="general.username"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltUserName" runat="server" Column="UserName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblIPAddress" runat="server" ResourceString="Unigrid.EventLog.Columns.IPAdress"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltIPAddress" runat="server" Column="IPAddress" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDocumentName" runat="server" ResourceString="general.documentname"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltDocumentName" runat="server" Column="DocumentName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblMachineName" runat="server" ResourceString="Unigrid.EventLog.Columns.MachineName"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltMachineName" runat="server" Column="EventMachineName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEventURL" runat="server" ResourceString="eventlogdetails.eventurl" />
                </div>
                <cms:TextSimpleFilter ID="fltEventURL" runat="server" Column="EventUrl" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblURLReferrer" runat="server" ResourceString="EventLogDetails.UrlReferrer"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltEventURLRef" runat="server" Column="EventUrlReferrer" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" runat="server" ResourceString="general.description"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltDescription" runat="server" Column="EventDescription" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblUserAgent" runat="server" ResourceString="EventLogDetails.UserAgent"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltUserAgent" runat="server" Column="EventUserAgent" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTimeBetween" runat="server" ResourceString="eventlog.timebetween"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:TimeSimpleFilter ID="fltTimeBetween" runat="server" Column="EventTime" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-label-cell">
                <asp:Panel ID="pnlToggleFilter" runat="server" Visible="true">
                    <asp:LinkButton ID="lnkToggleFilter" runat="server" CssClass="simple-advanced-link" />
                </asp:Panel>
            </div>
            <div class="filter-form-buttons-cell-wide-with-link">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" ResourceString="general.reset" OnClick="btnReset_Click" EnableViewState="false" />
                <cms:LocalizedButton ID="btnSearch" runat="server" ButtonStyle="Primary" ResourceString="general.filter" OnClick="btnSearch_Click" EnableViewState="false" />
            </div>
        </div>
    </div>
    <br />
</asp:Panel>

