<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniGrid_UniGrid"
     Codebehind="UniGrid.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/Pager/UIPager.ascx" TagPrefix="cms"
    TagName="UIPager" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Controls/AdvancedExport.ascx" TagPrefix="cms"
    TagName="AdvancedExport" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Controls/MassActions.ascx" TagPrefix="cms"
    TagName="MassActions" %>
<div id="<%=ClientID%>" class="unigrid dont-check-changes">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <asp:PlaceHolder ID="plcContextMenu" runat="server" />
    <asp:Panel ID="pnlHeader" runat="server" CssClass="unigrid-header">
        <asp:PlaceHolder ID="plcFilter" runat="server" />
        <asp:PlaceHolder ID="plcFilterForm" runat="server" Visible="false">
            <cms:FilterForm ID="filterForm" runat="server"
                FormCssClass="form-horizontal form-filter" GroupCssClass="filter-form-category" FieldGroupCssClass="filter-form-category-fields"
                FieldCaptionCellCssClass="filter-form-label-cell" FieldValueCellCssClass="filter-form-value-cell-wide" FormButtonPanelCssClass="form-group form-group-buttons" />
        </asp:PlaceHolder>
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" />
    </asp:Panel>
    <asp:Panel ID="pnlContent" runat="server" CssClass="unigrid-content">
        <cms:UIGridView ID="UniUiGridView" ShortID="v" runat="server" AllowPaging="True" PageSize="25" AutoGenerateColumns="False" AllowSorting="true">
            <HeaderStyle CssClass="unigrid-head" />
        </cms:UIGridView>
        <cms:UIPager ID="pagerElem" ShortID="p" runat="server" PagerMode="Postback" />
        
        <asp:HiddenField ID="hidSelection" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidSelectionHash" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidCmdName" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidCmdArg" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidActions" runat="server" EnableViewState="false" />
        <asp:HiddenField ID="hidActionsHash" runat="server" EnableViewState="false" />
    </asp:Panel>
    <cms:AdvancedExport ID="advancedExportElem" runat="server" ShortID="a" />
    <cms:MassActions ID="ctrlMassActions" runat="server" Visible="False"/>
</div>
