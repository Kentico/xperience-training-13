<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_HTMLEmailProperties"  Codebehind="HTMLEmailProperties.ascx.cs" %>

<script type="text/javascript">
    function insertItem() {
        window.RaiseHiddenPostBack();
    }
</script>

<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />

<div class="HTMLEmailProperties">
    <div>
        <cms:CMSUpdatePanel ID="plnEmailUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
                    Visible="false" />
                <div class="form-horizontal">
                    <asp:PlaceHolder runat="server" ID="plcLinkText">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblLinkText" runat="server" EnableViewState="false" ResourceString="dialogs.link.text"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox runat="server" ID="txtLinkText" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblTo" runat="server" EnableViewState="false" ResourceString="dialogs.email.to"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtTo" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblCc" runat="server" EnableViewState="false" ResourceString="dialogs.email.cc"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtCc" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblBcc" runat="server" EnableViewState="false" ResourceString="dialogs.email.bcc"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtBcc" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblSubject" runat="server" EnableViewState="false" ResourceString="dialogs.email.subject"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox runat="server" ID="txtSubject" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblBody" runat="server" EnableViewState="false" ResourceString="dialogs.email.body"
                                DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextArea runat="server" ID="txtBody"  />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <div class="Hidden">
            <cms:CMSUpdatePanel ID="plnEmailButtonsUpdate" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton" EnableViewState="false" />
                    <asp:Button ID="hdnButtonUpdate" runat="server" OnClick="hdnButtonUpdate_Click" CssClass="HiddenButton" EnableViewState="false" />
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </div>
</div>