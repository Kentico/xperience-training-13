<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Export_Site_media_library"  Codebehind="media_library.ascx.cs" %>

<script type="text/javascript">
    //<![CDATA[
    function medCheckChange() {
        medChck2.disabled = !medChck1.checked;
        medChck2.checked = false;
    }

    function medInitCheckboxes() {
        if (!medChck1.checked) {
            medChck2.disabled = true;
        }
    }
    //]]>
</script>

<asp:Panel runat="server" ID="pnlCheck" CssClass="wizard-section">
    <asp:Label ID="lblInfo" runat="Server" EnableViewState="false" CssClass="form-group" />
    <div class="checkbox-list-vertical content-block-50">
        <cms:CMSCheckBox ID="chkFiles" runat="server" />
        <cms:CMSCheckBox ID="chkPhysicalFiles" runat="server" />
    </div>
</asp:Panel>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />