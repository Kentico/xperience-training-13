<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_ColorPicker_ColorPicker_Control"  Codebehind="ColorPicker.ascx.cs" %>
<asp:TextBox ID="txtColor" runat="server" ReadOnly="true" CssClass="Color form-control input-width-20" />

<script type="text/javascript">
    //<![CDATA[
    function CloseWindow() {
        CloseDialog();
    }

    function SetColor(sourceid, destinationid, previewid, postback) {

        var txtColorElem = document.getElementById(sourceid);
        var destinationElem = wopener.document.getElementById(destinationid);
        var previewElem = wopener.document.getElementById(previewid);

        if (txtColorElem != null) {
            color = txtColorElem.value;
        }
        if (destinationElem != null) {
            destinationElem.value = color;
            if (destinationElem.onchange != null) {
                destinationElem.onchange();
            }
        }
        if (previewElem != null) {
            previewElem.style.backgroundColor = color;
        }
        if (postback) {
            if (destinationElem.fireEvent) {
                destinationElem.fireEvent('onchange');
            }
            else {
                var e = document.createEvent('HTMLEvents');
                e.initEvent('change', false, false);
                destinationElem.dispatchEvent(e);
            }
        }

        CloseWindow();

    }

    //]]>
</script>

<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />