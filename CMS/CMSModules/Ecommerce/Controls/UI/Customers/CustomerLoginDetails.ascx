<%@ Control Language="C#" AutoEventWireup="true"  CodeBehind="CustomerLoginDetails.ascx.cs"
    Inherits="CMSModules_Ecommerce_Controls_UI_Customers_CustomerLoginDetails" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>

<asp:Panel ID="pnlHeaderActions" runat="server" CssClass="FieldTopMenuPadding" Visible="false">
    <div class="cms-edit-menu">
        <cms:HeaderActions runat="server" ID="hdrActions" />
    </div>
</asp:Panel>


<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblUserName" runat="server" ResourceString="customers_edit.username" DisplayColon="true"
                EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtUserName" runat="server" MaxLength="100"
                EnableViewState="false" Enabled="false"  />
        </div>
    </div>
    <asp:Panel ID="pnlPassword" runat="server" EnableViewState="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPasswordLine1" runat="server" ResourceString="customer_edit_login_edit.lblpassword1"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPasswordLine2" runat="server" ResourceString="customer_edit_login_edit.lblpassword2"
                    DisplayColon="true" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtPassword2" runat="server" TextMode="Password"
                    MaxLength="100" EnableViewState="false" />
            </div>
        </div>
    </asp:Panel>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:LocalizedLabel ID="lblRegistrationNotification" runat="server" ResourceString="com.customer.registrednotification"
                DisplayColon="False" EnableViewState="false" CssClass="explanation-text" />
        </div>
    </div>
</div>
