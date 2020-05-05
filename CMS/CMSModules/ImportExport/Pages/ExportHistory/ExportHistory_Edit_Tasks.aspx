<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Pages_ExportHistory_ExportHistory_Edit_Tasks"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Export History - Tasks"  Codebehind="ExportHistory_Edit_Tasks.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector" TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-label-cell">
                <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                    EnableViewState="false" CssClass="control-label" />
            </div>
            <div class="filter-form-value-cell-wide">
                <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="plcControls">
    <cms:CMSUpdatePanel ID="pnlUpdateLink" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:LocalizedButton ID="btnDeleteAll" runat="server" ResourceString="exporthistory.deletealltasks"
                OnClick="lnkDeleteAll_Click" EnableViewState="false" ButtonStyle="Default" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" GridName="ExportHistory_Edit_Tasks_List.xml"
                OrderBy="TaskTime" IsLiveSite="false" ExportFileName="export_task" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
