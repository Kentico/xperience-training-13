<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MVTestReport.aspx.cs" Inherits="CMSModules_OnlineMarketing_Pages_Content_MVTest_MVTestReport"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MV test reports"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/FormControls/SelectConversion.ascx" TagName="SelectConversion"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/FormControls/SelectMVTCombination.ascx"
    TagName="SelectCombinaton" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>

<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="header-actions-container">
        <cms:ReportHeader runat="server" ID="reportHeader" />
    </div>
</asp:Content>
<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled;CMSMVTEnabled" />
    </asp:Panel>
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
    <cms:GraphType runat="server" ID="ucGraphType" />
    <asp:Panel ID="pnlRadioButtons" runat="server" CssClass="header-panel">
        <div class="radio-list-vertical">
            <cms:CMSRadioButton runat="server" ID="rbCount" ResourceString="conversion.count"
                AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
            <cms:CMSRadioButton runat="server" ID="rbValue" ResourceString="conversions.value"
                AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
            <cms:CMSRadioButton runat="server" ID="rbRate" ResourceString="abtesting.conversionsrate"
                AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
            <cms:CMSRadioButton runat="server" ID="rbCombinations" ResourceString="mvtests.conversionsbycombination"
                AutoPostBack="true" CssClass="PageReportRadioButton" GroupName="Radio" />
        </div>
    </asp:Panel>
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblConversions" runat="server" ResourceString="abtesting.conversions"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:SelectConversion runat="server" ID="ucConversions" SelectionMode="SingleDropDownList" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="pnlCombination">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCombination" runat="server" ResourceString="mvt.combination"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SelectCombinaton runat="server" ID="usCombination" PostbackOnChange="true" AllowAll="true"
                        AllowEmpty="false" ReturnColumnName="MVTCombinationName" />
                </div>
            </div>
        </asp:PlaceHolder>

    </div>
    <asp:Panel runat="server" ID="pnlContent">
    </asp:Panel>
</asp:Content>
