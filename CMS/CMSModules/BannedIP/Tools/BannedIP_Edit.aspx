<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Banned IP Properties" Inherits="CMSModules_BannedIP_Tools_BannedIP_Edit"
    Theme="Default"  Codebehind="BannedIP_Edit.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblIPAddress" EnableViewState="false" AssociatedControlID="txtIPAddress" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtIPAddress" runat="server" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvIPAddress" runat="server" ControlToValidate="txtIPAddress"
                    Display="Dynamic" EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblIPAddressBanType" EnableViewState="false" AssociatedControlID="drpIPAddressBanType" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSDropDownList ID="drpIPAddressBanType" runat="server" CssClass="DropDownField" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblIPAddressBanEnabled" EnableViewState="false" AssociatedControlID="chkIPAddressBanEnabled" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="chkIPAddressBanEnabled" runat="server" Checked="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblIPAddressBanReason" EnableViewState="false" AssociatedControlID="txtIPAddressBanReason" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextArea ID="txtIPAddressBanReason" runat="server" MaxLength="450" Rows="19" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton ID="radBanIP" runat="server" GroupName="IPAllowed" Checked="true" />
                    <cms:CMSRadioButton ID="radAllowIP" runat="server" GroupName="IPAllowed" />
                </div>
            </div>
        </div>
        <asp:PlaceHolder ID="plcIPOveride" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label runat="server" ID="lblIPAddressAllowOverride" EnableViewState="false" CssClass="control-label" AssociatedControlID="chkIPAddressAllowOverride" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkIPAddressAllowOverride" runat="server" CssClass="CheckBoxMovedLeft" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ResourceString="General.OK" />
            </div>
        </div>
    </div>
</asp:Content>