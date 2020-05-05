<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_URLSelector"  Codebehind="URLSelector.ascx.cs" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblProtocol" runat="server" EnableViewState="false" ResourceString="dialogs.link.protocol"
                DisplayColon="true" AssociatedControlID="drpProtocol" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpProtocol" />

        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ID="lblUrl" runat="server" ResourceString="dialogs.link.url"
                DisplayColon="true" AssociatedControlID="txtUrl" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtUrl" />
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="plcLinkText">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblLinkText" runat="server" EnableViewState="false" ResourceString="dialogs.link.text"
                    DisplayColon="true" AssociatedControlID="txtLinkText" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtLinkText" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>