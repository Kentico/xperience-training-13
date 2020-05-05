<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_Dialogs_ObjectBrowser"
    Title="Untitled Page" ValidateRequest="false" Theme="default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="ObjectBrowser.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/MacroTree.ascx" TagName="MacroTree" TagPrefix="cms" %>
<asp:Content runat="server" ContentPlaceHolderID="plcBeforeContent">
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="LocalizedHeading1" ResourceString="macros.console.evaluationinput" Level="4" />
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="editorElem" ID="lblExpression" CssClass="control-label" runat="server" ResourceString="macros.console.expression" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:MacroEditor ID="editorElem" runat="server" Text="CMSContext.Current" Width="100%" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="editorElem" ID="lblMode" CssClass="control-label" runat="server" ResourceString="macros.console.mode" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <div class="radio-list-vertical">
                    <cms:CMSRadioButton runat="server" ID="radVirtual" Checked="true" ResourceString="macros.console.mode.virtual" GroupName="Mode" />
                    <cms:CMSRadioButton runat="server" ID="radNormalMode" ResourceString="macros.console.mode.real" GroupName="Mode" />
                    <cms:CMSRadioButton runat="server" ID="radValues" ResourceString="macros.console.mode.realwithvalues" GroupName="Mode" />
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:LocalizedButton runat="server" ID="btnRefresh" ResourceString="ObjectBrowser.LoadExpression" ButtonStyle="Primary" />
                <cms:LocalizedButton runat="server" ID="btnClear" ResourceString="general.clear"
                    OnClick="btnClear_Click" ButtonStyle="Default" />
            </div>
        </div>
    </div>
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="headHierarchy" ResourceString="macros.console.resulthierarchy" Level="4" />
        <cms:MacroTree ID="treeElem" ShortID="t" runat="server" />
    </div>
    <div class="form-horizontal">
        <cms:LocalizedHeading Level="4" AssociatedControlID="txtOutput" ID="lblOutput" runat="server" ResourceString="macros.console.resultoutput" />
        <cms:CMSTextArea runat="server" ID="txtOutput" Rows="10" Width="100%" />
    </div>
</asp:Content>
