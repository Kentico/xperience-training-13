<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    EnableEventValidation="false"  Codebehind="ReportHtmlGraph_Edit.aspx.cs" Inherits="CMSModules_Reporting_Tools_ReportHtmlGraph_Edit" %>

<%@ Register Src="~/CMSFormControls/Inputs/LargeTextArea.ascx" TagName="LargeTextArea"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/HtmlBarGraph.ascx" TagName="ReportGraph"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagName="VersionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="WebpartProperties LightTabs">
        <cms:UITabs ID="tabControlElem" runat="server" />
        <div id="pnlWebPartForm_Properties" class="WebPartForm" runat="server">
            <asp:Panel ID="divScrolable" runat="server" CssClass="dialog-content-scrollable">
                <div class="form-horizontal" id="FormPanelHolder" runat="server">
                    <div class="ReportFormPanel" runat="server" style="overflow: hidden">
                        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                        <cms:CategoryPanel ID="DefaultPanel" runat="server" ResourceString="rep.default">
                            <asp:Panel runat="server" ID="DefaultNameRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDefaultNameRow" runat="server" ResourceString="general.displayname" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtDefaultName" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:LocalizableTextBox runat="server" ID="txtDefaultName"
                                            MaxLength="400" name="txtDefaultName" />
                                        <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDefaultName"
                                            Display="Dynamic"></cms:CMSRequiredFieldValidator>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultCodeNameRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDefaultCodeNameRow" runat="server" ResourceString="general.codename" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtDefaultCodeName" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CodeName runat="server" ID="txtDefaultCodeName" MaxLength="100"
                                            name="txtDefaultCodeName" />
                                        <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtDefaultCodeName"
                                            Display="Dynamic"></cms:CMSRequiredFieldValidator>
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultEnableExportRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDefaultEnableExportRow" runat="server" ResourceString="rep.enableexport" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkExportEnable" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkExportEnable" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DefaultSubscriptionRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDefaultSubscriptionRow" runat="server" ResourceString="rep.enablesubscription" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkSubscription" />
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
                                        <cms:LocalizedLabel ID="lblQueryQueryRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.Query" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtQueryQuery" />
                                    </div>
                                    <div class="editing-form-value-cell form-field-full-column-width">
                                        <cms:ExtendedTextArea runat="server" ID="txtQueryQuery" Name="txtQueryQuery" EditorMode="Advanced"
                                            Language="SQL" Width="560px" Height="240px" />
                                        <cms:LocalizedLabel runat="server" ID="lblQueryHelp" ResourceString="rep.queryhelp"
                                            CssClass="explanation-text" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="IsStoredProcedureRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblIsStoredProcedureRow" runat="server" ResourceString="rep.isstoredprocedure" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkIsStoredProcedure" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkIsStoredProcedure" name="chkQueryIsQuery" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="ConnectionStringRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblConnectionStringRow" runat="server" ResourceString="ConnectionString.Title" CssClass="control-label" DisplayColon="true" AssociatedControlID="ucSelectString" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:SelectString runat="server" ID="ucSelectString" DisplayInherit="true" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="QueryNoRecordText">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblQueryNoRecordText" runat="server" ResourceString="rep.graph.norecordtext" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtQueryNoRecordText" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" name="txtNoRecordText" ID="txtQueryNoRecordText" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="TitlePanel" runat="server" ResourceString="rep.title">
                            <asp:Panel runat="server" ID="TitleRow">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblTitleRow" runat="server" ResourceString="Reporting_ReportGraph_Edit.GraphTitle" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtGraphTitle" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtGraphTitle" MaxLength="150"
                                            name="txtGraphTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="LegendPanel" runat="server" ResourceString="rep.legend">
                            <asp:Panel runat="server" ID="LegendTitle">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblLegendTitle" runat="server" ResourceString="rep.graph.legendtitle" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtLegendTitle" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtLegendTitle" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="DisplayLegend">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblDisplayLegend" runat="server" ResourceString="rep.graph.displaylegend" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkDisplayLegend" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkDisplayLegend" name="chkDisplayLegend" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:CategoryPanel ID="SeriesPanel" runat="server" ResourceString="rep.series">
                            <asp:Panel runat="server" ID="SeriesItemNameFormat">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblSeriesItemNameFormat" runat="server" ResourceString="rep.graph.itemnameformat" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtItemNameFormat" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtItemNameFormat" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemValueFormat">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblSeriesItemValueFormat" runat="server" ResourceString="rep.graph.itemvalueformat" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtItemValueFormat" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtItemValueFormat" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemTooltip">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblSeriesItemTooltip" runat="server" ResourceString="rep.graph.itemtooltip" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtSeriesItemTooltip" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:LargeTextArea runat="server" ID="txtSeriesItemTooltip"
                                            name="txtSeriesItemTooltip" />
                                    </div>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="SeriesItemLink">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblSeriesItemLink" runat="server" ResourceString="rep.graph.itemlink" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtSeriesItemLink" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtSeriesItemLink" MaxLength="100"
                                            name="txtSeriesItemLink" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </cms:CategoryPanel>
                        <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="4" CssClass="anchor-dropup-dialog" />
                    </div>
                </div>
                <asp:HiddenField ID="txtNewGraphHidden" runat="server" />
                <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
                <asp:Panel runat="server" ID="pnlPreview" CssClass="ReportFormPanel" Visible="false">
                    <cms:ReportGraph ID="ctrlReportGraph" runat="server" Visible="false" RenderCssClasses="true"
                        IsLiveSite="false" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlVersions" CssClass="VersionTab ReportFormPanel"
                    Visible="false">
                    <cms:VersionList ID="versionList" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Content>