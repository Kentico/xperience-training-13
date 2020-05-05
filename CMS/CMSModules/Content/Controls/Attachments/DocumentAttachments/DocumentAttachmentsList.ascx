<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_Attachments_DocumentAttachments_DocumentAttachmentsList"
     Codebehind="DocumentAttachmentsList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="updPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:AttachmentsDataSource ID="dsAttachments" runat="server" GetBinary="true" AutomaticColumns="false"
            IsLiveSite="false" />
        <asp:Panel ID="pnlCont" runat="server">
            <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
            <asp:Label ID="lblWf" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
            <asp:Panel ID="pnlGrid" runat="server">
                
                <div class="content-block-50">
                    <asp:PlaceHolder ID="plcUploader" runat="server">
                        <cms:DirectFileUploader ID="newAttachmentElem" runat="server" InsertMode="true" UploadMode="DirectMultiple"
                            Multiselect="true" ShowProgress="true" />
                    </asp:PlaceHolder>
                </div>
                <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter" Visible="false">
                    <div class="form-horizontal form-filter attachment-filter dont-check-changes">
                        <div class="form-group">
                            <div class="filter-form-value-cell-wide form-search-container">
                                <cms:CMSTextBox ID="txtFilter" runat="server" />
                                <cms:CMSIcon ID="iconSearch" runat="server" CssClass="icon-magnifier" EnableViewState="false" />
                            </div>
                        </div>
                        <div class="form-group form-group-buttons">
                            <div class="filter-form-buttons-cell">
                                <cms:LocalizedButton ID="btnFilter" runat="server" ButtonStyle="Primary" ResourceString="general.search" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <cms:UniGrid ID="gridAttachments" ShortID="g" runat="server" ExportFileName="cms_attachment"
                    GridName="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DocumentAttachmentsList.xml" />
                <cms:LocalizedLabel ID="lblNoData" runat="server" ResourceString="attach.nodata"
                    Visible="false" />
            </asp:Panel>
        </asp:Panel>
        <div>
            <asp:Button ID="hdnPostback" CssClass="HiddenButton" runat="server" EnableViewState="false" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:Button ID="hdnFullPostback" CssClass="HiddenButton" runat="server" EnableViewState="false" />