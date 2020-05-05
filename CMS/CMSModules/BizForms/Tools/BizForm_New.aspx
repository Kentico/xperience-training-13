<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="BizForms - New BizForm" Inherits="CMSModules_BizForms_Tools_BizForm_New"
    Theme="Default"  Codebehind="BizForm_New.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" AssociatedControlID="txtFormDisplayName" ID="lblFormDisplayName" EnableViewState="false" ShowRequiredMark="True" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtFormDisplayName" runat="server"
                    MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvFormDisplayName" runat="server" ControlToValidate="txtFormDisplayName" Display="Dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblFormName" AssociatedControlID="txtFormName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtFormName" runat="server" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvFormName" runat="server" ControlToValidate="txtFormName" Display="Dynamic"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label CssClass="control-label" runat="server" ID="lblTableName" AssociatedControlID="txtTableName" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblPrefix" runat="server" EnableViewState="false" CssClass="form-control-text" />
                <cms:CodeName ID="txtTableName" runat="server" MaxLength="100" ShowHint="false" />
                <cms:CMSRequiredFieldValidator ID="rfvTableName" runat="server" ControlToValidate="txtTableName" Display="Dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"/>
            </div>
        </div>
    </div>
</asp:Content>