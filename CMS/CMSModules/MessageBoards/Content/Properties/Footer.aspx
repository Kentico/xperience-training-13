<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Content_Properties_Footer"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Content - Footer"  Codebehind="Footer.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlBody" EnableViewState="false">
        <div class="PageFooterLine">
            <div class="RightAlign">
                <cms:LocalizedButton ID="btnClose" runat="server" OnClientClick="return CloseDialog();"
                    ResourceString="general.close" ButtonStyle="Primary" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
