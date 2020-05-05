<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_Edit_Settings"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User edit - Custom fields"
     Codebehind="User_Edit_Settings.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Avatars/UserPictureEdit.ascx"
    TagName="UserPictureFormControl" TagPrefix="upfc" %>
<%@ Register Src="~/CMSFormControls/TimeZones/TimeZoneSelector.ascx" TagName="TimeZoneSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtNickName" CssClass="control-label" ID="lblNickName" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtNickName" MaxLength="200" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="UserPictureFormControl" CssClass="control-label" ID="lblUserPicture" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <upfc:UserPictureFormControl ID="UserPictureFormControl" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtUserSignature" CssClass="control-label" ID="lblUserSignature" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea runat="server" ID="txtUserSignature" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtUserDescription" CssClass="control-label" ID="lblUserDescription" runat="server" EnableViewState="false"
                        ResourceString="Administration-User_Edit_General.UserDescription" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextArea runat="server" ID="txtUserDescription" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtURLReferrer" CssClass="control-label" ID="lblURLReferrer" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtURLReferrer" MaxLength="450" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtCampaign" CssClass="control-label" ID="lblCampaign" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtCampaign" MaxLength="200" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="timeZone" CssClass="control-label" ID="lblTimeZone" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:TimeZoneSelector ID="timeZone" runat="server" UseZoneNameForSelection="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="activationDate" CssClass="control-label" ID="lblActivationDate" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker ID="activationDate" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblActivatedByUser" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label ID="lblUserFullName" CssClass="form-control-text" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label CssClass="control-label" ID="lblRegInfo" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:PlaceHolder runat="server" ID="plcUserLastLogonInfo" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="rbtnlGender" CssClass="control-label" ID="lblUserGender" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSRadioButtonList ID="rbtnlGender" runat="server" RepeatDirection="Horizontal" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="dtUserDateOfBirth" CssClass="control-label" ID="lblUserDateOfBirth" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:DateTimePicker ID="dtUserDateOfBirth" runat="server" EditTime="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtPosition" CssClass="control-label" ID="lblPosition" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtPosition" runat="server" MaxLength="200" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtUserSkype" CssClass="control-label" ID="lblUserSkype" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtUserSkype" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtUserIM" CssClass="control-label" ID="lblUserIM" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtUserIM" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="txtPhone" CssClass="control-label" ID="lblUserPhone" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtPhone" runat="server" MaxLength="26" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkLogActivities" CssClass="control-label" ID="lblLogActivities" runat="server" ResourceString="adm.user.lbllogactivities"
                        DisplayColon="true" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkLogActivities" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label AssociatedControlID="chkWaitingForActivation" CssClass="control-label" ID="lblWaitingForActivation" runat="server" EnableViewState="false" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkWaitingForActivation" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkUserShowIntroTile" CssClass="control-label" ID="lblUserShowIntroTile" runat="server" DisplayColon="true" EnableViewState="false" ResourceString="adm.user.lblUserShowWelcomeTile" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkUserShowIntroTile" runat="server" />
                </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="ButtonOK_Click"
                        EnableViewState="false" ResourceString="general.ok" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>