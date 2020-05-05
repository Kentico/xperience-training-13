<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SiteCultureChanger.ascx.cs" Inherits="CMSFormControls_Sites_SiteCultureChanger" %>

<script type="text/javascript">
    //<![CDATA[
    function OpenCultureChanger(siteId, culture) {
        if (siteId != '0') {
            modalDialog(pageChangeUrl + '?siteid=' + siteId + '&culture=' + culture, 'CutlureChange', 770, 200);
        }
    }
    //]]>	
</script>

<cms:CMSTextBox ID="txtCulture" runat="server" MaxLength="400" EnableViewState="false" /><cms:CMSButton runat="server" ID="btnChange" ButtonStyle="Default" />
<asp:HiddenField ID="hdnDocumentsChangeChecked" runat="server" />
<asp:Button runat="server" ID="btnHidden" CssClass="HiddenButton" OnClick="btnHidden_Click" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />