<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="BadWordsFilter.ascx.cs" Inherits="CMSModules_BadWords_Controls_BadWordsFilter" %>
<%@ Register Src="~/CMSModules/BadWords/FormControls/SelectBadWordAction.ascx" TagPrefix="cms"
    TagName="SelectBadWordAction" %>
<asp:Panel runat="server" ID="pnlFilter" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblExpression" runat="server" ResourceString="Unigrid.BadWords.Columns.WordExpression"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:CMSTextBox ID="txtExpression" runat="server"  MaxLength="50" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblAction" runat="server" ResourceString="Unigrid.BadWords.Columns.WordAction"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="filter-form-value-cell">
                <cms:SelectBadWordAction ID="ucBadWordAction" runat="server" AllowNoSelection="true" ReloadDataOnPostback="false" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell">
                <cms:LocalizedButton ID="btnReset" ButtonStyle="Default" runat="server" EnableViewState="false" />
                <cms:LocalizedButton ID="btnShow" runat="server" ResourceString="general.filter" ButtonStyle="Primary" OnClick="btnShow_Click" />
            </div>
        </div>
    </div>
</asp:Panel>