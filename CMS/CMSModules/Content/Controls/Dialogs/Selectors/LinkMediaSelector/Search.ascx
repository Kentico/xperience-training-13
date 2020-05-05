<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_Search"  Codebehind="Search.ascx.cs" %>

<asp:Literal ID="ltlScript" runat="server" />
<asp:Panel ID="pnlDialogSearch" runat="server">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel ID="lblSearchByName" runat="server" ResourceString="dialogs.view.searchbyname"
                    DisplayColon="true" CssClass="control-label" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtSearchByName" runat="server" />
                <cms:LocalizedButton ID="btnSearch" ButtonStyle="Default" ResourceString="general.search"
                    EnableViewState="false" runat="server" />
            </div>
        </div>
    </div>
</asp:Panel>
