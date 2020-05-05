<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Password" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - Password"  Codebehind="User_Edit_Password.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblPassword" runat="server" Text="Label" CssClass="control-label" AssociatedControlID="passStrength" ResourceString="Administration-User_Edit_Password.NewPassword" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblConfirmPassword" runat="server" Text="Label" CssClass="control-label" AssociatedControlID="txtConfirmPassword" ResourceString="Administration-User_Edit_Password.ConfirmPassword" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" 
                        MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <cms:LocalizedButton ID="btnSetPassword" runat="server" Text="" OnClick="btnSetPassword_Click" ResourceString="changepassword.btnok"
                        ButtonStyle="Primary" EnableViewState="false" />
               </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>