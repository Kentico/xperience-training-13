<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_ReportTable_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="ReportTable Edit" ValidateRequest="false" EnableEventValidation="false"
     Codebehind="~/CMSModules/Reporting/Tools/ReportTable_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/ReportTable.ascx" TagName="ReportTable"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Versioning/ObjectVersionList.ascx"
    TagName="VersionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
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
                                                ErrorMessage="general.requiresdisplayname" Display="Dynamic"></cms:CMSRequiredFieldValidator>
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
                                                ErrorMessage="general.requirescodename" Display="Dynamic"></cms:CMSRequiredFieldValidator>
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
                                <asp:Panel runat="server" ID="QueryRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblQueryRow" runat="server" ResourceString="rep.query" CssClass="control-label" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="txtQuery" />
                                        </div>
                                        <div class="editing-form-value-cell form-field-full-column-width">
                                            <cms:ExtendedTextArea runat="server" ID="txtQuery" Name="txtQuery" EditorMode="Advanced"
                                                Language="SQL" Width="560px" Height="240px" />
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
                            <cms:CategoryPanel ID="SkinPanel" runat="server" ResourceString="rep.table.skin">
                                <asp:Panel runat="server" ID="SkinSkinIdRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblSkinSkinIdRow" runat="server" ResourceString="rep.table.skinid" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtSkinID" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox runat="server" ID="txtSkinID" MaxLength="50"
                                                name="txtSkinID" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </cms:CategoryPanel>
                            <cms:CategoryPanel ID="PagingPanel" runat="server" ResourceString="rep.table.paging">
                                <asp:Panel runat="server" ID="PagingEnableRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblPagingEnableRow" runat="server" ResourceString="rep.table.enablepaging" CssClass="control-label" DisplayColon="true" AssociatedControlID="chkEnablePaging" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSCheckBox runat="server" ID="chkEnablePaging" name="chkEnablePaging" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="PagingPageSizeRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblPagingPageSizeRow" runat="server" ResourceString="rep.table.pagesize" CssClass="control-label" DisplayColon="true" AssociatedControlID="txtPageSize" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox runat="server" ID="txtPageSize" MaxLength="50"
                                                name="txt PageSize" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="PagingPageModeRow">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblPagingPageModeRow" runat="server" ResourceString="rep.table.pagingmode" CssClass="control-label" DisplayColon="true" AssociatedControlID="drpPageMode" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSDropDownList runat="server" class="DropDownField" name="drpPageMode" ID="drpPageMode" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </cms:CategoryPanel>
                            <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="4" CssClass="anchor-dropup-dialog" />
                        </div>
                    </div>
                </div>
                <asp:Panel runat="server" ID="pnlPreview" Visible="false" CssClass="ReportFormPanel">
                    <cms:ReportTable ID="ctrlReportTable" runat="server" RenderCssClasses="true" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlVersions" CssClass="VersionTab ReportFormPanel"
                    Visible="false">
                    <cms:VersionList ID="versionList" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
        <asp:HiddenField ID="txtNewTableHidden" runat="server" />
        <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
    </div>
</asp:Content>