<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_WinnerMailout"
     Codebehind="Newsletter_Issue_WinnerMailout.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>

<asp:Content runat="server" ID="cntBody" ContentPlaceHolderID="plcContent">
    <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false" />
    <asp:Panel runat="server" ID="pnlMailoutTime" CssClass="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMailoutTime" ResourceString="newsletter_winnermailout.mailouttime"
                    EnableViewState="false" ShowRequiredMark="True" DisplayColon="true" AssociatedControlID="dtpMailout" />
            </div>
            <div class="editing-form-value-cell">
                <cms:DateTimePicker runat="server" ID="dtpMailout" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedButton ID="btnSend" runat="server" EnableViewState="false" ResourceString="general.saveandclose"
        ButtonStyle="Primary" />
</asp:Content>