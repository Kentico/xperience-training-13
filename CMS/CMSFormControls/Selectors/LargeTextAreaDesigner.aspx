<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="LargeTextAreaDesigner.aspx.cs"
    Inherits="CMSFormControls_Selectors_LargeTextAreaDesigner" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Edit text" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:MacroEditor ID="txtText" Height="500px" Width="99%" runat="server" />

        <script type="text/javascript">
            //<![CDATA[
            // Retrieves the text from the form control
            function load() {
                var elem = wopener.document.getElementById('<%=EditorId%>');
                if (wopener && elem) {
                    var txtArea = document.getElementById('<%=txtText.Editor.ClientID%>');
                    var openerValue = elem.value;
                    if (txtArea) {
                        txtArea.value = openerValue;
                    }
                }
                return false;
            }
            
            // Returns the text to the form control
            function set() {
                var elem = wopener.document.getElementById('<%=EditorId%>');
                if (wopener && elem) {
                    if ((typeof(<%=txtText.Editor.EditorID%>) != 'undefined') && (<%=txtText.Editor.EditorID%> != null))
                    {   
                        elem.value = <%=txtText.Editor.EditorID%>.getValue();
                    }
                    else
                    {
                        elem.value = document.getElementById('<%=txtText.Editor.ClientID%>').value;
                    }
                    wopener.$cmsj(elem).trigger('change');
                }
                CloseDialog();
            }

            load();
            //]]>
        </script>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" ResourceString="general.saveandclose"
        OnClientClick="set();" />
</asp:Content>
