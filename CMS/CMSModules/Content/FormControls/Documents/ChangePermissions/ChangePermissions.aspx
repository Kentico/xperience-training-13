<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ChangePermissions.aspx.cs"
    Inherits="CMSModules_Content_FormControls_Documents_ChangePermissions_ChangePermissions"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/Security.ascx" TagName="Security"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">

    <script type="text/javascript">
        //<![CDATA[   
        function Close() {
            CloseDialog();
        }
        //]]>
    </script>

    <asp:Panel ID="pnlContent" runat="server">
        <cms:Security ID="securityElem" runat="server" IsLiveSite="false" />
        <cms:LocalizedLabel ID="lblInfo" runat="server" EnableViewState="false" Visible="false" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <cms:CMSUpdatePanel ID="updPanelFooter" runat="server">
        <ContentTemplate>
            <div class="FloatRight">
                <cms:LocalizedButton ID="btnSave" runat="server" ButtonStyle="Primary" ResourceString="general.apply" OnClick="btnSave_OnClick"
                    EnableViewState="false" />
                <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Primary" ResourceString="general.close"
                    EnableViewState="false" OnClientClick="CloseDialog();return false;" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
