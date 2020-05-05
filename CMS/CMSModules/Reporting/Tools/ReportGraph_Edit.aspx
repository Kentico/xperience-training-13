<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_ReportGraph_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    EnableEventValidation="false"  Codebehind="~/CMSModules/Reporting/Tools/ReportGraph_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Selectors/FontSelector.ascx" TagPrefix="cms"
    TagName="FontSelector" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/ReportGraph.ascx" TagName="ReportGraph"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagName="VersionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="WebpartProperties LightTabs">
        <cms:UITabs ID="tabControlElem" runat="server" />
        <div id="pnlWebPartForm_Properties" class="WebPartForm" runat="server">
            <asp:Panel ID="divScrolable" runat="server" CssClass="dialog-content-scrollable">
                <div id="FormPanelHolder" runat="server" class="form-horizontal">
                    <div class="ReportFormPanel" style="overflow: hidden">
                        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                        <cms:CategoryPanel ID="DefaultPanel" runat="server" ResourceString="rep.default">
                            <asp:Panel runat="server" ID="DefaultNameRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtDefaultName" ID="lblDefaultNameRow" runat="server" ResourceString="general.displayname" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:LocalizableTextBox runat="server" ID="txtDefaultName"
                                            MaxLength="400" name="txtDefaultName" />
                                        <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDefaultName"
                                            Display="Dynamic" ValidationGroup="Basic" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultCodeNameRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtDefaultCodeName" ID="lblDefaultCodeNameRow" runat="server" ResourceString="general.codename" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CodeName runat="server" ID="txtDefaultCodeName" MaxLength="100"
                                            name="txtDefaultCodeName" />
                                        <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtDefaultCodeName"
                                            Display="Dynamic" ValidationGroup="Basic" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultEnableExportRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkExportEnable" ID="lblDefaultEnableExportRow" runat="server" ResourceString="rep.enableexport" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkExportEnable" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultSubscriptionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkSubscription" ID="lblDefaultSubscriptionRow" runat="server" ResourceString="rep.enablesubscription" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkSubscription" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="QueryPanel" runat="server" ResourceString="rep.query">
                            <asp:Panel runat="server" ID="QueryQueryRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtQueryQuery" ID="lblQueryQueryRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.Query" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell form-field-full-column-width">
                                        <cms:ExtendedTextArea runat="server" ID="txtQueryQuery" Name="txtQueryQuery" EditorMode="Advanced"
                                            Language="SQL" Width="100%" Height="240px" />
                                        <cms:LocalizedLabel runat="server" ID="lblQueryHelp" ResourceString="rep.queryhelp"
                                            CssClass="explanation-text" EnableViewState="false" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="IsStoredProcedureRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkIsStoredProcedure" ID="lblIsStoredProcedureRow" runat="server" ResourceString="rep.isstoredprocedure" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkIsStoredProcedure" name="chkQueryIsQuery" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ConnectionStringRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucSelectString" ID="lblConnectionStringRow" runat="server" ResourceString="ConnectionString.Title" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:SelectString runat="server" ID="ucSelectString" DisplayInherit="true" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="QueryNoRecordText">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtQueryNoRecordText" ID="lblQueryNoRecordText" runat="server" ResourceString="rep.graph.norecordtext" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" name="txtNoRecordText" ID="txtQueryNoRecordText" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel runat="server" ID="ChartTypePanel" ResourceString="rep.charttype">
                            <asp:Panel runat="server" ID="ChartTypeRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpChartType" ID="lblChartTypeRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphType" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpChartType" name="drpChartType" onChange="typeChanged()" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <%--BAR--%>
                            <asp:Panel runat="server" ID="BarDrawingStyleRow" CssClass="Bar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpBarDrawingStyle" ID="lblBarDrawingStyleRow" runat="server" ResourceString="rep.graph.drawingstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpBarDrawingStyle"
                                            name="drpBarDrawingStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="BarOrientationRow" CssClass="Bar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpBarOrientation" ID="lblBarOrientationRow" runat="server" ResourceString="rep.graph.orientation" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpBarOrientation"
                                            name="drpBarOrientation" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="BarOverlayRow" CssClass="Bar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkBarOverlay" ID="lblBarOverlayRow" runat="server" ResourceString="rep.graph.overlay" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" CssClass="DropDownField" ID="chkBarOverlay" name="chkBarOverlay" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <%--BAR STACKED--%>
                            <asp:Panel runat="server" ID="StackedBarDrawingStyleRow" CssClass="StackedBar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpStackedBarDrawingStyle" ID="lblStackedBarDrawingStyleRow" runat="server" ResourceString="rep.graph.drawingstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpStackedBarDrawingStyle"
                                            name="drpStackedBarDrawingStyle" onChange="stackedBarStyleChanged()" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="BarStackedOrientationRow" CssClass="StackedBar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpBarStackedOrientation" ID="lblBarStackedOrientationRow" runat="server" ResourceString="rep.graph.orientation" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpBarStackedOrientation"
                                            name="drpBarStackedOrientation" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="StackedBar100ProcStacked" CssClass="StackedBar">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkStacked" ID="lblStackedBar100ProcStacked" runat="server" ResourceString="rep.graph.stacked" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkStacked" name="chkStacked" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <!--  PIE CHARTS  //-->
                            <asp:Panel runat="server" ID="PieDrawingStyleRow" CssClass="Pie">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPieDrawingStyle" ID="lblPieDrawingStyleRow" runat="server" ResourceString="rep.graph.drawingstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" onChange="pieStyleChanged()"
                                            ID="drpPieDrawingStyle" name="drpPieDrawingStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PieDrawingDesign" CssClass="Pie">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPieDrawingDesign" ID="lblPieDrawingDesign" runat="server" ResourceString="rep.graph.drawingdesign" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpPieDrawingDesign"
                                            name="drpPieDrawingDesign" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PieLabelStyleRow" CssClass="Pie">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPieLabelStyle" ID="lblPieLabelStyleRow" runat="server" ResourceString="rep.graph.labelstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpPieLabelStyle" name="drpPieLabelStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PieDoughnutRadiusRow" CssClass="Pie">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPieDoughnutRadius" ID="lblPieDoughnutRadiusRow" runat="server" ResourceString="rep.graph.doughnutradius" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpPieDoughnutRadius"
                                            name="drpPieDoughnutRadius" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PieOtherValue" CssClass="Pie">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtPieOtherValue" ID="lblPieOtherValue" runat="server" ResourceString="rep.graph.collectpieslices" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtPieOtherValue" name="txtPieOtherValue" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <!--  Line CHARTS  //-->
                            <asp:Panel runat="server" ID="LineDrawingStyleRow" CssClass="Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpLineDrawingStyle" ID="lblLineDrawingStyleRow" runat="server" ResourceString="rep.graph.drawingstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpLineDrawingStyle"
                                            name="drpLineDrawingStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <!--  General column type  //-->
                            <asp:Panel runat="server" ID="ShowAs3DRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkShowAs3D" ID="lblShowAs3DRow" runat="server" ResourceString="rep.graph.showas3D" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkShowAs3D" onclick="showAs3DClicked()" name="chkShowAs3D" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="RotateXRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtRotateX" ID="lblRotateXRow" runat="server" ResourceString="rep.graph.rotatex" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtRotateX" name="txtRotateX" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="RotateYRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtRotateY" ID="lblRotateYRow" runat="server" ResourceString="rep.graph.rotatey" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtRotateY" name="txtRotateY" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartWidthRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtChartWidth" ID="lblChartWidthRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphWidth" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtChartWidth" MaxLength="50" name="txtChartWidth" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartHeightRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtChartHeight" ID="lblChartHeightRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphHeight" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtChartHeight" MaxLength="50" name="txtChartHeight" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartShowGridRow" CssClass="Grid">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkShowGrid" ID="lblChartShowGridRow" runat="server" ResourceString="rep.graph.showgrid" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkShowGrid" name="chkShowGrid" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="TitlePanel" runat="server" ResourceString="rep.title">
                            <asp:Panel runat="server" ID="TitleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtGraphTitle" ID="lblTitleRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphTitle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtGraphTitle" MaxLength="150" name="txtGraphTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="TitleFontRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucTitleFont" ID="lblTitleFontRow" runat="server" ResourceString="rep.graph.titlefont" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FontSelector runat="server" ID="ucTitleFont" name="ucTitleFont" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="TitleColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucTitleColor" ID="lblTitleColorRow" runat="server" ResourceString="rep.graph.titlecolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucTitleColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="TitlePositionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpTitlePosition" ID="lblTitlePositionRow" runat="server" ResourceString="rep.graph.titleposition" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" ID="drpTitlePosition" CssClass="DropDownField" MaxLength="50"
                                            name="drpTitlePosition" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="LegendPanel" runat="server" ResourceString="rep.legend">
                            <asp:Panel runat="server" ID="LegendBgColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucLegendBgColor" ID="lblLegendBgColorRow" runat="server" ResourceString="rep.graph.legendbgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucLegendBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendBorderColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucLegendBorderColor" ID="lblLegendBorderColorRow" runat="server" ResourceString="rep.graph.bordercolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucLegendBorderColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendBorderSizeRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtLegendBorderSize" ID="lblLegendBorderSizeRow" runat="server" ResourceString="rep.graph.bordersize" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtLegendBorderSize" MaxLength="50" name="txtLegendBorderSize" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendBorderStyleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpLegendBorderStyle" ID="lblLegendBorderStyleRow" runat="server" ResourceString="rep.graph.borderstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpLegendBorderStyle"
                                            ID="drpLegendBorderStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendPositionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpLegendPosition" ID="lblLegendPositionRow" runat="server" ResourceString="rep.graph.position" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" ID="drpLegendPosition"
                                            MaxLength="50" name="drpLegendPosition" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendInsideRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkLegendInside" ID="lblLegendInsideRow" runat="server" ResourceString="rep.graph.legendinside" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkLegendInside" name="chkLegendInside" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="LegendTitle">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtLegendTitle" ID="lblLegendTitle" runat="server" ResourceString="rep.graph.legendtitle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtLegendTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="XAxisPanel" runat="server" ResourceString="rep.xaxis">
                            <asp:Panel runat="server" ID="XAxisTitleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtXAxisTitle" ID="lblXAxisTitleRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphXAxisTitle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtXAxisTitle" MaxLength="150" name="txtXAxisTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisTitleColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucXAxisTitleColor" ID="lblXAxisTitleColorRow" runat="server" ResourceString="rep.graph.titlecolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucXAxisTitleColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisAngleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtXAxisAngle" ID="lblXAxisAngleRow" runat="server" ResourceString="rep.graph.xaxisangle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtXAxisAngle" MaxLength="50" name="txtXAxisAngle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisFormatRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtXAxisFormat" ID="lblXAxisFormatRow" runat="server" ResourceString="rep.graph.xaxisformat" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" MaxLength="150" ID="txtXAxisFormat"
                                            name="txtXAxisFormat" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisTitleFontRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucXAxisTitleFont" ID="lblXAxisTitleFontRow" runat="server" ResourceString="rep.graph.titlefont" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FontSelector runat="server" ID="ucXAxisTitleFont" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisTitlePositionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpXAxisTitlePosition" ID="lblXAxisTitlePositionRow" runat="server" ResourceString="rep.graph.position" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpXAxisTitlePosition"
                                            ID="drpXAxisTitlePosition" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisLabelFont">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucXAxisLabelFont" ID="lblXAxisLabelFont" runat="server" ResourceString="rep.graph.axislabelfont" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FontSelector runat="server" name="ucXAxisLabelFont" ID="ucXAxisLabelFont" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisInterval">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtXAxisInterval" ID="lblXAxisInterval" runat="server" ResourceString="rep.graph.xaxisinterval" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtXAxisInterval" MaxLength="50"
                                            name="txtXAxisInterval" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="XAxisUseSort">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkXAxisSort" ID="lblXAxisUseSort" runat="server" ResourceString="rep.graph.usexsort" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkXAxisSort" name="chkXAxisSort" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="YAxisPanel" runat="server" ResourceString="rep.yaxis">
                            <asp:Panel runat="server" ID="YAxisTitleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtYAxisTitle" ID="lblYAxisTitleRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphYAxisTitle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtYAxisTitle" MaxLength="150"
                                            name="txtYAxisTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisTitleColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucYAxisTitleColor" ID="lblYAxisTitleColorRow" runat="server" ResourceString="rep.graph.titlecolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucYAxisTitleColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisAngleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtYAxisAngle" ID="lblYAxisAngleRow" runat="server" ResourceString="rep.graph.yaxisangle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtYAxisAngle" MaxLength="50"
                                            name="txtYAxisAngle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisFormatRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtYAxisFormat" ID="lblYAxisFormatRow" runat="server" ResourceString="rep.graph.yaxisformat" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtYAxisFormat" MaxLength="150"
                                            name="txtYAxisFormat" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisUseXSettingsRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkYAxisUseXSettings" ID="lblYAxisUseXSettingsRow" runat="server" ResourceString="rep.graph.usexsettings" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkYAxisUseXSettings" name="chkYAxisUseXSettings" onclick="checkXAxisSettings();" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisTitleFontRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucYAxisTitleFont" ID="lblYAxisTitleFontRow" runat="server" ResourceString="rep.graph.titlefont" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FontSelector runat="server" ID="ucYAxisTitleFont" name="ucYAxisTitleFont" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisTitlePositionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpYAxisTitlePosition" ID="lblYAxisTitlePositionRow" runat="server" ResourceString="rep.graph.position" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpYAxisTitlePosition"
                                            ID="drpYAxisTitlePosition" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="YAxisLabelFont">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucYAxisLabelFont" ID="lblYAxisLabelFont" runat="server" ResourceString="rep.graph.axislabelfont" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:FontSelector runat="server" name="ucYAxisLabelFont" ID="ucYAxisLabelFont" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="SeriesPanel" runat="server" ResourceString="rep.series">
                            <asp:Panel runat="server" ID="SeriesPrBgColorRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucSeriesPrBgColor" ID="lblSeriesPrBgColorRow" runat="server" ResourceString="rep.graph.primarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucSeriesPrBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesSecBgColorRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucSeriesSecBgColor" ID="lblSeriesSecBgColorRow" runat="server" ResourceString="rep.graph.secondarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucSeriesSecBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesGradientRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpSeriesGradient" ID="lblSeriesGradientRow" runat="server" ResourceString="rep.graph.gradient" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList class="DropDownField" name="drpSeriesGradient" ID="drpSeriesGradient"
                                            runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesBorderColorRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucSeriesBorderColor" ID="lblSeriesBorderColorRow" runat="server" ResourceString="rep.graph.bordercolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucSeriesBorderColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesBorderSizeRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtSeriesBorderSize" ID="lblSeriesBorderSizeRow" runat="server" ResourceString="rep.graph.bordersize" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtSeriesBorderSize" MaxLength="50"
                                            name="txtSeriesBorderSize" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesBorderStyleRow" CssClass="Common">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpSeriesBorderStyle" ID="lblSeriesBorderStyleRow" runat="server" ResourceString="rep.graph.borderstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList class="DropDownField" name="drpSeriesBorderStyle" ID="drpSeriesBorderStyle"
                                            runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesDisplayItemValue">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkSeriesDisplayItemValue" ID="lblSeriesDisplayItemValue" runat="server" ResourceString="rep.graph.displayitemvalue" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox name="chkSeriesDisplayItemValue" ID="chkSeriesDisplayItemValue" runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemValueFormat">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtItemValueFormat" ID="lblSeriesItemValueFormat" runat="server" ResourceString="rep.graph.itemvalueformat" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtItemValueFormat" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesLineColorRow" CssClass="Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucSeriesLineColor" ID="lblSeriesLineColorRow" runat="server" ResourceString="rep.graph.serieslinecolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucSeriesLineColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesLineBorderSizeRow" CssClass="Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtSeriesLineBorderSize" ID="lblSeriesLineBorderSizeRow" runat="server" ResourceString="rep.graph.serieslinesbordersize" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtSeriesLineBorderSize"
                                            MaxLength="50" name="txtSeriesLineBorderSize" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesLineBorderStyleRow" CssClass="Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpSeriesLineBorderStyle" ID="lblSeriesLineBorderStyleRow" runat="server" ResourceString="rep.graph.serieslineborderstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList class="DropDownField" name="drpSeriesLineBorderStyle" ID="drpSeriesLineBorderStyle"
                                            runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesSymbols" CssClass="Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpSeriesSymbols" ID="lblSeriesSymbols" runat="server" ResourceString="rep.graph.symbols" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList class="DropDownField" name="drpSeriesSymbols" ID="drpSeriesSymbols"
                                            runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemTooltip">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtSeriesItemTooltip" ID="lblSeriesItemTooltip" runat="server" ResourceString="rep.graph.itemtooltip" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:LargeTextArea runat="server" ID="txtSeriesItemTooltip" name="txtSeriesItemTooltip" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemLink">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtSeriesItemLink" ID="lblSeriesItemLink" runat="server" ResourceString="rep.graph.itemlink" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtSeriesItemLink" MaxLength="100"
                                            name="txtSeriesItemLink" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesValuesAsPercent" CssClass="StackedBar Bar Line">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkValuesAsPercent" ID="lblSeriesValuesAsPercent" runat="server" ResourceString="rep.graph.valuesaspercent" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox name="chkValuesAsPercent" ID="chkValuesAsPercent" runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="ChartAreaPanel" runat="server" ResourceString="rep.chartarea">
                            <asp:Panel runat="server" ID="ChartAreaPrBgColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucChartAreaPrBgColor" ID="lblChartAreaPrBgColorRow" runat="server" ResourceString="rep.graph.primarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucChartAreaPrBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaSecBgColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucChartAreaSecBgColor" ID="lblChartAreaSecBgColorRow" runat="server" ResourceString="rep.graph.secondarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucChartAreaSecBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaGradientRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpChartAreaGradient" ID="lblChartAreaGradientRow" runat="server" ResourceString="rep.graph.gradient" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpChartAreaGradient"
                                            ID="drpChartAreaGradient" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaBorderColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucChartAreaBorderColor" ID="lblChartAreaBorderColorRow" runat="server" ResourceString="rep.graph.bordercolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucChartAreaBorderColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaBorderSizeRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtChartAreaBorderSize" ID="lblChartAreaBorderSizeRow" runat="server" ResourceString="rep.graph.bordersize" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtChartAreaBorderSize"
                                            MaxLength="50" name="txtChartAreaBorderSize" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaBorderStyleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpChartAreaBorderStyle" ID="lblChartAreaBorderStyleRow" runat="server" ResourceString="rep.graph.borderstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpChartAreaBorderStyle"
                                            ID="drpChartAreaBorderStyle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaScaleMin">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtScaleMin" ID="lblChartAreaScaleMin" runat="server" ResourceString="rep.graph.scalemin" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" name="txtScaleMin" ID="txtScaleMin" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaScaleMax">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtScaleMax" ID="lblChartAreaScaleMax" runat="server" ResourceString="rep.graph.scalemax" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" name="txtScaleMax" ID="txtScaleMax" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaTenPowers">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkTenPowers" ID="lblChartAreaTenPowers" runat="server" ResourceString="rep.graph.tenpowers" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkTenPowers" name="chkTenPowers" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaReverseYRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="chkReverseY" ID="lblChartAreaReverseYRow" runat="server" ResourceString="rep.graph.reversey" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkReverseY" name="chkReverseY" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ChartAreaBorderSkinStyle">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpBorderSkinStyle" ID="lblChartAreaBorderSkinStyle" runat="server" ResourceString="rep.graph.borderskinstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList ID="drpBorderSkinStyle" runat="server" CssClass="DropDownField" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="PlotAreaPanel" runat="server" ResourceString="rep.plotarea">
                            <asp:Panel runat="server" ID="PlotAreaPrBgColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucPlotAreaPrBgColor" ID="lblPlotAreaPrBgColorRow" runat="server" ResourceString="rep.graph.primarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucPlotAreaPrBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PlotAreaSecBgColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucPlotAreSecBgColor" ID="lblPlotAreaSecBgColorRow" runat="server" ResourceString="rep.graph.secondarybgcolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucPlotAreSecBgColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PlotAreaGradientRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPlotAreaGradient" ID="lblPlotAreaGradientRow" runat="server" ResourceString="rep.graph.gradient" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList runat="server" CssClass="DropDownField" name="drpPlotAreaGradient"
                                            ID="drpPlotAreaGradient" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PlotAreaBorderColorRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="ucPlotAreaBorderColor" ID="lblPlotAreaBorderColorRow" runat="server" ResourceString="rep.graph.bordercolor" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:ColorPicker runat="server" ID="ucPlotAreaBorderColor" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PlotAreaBorderSizeRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="txtPlotAreaBorderSize" ID="lblPlotAreaBorderSizeRow" runat="server" ResourceString="rep.graph.bordersize" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtPlotAreaBorderSize"
                                            MaxLength="50" name="txtPlotAreaBorderSize" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="PlotAreaBorderStyleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel AssociatedControlID="drpPlotAreaBorderStyle" ID="lblPlotAreaBorderStyleRow" runat="server" ResourceString="rep.graph.borderstyle" CssClass="control-label" DisplayColon="true" EnableViewState="false" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList class="DropDownField" name="drpPlotAreaBorderStyle" ID="drpPlotAreaBorderStyle"
                                            runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="4" CssClass="anchor-dropup-dialog" />
                    </div>
                </div>
                <asp:HiddenField ID="txtNewGraphHidden" runat="server" />
                <asp:Panel runat="server" ID="pnlPreview" CssClass="ReportFormPanel" Visible="false">
                    <cms:ReportGraph ID="ctrlReportGraph" runat="server" Visible="false" RenderCssClasses="true" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlVersions" CssClass="VersionTab ReportFormPanel" Visible="false">
                    <cms:VersionList ID="versionList" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
