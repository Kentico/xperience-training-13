<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Delete.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Account - Delete" %>

<%@ Register Src="~/CMSAdminControls/AsyncLogDialog.ascx" TagName="AsyncLog"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncLog ID="ctlAsyncLog" runat="server" ProvideLogContext="true" LogContextNames="Accounts" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcBeforeContent" runat="server"
    EnableViewState="false">
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent" EnableViewState="false">
        <asp:Panel ID="pnlDelete" runat="server" EnableViewState="false">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="4" EnableViewState="false" ResourceString="om.account.deletequestion" />
            <asp:Panel ID="pnlAccountList" runat="server" Visible="false" CssClass="form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblAccounts" runat="server" EnableViewState="true" />
            </asp:Panel>
            <asp:Panel ID="plcCheck" runat="server" EnableViewState="false" CssClass="checkbox-list-vertical content-block-50">
                <cms:CMSCheckBox ID="chkBranches" runat="server"
                    EnableViewState="false" ResourceString="om.account.deletesubsidiaries" />
            </asp:Panel>
            <div class="btn-actions">
                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                    ResourceString="general.yes" EnableViewState="false" />
                <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Primary" ResourceString="general.no"
                    EnableViewState="false" />
            </div>
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
