<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="CategoryEdit.aspx.cs" Inherits="CMSModules_Categories_CMSPages_CategoryEdit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    Title="Message - Edit" %>

<%@ Register Src="~/CMSModules/Categories/Controls/CategoryEdit.ascx" TagName="CategoryEdit"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:CategoryEdit ID="catEdit" runat="server" Visible="true" IsLiveSite="true" DisplayOkButton="false" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" EnableViewState="False"
            ResourceString="general.ok" /><cms:LocalizedButton ID="btnCancel" runat="server"
               ButtonStyle="Primary" EnableViewState="False" ResourceString="general.cancel" />
    </div>
</asp:Content>
