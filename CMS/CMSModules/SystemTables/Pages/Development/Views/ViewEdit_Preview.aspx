<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_SystemTables_Pages_Development_Views_ViewEdit_Preview" Title="View - Preview"
    Theme="Default"  Codebehind="ViewEdit_Preview.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblShowItmes" runat="server" Visible="true" CssClass="control-label"
                    ResourceString="systbl.views.numberofitems" DisplayColon="true" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSDropDownList ID="drpItems" runat="server" AutoPostBack="true" />
            </div>
        </div>
    </div>
    <cms:LocalizedLabel ID="lblNoDataFound" runat="server" Visible="false" CssClass="FieldLabel"
        ResourceString="general.nodatafound" EnableViewState="false" />
    <cms:UIGridView ID="grdData" runat="server" AllowSorting="false" />
</asp:Content>