<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="Preview.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Preview_Preview" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/LanguageMenu.ascx" TagName="LanguageMenu"
    TagPrefix="cms" %>

<asp:Button ID="btnLanguage" CssClass="HiddenButton" runat="server" />
<asp:Button ID="btnDevice" CssClass="HiddenButton" runat="server" />
<asp:Panel runat="server" ID="pnlPreviewContent" DefaultButton="imgRefresh">
    <div class="FloatLeft btn-actions">
        <cms:SelectPath runat="server" ID="ucPath" IsLiveSite="false" OnChanged="ucPath_PathChanged" SinglePathMode="true" CssClass="cms-selector" />
        <cms:LanguageMenu runat="server" ID="ucSelectCulture" DisplayCompare="false" />
        <asp:PlaceHolder runat="server" ID="plcDevice">
        </asp:PlaceHolder>
        <cms:CMSAccessibleButton runat="server" ID="imgRefresh" OnClick="imgRefresh_clicked" IconCssClass="icon-rotate-right" IconOnly="True" />
    </div>
    <div class="Clear"></div>
</asp:Panel>
