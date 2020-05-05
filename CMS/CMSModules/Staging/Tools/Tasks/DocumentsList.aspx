<%@ Page Title="Synchronization - Pages list" Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Tasks_DocumentsList"
    MaintainScrollPositionOnPostback="true"  Codebehind="DocumentsList.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="Content3" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlListingInfo" runat="server" EnableViewState="false">
        <asp:Label ID="lblListingInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel ID="pnlSearch" runat="server" CssClass="form-horizontal form-filter">
        <div class="form-group">
            <div class="filter-form-value-cell-wide-200 form-search-container">
                <asp:Label AssociatedControlID="txtSearch" runat="server" CssClass="sr-only">
                    <%= GetString("general.search") %>
                </asp:Label>
                <cms:CMSTextBox ID="txtSearch" runat="server" />
                <cms:CMSIcon ID="iconSearch" runat="server" CssClass="icon-magnifier" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group form-group-buttons">
            <div class="filter-form-buttons-cell-wide">
                <cms:LocalizedButton ID="btnSearch"
                    runat="server" ResourceString="general.search" ButtonStyle="Primary" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlUniGrid" runat="server">
        <cms:UniGrid ID="uniGrid" runat="server" GridName="~/CMSModules/Staging/Tools/Tasks/DocumentsList.xml"
            OrderBy="DocumentName" IsLiveSite="false" ExportFileName="cms_document" />
    </asp:Panel>
</asp:Content>