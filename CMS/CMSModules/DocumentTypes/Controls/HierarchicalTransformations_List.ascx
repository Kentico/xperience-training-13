<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="HierarchicalTransformations_List.ascx.cs"
    Inherits="CMSModules_DocumentTypes_Controls_HierarchicalTransformations_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlFilter">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDocTypes" ResourceString="development.documenttypes" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtDocTypes" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLevel" ResourceString="development.level" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtLevel" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton runat="server" ID="btnShow" ButtonStyle="Primary" ResourceString="general.filter" />
            </div>
        </div>
    </div>
</asp:Panel>
<div >
    <cms:UniGrid runat="server" ID="ugTransformations" IsLiveSite="false" ExportFileName="cms_transformation"/>
</div>
