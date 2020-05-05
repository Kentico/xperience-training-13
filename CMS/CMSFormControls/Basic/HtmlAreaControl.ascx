<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="HtmlAreaControl.ascx.cs"
    Inherits="CMSFormControls_Basic_HtmlAreaControl" %>

<div class="control-group-inline">
    <cms:CMSHtmlEditor ID="editor" runat="server" />
</div>
<asp:Panel ID="pnlStamp" Visible="false" runat="server" CssClass="control-group-inline">
    <script type="text/javascript">
        $cmsj(document).ready(function () {
            $cmsj('#<%= btnStamp.ClientID %>').click(function () { InsertHTML('<%= StampValue %>', '<%= CurrentEditor.ClientID %>'); return false; });
        });
    </script>
    <cms:LocalizedButton ID="btnStamp" runat="server" ResourceString="formcontrol.htmlareacontrol.timestamp"
        ButtonStyle="Default" EnableViewState="false" />
</asp:Panel>