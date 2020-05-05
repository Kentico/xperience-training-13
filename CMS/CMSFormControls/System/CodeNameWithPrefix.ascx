<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CodeNameWithPrefix.ascx.cs" Inherits="CMSFormControls_System_CodeNameWithPrefix" %>

<div class="codename-with-prefix">
    <div class="control-group-inline">
        <cms:CMSTextBox ID="txtPrefix" runat="server" Visible="false" CssClass="input-width-40" />
        <cms:LocalizedLabel ID="lblPrefix" runat="server" Visible="false" CssClass="form-control-text input-width-40" />
        <asp:Label ID="lblJoiner" runat="server" CssClass="form-control-text input-width-5" />
        <cms:CMSTextBox ID="txtCodeName" runat="server" CssClass="input-width-60" />
    </div>
    <asp:PlaceHolder ID="plcInfo" runat="server">
        <div class="explanation-text">
            <cms:LocalizedLabel CssClass="input-width-40" ID="lblNamespace" runat="server" />
            <span class="input-width-5"></span>
            <cms:LocalizedLabel ID="lblClass" runat="server" CssClass="input-width-60" />
        </div>
    </asp:PlaceHolder>
</div>
<cms:CMSRequiredFieldValidator ID="rfvCodeNameNamespace" ControlToValidate="txtPrefix" runat="server" Display="Dynamic"></cms:CMSRequiredFieldValidator>
<cms:CMSRequiredFieldValidator ID="rfvCodeNameName" ControlToValidate="txtCodeName" runat="server" Display="Dynamic"></cms:CMSRequiredFieldValidator>