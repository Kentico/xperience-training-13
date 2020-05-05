<%@ Control Language="C#" AutoEventWireup="false"
    Inherits="CMSAdminControls_UI_UniSelector_Controls_SelectionDialog"  Codebehind="SelectionDialog.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Panel ID="pnlBody" runat="server">
    <asp:Panel ID="pnlFilter" runat="server" CssClass="header-panel" Visible="false">
    </asp:Panel>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
        <ContentTemplate>
            <asp:Panel ID="pnlSearch" runat="server" CssClass="header-panel" Visible="false"
                DefaultButton="btnSearch">
                <div class="form-horizontal form-filter">
                    <div class="form-group">
                        <div class="filter-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblSearch" DisplayColon="True" AssociatedControlID="txtSearch"
                                runat="server" EnableViewState="False" ResourceString="general.entersearch" />
                        </div>
                        <div class="filter-form-value-cell">
                            <cms:CMSTextBox ID="txtSearch" runat="server" MaxLength="300" />
                        </div>
                    </div>
                    <div class="form-group form-group-buttons">
                        <div class="filter-form-buttons-cell">
                            <cms:LocalizedButton ID="btnSearch" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="general.search" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlAll" runat="server" CssClass="header-panel btn-actions" Visible="false" EnableViewState="false" />
            <asp:Panel runat="server" ID="pnlContent" CssClass="dialog-content">
                <cms:UniGrid ID="uniGrid" runat="server" PageSize="10,25,50,100" RememberState="false" />
                <div class="ClearBoth"></div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel runat="server" ID="pnlHidden" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hidItem" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>