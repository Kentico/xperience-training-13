<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="GenerateHash.aspx.cs"
    Inherits="CMSModules_REST_FormControls_GenerateHash" Theme="Default" EnableEventValidation="true"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="REST Service - Generate authetication hash" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:LocalizedLabel runat="server" ID="lblInfo" ResourceString="rest.generateauthhashinfo"
        EnableViewState="false" /><br />
    <br />
    <cms:CMSTextArea runat="server" ID="txtUrls" EnableViewState="false" Width="100%" Rows="9" /><br />
    <div class="form-horizontal">
        <cms:LocalizedHeading runat="server" ID="lblSetting" Level="4" ResourceString="general.settings" EnableViewState="false" CssClass="anchor" />
        <div class="form-group">
	        <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" ID="lblExpiration" DisplayColon="true" class="control-label editing-form-label" ResourceString="rest.hashexpiration" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:DateTimePicker ID="dateExpiration" runat="server" DisplayNow="false"  />
	        </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:CMSButton ID="btnAuthenticate" runat="server" ButtonStyle="Primary" EnableViewState="false" />
    </div>
</asp:Content>
