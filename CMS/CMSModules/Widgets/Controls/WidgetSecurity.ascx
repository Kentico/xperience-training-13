<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Controls_WidgetSecurity"
     Codebehind="WidgetSecurity.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<cms:LocalizedLabel runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<asp:Table runat="server" ID="tblMatrix" CssClass="table table-hover permission-matrix"
    CellPadding="-1" CellSpacing="-1" EnableViewState="false">

    <asp:TableRow>
        <asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
    </asp:TableRow>
</asp:Table>
<div class="form-horizontal form-filter">
    <div class="form-group">
        <cms:LocalizedLabel ID="lblRolesInfo" runat="server" ResourceString="SecurityMatrix.RolesAvailability"
        Visible="false" EnableViewState="false" />
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                EnableViewState="false" AssociatedControlID="siteSelector" CssClass="control-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:SiteSelector ID="siteSelector" runat="server" AllowAll="false" OnlyRunningSites="false"
                IsLiveSite="false" />
        </div>
    </div>
</div>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniMatrix ID="gridMatrix" runat="server" QueryName="cms.widgetrole.getpermissionmatrix"
            RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemCodeNameColumn="RoleName" RowItemDisplayNameColumn="RoleDisplayName"
            ColumnItemDisplayNameColumn="PermissionDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
            ColumnItemTooltipColumn="PermissionDescription" ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" MaxFilterLength="100" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
