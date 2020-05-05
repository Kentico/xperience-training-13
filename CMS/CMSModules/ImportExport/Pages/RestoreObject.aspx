<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Pages_RestoreObject"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Restore single object"  Codebehind="RestoreObject.aspx.cs" %>


<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        var importTimerId = 0;

        // End timer function
        function StopTimer() {
            if (importTimerId) {
                clearInterval(importTimerId);
                importTimerId = 0;

                if (window.HideActivity) {
                    window.HideActivity();
                }
            }
        }

        // Start timer function
        function StartTimer() {
            if (window.Activity) {
                importTimerId = setInterval("window.Activity()", 500);
            }
        }
        //]]>
    </script>
    <asp:Panel runat="server" ID="pnlDetails" Visible="true">
        <div class="form-horizontal">
            <div class="form-group">
                <cms:LocalizedLabel ID="lblBeta" runat="server" Visible="false" CssClass="ErrorLabel"
                    EnableViewState="false" />
                <asp:Label ID="lblIntro" runat="server" CssClass="ContentLabel" EnableViewState="false" />
            </div>
            <div class="form-group">
                <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" />
                <cms:CMSListBox runat="server" ID="lstImports" CssClass="ContentListBoxLow" Enabled="false" Rows="7" />
            </div>
            <div class="form-group">
                <asp:Panel runat="server" ID="pnlLeftActions">
                    <cms:LocalizedLinkButton runat="server" ID="btnDelete" ResourceString="importconfiguration.deletepackage" OnClick="btnDelete_Click" />
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlProgress" CssClass="PageContent" runat="Server" Visible="false" EnableViewState="false">
        <div class="control-group-inline">
            <asp:Label ID="lblProgress" runat="server" CssClass="ContentLabel" />
        </div>
        <div class="control-group-inline">
            <cms:ActivityBar ID="ucActivityBar" runat="server" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent" Visible="false">
        <asp:Label ID="lblResult" runat="server" CssClass="ContentLabel" EnableViewState="false" />
    </asp:Panel>
    <cms:AsyncControl ID="ucAsyncControl" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnRestore" runat="server" ResourceString="General.Restore" ButtonStyle="Primary" />
    </div>
</asp:Content>
