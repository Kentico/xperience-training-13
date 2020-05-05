<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_HTMLAnchorProperties"  Codebehind="HTMLAnchorProperties.ascx.cs" %>

<asp:Literal ID="ltlScript" runat="server" />

<script type="text/javascript">
    function insertItem() {
        window.RaiseHiddenPostBack();
    }
</script>

<div class="HTMLAnchorProperties">
    <cms:CMSUpdatePanel ID="plnAnchorUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div>
                <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
                    Visible="false" />
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblLinkText" runat="server" EnableViewState="false" ResourceString="dialogs.anchor.linktext"
                                DisplayColon="true" AssociatedControlID="txtLinkText" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtLinkText" runat="server" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell editing-form-value-cell-offset">
                            <div class="radio-list-vertical">
                                <cms:CMSRadioButton ID="rbAnchorName" runat="server" AutoPostBack="true" OnCheckedChanged="rbAnchorName_CheckedChanged"
                                    CssClass="AnchorRadioButton" />
                                <div class="selector-subitem">
                                    <cms:CMSDropDownList ID="drpAnchorName" runat="server" CssClass="SmallDropDown" />
                                </div>
                                <cms:CMSRadioButton ID="rbAnchorId" runat="server" AutoPostBack="true" OnCheckedChanged="rbAnchorId_CheckedChanged" />
                                <div class="selector-subitem">
                                    <cms:CMSDropDownList ID="drpAnchorId" runat="server" CssClass="SmallDropDown" />
                                </div>
                                <cms:CMSRadioButton ID="rbAnchorText" runat="server" AutoPostBack="true" OnCheckedChanged="rbAnchorText_CheckedChanged" />
                                <div class="selector-subitem">
                                    <cms:CMSTextBox ID="txtAnchorText" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <div class="Hidden">
        <cms:CMSUpdatePanel ID="plnAnchorButtonsUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_Click" CssClass="HiddenButton" EnableViewState="false" />
                <asp:Button ID="btnHiddenUpdate" runat="server" OnClick="btnHiddenUpdate_Click" CssClass="HiddenButton" EnableViewState="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</div>