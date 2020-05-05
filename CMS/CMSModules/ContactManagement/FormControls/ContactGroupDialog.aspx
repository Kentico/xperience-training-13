<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ContactGroupDialog.aspx.cs" Inherits="CMSModules_ContactManagement_FormControls_ContactGroupDialog"
    Title="Contact group" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlBody" runat="server" CssClass="UniSelectorDialogBody">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="UniSelectorDialogGridArea">
                    <div class="UniSelectorDialogGridPadding">
                        <cms:UniGrid runat="server" ID="gridElem" OrderBy="ContactGroupDisplayName" ObjectType="om.contactgroup"
                            Columns="ContactGroupID,ContactGroupDisplayName,ContactGroupDescription" IsLiveSite="false" ShowActionsMenu="false">
                            <GridColumns>
                                <ug:Column ExternalSourceName="contactgroupdisplayname" Source="##ALL##" Caption="$om.contactgroup.displayname$"
                                    Wrap="false" CssClass="main-column-100">
                                    <Filter Type="text" Size="200" Source="ContactGroupDisplayName" />
                                </ug:Column>
                            </GridColumns>
                            <GridOptions DisplayFilter="true" ShowSelection="false" />
                        </cms:UniGrid>
                        <div class="ClearBoth">
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>