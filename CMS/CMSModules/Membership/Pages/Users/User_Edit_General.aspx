<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - General"
     Codebehind="User_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector" 
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/UserName.ascx" TagName="UserName" 
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="EnumSelector" 
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput" 
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagPrefix="cms" TagName="UniSelector" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="ucUserName" CssClass="control-label" ID="lblUserName" runat="server" EnableViewState="false" ResourceString="general.username"
                        DisplayColon="true" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:UserName ID="ucUserName" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtFullName" CssClass="control-label" ID="lblFullName" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.FullName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtFullName" runat="server" MaxLength="200" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtFirstName" CssClass="control-label" ID="lblFirstName" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.FirstName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtFirstName" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtMiddleName" CssClass="control-label" ID="lblMiddleName" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.MiddleName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtMiddleName" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtLastName" CssClass="control-label" ID="lblLastName" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.LastName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtLastName" runat="server" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtEmail" CssClass="control-label" ID="LabelEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EmailInput ID="txtEmail" runat="server"/>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="CheckBoxEnabled" CssClass="control-label" ID="lblEnabled" runat="server" EnableViewState="false" ResourceString="general.enabled"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="CheckBoxEnabled" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="drpPrivilege" CssClass="control-label" ID="lblPrivilege" runat="server" EnableViewState="false" ResourceString="user.privilegelevel"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:EnumSelector runat="server" ID="drpPrivilege" AssemblyName="CMS.Base" TypeName="CMS.Base.UserPrivilegeLevelEnum" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="drpMacroIdentity" CssClass="control-label" ID="lblMacroIdentity" runat="server" EnableViewState="false" ResourceString="user.macroidentity" ToolTipResourceString="user.macroidentity.tooltip"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:UniSelector ID="drpMacroIdentity" runat="server" SelectionMode="SingleDropDownList" ObjectType="CMS.MacroIdentity" ReturnColumnName="MacroIdentityID" OrderBy="MacroIdentityName" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkIsExternal" CssClass="control-label" ID="lblIsExternal" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.IsExternal" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIsExternal" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkIsDomain" CssClass="control-label" ID="lblIsDomain" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.UserIsDomain"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIsDomain" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkIsHidden" CssClass="control-label" ID="lblIsHidden" runat="server" EnableViewState="false" ResourceString="User_Edit_General.IsHidden"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIsHidden" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="cultureSelector" CssClass="control-label" ID="lblCulture" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.Culture" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SiteCultureSelector runat="server" ID="cultureSelector" IsLiveSite="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="lstUICulture" CssClass="control-label" ID="lblUICulture" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.UICulture" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="lstUICulture" runat="server" CssClass="DropDownField" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="lblCreatedInfo" CssClass="control-label" ID="lblCreated" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.UserCreated" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblCreatedInfo" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="chkIsMFRequired" CssClass="control-label" ID="lblIsRequiredMF" runat="server" EnableViewState="false" ResourceString="mfauthentication.label.isRequired"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIsMFRequired" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
              <div class="editing-form-label-cell">
                  <cms:LocalizedLabel AssociatedControlID="btnResetToken" EnableViewState="false" CssClass="control-label" ID="lblResetToken" runat="server" DisplayColon="true" />
              </div>
              <div class="editing-form-value-cell">
                  <div class="control-group-inline">
                      <cms:LocalizedButton ID="btnResetToken" runat="server" OnClick="btnResetToken_Click"
                            ButtonStyle="Default" EnableViewState="false" ResourceString="mfauthentication.tokenconfiguration.reset" />
                  </div>
              </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="lblLastLogonTime" CssClass="control-label" ID="lblLastLogon" runat="server" EnableViewState="false" ResourceString="Administration-User_Edit_General.LastLogon" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:Label CssClass="form-control-text" ID="lblLastLogonTime" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="plcUserLastLogonInfo" CssClass="control-label" ID="lblUserLastLogonInfo" runat="server" EnableViewState="false"
                        ResourceString="adm.user.lblUserLastLogonInfo" />
                </div>
                <div class="editing-form-value-cell">
                    <asp:PlaceHolder runat="server" ID="plcUserLastLogonInfo" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="btnResetLogonAttempts" CssClass="control-label" ID="lblInvalidLogonAttempts" runat="server" EnableViewState="false"
                        ResourceString="Administration-User_Edit_General.InvalidLogonAttempts" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <asp:Label CssClass="form-control-text" ID="lblInvalidLogonAttemptsNumber" runat="server" />
                        <cms:LocalizedButton ID="btnResetLogonAttempts" runat="server" OnClick="btnResetLogonAttempts_Click"
                            ButtonStyle="Default" EnableViewState="false" ResourceString="invalidlogonattempts.unlockaccount.reset" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="btnExtendValidity" CssClass="control-label" ID="lblPassExpiration" runat="server" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                        <asp:Label CssClass="form-control-text" ID="lblExpireIn" runat="server" />
                        <cms:LocalizedButton ID="btnExtendValidity" runat="server" OnClick="btnExtendValidity_Click"
                            ButtonStyle="Default" EnableViewState="false" ResourceString="passwordexpiration.extendvalidity" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel AssociatedControlID="txtUserStartingPath" CssClass="control-label" ID="lblUserStartingPath" runat="server" EnableViewState="false"
                        ResourceString="Administration-User_Edit_General.UserStartingPath" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtUserStartingPath" MaxLength="200"/>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click"
                        EnableViewState="false" ResourceString="general.ok" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
