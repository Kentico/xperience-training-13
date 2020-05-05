<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="TrackedLinksFilter.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_TrackedLinksFilter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Newsletters/FormControls/VariantFilter.ascx" TagName="VariantFilter"
    TagPrefix="cms" %>

<asp:Panel ID="pnl" runat="server" DefaultButton="btnShow">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblLink" runat="server" ResourceString="general.link" DisplayColon="true"
                    EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltLink" runat="server" Column="LinkTarget" IncludeNULLCondition="false" />
        </div>
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" runat="server" ResourceString="general.description"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <cms:TextSimpleFilter ID="fltDescription" runat="server" Column="LinkDescription" IncludeNULLCondition="false" />
        </div>
        <asp:PlaceHolder ID="plcVariants" runat="server" Visible="false">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblVariants" runat="server" ResourceString="abtesting.variants"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:VariantFilter ID="fltVariants" runat="server" AllowSelectAll="false" Visible="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnShow" runat="server" ButtonStyle="Primary" ResourceString="general.search"
                    EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Panel>
