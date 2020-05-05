<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Chat room properties – Password"
    Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_EditPassword" Theme="Default"  Codebehind="EditPassword.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" runat="server" ID="LabelPassword" DisplayColon="true" ResourceString="general.password"></cms:FormLabel>
                </div>
                <div class="editing-form-value-cell">
                    <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:FormLabel CssClass="control-label" runat="server" ID="LabelConfirmPassword" AssociatedControlID="TextBoxConfirmPassword" DisplayColon="true" ResourceString="general.confirmpassword"></cms:FormLabel>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="TextBoxConfirmPassword" runat="server" TextMode="Password"
                        MaxLength="100"></cms:CMSTextBox>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell editing-form-value-cell-offset">
                    <cms:CMSButton ID="btnSetPassword" runat="server" Text="" OnClick="ButtonSetPassword_Click"
                        ButtonStyle="Primary" EnableViewState="false" />
                    <cms:CMSButton ID="btnRemovePassword" runat="server" Text="" OnClick="ButtonRemovePassword_Click"
                        ButtonStyle="Primary" EnableViewState="false" Visible="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>