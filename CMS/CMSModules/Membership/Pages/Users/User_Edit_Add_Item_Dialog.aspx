<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    EnableEventValidation="false" Theme="Default"  Codebehind="User_Edit_Add_Item_Dialog.aspx.cs"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Add_Item_Dialog" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/Controls/SelectionDialog.ascx"
    TagName="SelectionDialog" TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:SelectionDialog runat="server" ID="selectionDialog" IsLiveSite="false" />
    <div class="dialog-user-add-item">
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel Hidden" EnableViewState="false" />
        <asp:Panel runat="server" ID="pnlDateTime">
            <cms:LocalizedHeading runat="server" Level="4" ID="pnlDateTimeHeading"></cms:LocalizedHeading>
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblValidTo" ResourceString="membership.validto"
                            DisplayColon="true" AssociatedControlID="ucDateTime:txtDateTime" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:DateTimePicker ID="ucDateTime" runat="server" EditTime="true" />
                    </div>
                </div>
                <asp:Panel ID="pnlSendNotification" runat="server" class="form-group" Visible="false">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSendNotification" ResourceString="membership.sendnotification" DisplayColon="true"
                            AssociatedControlID="chkSendNotification" EnableViewState="false" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkSendNotification" runat="server" Checked="false" />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>
    </div>
</asp:Content>