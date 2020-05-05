<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="EditSubscription.aspx.cs" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Theme="Default" Inherits="CMSModules_Reporting_LiveDialogs_EditSubscription" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/SubscriptionEdit.ascx" TagName="SubscriptionEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:SubscriptionEdit runat="server" ID="subEdit" SimpleMode="true" IsLiveSite="true" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div id="divFooter" runat="server" style="position: absolute; width: 100%">
        <div class="ReportEditButtons">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click" />
            <cms:CMSButton ID="btnCancel" runat="server" OnClientClick="return CloseDialog();"
                ButtonStyle="Primary" />
        </div>
    </div>
</asp:Content>
