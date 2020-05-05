<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_Layout"
     Codebehind="Layout.ascx.cs" %>
<cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <div class="form-horizontal">
            <cms:LocalizedHeading runat="server" ID="headingGeneral" ResourceString="general.general" Level="4" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="lblLayoutType" ResourceString="pagelayout.type" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="radio-list-vertical">
                        <cms:CMSRadioButton ID="radAutoLayout" runat="server" AutoPostBack="true"
                            OnCheckedChanged="radLayout_CheckedChanged" ResourceString="formlayout.autolayout" GroupName="Layout" />
                        <cms:CMSRadioButton ID="radCustomLayout" runat="server" AutoPostBack="true"
                            OnCheckedChanged="radLayout_CheckedChanged" ResourceString="formlayout.customlayout" GroupName="Layout" />
                        <asp:Panel ID="pnlLayoutType" runat="server" CssClass="selector-subitem">
                            <cms:CMSDropDownList runat="server" ID="drpLayoutType" AutoPostBack="true" OnSelectedIndexChanged="drpLayoutType_SelectedIndexChanged" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
        <asp:Panel ID="pnlCustomLayout" runat="server">
            <div class="form-horizontal">
                <asp:Panel ID="pnlHtmlInsert" runat="server">
                    <cms:LocalizedHeading runat="server" ID="hdnInsertField" ResourceString="DocumentType_Edit_Form.insertfield" Level="4" />
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel runat="server" ID="lblLayoutElement" ResourceString="pagelayout.layoutelement" CssClass="control-label" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList runat="server" ID="drpFieldType" UseResourceStrings="true" CssClass="LongDropDownList" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel runat="server" ID="lblForField" ResourceString="DocumentType_Edit_Form.forfield" CssClass="control-label" DisplayColon="true" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSDropDownList runat="server" ID="drpAvailableFields" CssClass="LongDropDownList" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="editing-form-value-cell-offset editing-form-value-cell">
                            <cms:LocalizedButton ID="btnInsert" runat="server" ButtonStyle="Default" OnClientClick="ButtonInsert(); return false;"
                                ResourceString="dialogs.actions.insert" />
                        </div>
                    </div>
                </asp:Panel>
                <asp:PlaceHolder runat="server" ID="plcHTML">
                    <cms:CMSHtmlEditor ID="htmlEditor" runat="server" Height="340px" />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcASCX" Visible="false">
                    <asp:Panel runat="server" ID="pnlDirectives" CssClass="NORTL CodeDirectives">
                        <asp:Label runat="server" ID="lblDirectives" EnableViewState="false" />
                    </asp:Panel>
                    <cms:MacroEditor runat="server" ID="txtCode" ShortID="e" />
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
    </asp:Panel>
    <cms:FormSubmitButton runat="server" ID="btnSave" />
</cms:ObjectLockingPanel>
