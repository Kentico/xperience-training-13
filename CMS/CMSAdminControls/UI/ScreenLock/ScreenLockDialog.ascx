<%@ Control Language="C#" AutoEventWireup="True" Inherits="CMSAdminControls_UI_ScreenLock_ScreenLockDialog"  Codebehind="ScreenLockDialog.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

<div id="screenLockWarningDialog" class="message-panel alert-warning screen-lock-warning-dialog" style="display: none;">
    <div>
        <div class="screen-lock-warning-cell">
            <cms:LocalizedLabel ID="lblWarningTextStart" runat="server" ResourceString="screenlock.warningtext" EnableViewState="false" />
            <span id="screenLockTimeDialog">0</span>
            <cms:LocalizedLabel ID="lblWarningTextEnd" runat="server" ResourceString="screenlock.seconds" EnableViewState="false" />
            <asp:LinkButton ID="btnCancelCountdown" runat="server" CssClass="alert-link" OnClientClick="CancelScreenLockCountdown(); return false;" EnableViewState="false"><%=GetString("general.cancel")%></asp:LinkButton>
        </div>
    </div>
</div>

<div id="screenLockBackground" class="ui-widget-overlay screenlock-overlay" style="display: none;"></div>
<div id="screenLockDialog" class="ModalPopupDialog screen-lock-dialog" style="display: none;">
    <asp:Panel ID="pnlScreenLock" runat="server">
        <div class="PageHeader">
            <cms:PageTitle ID="screenLockTitle" runat="server" EnableViewState="false" />
        </div>
        <asp:Panel runat="server" ID="pnlScreenLockContent" class="dialog-content" Style="width: 400px; height: 201px;">
            <cms:LocalizedLabel runat="server" ID="lblInstructions" CssClass="form-info" EnableViewState="false" />
            <div class="screen-lock-dialog-form">
                <div class="form-horizontal">
                    <div class="form-group hide" id="tokenBox">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel runat="server" ID="lblTokenInfo" CssClass="control-label" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:LocalizedLabel runat="server" ID="lblTokenID" CssClass="form-control-text" />
                        </div>
                    </div>

                    <div class="form-group" id="usernameBox">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblUserName" runat="server" CssClass="control-label" ResourceString="logonform.username" EnableViewState="false" AssociatedControlID="txtUserName" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="100" Enabled="false" CssClass="form-control input-width-60" />
                            <%-- CHROME AUTO-FILL BUG WORKAROUND --%>
                            <input class="hide" />
                            <%-- CHROME AUTO-FILL BUG WORKAROUND END --%>
                        </div>
                    </div>

                    <div class="form-group" id="passwordBox">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblPassword" runat="server" CssClass="control-label" ResourceString="logonform.password" EnableViewState="false" AssociatedControlID="txtScreenLockDialogPassword" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtScreenLockDialogPassword" CssClass="form-control input-width-60" runat="server" onkeypress="return ScreenLockEnterHandler(event)" TextMode="Password" />
                        </div>
                    </div>

                    <div class="form-group hide" id="passcodeBox">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel ID="lblPasscode" runat="server" CssClass="control-label" ResourceString="logonform.passcode" EnableViewState="false" AssociatedControlID="txtPasscode" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtPasscode" runat="server" MaxLength="100" Enabled="true" CssClass="form-control input-width-60" />
                        </div>
                    </div>
                    <asp:Label runat="server" ID="lblScreenLockWarningLogonAttempts" CssClass="form-control-error hide" />
                    <span id="screenLockDialogWarning" class="Red hide"><%=GetString("screenlock.wrongpassword")%></span>
                </div>
            </div>
        </asp:Panel>
        <div class="dialog-footer">
            <input type="button" class="btn btn-primary" id="screenLockSignInButton" value="<%=GetString("screenlock.signin")%>"
                onclick="ScreenLockRedirectToLogon('<%=URLHelper.ResolveUrl("~/Admin/")%>'); return false;" style="display: none;" />
            <input type="submit" class="btn btn-primary" id="screenLockUnlockButton" value="<%=GetString("screenlock.unlock")%>"
                onclick="ScreenLockValidateUser(); return false;" />
            <cms:LocalizedButton runat="server" ID="btnScreenLockSignOut" ResourceString="signoutbutton.signout" ButtonStyle="Primary" OnClientClick="ScreenLockLogoutUser(); return false;" />
        </div>
    </asp:Panel>
</div>