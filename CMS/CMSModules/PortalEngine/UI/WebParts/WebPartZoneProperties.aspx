<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartZoneProperties"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Web part zone - Properties"  Codebehind="WebPartZoneProperties.aspx.cs" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/WebPartZoneProperties.ascx" TagPrefix="cms" TagName="WebPartZoneProperties" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">

        function RefreshPage() {
            if (typeof (wopener) === 'undefined') {
                wopener = parent.wopener;
            }

            if (wopener.RefreshPage) {
                wopener.RefreshPage();
            }
        }


        function UpdateVariantPosition(itemCode, variantId) {
            if (typeof (wopener) === 'undefined') {
                wopener = parent.wopener;
            }

            if (wopener.UpdateVariantPosition) {
                wopener.UpdateVariantPosition(itemCode, variantId);
            }
        }

        function StoreSelectedTab(selectedTab) {
            var field = document.getElementById('hdnSelectedTab');
            if (field) field.value = selectedTab;
        }
    </script>
    <div class="WebPartZoneProperties">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="plcDynamicContent"></asp:PlaceHolder>
                <input type="hidden" id="hdnSelectedTab" name="hdnSelectedTab" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
    <br />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatLeft">
        <cms:CMSCheckBox runat="server" ID="chkRefresh" Checked="true" />
    </div>
    <div class="FloatRight">
        <cms:CMSUpdatePanel ID="pnlButtons" runat="server">
            <ContentTemplate>
                <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" Visible="true" OnClick="btnOK_Click" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
