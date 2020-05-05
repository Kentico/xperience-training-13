<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AdvancedExport.ascx.cs"
    Inherits="CMSAdminControls_UI_UniGrid_Controls_AdvancedExport" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/OrderByControl.ascx" TagName="OrderByControl"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" UpdateMode="Conditional" runat="server">
    <ContentTemplate>
        <cms:ModalPopupDialog ID="mdlAdvancedExport" runat="server" BackgroundCssClass="ModalBackground"
            CssClass="ModalPopupDialog advanced-export">
            <asp:Panel ID="pnlAdvancedExport" runat="server" Visible="false" CssClass="DialogPageBody" Width="700px">
                <div class="PageHeader">
                    <cms:PageTitle ID="advancedExportTitle" runat="server" EnableViewState="false" />
                </div>
                <asp:Panel ID="pnlScrollable" runat="server" CssClass="DialogPageContent DialogScrollableContent">
                    <div class="PageBody">
                        <asp:PlaceHolder ID="plcAdvancedExport" runat="server">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblExportTo" runat="server" ResourceString="export.exportto"
                                            DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="drpExportTo" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSDropDownList ID="drpExportTo" runat="server" OnSelectedIndexChanged="drpExportTo_SelectedIndexChanged"
                                            AutoPostBack="true" />
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="plcDelimiter" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblDelimiter" runat="server" ResourceString="export.delimiter"
                                                DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="drpDelimiter" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSDropDownList ID="drpDelimiter" runat="server" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcExportRawData" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblExportRawData" runat="server" ResourceString="export.exportrawdata"
                                                DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="chkExportRawData" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSCheckBox ID="chkExportRawData" runat="server" OnCheckedChanged="chkExportRawData_CheckedChanged"
                                                AutoPostBack="true" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblCurrentPageOnly" runat="server" ResourceString="export.currentpage"
                                            DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="chkCurrentPageOnly" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox ID="chkCurrentPageOnly" runat="server" Checked="true" AutoPostBack="true" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblRecords" runat="server" ResourceString="export.records"
                                            DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="txtRecords" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox ID="txtRecords" runat="server" />
                                        <cms:CMSRegularExpressionValidator ID="revRecords" runat="server" Display="Dynamic"
                                            ControlToValidate="txtRecords" EnableClientScript="true" />
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="plcExportHeader" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblExportHeader" runat="server" ResourceString="export.columnheader"
                                                DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="chkExportHeader" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSCheckBox ID="chkExportHeader" runat="server" Checked="true" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel ID="lblOrderBy" runat="server" ResourceString="export.orderby"
                                            DisplayColon="true" EnableViewState="false" CssClass="control-label" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:OrderByControl ID="orderByElem" runat="server" ShortID="o" DelayedReload="True"
                                            Mode="DropDownList" />
                                        <cms:ExtendedTextArea ID="txtOrderBy" runat="server" EditorMode="Basic" Language="SQL" />
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="plcWhere" runat="server">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel ID="lblWhereCondition" runat="server" ResourceString="export.where"
                                                DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="txtWhereCondition" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:ExtendedTextArea ID="txtWhereCondition" runat="server" EditorMode="Basic" Language="SQL" />
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="form-group">
                                    <cms:LocalizedHeading ID="headColumns" runat="server" Level="4" EnableViewState="false" ResourceString="export.columns" />
                                    <div class="form-group">
                                        <cms:LocalizedButton ID="btnSelectAll" runat="server" ResourceString="export.selectall"
                                            EnableViewState="false" ButtonStyle="Default" />
                                        <cms:LocalizedButton ID="btnDeselectAll" runat="server" ResourceString="export.deselectall"
                                            EnableViewState="false" ButtonStyle="Default" />
                                        <cms:LocalizedButton ID="btnDefaultSelection" runat="server" ResourceString="export.defaultselection"
                                            ButtonStyle="Default" />
                                    </div>
                                    <div class="form-group">
                                        <div class="ColumnValidator">
                                            <asp:CustomValidator ID="cvColumns" runat="server" Display="Dynamic" EnableClientScript="true" CssClass="form-control-error"/>
                                        </div>
                                    </div>
                                    <cms:CMSCheckBoxList ID="chlColumns" runat="server" CssClass="ColumnsCheckBoxList" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
                <div class="PageFooterLine">
                    <div class="Buttons">
                        <cms:LocalizedButton ID="btnPreview" runat="server" EnableViewState="false" ResourceString="general.preview"
                            ButtonStyle="Default" OnClick="btnPreview_Click" />
                        <cms:LocalizedButton ID="btnExport" runat="server" EnableViewState="false" ResourceString="general.export"
                            ButtonStyle="Primary" OnClick="btnExport_Click" />
                    </div>
                </div>
            </asp:Panel>
        </cms:ModalPopupDialog>
        <asp:HiddenField ID="hdnDefaultSelection" runat="server" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:HiddenField ID="hdnParameter" runat="server" />
<asp:Button ID="btnFullPostback" UseSubmitBehavior="false" runat="server" CssClass="HiddenButton"
    OnClick="btnFullPostback_Click" Style="display: none;" />
