<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_Content_CombinationPanel"
     Codebehind="CombinationPanel.ascx.cs" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/FormControls/SelectMVTCombination.ascx"
    TagName="CombinationSelector" TagPrefix="cms" %>

<asp:Panel ID="pnlMvtCombination" runat="server" CssClass="mvt-combination-panel control-group-inline">
    <cms:LocalizedLabel ID="lblCombination" runat="server" AssociatedControlID="cs" DisplayColon="true" CssClass="form-control-text control-label" />
    <cms:CombinationSelector ID="combinationSelector" ShortID="cs" runat="server" HighlightEnabled="true" Visible="true" />
    <asp:PlaceHolder ID="plcEditCombination" runat="server">
        <cms:CMSCheckBox ID="chkEnabled" runat="server" CssClass="chk-enabled" />
        <cms:LocalizedLabel ID="lblCustomName" runat="server" AssociatedControlID="txtCustomName" DisplayColon="true" CssClass="form-control-text control-label" />
        <cms:CMSTextBox ID="txtCustomName" runat="server" MaxLength="200" CssClass="input-width-60" />
        <cms:LocalizedButton ID="btnChange" runat="server" ButtonStyle="Default" />
        <asp:PlaceHolder ID="plcUseCombination" runat="server" Visible="false">
            <cms:LocalizedButton ID="btnUseCombination" runat="server" ButtonStyle="Primary" CssClass="btn-use-combination" />
        </asp:PlaceHolder>
        <cms:LocalizedLabel ID="lblSaved" runat="server"></cms:LocalizedLabel>
    </asp:PlaceHolder>
    <asp:HiddenField ID="hdnCurrentCombination" runat="server" />
</asp:Panel>
