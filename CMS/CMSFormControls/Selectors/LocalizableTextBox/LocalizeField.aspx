<%@ Page Language="C#" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSFormControls_Selectors_LocalizableTextBox_LocalizeField"
    Title="Localize field"  Codebehind="LocalizeField.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Selectors/LocalizableTextBox/ResourceStringSelector.ascx"
    TagName="ResourceSelector" TagPrefix="cms" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" CssClass="LocalizeField" runat="server">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset">
                            <cms:CMSRadioButtonList ID="lstExistingOrNew" runat="server" AutoPostBack="true" CssClass="radio-list-vertical radio" UseResourceStrings="true">
                                <asp:ListItem Value="new" Text="localizable.createnew" Selected="True" />
                                <asp:ListItem Value="existing" Text="localizable.useexisting" />
                            </cms:CMSRadioButtonList>
                        </div>
                    </div>
                    <asp:Panel ID="pnlNewKey" runat="server">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblNewKey" runat="server" ResourceString="localizable.newkey"
                                    DisplayColon="true" CssClass="control-label" EnableViewState="false" />                               
                            </div>
                            <div class="editing-form-value-cell">
                                <asp:Label ID="lblPrefix" runat="server" EnableViewState="false" CssClass="form-control-text hidden" />
                                <cms:CMSTextBox ID="txtNewResource" runat="server" Visible="true" MaxLength="200" CssClass="form-control" />
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlExistingKey" runat="server" Visible="false">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel ID="lblExistingKey" runat="server" ResourceString="localizable.existingkey"
                                    DisplayColon="true" CssClass="control-label" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:ResourceSelector ID="resourceSelector" runat="server" IsLiveSite="false" />
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>