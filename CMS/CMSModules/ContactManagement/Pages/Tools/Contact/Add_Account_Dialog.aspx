<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    EnableEventValidation="false" Theme="Default"  Codebehind="Add_Account_Dialog.aspx.cs"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Contact_Add_Account_Dialog" %>

<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ContactRoleSelector.ascx"
    TagName="ContactRoleSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/Controls/SelectionDialog.ascx"
    TagName="SelectionDialog" TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SelectionDialog runat="server" ID="selectionDialog" IsLiveSite="false" />
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" class="dialog-content">
        <ContentTemplate>
            <cms:LocalizedHeading ID="pnlRoleHeading" runat="server" Level="4" Visible="false" ResourceString="om.account.addwithrole" />
            <asp:Panel runat="server" ID="pnlRole">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblAddAccounts" runat="server"
                                EnableViewState="false" CssClass="control-label" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:ContactRoleSelector ID="contactRoleSelector" ShortID="rs" runat="server" IsLiveSite="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
