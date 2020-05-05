<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Controls_OpenedByFilter"
     Codebehind="OpenedByFilter.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/VariantFilter.ascx" TagName="VariantFilter"
    TagPrefix="cms" %>

<asp:Panel ID="pnl" runat="server" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblEmail" runat="server" ResourceString="general.email" DisplayColon="true" EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltEmail" runat="server" Column="OpenedEmailEmail" IncludeNULLCondition="false" />
        </div>
        <asp:PlaceHolder ID="plcDate" runat="server">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblOpenedBetween" runat="server" ResourceString="newsletter_issue.openedbetween"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:TimeSimpleFilter ID="fltOpenedBetween" runat="server" Column="OpenedEmailTime" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcVariants" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblVariants" runat="server" ResourceString="abtesting.variants"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:VariantFilter ID="fltVariants" runat="server" Visible="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnShow" runat="server" ButtonStyle="Primary" ResourceString="general.search" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
