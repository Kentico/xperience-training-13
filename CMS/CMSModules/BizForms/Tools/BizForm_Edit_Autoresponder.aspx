<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_BizForms_Tools_BizForm_Edit_Autoresponder"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="BizForm_Edit_Autoresponder.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Inputs/EmailInput.ascx" TagName="EmailInput"
    TagPrefix="cms" %>

<asp:Content ID="aS" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:HeaderActions ID="menu" ShortID="m" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
<asp:Content ID="plcContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlUsers" runat="server" CssClass="form-custom-layout">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblEmailField" runat="server" EnableViewState="False" ResourceString="bizform_edit_autoresponder.lblemailfield" AssociatedControlID="drpEmailField" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSDropDownList ID="drpEmailField" runat="server" CssClass="DropDownField" AutoPostBack="true" />
                </div>
            </div>
            <asp:PlaceHolder ID="pnlCustomLayout" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblEmailFrom" runat="server" EnableViewState="False" ShowRequiredMark="True"  ResourceString="bizform_edit_autoresponder.lblEmailFrom" AssociatedControlID="txtEmailFrom" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:EmailInput ID="txtEmailFrom" runat="server"/>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblEmailSubject" runat="server" EnableViewState="False" ShowRequiredMark="True" ResourceString="general.subject"
                            DisplayColon="true" AssociatedControlID="txtEmailSubject" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtEmailSubject" runat="server" MaxLength="200" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="control-group-inline">
                        <cms:LocalizedButton ID="btnGenerateLayout" runat="server" OnClientClick="SetContent(GenerateTableLayout()); return false;"
                            ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnGenerateLayout" />
                    </div>
                    <div class="control-group-inline">
                        <div class="editor">
                            <cms:CMSHtmlEditor FullPage="True" ID="htmlEditor" runat="server" Width="650px" Height="300px" />
                        </div>
                        <div class="fields">
                            <cms:LocalizedLabel ID="lblAvailableFields" runat="server" EnableViewState="false"
                                CssClass="input-label" ResourceString="Bizform_Edit_Autoresponder.AvailableFields" />
                            <cms:CMSListBox ID="lstAvailableFields" runat="server" CssClass="fields-list" Rows="13" />
                            <div class="btns-vertical">
                                <cms:LocalizedButton ID="btnInsertLabel" runat="server" ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnInsertLabel" />
                                <cms:LocalizedButton ID="btnInsertInput" runat="server" ButtonStyle="Default" ResourceString="Bizform_Edit_Autoresponder.btnInsertInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <asp:Literal ID="ltlConfirmDelete" runat="server" />
    </asp:Panel>
</asp:Content>
