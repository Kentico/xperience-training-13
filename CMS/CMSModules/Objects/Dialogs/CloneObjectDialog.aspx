<%@ Page Language="C#" AutoEventWireup="false"  Codebehind="CloneObjectDialog.aspx.cs"
    Inherits="CMSModules_Objects_Dialogs_CloneObjectDialog" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Clone Object" Theme="Default" %>

<%@ Register Src="~/CMSModules/Objects/Controls/CloneObject.ascx" TagPrefix="cms"
    TagName="CloneObject" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server">
        <asp:PlaceHolder runat="server" ID="plcForm">
            <cms:CloneObject ID="cloneObjectElem" runat="server" />
        </asp:PlaceHolder>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnClone" runat="server" ButtonStyle="Primary" ResourceString="general.clone" OnClick="btnClone_Click" />
</asp:Content>
