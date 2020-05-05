<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="EditFile.ascx.cs" Inherits="CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_EditFile" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
    <ContentTemplate>
        <asp:Button CssClass="HiddenButton" runat="server" ID="btnAction" OnClick="btnAction_Click" />
        <div runat="server" id="pnlMenu" class="PreviewMenu">
            <div class="cms-edit-menu">
                <cms:HeaderActions runat="server" ID="headerActions" IsLiveSite="false" PerformFullPostBack="false" />
            </div>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Panel ID="pnlContent" runat="server" CssClass="PreviewBody">
    <div class="PageContent">
        <cms:MessagesPlaceHolder runat="server" ID="pnlMessagePlaceholder" IsLiveSite="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" ID="lblName" EnableViewState="false" DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtName" MaxLength="100" CssClass="form-control" />
                    <asp:Label runat="server" ID="lblExt" EnableViewState="false" CssClass="form-control-text" />
                </div>
            </div>
        </div>
        <table style="width: 100%;" cellpadding="0">
            <tr>
                <td>
                    <cms:ExtendedTextArea ID="txtContent" runat="server" EnableViewState="true" EditorMode="Advanced"
                        Language="CSS" Width="98%" Height="460px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
