<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Filter.ascx.cs" Inherits="CMSModules_Modules_Controls_Class_Filter" %>
<asp:Panel runat="server" DefaultButton="btnSearch" CssClass="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblClassDisplayName" runat="server" Visible="true" ResourceString="general.classdisplayname" DisplayColon="true" AssociatedControlID="txtClassDisplayName" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSTextBox ID="txtClassDisplayName" runat="server" />
        </div>
    </div>
    <div class="form-group">
        <div class="filter-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblClassTableName" runat="server" Visible="true" ResourceString="sysdev.class_list.classtablename" DisplayColon="true" AssociatedControlID="txtClassTableName" />
        </div>
        <div class="filter-form-value-cell-wide">
            <cms:CMSTextBox ID="txtClassTableName" runat="server" />
        </div>
    </div>
    <div class="form-group form-group-buttons">
        <div class="filter-form-buttons-cell-wide">
            <cms:LocalizedButton ID="btnReset" runat="server" ResourceString="general.reset"
                EnableViewState="false" ButtonStyle="Default" OnClick="btnReset_Click" />
            <cms:LocalizedButton ID="btnSearch" runat="server" ResourceString="general.search"
                EnableViewState="false" ButtonStyle="Primary" OnClick="btnShow_Click" />
        </div>
    </div>
</asp:Panel>
