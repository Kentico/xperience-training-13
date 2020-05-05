<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Controls_GroupFilter"  Codebehind="GroupFilter.ascx.cs" %>
<asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblGroupName" ResourceString="groups.groupname" DisplayColon="true" runat="server" CssClass="control-label" EnableViewState="false" />
            </div>
            <div class="filter-form-condition-cell">
                <cms:CMSDropDownList ID="drpGroupName" runat="server" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtGroupName" runat="server" MaxLength="200" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblGroupStatus" ResourceString="groups.status" runat="server" DisplayColon="true" CssClass="control-label" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSDropDownList ID="drpGroupStatus" runat="server" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Default" EnableViewState="false" ResourceString="general.reset" OnClick="btnReset_Click" />
                <cms:LocalizedButton ResourceString="general.search" ID="btnSearch" runat="server" ButtonStyle="Primary" EnableViewState="false" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>
</asp:Panel>
