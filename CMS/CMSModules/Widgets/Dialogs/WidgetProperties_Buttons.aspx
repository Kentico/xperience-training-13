<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_Dialogs_WidgetProperties_Buttons" Theme="default"  Codebehind="WidgetProperties_Buttons.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlScroll" CssClass="dialog-footer">
        <div class="FloatLeft">
            <cms:CMSCheckBox runat="server" ID="chkRefresh" Checked="true" />
        </div>
        <cms:CMSButton ID="btnApply" runat="server" ButtonStyle="Default" />
        <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Default" />
        <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" />
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
