<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_ReportValue_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="ReportValue Edit"  Codebehind="~/CMSModules/Reporting/Tools/ReportValue_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/ReportValue.ascx" TagName="ReportValue"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagName="VersionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <div class="WebpartProperties LightTabs">
        <cms:UITabs ID="tabControlElem" runat="server" />
        <div id="pnlWebPartForm_Properties" class="WebPartForm" runat="server">
            <asp:Panel ID="divScrolable" runat="server" CssClass="dialog-content-scrollable">
                <div id="divPanelHolder" runat="server" class="form-horizontal">
                    <div style="overflow: hidden">
                        <div id="FormPanelHolder" class="ReportFormPanel">
                            <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                            <cms:CategoryPanel ID="DefaultPanel" runat="server" ResourceString="rep.default">
                                <asp:Panel runat="server" ID="DisplayNameRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblDisplayNameRow" runat="server" ResourceString="general.displayname" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtDisplayName" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:LocalizableTextBox runat="server" ID="txtDisplayName"
                                                MaxLength="400" name="txtDefaultName" />
                                            <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                                                Display="Dynamic"></cms:CMSRequiredFieldValidator>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="CodeNameRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblCodeNameRow" runat="server" ResourceString="general.codename" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtCodeName" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CodeName runat="server" ID="txtCodeName" MaxLength="100"
                                                name="DefaultCodeName" />
                                            <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" ControlToValidate="txtCodeName"
                                                Display="Dynamic"></cms:CMSRequiredFieldValidator>
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
                                <asp:Panel runat="server" ID="QueryRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblQueryRow" runat="server" ResourceString="rep.query" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtQuery" />
                                        </div>
                                        <div class="editing-form-value-cell form-field-full-column-width">
                                            <cms:ExtendedTextArea runat="server" ID="txtQuery" Name="txtQuery" EditorMode="Advanced"
                                                Language="SQL" Width="560px" Height="240px" />
                                            <cms:LocalizedLabel runat="server" ID="lblQueryHelp" ResourceString="rep.valuehelp"
                                                CssClass="explanation-text" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="IsProcedureRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblIsProcedureRow" runat="server" ResourceString="rep.isstoredprocedure" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkIsProcedure" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSCheckBox runat="server" ID="chkIsProcedure" name="chkIsProcedure" />
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
                            </cms:CategoryPanel>
                            <cms:CategoryPanel ID="FormatPanel" runat="server" ResourceString="Format">
                                <asp:Panel runat="server" ID="FormatStringRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblFormatStringRow" runat="server" ResourceString="Formatting string" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtFormatString" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox runat="server" ID="txtFormatString" MaxLength="200"
                                                name="txtFormatString" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </cms:CategoryPanel>
                            <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="4" CssClass="anchor-dropup-dialog" />
                        </div>
                    </div>
                </div>
                <asp:Panel runat="server" ID="pnlPreview" CssClass="ReportFormPanel" Visible="false">
                    <cms:ReportValue ID="ctrlReportValue" runat="server" RenderCssClasses="true" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlVersions" CssClass="VersionTab ReportFormPanel"
                    Visible="false">
                    <cms:VersionList ID="versionList" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
        <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
    </div>
</asp:Content>