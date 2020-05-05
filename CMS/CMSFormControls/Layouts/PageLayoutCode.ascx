<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="PageLayoutCode.ascx.cs"
    Inherits="CMSFormControls_Layouts_PageLayoutCode" %>

<div class="form-horizontal">
    <asp:Panel runat="server" ID="pnl" CssClass="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblElements" ResourceString="PageLayout.LayoutElement" DisplayColon="true" CssClass="control-label" AssociatedControlID="drpElements" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpElements" />
            <cms:LocalizedButton ButtonStyle="Default" runat="server" ID="btn" ResourceString="dialogs.actions.insert" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlType" CssClass="form-group" runat="server">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblType" ResourceString="PageLayout.Type" DisplayColon="true" CssClass="control-label" AssociatedControlID="drpType" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpType" OnSelectedIndexChanged="drpType_selectedIndexChanged" AutoPostBack="true" />
        </div>
    </asp:Panel>
    <div class="ClearBoth">
    </div>
</div>
<asp:Panel runat="server" ID="plcDirectives" CssClass="NORTL CodeDirectives">
    <asp:Label runat="server" ID="ltlDirectives" EnableViewState="false" />
</asp:Panel>
<cms:MacroEditor ID="tbLayoutCode" runat="server" Width="98%" Height="300px" />
