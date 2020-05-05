<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_SearchIndex_Search"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="SearchIndex_Search.aspx.cs" %>

<%@ Register Src="SearchTransformationItem.ascx" TagName="SearchTransformationItem"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagName="UIPager"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="searchPnl" runat="server">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSearchFor" AssociatedControlID="txtSearchFor"
                        DisplayColon="true" ResourceString="srch.dialog.searchfor" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtSearchFor" MaxLength="1000" />
                </div>
            </div>
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSearchMode" AssociatedControlID="drpSearchMode"
                        DisplayColon="true" ResourceString="srch.dialog.mode" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList runat="server" ID="drpSearchMode" CssClass="DropDownField" />
                </div>
            </div>
            <div class="form-group form-group-buttons">
                <div class="filter-form-buttons-cell">
                    <cms:LocalizedButton runat="server" ID="btnSearch" ButtonStyle="Primary" OnClick="btnSearch_Click" ResourceString="general.search" />
                </div>
            </div>
        </div>
        <asp:Label CssClass="control-label" runat="server" ID="lblNoResults" EnableViewState="false" Visible="false" />
        <br />
        <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" CssClass="SearchErrorMessage">
            <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="ErrorLabel" />
        </asp:Panel>
        <cms:BasicRepeater runat="server" ID="repSearchResults">
            <ItemTemplate>
                <cms:SearchTransformationItem ID="srchItem" runat="server" />
            </ItemTemplate>
        </cms:BasicRepeater>
        <cms:UIPager runat="server" ID="pgrSearch" PagerMode="Querystring"></cms:UIPager>
    </asp:Panel>
</asp:Content>