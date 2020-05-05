<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Controls_ExportConfiguration"
     Codebehind="ExportConfiguration.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:MessagesPlaceHolder runat="server" ID="pnlMessages" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblFileName" EnableViewState="false" ResourceString="general.filename"
                        DisplayColon="true" AssociatedControlID="txtFileName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtFileName" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblSite" runat="server" EnableViewState="false" ResourceString="general.site"
                        DisplayColon="true" AssociatedControlID="siteSelector" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" />
                </div>
            </div>
            <div class="radio-list-vertical">
                <asp:PlaceHolder ID="plcNone" runat="server" Visible="false">
                    <cms:CMSRadioButton ID="radNone" runat="server" GroupName="Export" AutoPostBack="false" />
                </asp:PlaceHolder>
                <cms:CMSRadioButton ID="radAll" runat="server" GroupName="Export" AutoPostBack="false" />
                <cms:CMSRadioButton ID="radDate" runat="server" GroupName="Export" AutoPostBack="false" />

                <div class="selector-subitem">
                    <cms:DateTimePicker runat="server" ID="dtDate" DisplayNow="false" DisplayNA="false"
                        RenderDisableScript="true" />
                </div>
                <cms:CMSRadioButton ID="radExport" runat="server" GroupName="Export" Enabled="false"
                    AutoPostBack="false" />

                <div class="selector-subitem">
                    <cms:CMSListBox runat="server" ID="lstExports" CssClass="ContentListBoxLow" Rows="7" />
                </div>
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>