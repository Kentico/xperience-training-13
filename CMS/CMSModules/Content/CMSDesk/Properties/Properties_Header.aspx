<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_CMSDesk_Properties_Properties_Header"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master" Title="Properties"  Codebehind="Properties_Header.aspx.cs" %>

<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">

        function CheckChanges() {
            if (parent.frames['content'].CheckChanges) {
                return parent.frames['content'].CheckChanges();
            }
            return true;
        }

    </script>
</asp:Content>

