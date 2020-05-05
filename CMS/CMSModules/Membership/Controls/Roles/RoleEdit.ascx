<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Roles_RoleEdit"  Codebehind="RoleEdit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblRoleDisplayName" runat="server" EnableViewState="false"
                ResourceString="Administration-Role_Edit_General.DisplayName" ShowRequiredMark="True" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtRoleDisplayName" runat="server" MaxLength="100" />&nbsp;<cms:CMSRequiredFieldValidator
                ID="rfvDisplayName" runat="server" EnableViewState="false" ControlToValidate="txtRoleDisplayName:cntrlContainer:textbox"
                Display="dynamic" ValidationGroup="clickOK" />
        </div>
    </div>
    <asp:PlaceHolder ID="plcCodeName" runat="server">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblRoleCodeName" runat="server" EnableViewState="false" ResourceString="Administration-Role_Edit_General.RoleCodeName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtRoleCodeName" runat="server" MaxLength="100" />&nbsp;<cms:CMSRequiredFieldValidator
                    ID="rfvCodeName" runat="server" EnableViewState="false" ControlToValidate="txtRoleCodeName"
                    Display="dynamic" ValidationGroup="clickOK" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblDescription" runat="server" EnableViewState="false" ResourceString="Administration-Role_New.Description"
                AssociatedControlID="txtDescription" />
        </div>
        <div class="editing-form-value-cell">
            <cms:LocalizableTextBox ID="txtDescription" runat="server" TextMode="MultiLine" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcIsDomain">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIsDomain" runat="server" EnableViewState="false" ResourceString="Administration-Role_Edit_General.RoleIsDomain"
                    DisplayColon="true" AssociatedControlID="chkIsDomain" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkIsDomain" runat="server" CssClass="CheckBoxMovedLeft" />
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcIsPublic" Visible="false">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblIsAdmin" runat="server" EnableViewState="false" ResourceString="groups.role.isgroupadmin"
                    DisplayColon="true" AssociatedControlID="chkIsAdmin" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkIsAdmin" CssClass="CheckBoxMovedLeft" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="form-group">
        <div class="editing-form-value-cell editing-form-value-cell-offset">
            <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click"
                EnableViewState="false" ValidationGroup="clickOK" ResourceString="general.ok" />
        </div>
    </div>
</div>