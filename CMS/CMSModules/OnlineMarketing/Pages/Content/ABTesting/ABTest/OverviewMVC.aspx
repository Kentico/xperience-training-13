<%@ Page Language="C#" AutoEventWireup="false"  Codebehind="OverviewMVC.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="A/B Test Overview" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_OverviewMVC"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" CssClass="ab-overview">
        <div id="Filter" runat="server" visible="true">
            <div class="form-horizontal form-filter">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblConversions" CssClass="control-label" runat="server" ResourceString="abtesting.conversiongoals" DisplayColon="True" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:CMSDropDownList runat="server" ID="drpConversions" UseResourceStrings="true" AutoPostBack="true" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblSuccessMetrics" CssClass="control-label" runat="server" ResourceString="abtesting.metric" DisplayColon="True" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:CMSDropDownList ID="drpSuccessMetric" runat="server" UseResourceStrings="true" AutoPostBack="true">
                            <asp:ListItem Value="conversionrate" Text="abtesting.conversionsrate" Selected="true" />
                            <asp:ListItem Value="conversionvalue" Text="abtesting.conversionsvalue" />
                            <asp:ListItem Value="averageconversionvalue" Text="abtesting.averageconversionvalue" />
                            <asp:ListItem Value="conversioncount" Text="abtesting.conversionscount" />
                        </cms:CMSDropDownList>
                    </div>
                </div>
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblCountingMethodology" CssClass="control-label" runat="server" ResourceString="abtesting.conversiontype" DisplayColon="True" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:CMSDropDownList ID="drpCountingMethodology" runat="server" UseResourceStrings="true" AutoPostBack="true">
                            <asp:ListItem Value="absessionconversionfirst" Text="abtesting.conversiontype.visitor" Selected="true" />
                            <asp:ListItem Value="absessionconversion%" Text="abtesting.conversiontype.session" />
                            <asp:ListItem Value="abconversion" Text="abtesting.conversiontype.transaction" />
                        </cms:CMSDropDownList>
                    </div>
                </div>
            </div>
            <div class="btn-actions">
                <cms:CMSButtonGroup runat="server" ID="samplingElem" />
                <cms:CMSButtonGroup runat="server" ID="graphDataElem" />
            </div>
        </div>
        <div id="Report" runat="server" class="report-panel">
            <asp:Panel ID="pnlReport" runat="server">
                <cms:DisplayReport ID="displayReport" runat="server" IsLiveSite="false" />
                <asp:Panel ID="pnlNoData" runat="server" Visible="false">
                    <cms:LocalizedLabel runat="server" ResourceString="abtest.nodata" EnableViewState="False" />
                    <br /><br />
                </asp:Panel>
            </asp:Panel>
        </div>
        <div class="summary" id="Summary" runat="server" visible="false">
            <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="abtesting.overview.summary.title" EnableViewState="false" />
            <table class="summary-table">
                <tr>
                    <th>
                        <cms:LocalizedLiteral DisplayColon="True" runat="server" EnableViewState="false" ID="lblVisits" ResourceString="abtesting.overview.summary.visits" />
                    </th>
                    <td>
                        <strong>
                            <asp:Literal ID="lblTotalVisitors" runat="server" />
                        </strong>
                    </td>
                </tr>
                <tr>
                    <th>
                        <cms:LocalizedLiteral DisplayColon="True" runat="server" EnableViewState="false" ResourceString="abtesting.overview.summary.conversions" />
                    </th>
                    <td>
                        <strong>
                            <asp:Literal ID="lblTotalConversions" runat="server" />
                        </strong>
                    </td>
                </tr>
                <tr>
                    <th>
                        <cms:LocalizedLiteral DisplayColon="True" runat="server" EnableViewState="false" ResourceString="abtesting.page" />
                    </th>
                    <td>
                        <a runat="server" id="lnkTest" Visible="false"></a>
                        <asp:Literal ID="lblTest" Visible="false" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <cms:LocalizedLiteral DisplayColon="True" runat="server" EnableViewState="false" ResourceString="general.status" />
                    </th>
                    <td>
                        <asp:Label ID="lblStatus" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <cms:LocalizedLiteral DisplayColon="True" ID="txtDuration" runat="server" EnableViewState="false" ResourceString="abtesting.daysrunning" />
                    </th>
                    <td>
                        <asp:Label ID="lblDuration" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="ClearBoth"></div>
        <cms:UniGrid ID="gridElem" runat="server" OnOnExternalDataBound="gridElem_OnExternalDataBound" OnOnAction="gridElem_OnAction" OrderBy="ABVariantIsOriginal DESC, ABVariantDisplayName">
            <GridActions>
                <ug:ButtonAction Name="selectwinner" ExternalSourceName="selectwinner" Caption="abtesting.selectwinner" Confirmation="$abtesting.gridwinnerselectionconfirmation$"
                    CommandArgument="ABVariantGuid" ButtonClass="btn btn-secondary" />
            </GridActions>
            <GridColumns>
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.variantdisplayname$" ExternalSourceName="name" Wrap="false" />
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.conversionsovervisits$" ExternalSourceName="conversionsovervisits" Wrap="false" AllowSorting="false" ID="columnConversionsOverVisits" />
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.conversionsrate$" ExternalSourceName="conversionrate" Wrap="false" AllowSorting="false" ID="columnConversionRateInterval" />
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.chancetobeatoriginal$" ExternalSourceName="chancetobeatoriginal" Wrap="false" AllowSorting="false" ID="columnChanceToBeatOriginal" />
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.totalconversionvalue$" ExternalSourceName="conversionvalue" Wrap="false" AllowSorting="false" Visible="False" ID="columnConversionValue" />
                <ug:Column runat="server" Source="ABVariantGuid" Caption="$abtesting.averageconversionvalue$" ExternalSourceName="averageconversionvalue" Wrap="false" AllowSorting="false" Visible="False" ID="columnAvgConversionValue" />
                <ug:Column runat="server" Source="ABVariantGuid" ExternalSourceName="improvement" Wrap="false" AllowSorting="false" ID="columnImprovement" />
                <ug:Column runat="server" CssClass="main-column-100" />
            </GridColumns>
        </cms:UniGrid>
    </asp:Panel>
</asp:Content>
