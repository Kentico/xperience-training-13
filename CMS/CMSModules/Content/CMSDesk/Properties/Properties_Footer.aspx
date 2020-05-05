<%@ Page Language="C#" Theme="Default" AutoEventWireup="true"
    Inherits="CMSModules_Content_CMSDesk_Properties_Properties_Footer" EnableEventValidation="false"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Properties - footer"  Codebehind="Properties_Footer.aspx.cs" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="PageFooterLine">
    <asp:Panel ID="pnlFooter" runat="server" CssClass="TextRight">
        <cms:LocalizedButton ID="btnClose" runat="server" ResourceString="general.close" ButtonStyle="Primary" OnClientClick="javascript:parent.frames['header'].CloseDialog(); return false;" />
    </asp:Panel>
    </div>
</asp:Content>