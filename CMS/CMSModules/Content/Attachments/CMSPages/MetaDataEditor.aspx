<%@ Page Title="Edit metadata" Language="C#" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master"
    AutoEventWireup="true"  Codebehind="MetaDataEditor.aspx.cs" Inherits="CMSModules_Content_Attachments_CMSPages_MetaDataEditor"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/MetaDataEdit.ascx" TagName="MetaDataEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageContent">
        <cms:metadataeditor id="metaDataEditor" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:localizedbutton id="btnSave" runat="server" ButtonStyle="Primary" resourcestring="general.saveandclose"
            enableviewstate="false" />
        <cms:localizedbutton id="btnClose" runat="server" ButtonStyle="Primary" resourcestring="general.close"
            enableviewstate="false" onclientclick="CloseDialog();" />
    </div>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
