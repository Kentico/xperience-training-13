<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Activities_Controls_UI_Activity_Filter"
     Codebehind="Filter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Activities/FormControls/ActivityTypeSelector.ascx"
    TagName="ActivityTypeSel" TagPrefix="cms" %>
<asp:Panel ID="pnl" runat="server" DefaultButton="btnFilter">
    <div class="form-horizontal form-filter">
        <asp:PlaceHolder runat="server" ID="plcSite" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false"
                        ResourceString="general.site" DisplayColon="true" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" EnableViewState="false" ResourceString="general.type"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:ActivityTypeSel ID="drpType" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblName" runat="server" EnableViewState="false" ResourceString="om.activity.gridtitle"
                    DisplayColon="true" />
            </div>
            <cms:TextSimpleFilter ID="fltName" runat="server" Column="ActivityTitle" />
        </div>
        <asp:PlaceHolder runat="server" ID="plcCon" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblContactFirstName" runat="server" EnableViewState="false" ResourceString="om.contact.firstname"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltContactFirstName" runat="server" Column="ContactFirstName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblContactMiddleName" runat="server" EnableViewState="false" ResourceString="om.contact.middlename"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltContactMiddleName" runat="server" Column="ContactMiddleName" />
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblContactLastName" runat="server" EnableViewState="false" ResourceString="om.contact.lastname"
                        DisplayColon="true" />
                </div>
                <cms:TextSimpleFilter ID="fltContactLastName" runat="server" Column="ContactLastName" />
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblTimeBetween" runat="server" EnableViewState="false" ResourceString="eventlog.timebetween"
                    DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:TimeSimpleFilter ID="fltTimeBetween" runat="server" Column="ActivityCreated" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" />
                <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Primary" ResourceString="general.filter" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
