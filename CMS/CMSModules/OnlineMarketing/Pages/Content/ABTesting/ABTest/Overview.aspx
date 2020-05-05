<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Overview.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="A/B Test Overview" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_Overview"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms"
    TagName="SmartTip" %>

<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SmartTip runat="server" ID="tipHowToOverview" />
    <asp:Panel runat="server" CssClass="ab-overview">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblConversions" CssClass="control-label" runat="server" ResourceString="abtesting.conversiongoals" DisplayColon="True" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:SelectConversion runat="server" ID="drpConversions" SelectionMode="SingleDropDownList" PostbackOnDropDownChange="true" />
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
            <div id="AdvancedControls" runat="server" style="display: none">
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
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel ID="lblCulture" CssClass="control-label" runat="server" ResourceString="abtesting.culture" DisplayColon="True" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:UniSelector ID="drpCulture" runat="server" ObjectType="CMS.Culture" ObjectSiteName="#currentsite"
                            AllowAll="True" AllowEmpty="False" ReturnColumnName="CultureCode" AllRecordValue="" />
                    </div>
                </div>
            </div>
            <a href="#" id="spnShowAdvancedFilters" runat="server" class="simple-advanced-link" />
        </div>
        <div class="btn-actions">
            <cms:CMSButtonGroup runat="server" ID="samplingElem" />
            <cms:CMSButtonGroup runat="server" ID="graphDataElem" />
        </div>
        <div class="report-panel">
            <asp:Panel ID="pnlReport" runat="server">
                <cms:DisplayReport ID="displayReport" runat="server" IsLiveSite="false" />
                <asp:Panel ID="pnlNoData" runat="server" Visible="false">
                    <cms:LocalizedLabel runat="server" ResourceString="abtest.nodata" EnableViewState="False" />
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
        <cms:UniGrid ID="gridElem" runat="server" OnOnExternalDataBound="gridElem_OnExternalDataBound" OnOnAction="gridElem_OnAction" OrderBy="ABVariantID" Visible="false">
            <GridActions>
                <ug:Action Name="selectwinner" ExternalSourceName="selectwinner" Caption="abtesting.selectwinner" FontIconClass="icon-trophy" Confirmation="$abtesting.gridwinnerselectionconfirmation$" CommandArgument="ABVariantName" />
            </GridActions>
            <GridColumns>
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.variantdisplayname$" ExternalSourceName="name" Wrap="false" />
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.conversionsovervisits$" ExternalSourceName="conversionsovervisits" Wrap="false" AllowSorting="false" ID="columnConversionsOverVisits" />
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.conversionsrate$" ExternalSourceName="conversionrate" Wrap="false" AllowSorting="false" ID="columnConversionRateInterval" />
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.chancetobeatoriginal$" ExternalSourceName="chancetobeatoriginal" Wrap="false" AllowSorting="false" ID="columnChanceToBeatOriginal" />
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.totalconversionvalue$" ExternalSourceName="conversionvalue" Wrap="false" AllowSorting="false" Visible="False" ID="columnConversionValue" />
                <ug:Column runat="server" Source="ABVariantName" Caption="$abtesting.averageconversionvalue$" ExternalSourceName="averageconversionvalue" Wrap="false" AllowSorting="false" Visible="False" ID="columnAvgConversionValue" />
                <ug:Column runat="server" Source="ABVariantName" ExternalSourceName="improvement" Wrap="false" AllowSorting="false" ID="columnImprovement" />
                <ug:Column runat="server" CssClass="main-column-100" />
            </GridColumns>
        </cms:UniGrid>
    </asp:Panel>
</asp:Content>
