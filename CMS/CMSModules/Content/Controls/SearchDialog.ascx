<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_SearchDialog"  Codebehind="SearchDialog.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>

<asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblIndexes" runat="server" ResourceString="Search.lblSiteIndexes"
                    EnableViewState="false" DisplayColon="true" CssClass="control-label" AssociatedControlID="drpIndexes" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSDropDownList runat="server" ID="drpIndexes" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblSearchFor" AssociatedControlID="txtSearchFor"
                    CssClass="control-label" EnableViewState="false" ResourceString="search.lblsearch" ShowRequiredMark="True" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSTextBox runat="server" ID="txtSearchFor" MaxLength="1000" />
                <cms:CMSRequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtSearchFor"
                    EnableViewState="false" Display="Dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSearchMode" runat="server" ResourceString="srch.dialog.mode"
                    DisplayColon="true" EnableViewState="false" CssClass="control-label" AssociatedControlID="drpSearchMode" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:CMSDropDownList ID="drpSearchMode" runat="server" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcLang" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblLanguage" runat="server" ResourceString="general.language"
                        EnableViewState="false" DisplayColon="true" CssClass="control-label" AssociatedControlID="drpLanguage" />
                </div>
                <div class="filter-form-condition-cell">
                    <cms:CMSDropDownList ID="drpLanguage" runat="server" />
                </div>
                <asp:Panel runat="server" ID="pnlCultureElem" CssClass="filter-form-value-cell">
                    <cms:SiteCultureSelector runat="server" ID="cultureElem" />
                </asp:Panel>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcPublished" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblOnlyPublished" ResourceString="filter.onlypublished" AssociatedControlID="chkOnlyPublished" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:CMSCheckBox ID="chkOnlyPublished" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton runat="server" ID="btnSearch" ButtonStyle="Primary" ResourceString="general.search" />
            </div>
        </div>
    </div>
</asp:Panel>
