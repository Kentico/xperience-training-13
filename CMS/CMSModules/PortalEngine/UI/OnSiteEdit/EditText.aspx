<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="EditText.aspx.cs" Inherits="CMSModules_PortalEngine_UI_OnSiteEdit_EditText"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Edit text" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/Editable/EditableText.ascx"
    TagName="EditableText" TagPrefix="cms" %>

<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />
    <div id="CKToolbar">
    </div>
    <cms:MessagesPlaceHolder  runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" OffsetX="16" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlEditor" runat="server" CssClass="EditableTextContainer" >
        <cms:EditableText ID="ucEditableText" runat="server" />
    </asp:Panel>
</asp:Content>