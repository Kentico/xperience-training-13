<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_EditFolder"  Codebehind="EditFolder.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Panel ID="pnlFolderEdit" runat="server" EnableViewState="false">
    <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs">
        <cms:JQueryTab ID="tabGeneral" runat="server">
            <ContentTemplate>
                <cms:HeaderActions ID="headerActions" runat="server" PanelCssClass="cms-edit-menu" />
                <asp:Panel ID="pnlContent" runat="server">
                    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:LocalizedLabel CssClass="control-label" ID="lblFolderName" runat="server" DisplayColon="true"
                                    ResourceString="media.folder.foldername" EnableViewState="false" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:CMSTextBox ID="txtFolderName" runat="server" MaxLength="50"
                                    EnableViewState="false" />
                            </div>
                        </div>
                        <asp:PlaceHolder runat="server" ID="plcSubmit">
                        <div class="form-group">
                            <div id="divButtons" runat="server" enableviewstate="false" class="editing-form-value-cell editing-form-value-cell-offset">
                                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOk_Click" ButtonStyle="Primary" ResourceString="general.saveandclose"
                                    EnableViewState="false" />
                            </div>
                        </div>
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:JQueryTab>
    </cms:JQueryTabContainer>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Panel>