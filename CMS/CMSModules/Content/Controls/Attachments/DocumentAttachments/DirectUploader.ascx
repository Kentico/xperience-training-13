<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Attachments_DocumentAttachments_DirectUploader"
     Codebehind="DirectUploader.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:cmsupdatepanel id="updPanel" runat="server" updatemode="Conditional">
    <ContentTemplate>
        <cms:CMSPanel runat="server" id="pnlAttachments" ShortID="p">
            <div class="New">
                <asp:PlaceHolder ID="plcUploader" runat="server">
                    <cms:DirectFileUploader ID="newAttachmentElem" runat="server" InsertMode="true" UploadMode="DirectSingle" />
                </asp:PlaceHolder>
            </div>
            <cms:AttachmentsDataSource ID="dsAttachments" runat="server" GetBinary="false" IsLiveSite="false" />
            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Visible="false" EnableViewState="false" />
            <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
            <asp:Panel ID="pnlGrid" runat="server">
                <cms:UniGrid ID="gridAttachments" runat="server" GridName="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DirectUploader.xml" />
            </asp:Panel>
            <div>
                <asp:Button ID="hdnPostback" CssClass="HiddenButton" runat="server" EnableViewState="false" />
                <asp:HiddenField ID="hdnAttachName" runat="server" />
                <asp:HiddenField ID="hdnAttachGuid" runat="server" />
            </div>
        </cms:CMSPanel>
    </ContentTemplate>
</cms:cmsupdatepanel>
<asp:Button id="hdnFullPostback" cssclass="HiddenButton" runat="server" enableviewstate="false" />
