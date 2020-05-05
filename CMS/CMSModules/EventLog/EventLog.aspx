<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EventLog_EventLog"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Event log"
    Theme="Default"  Codebehind="EventLog.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EventLog/Controls/EventLog.ascx" TagName="EventLog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModuleInfo" %>

<asp:Content ID="cntSiteSelector" ContentPlaceHolderID="plcSiteSelector" runat="server">
    <asp:Panel ID="pnlSites" runat="server">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel runat="server" CssClass="control-label" ID="lblSite" EnableViewState="false" ResourceString="EventLogList.Label" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DisabledModuleInfo runat="server" ID="disabledModuleInfo" TestSettingKeys="CMSLogToDatabase" KeyScope="Global" InfoText="{$eventlog.disabledmodule$}" />
    <cms:EventLog ID="eventLog" ShortID="ev" runat="server" ShowFilter="true" />
</asp:Content>