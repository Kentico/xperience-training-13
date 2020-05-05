<%@ Page Language="C#" AutoEventWireup="true" 
    Inherits="CMSModules_Widgets_Dialogs_WidgetSelector" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" Title="Add widget"  Codebehind="WidgetSelector.aspx.cs" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/WidgetSelector.ascx" TagName="WidgetSelector"
    TagPrefix="cms" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlSelector">
        <cms:WidgetSelector runat="server" IsLiveSite="false" ID="selectElem" />
        <cms:LocalizedHidden ID="hdnMessage" runat="server" Value="{$widgets.NoWidgetSelected$}" EnableViewState="false" />
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="plcFooter">
    <cms:LocalizedButton runat="server" ID="btnOk" ResourceString="general.select" ButtonStyle="Primary" EnableViewState="false" />
</asp:Content>
