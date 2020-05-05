<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Notifications_Development_Templates_Template_Edit_Text"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Templates edit - text"
     Codebehind="Template_Edit_Text.aspx.cs" %>

<%@ Register Src="~/CMSModules/Notifications/Controls/TemplateText.ascx" TagName="TemplateText"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder runat="server" ID="plcMacros" Visible="false">
        <a href="#" onclick="$cmsj('#macrosHelp').toggleClass('hidden');">
            <asp:Label ID="lnkMoreMacros" runat="server" CssClass="InfoLabel" EnableViewState="false" /></a>
        <div class="content-block-50 hidden" id="macrosHelp">
            <strong>
                <cms:LocalizedLabel ID="lblHelpHeader" runat="server" CssClass="InfoLabel" DisplayColon="true"
                    EnableViewState="false" />
            </strong>
            <asp:Table ID="tblHelp" runat="server" EnableViewState="false" GridLines="Horizontal"
                CellPadding="3" CellSpacing="0" Width="100%">
            </asp:Table>
        </div>
    </asp:PlaceHolder>
    <cms:TemplateText runat="server" ID="templateTextElem" />
</asp:Content>