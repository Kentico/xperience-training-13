<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Pages_ExportObject"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Export single object"  Codebehind="ExportObject.aspx.cs" %>


<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDetails" Visible="true">
        <div class="form-horizontal">
            <div class="form-group">
                <cms:LocalizedLabel ID="lblBeta" runat="server" Visible="false" CssClass="ErrorLabel" EnableViewState="false" />
                <asp:Label ID="lblIntro" runat="server" EnableViewState="false" />
            </div>
            <asp:PlaceHolder ID="plcExportDetails" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblFileName" runat="server" CssClass="control-label" AssociatedControlID="txtFileName"
                            EnableViewState="false" ResourceString="general.filename" Font-Bold="true" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtFileName" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlContent" Visible="false">
        <div class="control-group-inline">
            <asp:Label CssClass="ContentLabel" ID="lblResult" runat="server" EnableViewState="false" />
        </div>
    </asp:Panel>
    <cms:AsyncControl ID="ucAsyncControl" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatLeft">
        <cms:ActivityBar ID="ucActivityBar" runat="server" Visible="False" />
    </div>
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" />
        <cms:LocalizedButton ID="btnDownload" runat="server" ResourceString="ExportObject.Download" Visible="false" ButtonStyle="Primary" EnableViewState="false" />
    </div>
</asp:Content>
