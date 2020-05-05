<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ContactStatusDialog.aspx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_ContactStatusDialog" Title="Contact status"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlBody" runat="server" CssClass="PageContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactStatusDisplayName" ObjectType="om.contactstatus"
                    Columns="ContactStatusID,ContactStatusDisplayName,ContactStatusDescription"
                    IsLiveSite="false" ShowActionsMenu="false">
                    <GridColumns>
                        <ug:Column runat="server" ExternalSourceName="contactstatusdisplayname" Source="##ALL##" Caption="$om.contactstatus.displayname$"
                            Wrap="false" CssClass="main-column-100">
                            <Filter Type="text" Size="200" Source="ContactStatusDisplayName" />
                        </ug:Column>
                    </GridColumns>
                    <GridOptions DisplayFilter="true" ShowSelection="false" />
                </cms:UniGrid>
                <div class="ClearBoth">
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="footer" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnReset" runat="server" ButtonStyle="Primary" EnableViewState="False"
            ResourceString="general.reset" Visible="false" />
    </div>
</asp:Content>