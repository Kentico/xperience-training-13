<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Properties.aspx.cs" Inherits="CMSModules_Modules_Pages_Module_UserInterface_Properties"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register TagPrefix="cms" TagName="AnchorDropup" Src="~/CMSAdminControls/UI/PageElements/AnchorDropup.ascx" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlTab" CssClass="TabsPageContent">
        <asp:Panel runat="server" ID="pnlFormArea" CssClass="WebPartForm">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" OffsetX="10" OffsetY="10" IsLiveSite="false" />
            <cms:BasicForm runat="server" ID="form" HtmlAreaToolbarLocation="Out:CKToolbar" Enabled="true"
                DefaultCategoryName="{$general.default$}" AllowMacroEditing="true"
                IsLiveSite="false" MarkRequiredFields="true" />
            <br class="ClearBoth" />
        </asp:Panel>
    </asp:Panel>
    <cms:AnchorDropup ID="anchorDropup" runat="server" MinimalAnchors="5" />
</asp:Content>
