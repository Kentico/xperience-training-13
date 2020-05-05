<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="AuthorizationSetup.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetup" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>

<%@ Register TagPrefix="cms" TagName="SalesForceError" Src="~/CMSModules/ContactManagement/Controls/UI/SalesForce/Error.ascx" %>
<%@ Import Namespace="System.Linq" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
    <asp:HiddenField ID="CredentialsHiddenField" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="MessageHiddenField" runat="server" EnableViewState="false" />
    <asp:Panel ID="MainMessagePanel" runat="server" EnableViewState="false" Visible="false">
        <p id="MessageLabel" runat="server" enableviewstate="false" visible="false"></p>
    </asp:Panel>
    <asp:Panel ID="MainPanel" runat="server" EnableViewState="false">
        <cms:SalesForceError ID="SalesForceError" runat="server" EnableViewState="false" MessagesEnabled="true" />
        <p><%= HTMLHelper.HTMLEncode(GetString("sf.authorization.instructions1"))%></p>
        <p style="font-weight: bold">
            <asp:Literal ID="RedirectUrlLiteral" runat="server" EnableViewState="false"></asp:Literal>
        </p>

        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="ClientIdentifierLabel" runat="server" AssociatedControlID="ClientIdentifierTextBox" DisplayColon="true" EnableViewState="false" ResourceString="sf.consumerkey">Consumer key</cms:LocalizedLabel>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="ClientIdentifierTextBox" runat="server" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="ClientSecretLabel" runat="server" AssociatedControlID="ClientSecretTextBox" DisplayColon="true" EnableViewState="false" ResourceString="sf.consumersecret"></cms:LocalizedLabel>
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="ClientSecretTextBox" runat="server" EnableViewState="false" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <script type="text/javascript">

        $cmsj(document).ready(function () {
            var credentialsField = document.getElementById('<%= CredentialsHiddenField.ClientID %>');
            if (credentialsField != null && credentialsField.value != null && credentialsField.value != '') {
                if (wopener) {
                    var sourceCredentialsField = wopener.document.getElementById('<%= SourceCredentialsHiddenFieldClientId %>');
                    if (sourceCredentialsField != null) {
                        sourceCredentialsField.value = credentialsField.value;
                    }
                    var messageField = document.getElementById('<%= MessageHiddenField.ClientID %>');
                    if (messageField != null && messageField.value != null && messageField.value != '') {
                        var sourceMessageLabel = wopener.document.getElementById('<%= SourceMessageLabelClientId %>');
                        if (sourceMessageLabel != null) {
                            $cmsj(sourceMessageLabel).html(messageField.value);
                        }
                    }
                    CloseDialog();
                }
                else {
                    __doPostBack("SaveButton", null);
                }
            }
        });

    </script>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div id="FooterPanel" runat="server" class="FloatRight">
        <cms:LocalizedButton ID="ConfirmButton" runat="server" ButtonStyle="Primary" EnableViewState="False" ResourceString="sf.authorize" />
    </div>
</asp:Content>
