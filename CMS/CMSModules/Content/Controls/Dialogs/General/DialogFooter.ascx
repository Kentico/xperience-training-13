<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Dialogs_General_DialogFooter"
     Codebehind="DialogFooter.ascx.cs" %>
<asp:HiddenField ID="hdnSelected" runat="server" />
<asp:HiddenField ID="hdnAnchors" runat="server" />
<asp:HiddenField ID="hdnIds" runat="server" />
<script type="text/javascript">
    //<![CDATA[
    function insertItem() {
        if ((window.parent.frames['insertContent']) && (window.parent.frames['insertContent'].insertItem)) {
            window.parent.frames['insertContent'].insertItem();
        }
        return false;
    }
    //]]>
</script>
<div class="FloatRight">
    <cms:LocalizedButton ID="btnInsert" runat="server" ResourceString="general.saveandclose"
        ButtonStyle="Primary" EnableViewState="false" OnClientClick="return insertItem();" />
</div>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Button ID="btnHidden" runat="server" EnableViewState="false" CssClass="HiddenButton"
    OnClick="btnHidden_Click" />