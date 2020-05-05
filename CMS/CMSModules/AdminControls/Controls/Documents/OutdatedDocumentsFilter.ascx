<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="OutdatedDocumentsFilter.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Documents_OutdatedDocumentsFilter" %>
<asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblFilter" AssociatedControlID="txtFilter" runat="server"
                    EnableViewState="false" ResourceString="MyDesk.OutdatedDocuments.Filter" DisplayColon="true" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSTextBox ID="txtFilter" runat="server" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList ID="drpFilter" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDocumentName" runat="server" EnableViewState="false" ResourceString="general.documentname"
                    DisplayColon="true" AssociatedControlID="txtDocumentName" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSDropDownList ID="drpDocumentName" runat="server" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtDocumentName" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDocumentType" runat="server" EnableViewState="false" ResourceString="general.type"
                    DisplayColon="true" AssociatedControlID="txtDocumentType" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSDropDownList ID="drpDocumentType" runat="server" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtDocumentType" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" ResourceString="general.reset" OnClick="btnReset_Click" />
                <cms:LocalizedButton ID="btnShow" runat="server" EnableViewState="false" ButtonStyle="Primary"
                    ResourceString="general.search" OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
