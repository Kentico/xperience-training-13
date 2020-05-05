<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_EmailQueue_EmailQueue"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="EmailQueue.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/EmailQueue/Controls/EmailQueue.ascx" TagName="EmailQueueGrid"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSEmailsEnabled" InfoText="{$emailqueue.emailsdisabled$}" />
    <cms:LocalizedHeading runat="server" ID="headListingText" Level="4" CssClass="listing-title" ResourceString="EmailQueue.Queue.Text" EnableViewState="false" />
    <span class="InfoLabel">
        <cms:CMSIcon runat="server" ID="icShowFilter" CssClass="icon-caret-down cms-icon-30" />
        <cms:LocalizedLinkButton ID="btnShowFilter" runat="server" OnClick="btnShowFilter_Click" />
    </span>
    <asp:PlaceHolder ID="plcFilter" runat="server" Visible="false">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblFrom" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.from" />
                </div>
                <cms:TextSimpleFilter ID="fltFrom" runat="server" Column="EmailFrom" Size="100" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.toemail" />
                </div>
                <cms:TextSimpleFilter ID="fltTo" runat="server" Column="EmailTo" Size="100" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.subject" />
                </div>
                <cms:TextSimpleFilter ID="fltSubject" runat="server" Column="EmailSubject" Size="450" />
            </div>
            <asp:Panel ID="pnlBodyFilter" runat="server" class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblBody" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="general.body" />
                </div>
                <cms:TextSimpleFilter ID="fltBody" runat="server" Column="EmailBody" />
            </asp:Panel>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblPriority" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="emailqueue.priority" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSDropDownList ID="drpPriority" runat="server" CssClass="DropDownFieldFilter" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblStatus" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="emailqueue.status" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSDropDownList ID="drpStatus" runat="server" CssClass="DropDownFieldFilter" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblErrorMessage" runat="server" EnableViewState="false" DisplayColon="true"
                        ResourceString="emailqueue.lastsendresult" />
                </div>
                <cms:TextSimpleFilter ID="fltLastResult" runat="server" Column="EmailLastSendResult" />
            </div>
            <div class="form-group form-group-buttons">
                <div class="filter-form-buttons-cell-wide">
                    <cms:LocalizedButton ID="btnFilter" runat="server" OnClick="btnFilter_Clicked" ButtonStyle="Primary"
                        EnableViewState="false" ResourceString="General.search" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <cms:EmailQueueGrid ID="gridEmailQueue" runat="server" />
</asp:Content>
